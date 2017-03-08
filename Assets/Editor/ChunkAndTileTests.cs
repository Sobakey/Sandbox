using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[TestFixture]
public class ChunkAndTileTests
{
	private GameObject gameObject;
	private BlockManager blockManager;
	private PerlinNoizeGenerator perlinNoizeGenerator;


	[SetUp]
	public void SetUp()
	{
		ChunkManager.Instance.Reset();
		gameObject = new GameObject();
		blockManager = gameObject.AddComponent<BlockManager>();
		perlinNoizeGenerator = new PerlinNoizeGenerator(new Vector2Int(0, 0), 10, 1, 1, 1, 1);
	}

	[Test]
	public void ChunkFindByCoordsTest()
	{
		var pos = new Vector2Int(0, 0);
		var chunk = new Chunk(blockManager, perlinNoizeGenerator, pos);
		ChunkManager.Instance.AddChunk(chunk);

		Assert.IsTrue(ChunkManager.Instance.HasChunkAtPos(pos));
	}


	[Test]
	public void TileByWorldPos(
		[NUnit.Framework.Range(0, ChunkManager.CHUNK_SIZE - 1, 1)] int x,
		[NUnit.Framework.Range(0, ChunkManager.CHUNK_SIZE - 1, 1)] int y,
		[NUnit.Framework.Values(0, -1)] int chunkPosX,
		[NUnit.Framework.Values(0, -1)] int chunkPosY,
		[NUnit.Framework.Values(1, -1)]	int mul)
	{
		var sign = Mathf.Sign(mul);
		var chunk = new Chunk(blockManager, perlinNoizeGenerator, new Vector2Int(chunkPosX, chunkPosY));
		var targetCoord = new Vector2Int(ChunkManager.CHUNK_SIZE - (x + 1), ChunkManager.CHUNK_SIZE - (y + 1));
		if (mul > 0)
		{
			targetCoord = new Vector2Int(x, y);
		}

		var resString = new Func<Vector2, string>((p) =>  string.Format("World pos is {0}", p));
		var xRand = UnityEngine.Random.Range(.1f, .99f);
		var yRand = UnityEngine.Random.Range(.1f, .99f);

		var fPos = new Vector2((float) (sign * x + sign * xRand), (float) (sign * y + sign * yRand));
		var coord = chunk.GetTileCoord(fPos);
		Assert.AreEqual(targetCoord, coord, resString(fPos));
	}
}