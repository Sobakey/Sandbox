using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public GameObject player;
    public int chunkSize = 64;
    public float pMod = 0.05f;
    public float pHeightMod = 10f;
    public float heightMod = 20f;

    private BlockManager blockManager;
    private Block[,] blocks;

	void Start () {
        blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
        blocks = new Block[chunkSize, chunkSize];

        GenerateBlocks();
        SpawnBlocks();
	}
	
    private void GenerateBlocks()
    {
        float seed = Random.Range(0.1f,1.9f);

        int playerSpawnX = Random.Range(0,chunkSize);
        for (int x = 0; x < chunkSize; x++)
        {
            float pValue = Mathf.PerlinNoise(x* pMod + seed, 5*pMod + seed);
            int pHeight = Mathf.RoundToInt(pValue * pHeightMod + heightMod);

            for (int y = 0; y < chunkSize; y++)
            {
                if (y <= pHeight)
                {
                    if (y == pHeight - 1)
                    {
                        blocks[x, y] = blockManager.FindBlock(1); //grass
                    }
                    else if (y == pHeight)
                    {
                        if (Random.value < 0.4f)
                        {
                          blocks[x, y] = blockManager.FindBlock(4); //tall_grass
                        }
                    }
                    else if (y < pHeight - Random.Range(4,16) || y > pHeight - 1)
                    {
                        blocks[x, y] = blockManager.FindBlock(3); //dirt
                    }
                    else
                    {
                        blocks[x, y] = blockManager.FindBlock(2); //stone
                    }
                    
                }
                else
                {
                    blocks[x, y] = blockManager.FindBlock(0);
                }
            }
            if (x == playerSpawnX)
            {
                SpawnPlayer(x,pHeight +1);
            }
        }
    }

    private void SpawnPlayer(float x , float y)
    {
       GameObject player_object =  GameObject.Instantiate(player,new Vector3(x,y),Quaternion.identity) as GameObject;
    }

    private void SpawnBlocks()
    {
        GameObject parentBlocks = new GameObject("blocks");
            
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                if (blocks[x,y]!=null)
                {
                    GameObject block_GameObject = new GameObject();

                    block_GameObject.transform.parent = parentBlocks.transform;
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = blocks[x, y].sprite;
                    block_GameObject.name = blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3(x, y);
                    if (blocks[x,y].isSolid == true)
                    {
                    BoxCollider2D bcs = block_GameObject.AddComponent<BoxCollider2D>();
                    }
                    else
                    {
                    BoxCollider2D bc = block_GameObject.AddComponent<BoxCollider2D>();
                    bc.isTrigger = true;

                    }
                }
              
            }
        }
    }

  public void DestroyBlock(GameObject block)
    {
        int x = (int)block.transform.position.x;
        int y = (int)block.transform.position.y;
        GameObject tallGrass = GameObject.Find("tall_grass");


        if (block.name == "grass")
        {
          

            if (blocks[x,y+1] != null )
            {
                Debug.Log("с травой");
                GameObject.Destroy(GameObject.Find("tall_grass"));
                blocks[x, y+1] = blockManager.FindBlock(0);
               //TODO удалить блок с травой
                GameObject.Destroy(block);
            }
            else
            {
                Debug.Log("без травы");
                blocks[x, y] = blockManager.FindBlock(0);
                GameObject.Destroy(block);
            }
    
        }
        else
        {
        blocks[x, y] = blockManager.FindBlock(0);
        GameObject.Destroy(block);
        Debug.Log("иное");

        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
