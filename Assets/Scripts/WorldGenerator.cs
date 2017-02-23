﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public LayerMask ObstacleLayer;
    public GameObject player;
    public static int chunkHeight = 64;
    //Количиство чанков которые должны присутствовать на сцене одновременно
    public int viewDistance = 3;
	public Material mat;
	public Material matDark;
    public Material matBG;

    private BlockManager blockManager;
    private Dictionary<int, Chunk> chunks;
    private ItemDatabase itemDatabase;

	void Start () {
        blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
        itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
        chunks = new Dictionary<int, Chunk>();
        player = SpawnPlayer(Chunk.size/2, chunkHeight/2 + 2);
	}
	
    private GameObject SpawnPlayer(float x , float y)
    {
       GameObject player_object =  GameObject.Instantiate(player,new Vector3(x,y),Quaternion.identity) as GameObject;
        return player_object;
    }

    private void SpawnBlocks(Chunk chunk)
    {
        GameObject parentBlocks = new GameObject();
            
        for (int x = 0; x < Chunk.size; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                if (chunk.blocks[x,y]!=null)
                {
                    GameObject block_GameObject = new GameObject();
                    block_GameObject.transform.SetParent(parentBlocks.transform);
                    parentBlocks.name = "chunk " + ChunkPosToWorldPos(x,y,chunk.position);
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = chunk.blocks[x, y].GetSprite();
                    block_GameObject.name = chunk.blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3((chunk.position*Chunk.size)+x, y);
					sr.material = mat;
                    block_GameObject.layer = 13;//TODO: Хардкод для слоя препядствий
                    BoxCollider2D bc = block_GameObject.AddComponent<BoxCollider2D>();
                    chunk.blockObjects[x, y] = block_GameObject;

                    if (chunk.blocks[x,y].isSolid != true)
                    {                   
                        bc.isTrigger = true;
                        block_GameObject.tag = "tall_grass";
                    }
                    
                }
              
            }
        }
    }

    public void UpdateChunk(Chunk chunk)
    {
      //  GameObject parentBlocks = new GameObject();

        for (int x = 0; x < Chunk.size; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                if (chunk.blocks[x,y] != null && chunk.blockObjects[x,y] == null)
                {
                    GameObject block_GameObject = new GameObject();
                   // block_GameObject.transform.SetParent(parentBlocks.transform);
                   // parentBlocks.name = "chunk " + ChunkPosToWorldPos(x, y, chunk.position);
                    SpriteRenderer sr = block_GameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = chunk.blocks[x, y].GetSprite();
                    block_GameObject.name = chunk.blocks[x, y].display_name;
                    block_GameObject.tag = "Block";
                    block_GameObject.transform.position = new Vector3((chunk.position * Chunk.size) + x, y);
                    sr.material = mat;


                    //if (chunk.blocks[x, y - 1] != null && chunk.blocks[x, y - 1].isSolid)
                    //{
                    //    sr.material = matDark;
                    //}

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
                    GameObject.Destroy(chunk.blockObjects[x,y]);
                }

            }
        }
    }

    private Chunk ChunkAtPos(float x)
    {
        int chunkIndex = Mathf.FloorToInt(x / Chunk.size);

        Chunk chunk;
        if(chunks.TryGetValue(chunkIndex, out chunk)){
            return chunk;
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

    private void ToMoveBack(GameObject block)
    {
        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, block.transform.position.z + 1);
        BoxCollider2D bc = block.GetComponent<BoxCollider2D>();
        bc.enabled = false;
        SpriteRenderer srBG = block.GetComponent<SpriteRenderer>();
        srBG.material = matBG;
        block.layer = 0;
    }

    public void DestroyBlock(GameObject block, GameObject block_down)
    {      
        Vector3 blockPos = block.transform.position;
        Vector2 chunkPos = WorldPosToChunkPos(blockPos.x, blockPos.y);

        SpriteRenderer sr = block_down.GetComponent<SpriteRenderer>();
        sr.material = mat;

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
                        
                        GameObject.Destroy(grass); //with tall_grass
                        ToMoveBack(block);
                        chunk.blocks[x, y+1] = blockManager.FindBlock(0);
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

    public void PlaceBlock(Block block, Vector3 pos, GameObject go)
    {
        Chunk chunk = ChunkAtPos(pos.x);
        Vector2 chunkPos = WorldPosToChunkPos(pos.x, pos.y);

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.material = matDark;


        chunk.blocks[(int)chunkPos.x, (int)chunkPos.y] = block;
        UpdateChunk(chunk);
    }

    //Метод потребляет огромное количество времени, а смысл его не ясен
    void Update () 
    {
        int playerChunk = Chunk.GetChunkIndexAtPos(player.transform.position);

        for (int i = playerChunk - viewDistance; i < playerChunk + viewDistance; i++)
        {
            if(!chunks.ContainsKey(i))
            {
                Chunk newChunk = new Chunk(blockManager, i);
                newChunk.GenerateBlocks();
                SpawnBlocks(newChunk);
                chunks.Add(newChunk.position, newChunk);
            }
        }
  
        foreach (Chunk chunk in chunks.Values)
        {
            if (chunk.position < playerChunk - viewDistance || chunk.position > playerChunk + viewDistance)
            {
                chunk.Destroy();
                chunks.Remove(chunk.position);
                break;
            }
        }
	}
}
