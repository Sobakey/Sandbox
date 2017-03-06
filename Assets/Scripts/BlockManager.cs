using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
	public Material mainMaterial;
	public Material backgroundMaterial;
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

	public void CreateTileAtPos(int blockId, Vector2 pos)
	{
		Chunk chunk;
		if (ChunkManager.Instance.TryGetChunk(pos.ToInt(), out chunk))
		{
			var block = FindBlock(blockId);
			chunk.CreateTileAtPos(block, pos);
		}
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

	public GameObject GetBlockObject()
	{
		GameObject blockObject;
		if (blocksPool.Count != 0)
		{
			blockObject = blocksPool.Dequeue();
			blockObject.SetActive(true);
		}
		else
		{
			blockObject = new GameObject();
			blockObject.AddComponent<SpriteRenderer>();
		}
		return blockObject;
	}


	public void PushToPool(GameObject block)
	{
		blocksPool.Enqueue(block);
		block.transform.parent = blockObjectsPool.transform;
	}
}