using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ChunkManager
{
	public const int CHUNK_SIZE = 32;
	private static ChunkManager instance;

	public int GenerateDistance = 1;

	public static ChunkManager Instance
	{
		get { return instance ?? (instance = new ChunkManager()); }
	}

	private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

	private ChunkManager()
	{
	}

	public bool TryGetChunk(Vector2Int pos, out Chunk chunk)
	{
		return chunks.TryGetValue(pos, out chunk);
	}

	public bool TryGetChunk(Vector2 pos, out Chunk chunk)
	{
		return chunks.TryGetValue(GetChunkCoordAtPos(pos), out chunk);
	}

	/// <summary>
	///  Копия коллекции чанков, изменение коллекции не меняет саму коллекцию
	/// Изменение чанка меняет чанк
	/// </summary>
	public Chunk[] Chunks
	{
		get { return chunks.Values.ToArray(); }
	}

	public Vector2Int GetChunkCoordAtPos(Vector3 pos)
	{
		return new Vector2Int(Mathf.FloorToInt(pos.x / CHUNK_SIZE), Mathf.FloorToInt(pos.y / CHUNK_SIZE));
	}


	public void AddChunk(Chunk chunk)
	{
		chunks.Add(chunk.coords, chunk);
	}

	public bool HasChunkAtPos(Vector2Int pos)
	{
		return chunks.ContainsKey(pos);
	}

	public void UpdateChunks(BlockManager blockManager, PerlinNoizeGenerator perlinNoizeGenerator,
		Vector2 playerPosition)
	{
		var playerPos = GetChunkCoordAtPos(playerPosition);

		for (int x = playerPos.x - GenerateDistance; x <= playerPos.x + GenerateDistance; x++)
		{
			for (int y = playerPos.y - GenerateDistance; y <= playerPos.y + GenerateDistance; y++)
			{
				var pos = new Vector2Int(x, y);
				if (!HasChunkAtPos(pos))
				{
					Chunk newChunk = new Chunk(blockManager, perlinNoizeGenerator, pos);
					newChunk.Construct();
					AddChunk(newChunk);
				}
			}
		}

		foreach (Chunk chunk in Chunks)
		{
			var xDiff = Mathf.Abs(Mathf.Abs(chunk.coords.x) - Mathf.Abs(playerPos.x));
			var yDiff = Mathf.Abs(Mathf.Abs(chunk.coords.y) - Mathf.Abs(playerPos.y));
			if (xDiff > GenerateDistance || yDiff > GenerateDistance)
			{
				chunk.Destroy();
				RemoveChunk(chunk);
			}
		}
	}

	public void RemoveChunk(Chunk chunk)
	{
		chunks.Remove(chunk.coords);
	}

	public void RemoveChunk(Vector2Int pos)
	{
		chunks.Remove(pos);
	}

	public void Reset()
	{
		Clear();
		chunks.Clear();
	}

	public void Clear()
	{
		if (chunks != null)
		{
			foreach (var key in chunks.Keys.ToArray())
			{
				chunks[key].Destroy();
				chunks.Remove(key);
			}
		}
	}
}