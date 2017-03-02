//#define SPAWN_ONLY_SURFACE

using Sandbox;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour
{
	public LayerMask dropLayer;
	public GameObject player;

	//Количиство чанков которые должны присутствовать на сцене одновременно
	public int viewDistance = 3;

	public Material mat;
	public Material matDark;
	public Material matBG;
	public int seed;
	public bool isRandomSeed = true;
	public int worldHeight = 255;
	public float scale = 1.0f;
	public int octaves = 1;
	public float persistance = 1.0f;
	public float lacunarity = 1.0f;
	private BlockManager blockManager;
	private Dictionary<Vector2Int, Chunk> visibleChunks;
	private ItemDatabase itemDatabase;
	private PerlinNoizeGenerator perlinNoizeGenerator;

	void Start()
	{
		blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
		itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
		Rebuild();
		if (Application.isPlaying)
		{
			var playerX = Chunk.size / 2;
			var playerPos = new Vector2(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
			player = SpawnPlayer(playerPos);
		}
	}

	public void Rebuild()
	{
		if (visibleChunks != null)
		{
			foreach (var key in visibleChunks.Keys.ToArray())
			{
				visibleChunks[key].Destroy();
				visibleChunks.Remove(key);
			}
		}
		if (isRandomSeed)
		{
			seed = Random.Range(-5000, 5000);
		}
		visibleChunks = new Dictionary<Vector2Int, Chunk>();

		perlinNoizeGenerator = new PerlinNoizeGenerator(new Vector2Int(seed, seed), worldHeight, scale, octaves, persistance,
			lacunarity);
		if (Application.isPlaying)
		{
			var playerX = Chunk.size / 2;
			player.transform.position = new Vector2(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
		}
	}

	private GameObject SpawnPlayer(Vector2 pos)
	{
		GameObject player_object = GameObject.Instantiate(player, pos, Quaternion.identity) as GameObject;
		return player_object;
	}

	private Chunk ChunkAtPos(Vector2Int pos)
	{
		Chunk chunk;
		if (visibleChunks.TryGetValue(pos, out chunk))
		{
			return chunk;
		}

		return null;
	}

	private void ToMoveBack(GameObject block)
	{
		block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, 1);
		BoxCollider2D bc = block.GetComponent<BoxCollider2D>();
		bc.isTrigger = true;
		// bc.enabled = false;
		SpriteRenderer srBG = block.GetComponent<SpriteRenderer>();
		srBG.material = matBG;
		block.layer = 8;
	}

	public void DestroyBlock(GameObject block)
	{
		Vector3 blockPos = block.transform.position;
		Vector2Int chunkPos = Chunk.GetChunkCoordAtPos(blockPos);

		// SpriteRenderer sr = block_down.GetComponent<SpriteRenderer>();
		//   sr.material = mat;

		int x = (int) chunkPos.x;
		int y = (int) chunkPos.y;

		Chunk chunk = ChunkAtPos(chunkPos);
		if (chunk.blocks[x, y] != null)
		{
			foreach (Drop drop in chunk.blocks[x, y].drops)
			{
				if (drop.DropChanceSucess())
				{
					GameObject dropObject = new GameObject();
					dropObject.transform.position = block.transform.position;
					dropObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					dropObject.AddComponent<SpriteRenderer>().sprite = itemDatabase.FindItem(drop.itemName).sprite;
					dropObject.AddComponent<PolygonCollider2D>();
					dropObject.AddComponent<Rigidbody2D>();
					dropObject.layer = dropLayer;
					dropObject.AddComponent<Magnetism>().target = GameObject.FindWithTag("player").transform;
					dropObject.name = drop.itemName;
				}
			}
		}

		if (block.name == "grass")
		{
			GameObject[] tallGrass = GameObject.FindGameObjectsWithTag("tall_grass");

			if (chunk.blocks[x, y + 1] != null)
			{
				foreach (GameObject grass in tallGrass)
				{
					if (grass.transform.position.x == block.transform.position.x)
					{
						GameObject.Destroy(grass); //with tall_grass
						ToMoveBack(block);
						chunk.blocks[x, y + 1] = blockManager.FindBlock(0);
						//GameObject.Destroy(block);
					}
				}
			}
			else
			{
				ToMoveBack(block);
				chunk.blocks[x, y] = blockManager.FindBlock(0); //without tall_grass
				//GameObject.Destroy(block);
			}
		}
		else
		{
			ToMoveBack(block);
			chunk.blocks[x, y] = blockManager.FindBlock(0);
			if (block.gameObject.CompareTag("tall_grass"))
			{
				GameObject.Destroy(block);
			}
		}
	}

	public void PlaceBlock(Block block, Vector3 pos /*, GameObject go*/)
	{
		Chunk chunk = ChunkAtPos(pos.ToInt());
		Vector2Int tilePos = Chunk.GetTilePositionAtPos(pos);

		chunk.blocks[tilePos.x, tilePos.y] = block;
		chunk.CreateTile(tilePos.x, tilePos.y);
	}

	void Update()
	{
		if (Application.isPlaying)
		{
			var playerPos = Chunk.GetChunkCoordAtPos(player.transform.position);
			UpdateChunks(playerPos);

			foreach (Chunk chunk in visibleChunks.Values.ToArray())
			{
				var xDiff = Mathf.Abs(Mathf.Abs(chunk.coords.x) - Mathf.Abs(playerPos.x));
				var yDiff = Mathf.Abs(Mathf.Abs(chunk.coords.y) - Mathf.Abs(playerPos.y));
				if (xDiff > viewDistance || yDiff > viewDistance)
				{
					chunk.Destroy();
					visibleChunks.Remove(chunk.coords);
				}
			}
		}
	}

	void UpdateChunks(Vector2Int playerPos)
	{
		for (int x = playerPos.x - viewDistance; x <= playerPos.x + viewDistance; x++)
		{
			for (int y = playerPos.y - viewDistance; y <= playerPos.y + viewDistance; y++)
			{
				var pos = new Vector2Int(x, y);
				if (!visibleChunks.ContainsKey(pos))
				{
					Chunk newChunk = new Chunk(blockManager, pos);
					newChunk.GenerateBlocksInfos(perlinNoizeGenerator);
					if (!newChunk.IsEmpty)
					{
						StartCoroutine(GenerateTiles(newChunk));
						visibleChunks.Add(pos, newChunk);
					}
					else
					{
						newChunk.Destroy();
					}
				}
			}
		}
	}
	public IEnumerator GenerateTiles(Chunk chunk)
	{
		for (int x = 0; x < Chunk.size; x++)
		{
			for (int y = 0; y < Chunk.size; y++)
			{
				if (chunk.blocks[x, y] != null)
				{
					chunk.CreateTile(x, y);
				}
				if (Time.time > .03f)
					yield return null;
			}
		}
	}
	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
		if (perlinNoizeGenerator != null)
		{
			var startFrom = -1000;
			var ends = 1000;
			var prevPos = new Vector2(startFrom, perlinNoizeGenerator.GetHeight(startFrom));
			var nextPos = new Vector2(0, 0);
			for (int x = startFrom; x < ends; x++)
			{
				nextPos.x = x;
				nextPos.y = perlinNoizeGenerator.GetHeight(x);
				Gizmos.DrawLine(prevPos, nextPos);
				prevPos = nextPos;
			}
		}
	}
}