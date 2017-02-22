using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    public Block[] blocks;
    private Dictionary<int, Block> blocksCache = new Dictionary<int, Block>();
    private Dictionary<string, byte> blocksNameCache = new Dictionary<string, byte>();

    void Start(){
        foreach (var item in blocks)
        {
            blocksCache.Add(item.id, item);
            blocksNameCache.Add(item.display_name, item.id);
        }
    }

    public Block FindBlock (byte id)
    {
        Block res;
        if(blocksCache.TryGetValue(id, out res)){
            return res;
        }
        return null;
    }


    public Block FindBlock(string name)
    {
        byte id;
        if(blocksNameCache.TryGetValue(name, out id)){
            return FindBlock(id);
        }
        return null;
    }
}
