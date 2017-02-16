using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public GameObject player;
    public static int chunkHeight = 64;
    public int viewDistance = 3;

    private BlockManager blockManager;
    private List<Chunk> chunks;
    private ItemDatabase itemDatabase;

	void Start () {
        blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
        itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
        chunks = new List<Chunk>();
        player = SpawnPlayer(Chunk.size/2, chunkHeight/2 + 2);
	}
	
   

    private GameObject SpawnPlayer(float x , float y)
    {
       GameObject player_object =  GameObject.Instantiate(player,new Vector3(x,y),Quaternion.identity) as GameObject;
        return player_object;
    }

    private void SpawnBlocks(Chunk chunk)
    {
        GameObject parentBlocks = new GameObject("blocks");
            
        for (int x = 0; x < Chunk.size; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                if (chunk.blocks[x,y]!=null)
                {
                    GameObject block_GameObject = new GameObject();

                    block_GameObject.transform.parent = parentBlocks.transform;
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = chunk.blocks[x, y].sprite;
                    block_GameObject.name = chunk.blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3((chunk.position*Chunk.size)+x, y);
                    chunk.blockObjects[x, y] = block_GameObject;
                    if (chunk.blocks[x,y].isSolid == true)
                    {
                    BoxCollider2D bcs = block_GameObject.AddComponent<BoxCollider2D>();
                    }
                    else
                    {
                    BoxCollider2D bc = block_GameObject.AddComponent<BoxCollider2D>();
                    bc.isTrigger = true;
                    block_GameObject.tag = "tall_grass";
                    }
                    
                }
              
            }
        }
    }

    private Chunk ChunkAtPos(float x)
    {
        int chunkIndex = Mathf.FloorToInt(x / Chunk.size);

        foreach (Chunk chunk in chunks)
        {
            if (chunk.position == chunkIndex)
            {
                return chunk;
            }
        }
         
        return null;
    }

    public Vector2 WorldPosToChunkPos(float x, float y)
    {
        int xPos = Mathf.RoundToInt(x - (ChunkAtPos(x).position * Chunk.size));
        int yPos = Mathf.RoundToInt(y);

        return new Vector2(xPos, yPos);
    }

    public Vector2 ChunkPosToWorldPos(int x, int y, int chunkPos)
    {
        int xPos = Mathf.FloorToInt(x + (chunkPos * Chunk.size));
        int yPos = Mathf.FloorToInt(y);

        return new Vector2(xPos, yPos);
    }

    public void DestroyBlock(GameObject block)
    {
        Vector3 blockPos = block.transform.position;
        Vector2 chunkPos = WorldPosToChunkPos(blockPos.x, blockPos.y);

        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;

        Chunk chunk = ChunkAtPos(blockPos.x);

        foreach (Drop drop in chunk.blocks[x,y].drops)
        {
            if (drop.DropChanceSucess())
            {
                GameObject dropObject = new GameObject();
                dropObject.transform.position = block.transform.position;
                dropObject.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                dropObject.AddComponent<SpriteRenderer>().sprite = itemDatabase.FindItem(drop.itemName).sprite;
                dropObject.AddComponent<PolygonCollider2D>();
                dropObject.AddComponent<Rigidbody2D>();
                dropObject.layer = 10;
                dropObject.AddComponent<Magnetism>().target = GameObject.FindWithTag("player").transform;
                dropObject.name = drop.itemName;
            }
        }

        if (block.name == "grass")
        {
            GameObject[] tallGrass = GameObject.FindGameObjectsWithTag("tall_grass");

            if (chunk.blocks[x,y+1] != null )
            {
                foreach (GameObject grass in tallGrass)
                {
                    if (grass.transform.position.x == block.transform.position.x)
                    {
                        Debug.Log("с травой");
                        GameObject.Destroy(grass);
                        chunk.blocks[x, y+1] = blockManager.FindBlock(0);
                        GameObject.Destroy(block);
                    }

                }
                
            }
            else
            {
                Debug.Log("без травы");
                chunk.blocks[x, y] = blockManager.FindBlock(0);
                GameObject.Destroy(block);
            }
    
        }
        else
        {
        chunk.blocks[x, y] = blockManager.FindBlock(0);
        GameObject.Destroy(block);
        Debug.Log("иное");
         
        }
    }

    void Update () {
        //TODO fix bug with chunk generation on view distance
        int playerChunk = Mathf.FloorToInt(player.transform.position.x/(Chunk.size/2));

        for (int i = playerChunk - viewDistance; i < playerChunk +viewDistance; i++)
        {
            bool spawn = true;
            foreach (Chunk chunk in chunks)
            {
                if (chunk.position == i)
                {
                    spawn = false;
                }
            }

            if (spawn)
            {
                Chunk newChunk = new Chunk(blockManager, i);
                newChunk.GenerateBlocks();
                SpawnBlocks(newChunk);
                chunks.Add(newChunk);
            }
        }

        foreach (Chunk chunk in chunks)
        {
            if (chunk.position < playerChunk - viewDistance || chunk.position > playerChunk + viewDistance)
            {
                chunk.Destroy();
                chunks.Remove(chunk);
                break;
            }
        }
	}
}
