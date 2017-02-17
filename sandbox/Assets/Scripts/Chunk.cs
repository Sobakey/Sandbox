using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk  {

    public static int size = 32;
    public int position;
    public Block[,] blocks;
    public float pMod = 0.05f;
    public float pHeightMod = 10f;
    public float heightMod = 20f;
    public GameObject[,] blockObjects;

    private static float heightModifier = 20f;
    private BlockManager blockManager;

    public Chunk(BlockManager blockManager, int position)
    {
        this.blockManager = blockManager;
        this.position = position;
        blocks = new Block[size, WorldGenerator.chunkHeight];
        blockObjects = new GameObject[size, WorldGenerator.chunkHeight];
    }

    public void GenerateBlocks()
    {
        float seed = 0;// Random.Range(0.1f, 30.9f);

        for (int x = 0; x < size; x++)
        {
            float pValue = Mathf.PerlinNoise((position*size + x) * pMod + seed, 5 * pMod + seed);
            int pHeight = Mathf.RoundToInt(pValue * pHeightMod + heightMod);

            for (int y = 0; y < WorldGenerator.chunkHeight; y++)
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
                    else if (y < pHeight - Random.Range(4, 16) || y > pHeight - 1)
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
           
        }
    }

    public void Destroy()
    {      //TODO destroy parent object
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < WorldGenerator.chunkHeight; y++)
            {
                blocks[x, y] = null;
                GameObject.Destroy(blockObjects[x, y]);
            }
        }
        //GameObject.Destroy(blockObjects[size - 1, WorldGenerator.chunkHeight - 1].transform.parent.gameObject);

    }

}
