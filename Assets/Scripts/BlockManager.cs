using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
	public Material mainMaterial;
	public Material backgroundMaterial;
	private static Material _mainMaterial;
	private static Material _backgroundMaterial;
	private static ItemDatabase _itemDatabase;
	private static BlockManager _blockmanager;
	public Block[] blocks;
	private readonly Dictionary<int, Block> blocksCache = new Dictionary<int, Block>();
	private readonly Dictionary<string, int> blocksNameCache = new Dictionary<string, int>();

	private static Queue<GameObject> blocksPool = new Queue<GameObject>();

	private static GameObject _blockObjectPool;
	private static GameObject blockObjectsPool
	{
		get { return _blockObjectPool ?? (_blockObjectPool = new GameObject()); }
	}

	void Start()
	{
		_mainMaterial = mainMaterial;
		_backgroundMaterial = backgroundMaterial;
		_itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
		_blockmanager = GameObject.Find("GameManager").GetComponent<BlockManager>();
		foreach (var item in blocks)
		{
			blocksCache.Add(item.id, item);
			blocksNameCache.Add(item.display_name, item.id);
		}
	}

	public Block FindBlock(int id)
	{
		Block res;
		if (blocksCache.TryGetValue(id, out res))
		{
			return res;
		}
		return null;
	}


	public Block FindBlock(string blockName)
	{
		int id;
		if (blocksNameCache.TryGetValue(blockName, out id))
		{
			return FindBlock(id);
		}
		return null;
	}

	public GameObject CreateTile(Vector2Int pos, Chunk chunk)
	{
		var blockTag = "Block";
		bool isTrigger = false;
		var position = new Vector3((chunk.coords.x * ChunkManager.chunkSize) + pos.x + .5f, (chunk.coords.y * ChunkManager.chunkSize) + pos.y + .5f);

		if (chunk.blocks[pos.x, pos.y].isSolid != true)
		{
			isTrigger = true;
			blockTag = "tall_grass";
		}
		var block = GetBlock(chunk.gameObject.transform, chunk.blocks[pos.x, pos.y].GetSprite(), position,
			chunk.blocks[pos.x, pos.y].display_name, blockTag, isTrigger);
		block.layer = 13;
		return block;
	}

	public void CreateTileAtPos(int blockId, Vector2 pos)
	{
		Chunk chunk;
		if (ChunkManager.Instance.TryGetChunk(pos.ToInt(), out chunk))
		{
			var block = FindBlock(blockId);
			chunk.CreateTileAtPos(block, pos);
		}

	}

	public Vector2Int GetTilePositionAtPos(Vector3 pos)
	{
		var v = ChunkManager.Instance.GetChunkCoordAtPos(pos);
		v.x += Mathf.RoundToInt(pos.x % ChunkManager.chunkSize);
		v.y += Mathf.RoundToInt(pos.y % ChunkManager.chunkSize);
		return v;
	}

	public GameObject GetTileAtPos(Vector2 pos)
	{
		Chunk chunk;
		if (ChunkManager.Instance.TryGetChunk(pos, out chunk))
		{
			var tilePos = chunk.GetTileCoord(pos);
			return chunk.blockObjects[tilePos.x, tilePos.y];
		}
		return null;
	}

	private GameObject GetBlock(Transform parent, Sprite sprite, Vector3 position, string displayName,
		string blockTag, bool isTrigger)
	{
		GameObject blockObject = null;
		bool cached = false;
		if (blocksPool.Count != 0)
		{
			blockObject = blocksPool.Dequeue();
			blockObject.SetActive(true);
			cached = true;
		}
		else
		{
			blockObject = new GameObject();
		}
		blockObject.transform.parent = parent;
		var sr = cached ? blockObject.GetComponent<SpriteRenderer>() : blockObject.AddComponent<SpriteRenderer>();
		sr.sprite = sprite;
		blockObject.name = displayName;
		blockObject.tag = blockTag;
		blockObject.transform.position = position;
		sr.material = _mainMaterial;
		blockObject.layer = 13; //TODO: Хардкод для слоя препядствий света
		var bc = cached ? blockObject.GetComponent<BoxCollider2D>() : blockObject.AddComponent<BoxCollider2D>();
		bc.isTrigger = isTrigger;
		return blockObject;
	}

	public void Dispose(GameObject block)
	{
		blocksPool.Enqueue(block);
		block.transform.SetParent(blockObjectsPool.transform);
		block.SetActive(false);
	}

	public void DestroyBlock(GameObject block)
	{
		Vector2Int chunkPos = ChunkManager.Instance.GetChunkCoordAtPos(block.transform.position);

		Chunk chunk;
		if (ChunkManager.Instance.TryGetChunk(chunkPos, out chunk))
		{
			var tilePos = chunk.GetTileCoord(block.transform.position);
			if (chunk.blocks[tilePos.x, tilePos.y] != null)
			{
				foreach (Drop drop in chunk.blocks[tilePos.x, tilePos.y].drops)
				{
					if (drop.DropChanceSucess())
					{
						GameObject dropObject = new GameObject();
						dropObject.transform.position = block.transform.position;
						dropObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
						dropObject.AddComponent<SpriteRenderer>().sprite = _itemDatabase.FindItem(drop.itemName).sprite;
						dropObject.AddComponent<PolygonCollider2D>();
						dropObject.AddComponent<Rigidbody2D>();
						dropObject.layer = LayerMask.NameToLayer("drop");
						dropObject.AddComponent<Magnetism>().target = GameObject.FindWithTag("player").transform;
						dropObject.name = drop.itemName;
					}
				}
			}

			if (block.name == "grass")
			{
				GameObject[] tallGrass = GameObject.FindGameObjectsWithTag("tall_grass");

				if (chunk.blocks[tilePos.x, tilePos.y + 1] != null)
				{
					foreach (GameObject grass in tallGrass)
					{
						if (grass.transform.position.x == block.transform.position.x)
						{
							GameObject.Destroy(grass); //with tall_grass
							ToMoveBack(block);
							chunk.blocks[chunkPos.x, chunkPos.y + 1] = FindBlock(0);
							//GameObject.Destroy(block);
						}
					}
				}
				else
				{
					ToMoveBack(block);
					chunk.blocks[tilePos.x, tilePos.y] = FindBlock(0); //without tall_grass
					//GameObject.Destroy(block);
				}
			}
			else
			{
				ToMoveBack(block);
				chunk.blocks[tilePos.x, tilePos.y] = FindBlock(0);
				if (block.gameObject.CompareTag("tall_grass"))
				{
					GameObject.Destroy(block);
				}
			}
		}
	}
	private void ToMoveBack(GameObject block)
	{
		block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, 1);
		BoxCollider2D bc = block.GetComponent<BoxCollider2D>();
		bc.isTrigger = true;
		// bc.enabled = false;
		SpriteRenderer srBG = block.GetComponent<SpriteRenderer>();
		srBG.material = _backgroundMaterial;
		block.layer = 8;
	}
}