using System;
using Sandbox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Chunk
{
	public Block[,] blocks;
	public GameObject[,] blockObjects;
	private BlockManager blockManager;

	public GameObject gameObject;
	public Vector2Int coords;

	public bool IsEmpty { get; private set; }


	public Vector2 GetWorldPos()
	{
		return new Vector2(coords.x, coords.y);
	}

	public Vector2Int GetTileCoord(Vector2 pos)
	{
		return new Vector2Int((int)((ChunkManager.CHUNK_SIZE + (pos.x % ChunkManager.CHUNK_SIZE)) % ChunkManager.CHUNK_SIZE),
			(int)((ChunkManager.CHUNK_SIZE + (pos.y % ChunkManager.CHUNK_SIZE)) % ChunkManager.CHUNK_SIZE));
	}

	public Chunk(BlockManager blockManager, Vector2Int position)
	{
		IsEmpty = true;
		this.blockManager = blockManager;
		this.coords = position;
		blocks = new Block[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];
		blockObjects = new GameObject[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];
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
					if (absoluteY == pHeight - 1 && absoluteY < 100)
					{
						blocks[x, y] = blockManager.FindBlock(1); //grass
					}
					if ((absoluteY == pHeight || absoluteY == pHeight - 1) && absoluteY > 250)
					{
						blocks[x, y] = blockManager.FindBlock(5); //snow
					}
					else if (absoluteY == pHeight && absoluteY < 100)
					{
						if (Random.value < 0.4f)
						{
							blocks[x, y] = blockManager.FindBlock(4); //tall_grass
						}
					}
					else if (absoluteY < pHeight - Random.Range(4, 16) || absoluteY > pHeight - 1 || absoluteY > 100)
					{
						blocks[x, y] = blockManager.FindBlock(3); //stone
					}
					else
					{
						blocks[x, y] = blockManager.FindBlock(2); //dirt
					}
					IsEmpty = false;
				}
				else
				{
					blocks[x, y] = blockManager.FindBlock(0);
				}
			}
		}
	}

	public void GenerateTiles()
	{
		for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
		{
			for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
			{
				if (blocks[x, y] != null)
				{
					blockObjects[x,y] = blockManager.CreateTile(new Vector2Int(x,y), this);
				}
			}
		}
	}

	public void CreateTileAtPos(Block block, Vector2 pos)
	{
		Vector2Int tilePos = blockManager.GetTilePositionAtPos(pos);
		blocks[tilePos.x, tilePos.y] = block;
		blockManager.CreateTile(tilePos, this);
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
					blockManager.Dispose(blockObjects[x, y]);
				}
				blocks[x, y] = null;
			}
		}
		//пока удаляем, после надо думать как организовать хранение в памяти, а после в файле
		if (Application.isPlaying)
			Object.Destroy(gameObject);
#if UNITY_EDITOR
		else
			Object.DestroyImmediate(gameObject);
#endif
	}
}