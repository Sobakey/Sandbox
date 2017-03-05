#define DRAW_GIZMOS
#undef DRAW_GIZMOS

using UnityEngine;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour
{
	public GameObject player;

	//Количиство чанков вокруг игрока, которые должны присутствовать на сцене одновременно
	public int viewDistance = 3;

	public int seed;
	public bool isRandomSeed = true;
	public int worldHeight = 255;
	public float scale = 1.0f;
	public int octaves = 1;
	public float persistance = 1.0f;
	public float lacunarity = 1.0f;
	private BlockManager blockManager;

	private PerlinNoizeGenerator perlinNoizeGenerator;

	void Start()
	{
		blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
		Rebuild();
		if (Application.isPlaying)
		{
			var playerX = ChunkManager.CHUNK_SIZE / 2;
			var playerPos = new Vector2(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
			player = SpawnPlayer(playerPos);
		}
	}

	public void Rebuild()
	{
		if (isRandomSeed)
		{
			seed = Random.Range(-5000, 5000);
		}
		ChunkManager.Instance.Reset();

		perlinNoizeGenerator = new PerlinNoizeGenerator(new Vector2Int(seed, seed), worldHeight, scale, octaves,
			persistance, lacunarity);
		if (Application.isPlaying)
		{
			var playerX = ChunkManager.CHUNK_SIZE / 2;
			player.transform.position = new Vector2(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
		}
	}

	private GameObject SpawnPlayer(Vector2 pos)
	{
		GameObject player_object = GameObject.Instantiate(player, pos, Quaternion.identity) as GameObject;
		return player_object;
	}

	void Update()
	{
		if (Application.isPlaying)
		{
			ChunkManager.Instance.UpdateChunks(blockManager, perlinNoizeGenerator, player.transform.position,
				viewDistance);
		}
	}

	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
#if DRAW_GIZMOS
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
#endif
	}
}