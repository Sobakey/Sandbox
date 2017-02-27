//#define SPAWN_ONLY_SURFACE

using Sandbox;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour
{
    public LayerMask ObstacleLayer;
    public GameObject player;
    public static int chunkHeight = 64;
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
            var playerPos = ChunkPosToWorldPos(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
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
            }
        }
        if (isRandomSeed)
        {
            seed = Random.Range(-5000, 5000);
        }
        visibleChunks = new Dictionary<Vector2Int, Chunk>();

        perlinNoizeGenerator = new PerlinNoizeGenerator(new Vector2Int(seed, seed), worldHeight, scale, octaves, persistance, lacunarity);
        if (Application.isPlaying)
        {
            var playerX = Chunk.size / 2;
            player.transform.position = ChunkPosToWorldPos(playerX, perlinNoizeGenerator.GetHeight(playerX) + 1);
        }
    }

    private GameObject SpawnPlayer(Vector2 pos)
    {
        GameObject player_object = GameObject.Instantiate(player, pos, Quaternion.identity) as GameObject;
        return player_object;
    }


    private void SpawnBlocks(Chunk chunk)
    {
        GameObject parentBlocks = new GameObject();

        for (int x = 0; x < Chunk.size; x++)
        {
            for (int y = 0; y < Chunk.size; y++)
            {
                if (chunk.blocks[x, y] != null)
                {
                    GameObject block_GameObject = new GameObject();
                    block_GameObject.transform.SetParent(parentBlocks.transform);
                    //parentBlocks.name = "chunk " + ChunkPosToWorldPos(x, y, chunk.position);
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = chunk.blocks[x, y].GetSprite();
                    block_GameObject.name = chunk.blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3((chunk.coords.x * Chunk.size) + x, (chunk.coords.y * Chunk.size) + y);
                    sr.material = mat;
                    block_GameObject.layer = 13;//TODO: Хардкод для слоя препядствий
                    BoxCollider2D bc = block_GameObject.AddComponent<BoxCollider2D>();
                    chunk.blockObjects[x, y] = block_GameObject;

                    if (chunk.blocks[x, y].isSolid != true)
                    {
                        bc.isTrigger = true;
                        block_GameObject.tag = "tall_grass";
                    }
                }
            }
        }
    }

    public void RecalculateChunk(Chunk chunk)
    {
        for (int x = 0; x < Chunk.size; x++)
        {
            for (int y = 0; y < Chunk.size; y++)
            {
                if (chunk.blocks[x, y] != null && chunk.blockObjects[x, y] == null)
                {
                    GameObject block_GameObject = new GameObject();
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = chunk.blocks[x, y].GetSprite();
                    block_GameObject.name = chunk.blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3((chunk.coords.x * Chunk.size) + x, y);
                    sr.material = mat;

                    BoxCollider2D bc = block_GameObject.AddComponent<BoxCollider2D>();
                    chunk.blockObjects[x, y] = block_GameObject;

                    if (chunk.blocks[x, y].isSolid != true)
                    {
                        bc.isTrigger = true;
                        block_GameObject.tag = "tall_grass";
                    }
                }
                else if (chunk.blocks[x, y] == null && chunk.blockObjects[x, y] != null)
                {
                    // GameObject.Destroy(chunk.blockObjects[x,y]);
                }

            }
        }
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

    public Vector2 ChunkPosToWorldPos(int x, int y)
    {
        int xPos = Mathf.FloorToInt(x);
        int yPos = Mathf.FloorToInt(y);

        return new Vector2(xPos, yPos);
    }

    private void ToMoveBack(GameObject block)
    {
        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, block.transform.position.z + 1);
        BoxCollider2D bc = block.GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
        // bc.enabled = false;
        SpriteRenderer srBG = block.GetComponent<SpriteRenderer>();
        srBG.material = matBG;
        block.layer = 8;
    }

    public void DestroyBlock(GameObject block, GameObject block_down)
    {
        Vector3 blockPos = block.transform.position;
        Vector2Int chunkPos = Chunk.GetChunkCoordAtPos(blockPos);

        // SpriteRenderer sr = block_down.GetComponent<SpriteRenderer>();
        //   sr.material = mat;

        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;

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
                    dropObject.layer = 10;
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
            if (block.gameObject.tag == "tall_grass")
            {
                GameObject.Destroy(block);
            }
        }
    }

    public void PlaceBlock(Block block, Vector3 pos/*, GameObject go*/)
    {
        Chunk chunk = ChunkAtPos(pos.ToInt());
        Vector2Int chunkPos = Chunk.GetChunkCoordAtPos(pos);

        //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        chunk.blocks[(int)chunkPos.x, (int)chunkPos.y] = block;
        RecalculateChunk(chunk);
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
                    newChunk.GenerateBlocks(perlinNoizeGenerator);
                    if (!newChunk.IsEmpty)
                    {
                        SpawnBlocks(newChunk);
                        visibleChunks.Add(pos, newChunk);
                    }
                }
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
