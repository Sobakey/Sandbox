using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block {
    public string display_name;
    public int id;
    public Sprite[] sprites;
    public bool isSolid = true;
    public Drop[] drops;

    public Sprite GetSprite(){
        return sprites[Random.Range(0, sprites.Length - 1)];
    }
}
