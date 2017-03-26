using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
	public Material mainMaterial;
	public Material backgroundMaterial;
	public Block[] blocks;
	private Block[] blockAccessor;

	private static Queue<GameObject> blocksPool = new Queue<GameObject>();

	public Queue<Func<GameObject>> tileCreateQueue = new Queue<Func<GameObject>>();

	private static GameObject _blockObjectPool;

	private static GameObject blockObjectsPool
	{
		get { return _blockObjectPool ?? (_blockObjectPool = new GameObject()); }
	}

	void Start()
	{
		blockAccessor = new Block[blocks.Max(x=>x.id) + 1];

		foreach (var item in blocks)
		{
			blockAccessor[item.id] = item;
		}
	}

	void Update()
	{
		PerformMainQueue();
	}

	public void QueueTileToMainThread(RequestedTile rTile)
	{
		lock (mainQueue)
		{
			mainQueue.Enqueue(rTile);
		}
	}
	private Queue<RequestedTile> mainQueue = new Queue<RequestedTile>();
	private void PerformMainQueue()
	{
		lock (mainQueue)
		{
			var counter = 0;
			while (mainQueue.Count > 0 && counter < ChunkManager.CHUNK_SIZE * 9)
			{
				var rTile = mainQueue.Dequeue();
				PerformTileCreation(rTile);
				counter++;
			}
		}
	}

	public void PerformTileCreation(RequestedTile rTile)
	{
		GameObject blockObject = GetBlockObject();
		blockObject.transform.parent = rTile.transform;
		var sr = blockObject.GetComponent<SpriteRenderer>();
		sr.sprite = rTile.sprite;
		blockObject.name = rTile.name;
		blockObject.tag = rTile.tag;
		blockObject.transform.position = rTile.position;
		sr.material = mainMaterial;
		blockObject.layer = 13; //TODO: Хардкод для слоя препядствий света
		if (rTile.isOpenBlock)
		{
			AssignCollider(blockObject, rTile.isTrigger);
		}
		rTile.chunk.AssignBlockObject(blockObject, rTile.pos);
	}

	public void AssignCollider(GameObject blockObject, bool isTrigger)
	{
		var bc = blockObject.GetComponent<BoxCollider2D>();
		if (!bc)
		{
			bc = blockObject.AddComponent<BoxCollider2D>();
		}
		bc.isTrigger = isTrigger;
	}

	//Это очень часто вызываемая функция, дополнительные проверки вызывают проблемы с производительностью.
	//Мы должны считать что программист не передает сюда некорректные значения!
	public Block FindBlock(int id)
	{
		return blockAccessor[id];
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