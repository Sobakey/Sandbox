using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    public Block[] blocks;
    private Dictionary<int, Block> blocksCache = new Dictionary<int, Block>();
    private Dictionary<string, int> blocksNameCache = new Dictionary<string, int>();

    void Start(){
        foreach (var item in blocks)
        {
            blocksCache.Add(item.id, item);
            blocksNameCache.Add(item.display_name, item.id);
        }
    }

    public Block FindBlock (int id)
    {
        Block res;
        if(blocksCache.TryGetValue(id, out res)){
            return res;
        }
        return null;
    }


    public Block FindBlock(string name)
    {
        int id;
        if(blocksNameCache.TryGetValue(name, out id)){
            return FindBlock(id);
        }
        return null;
    }
}
