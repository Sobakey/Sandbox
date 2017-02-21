using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    public List<Block> blocks;

    public Block FindBlock (byte id)
    {
        foreach (Block block in blocks)
        {
            if (block.id == id)
            {
                return block;
            }
        }
        return null;
    }

    public Block FindBlock(string name)
    {
        foreach (Block block in blocks)
        {
            if (block.display_name == name)
            {
                return block;
            }
        }
        return null;
    }
}
