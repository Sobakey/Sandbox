using Sandbox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk  {

    public static int size = 32;
    public Block[,] blocks;
    public GameObject[,] blockObjects;
    private BlockManager blockManager;

    public Vector2Int coords;

    public bool IsEmpty {get; private set;}
    //TODO: Код реализован только для горизонтальной оси
    public static Vector2Int GetChunkCoordAtPos(Vector3 pos){
        return new Vector2Int(Mathf.FloorToInt(pos.x / Chunk.size), Mathf.FloorToInt(pos.y / Chunk.size));
    }

    public static Vector2Int GetTilePositionAtPos(Vector3 pos)
    {
        var v = GetChunkCoordAtPos(pos);
        v.x += Mathf.FloorToInt((pos.x % Chunk.size));
        v.y += Mathf.FloorToInt((pos.y % Chunk.size));
        return v;
    }

    public Chunk(BlockManager blockManager, Vector2Int position)
    {
        IsEmpty = true;
        this.blockManager = blockManager;
        this.coords = position;
        blocks = new Block[size, size];
        blockObjects = new GameObject[size, size];
    }

    public void GenerateBlocks(PerlinNoizeGenerator perlinNoizeGenerator)
    {  
        for (int x = 0; x < size; x++)
        {
            float pHeight = perlinNoizeGenerator.GetHeight(x + (coords.x * size));

            for (int y = 0; y < size; y++)
            {
                var absoluteY = y + (coords.y * size);
                if (absoluteY <= pHeight)
                {
                    if (absoluteY == pHeight - 1)
                    {
                        blocks[x, y] = blockManager.FindBlock(1); //grass
                    }
                    else if (absoluteY == pHeight)
                    {
                        if (Random.value < 0.4f)
                        {
                            blocks[x, y] = blockManager.FindBlock(4); //tall_grass
                        }
                    }
                    else if (absoluteY < pHeight - Random.Range(4, 16) || absoluteY > pHeight - 1)
                    {
                        blocks[x, y] = blockManager.FindBlock(3); //dirt
                    }
                    else
                    {
                        blocks[x, y] = blockManager.FindBlock(2); //stone
                    }
                    IsEmpty = false;
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
                    var obj = blockObjects[x, y];
                    if(obj != null){                       
					    parentObj = blockObjects [x, y].transform.parent;
                    }
				}
                blocks[x, y] = null;
                GameObject.Destroy(blockObjects[x, y]);
            }
        }
        if(parentObj != null)
            GameObject.Destroy(parentObj.gameObject);
    }

}
