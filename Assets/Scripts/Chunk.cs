using Sandbox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk  {

    public static int size = 32;
    public Block[,] blocks;
    public GameObject[,] blockObjects;

    private static float heightModifier = 20f;
    private BlockManager blockManager;

    public Vector2Int coords;
  
    //TODO: Код реализован только для горизонтальной оси
    public static Vector2Int GetChunkCoordAtPos(Vector3 pos){
        return new Vector2Int(Mathf.FloorToInt(pos.x / Chunk.size), Mathf.FloorToInt(pos.y / Chunk.size));
    }

    public Chunk(BlockManager blockManager, Vector2Int position)
    {
        this.blockManager = blockManager;
        this.coords = position;
        blocks = new Block[size, size];
        blockObjects = new GameObject[size, size];
    }

    public void GenerateBlocks(int seed, float pMod, float pHeightMod, float heightMod)
    {  //TODO tune seed

        var rnd = new System.Random(seed);
        var offsetX = rnd.Next(-5000, 5000);

        for (int x = 0; x < size; x++)
        {
            float pValue = Mathf.PerlinNoise((coords.x*size + x) * pMod + offsetX, 5 * pMod + (size * coords.y));
            int pHeight = Mathf.RoundToInt(pValue * pHeightMod + heightMod);

            for (int y = 0; y < size; y++)
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
    {
        //TODO убирать в память чанки и обьекты на них
        Transform parentObj = null;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
				
				if (parentObj == null) {
					parentObj = blockObjects [x, y].transform.parent;
				}
                blocks[x, y] = null;
                GameObject.Destroy(blockObjects[x, y]);
            }
        }
        GameObject.Destroy(parentObj.gameObject);

    }

}
