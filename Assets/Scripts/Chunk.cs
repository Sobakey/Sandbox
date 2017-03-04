using System;
using Sandbox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Chunk
{
    public static int size = 32;
    public Block[,] blocks;
    public GameObject[,] blockObjects;
    private BlockManager blockManager;

	public GameObject gameObject;
    public Vector2Int coords;

    public bool IsEmpty { get; private set; }


	public Vector2 GetWorldPos()
	{
		int xPos = Mathf.FloorToInt(coords.x);
		int yPos = Mathf.FloorToInt(coords.y);

		return new Vector2(xPos, yPos);
	}

    public Chunk(BlockManager blockManager, Vector2Int position)
    {
        IsEmpty = true;
        this.blockManager = blockManager;
        this.coords = position;
        blocks = new Block[size, size];
        blockObjects = new GameObject[size, size];

	    gameObject = new GameObject();
    }

    public void GenerateBlocksInfos(PerlinNoizeGenerator perlinNoizeGenerator)
    {
        for (int x = 0; x < size; x++)
        {
            int pHeight = perlinNoizeGenerator.GetHeight(x + (coords.x * size));

            for (int y = 0; y < size; y++)
            {
                var absoluteY = y + (coords.y * size);
                if (absoluteY <= pHeight)
                {
                    if (absoluteY == pHeight - 1 && absoluteY < 100 )
                    {
                        blocks[x, y] = blockManager.FindBlock(1); //grass
                    }
                    if ((absoluteY == pHeight || absoluteY == pHeight-1) && absoluteY > 250)
                    {
                        blocks[x, y] = blockManager.FindBlock(5); //snow
                    }
                    else if (absoluteY == pHeight && absoluteY < 100)
                    {
                        if (Random.value < 0.4f)
                        {
                            blocks[x, y] = blockManager.FindBlock(4); //tall_grass
                        }
                    }
                    else if (absoluteY < pHeight - Random.Range(4, 16) || absoluteY > pHeight - 1  || absoluteY > 100)
                    {
                        blocks[x, y] = blockManager.FindBlock(3); //stone
                    }
                    else 
                    {
                        blocks[x, y] = blockManager.FindBlock(2); //dirt
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

	public IEnumerator GenerateTiles()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (blocks[x, y] != null)
				{
					CreateTile(x, y);
				}
			}
		}
		yield return null;
	}

	public void CreateTile(int x, int y)
	{
		var blockTag = "Block";
		bool isTrigger = false;
		var position = new Vector3((coords.x * Chunk.size) + x, (coords.y * Chunk.size) + y);

		if (blocks[x, y].isSolid != true)
		{
			isTrigger = true;
			blockTag = "tall_grass";
		}
		var block = BlockManager.GetBlock(gameObject.transform, blocks[x, y].GetSprite(), position,
			blocks[x, y].display_name, blockTag, isTrigger);
		block.layer = 13;
		blockObjects[x, y] = block;
	}

    public void Destroy()
    {
        //TODO убирать в память чанки и обьекты на них
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {


                    var obj = blockObjects[x, y];
                    if (obj != null)
                    {
	                    BlockManager.DestroyBlock(blockObjects[x, y]);
                    }
                blocks[x, y] = null;
            }
        }
	    //пока удаляем, после надо думать как организовать хранение в памяти, а после в файле
        Object.Destroy(gameObject);
    }

}
