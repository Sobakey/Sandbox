using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Chunk
{
	private Block[,] blocks;
	public GameObject[,] blockObjects;
	private BlockManager blockManager;
	public int[,] surroundMap;


	public GameObject gameObject;
	public Vector2Int coords;

	public bool IsEmpty { get; private set; }


	public Vector2 GetWorldPos()
	{
		return new Vector2(coords.x, coords.y);
	}

	public Vector2 GetTileWorldPos(Vector2Int pos)
	{
		return new Vector2((coords.x * ChunkManager.CHUNK_SIZE) + pos.x + .5f,
			(coords.y * ChunkManager.CHUNK_SIZE) + pos.y + .5f);
	}

	public Vector2Int GetTileCoord(Vector2 pos)
	{
		return new Vector2Int((int) ((ChunkManager.CHUNK_SIZE + (pos.x % ChunkManager.CHUNK_SIZE)) % ChunkManager.CHUNK_SIZE),
			(int) ((ChunkManager.CHUNK_SIZE + (pos.y % ChunkManager.CHUNK_SIZE)) % ChunkManager.CHUNK_SIZE));
	}

	public Chunk(BlockManager blockManager, Vector2Int position)
	{
		IsEmpty = true;
		this.blockManager = blockManager;
		this.coords = position;
		blocks = new Block[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];
		blockObjects = new GameObject[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];
		surroundMap = new int[blocks.GetLength(0), blocks.GetLength(1)];
		gameObject = new GameObject();
	}

	public void GenerateBlocksInfos(PerlinNoizeGenerator perlinNoizeGenerator)
	{
		for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
		{
			int pHeight = perlinNoizeGenerator.GetHeight(x + (coords.x * ChunkManager.CHUNK_SIZE));

			for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
			{
				var absoluteY = y + (coords.y * ChunkManager.CHUNK_SIZE);
				if (absoluteY <= pHeight)
				{
					var tranparent = false;
					if (absoluteY == pHeight && absoluteY < 100)
					{
                        if (Random.value < 0.4f)
                        {
                            blocks[x, y] = blockManager.FindBlock(4); //tall_grass
                        }
						tranparent = true;
					}
                    else if (absoluteY == pHeight-1 && absoluteY < 100)
                    {
                            blocks[x, y] = blockManager.FindBlock(1); //grass
                    }
                    else if (absoluteY < pHeight - Random.Range(4, 16) || absoluteY > pHeight - 1 || absoluteY > 100)
                    {
                        blocks[x, y] = blockManager.FindBlock(3); //stone
                    }
                    else
                    {
                        blocks[x, y] = blockManager.FindBlock(2); //dirt
                    }
                    if ((absoluteY == pHeight || absoluteY == pHeight - 1) && absoluteY > 250)
					{
						blocks[x, y] = blockManager.FindBlock(5); //snow
					}

					IsEmpty = false;
					if (!tranparent)
					{
						if (x > 0)
						{
							surroundMap[x - 1, y] += 1;
						}
						if (y > 0)
						{
							surroundMap[x, y - 1] += 1;
						}
						if (x < ChunkManager.CHUNK_SIZE - 1)
						{
							surroundMap[x + 1, y] += 1;
						}
						if (y < ChunkManager.CHUNK_SIZE - 1)
						{
							surroundMap[x, y + 1] += 1;
						}
					}
					else
					{
						surroundMap[x, y] = 4;
					}
				}
				else
				{
					blocks[x, y] = blockManager.FindBlock(0);
				}
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="transparent">Указывает что блок который удаляется прозрачный и не оказывает влияния на коллайдеры соседей</param>
	public void RemoveBlock(Vector2Int pos, bool transparent)
	{
		blocks[pos.x, pos.y] = blockManager.FindBlock(0);
		//TODO: перезаполнение карты коллайдеров сосдних блоков
	}

	public bool IsOpenBlock(Vector2Int coord)
	{
		return surroundMap[coord.x, coord.y] < 4;
	}

	public bool IsSolidBlock(Vector2Int coord)
	{
		return blocks[coord.x, coord.y].isSolid;
	}

	public Sprite GetBlockSprite(Vector2Int coord)
	{
		return blocks[coord.x, coord.y].GetSprite();
	}

	public Drop[] GetBlockDrop(Vector2Int coord)
	{
		return blocks[coord.x, coord.y].drops;
	}

	public bool HasBlockAtCoord(Vector2Int coord)
	{
		return blocks[coord.x, coord.y] != null;
	}

	public string GetBlockName(Vector2Int coord)
	{
		return blocks[coord.x, coord.y].display_name;
	}

	public void GenerateTiles()
	{
		for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
		{
			for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
			{
				if (blocks[x, y] != null)
				{
					blockObjects[x, y] = CreateTile(new Vector2Int(x, y));
				}
			}
		}
	}

	public void CreateTileAtPos(Block block, Vector2 pos)
	{
		Vector2Int tilePos = GetTileCoord(pos);
		blocks[tilePos.x, tilePos.y] = block;
		CreateTile(tilePos);
	}

	public GameObject CreateTile(Vector2Int pos)
	{
		var blockTag = "Block";
		bool isTrigger = false;

		//Добавление .5f позволяет нам устанавливать блок так что б он был от 0 до 1, а не от -.5f до .5f
		var position = GetTileWorldPos(pos);

		if (!IsSolidBlock(pos))
		{
			isTrigger = true;
			blockTag = "tall_grass";
		}
		var block = InstantiateTile(gameObject.transform, GetBlockSprite(pos), position,
			GetBlockName(pos), blockTag, IsOpenBlock(pos), isTrigger);
		block.layer = 13;
		return block;
	}

	public GameObject InstantiateTile(Transform parent, Sprite sprite, Vector3 position, string displayName,
		string blockTag, bool isOpenBlock, bool isTrigger)
	{
		GameObject blockObject = blockManager.PopGameObject();
		blockObject.transform.parent = parent;

		var sr = blockObject.GetComponent<SpriteRenderer>();
		if (!sr)
		{
			sr = blockObject.AddComponent<SpriteRenderer>();
		}
		sr.sprite = sprite;
		blockObject.name = displayName;
		blockObject.tag = blockTag;
		blockObject.transform.position = position;
		sr.material = blockManager.mainMaterial;
		blockObject.layer = 13; //TODO: Хардкод для слоя препядствий света
		if (isOpenBlock)
		{
			AssignCollider(blockObject, isTrigger);
		}
		return blockObject;
	}

	private void AssignCollider(GameObject blockObject, bool isTrigger)
	{
		var bc = blockObject.GetComponent<BoxCollider2D>();
		if (!bc)
		{
			bc = blockObject.AddComponent<BoxCollider2D>();
		}
		bc.isTrigger = isTrigger;
	}

	public void DestroyTile(GameObject block)
	{
		var tileCoord = GetTileCoord(block.transform.position);
		if (HasBlockAtCoord(tileCoord))
		{
			foreach (Drop drop in GetBlockDrop(tileCoord))
			{
				if (drop.DropChanceSucess())
				{
					drop.Instantiate(block.transform.position);
				}
			}
		}

		if (block.name == "grass")
		{
			// TODO: даже знать не хочу что тут происходит
			var upCoord = new Vector2Int(tileCoord.x, tileCoord.y + 1);
			if (HasBlockAtCoord(upCoord))
			{
				var grass = blockObjects[upCoord.x, upCoord.y];
				if (grass.CompareTag("tall_grass"))
				{
					Object.Destroy(grass); //with tall_grass
					ToMoveBack(block);
					RemoveBlock(upCoord, transparent: true);
				}
			}
			else
			{
				ToMoveBack(block);
				RemoveBlock(tileCoord, transparent: true); //without tall_grass
			}
		}
		else
		{
			if (block.CompareTag("tall_grass"))
			{
				Object.Destroy(block);
			}

			ToMoveBack(block);
			RemoveBlock(tileCoord, transparent: false);
		}
		//TODO: блоки смещенные на задний план не должны попадать в пул
		//blockManager.PushToPool(block);
	}

	private void ToMoveBack(GameObject block)
	{
		block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, 1);
		BoxCollider2D bc = block.GetComponent<BoxCollider2D>();
		if (bc)
		{
			bc.isTrigger = true;
		}
		SpriteRenderer srBG = block.GetComponent<SpriteRenderer>();
		srBG.material = blockManager.backgroundMaterial;
		block.layer = 8;
	}

	public void Destroy()
	{
		//TODO убирать в память чанки и обьекты на них
		for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
		{
			for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
			{
				var obj = blockObjects[x, y];
				if (obj != null)
				{
					var block = blockObjects[x, y];
					blockManager.PushToPool(block);
					var bc = block.GetComponent<BoxCollider2D>();
					if (bc)
					{
						Object.Destroy(bc);
					}
					block.SetActive(false);
				}
				blocks[x, y] = null;
			}
		}
		//TODO: пока удаляем, после надо думать как организовать хранение в памяти, а после в файле
		if (Application.isPlaying)
			Object.Destroy(gameObject);
#if UNITY_EDITOR
		else
			Object.DestroyImmediate(gameObject);
#endif
	}
}