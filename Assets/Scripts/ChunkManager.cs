using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sandbox
{
	public class ChunkManager
	{
		public static int size = 32;
		private static ChunkManager instance;
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
			return new Vector2Int(Mathf.FloorToInt(pos.x / size), Mathf.FloorToInt(pos.y / size));
		}

		public Vector2Int GetTilePositionAtPos(Vector3 pos)
		{
			var v = GetChunkCoordAtPos(pos);
			v.x += Mathf.FloorToInt((pos.x % Chunk.size));
			v.y += Mathf.FloorToInt((pos.y % Chunk.size));
			return v;
		}

		public GameObject GetTileAtPos(Vector2 pos)
		{
			Chunk chunk;
			if (TryGetChunk(GetChunkCoordAtPos(pos), out chunk))
			{
				return chunk.blockObjects[Mathf.FloorToInt((pos.x % Chunk.size)), Mathf.FloorToInt((pos.y % Chunk.size))];
			}
			return null;
		}

		public void AddChunk(Chunk chunk)
		{
			chunks.Add(chunk.coords, chunk);
		}

		public bool HasChunkAtPos(Vector2Int pos)
		{
			return chunks.ContainsKey(pos);
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
}