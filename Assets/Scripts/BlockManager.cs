using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
	//Хитровыебаный способ передать из скрипта в статическую переменную материал
	public Material material;
	private static Material _material;

	public Block[] blocks;
	private Dictionary<int, Block> blocksCache = new Dictionary<int, Block>();
	private Dictionary<string, int> blocksNameCache = new Dictionary<string, int>();

	private static Queue<GameObject> blocksPool = new Queue<GameObject>();

	private static GameObject _blockObjectPool;
	private static GameObject blockObjectsPool
	{
		get { return _blockObjectPool ?? (_blockObjectPool = new GameObject()); }
	}

	void Start()
	{
		_material = material;
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


	public Block FindBlock(string name)
	{
		int id;
		if (blocksNameCache.TryGetValue(name, out id))
		{
			return FindBlock(id);
		}
		return null;
	}

	public static GameObject GetBlock(Transform parent, Sprite sprite, Vector3 position, string displayName,
		string tag, bool isTrigger)
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
		blockObject.transform.SetParent(parent);
		var sr = cached ? blockObject.GetComponent<SpriteRenderer>() : blockObject.AddComponent<SpriteRenderer>();
		sr.sprite = sprite;
		blockObject.name = displayName;
		blockObject.tag = tag;
		blockObject.transform.position = position;
		sr.material = _material;
		blockObject.layer = 13; //TODO: Хардкод для слоя препядствий
		var bc = cached ? blockObject.GetComponent<BoxCollider2D>() : blockObject.AddComponent<BoxCollider2D>();
		bc.isTrigger = isTrigger;
		return blockObject;
	}

	public static void DestroyBlock(GameObject block)
	{
		blocksPool.Enqueue(block);
		block.transform.SetParent(blockObjectsPool.transform);
		block.SetActive(false);
	}
}