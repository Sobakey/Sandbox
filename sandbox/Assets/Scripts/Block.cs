using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block {
    public string display_name;
    public byte id;
    public Sprite sprite;
    public bool isSolid = true;
    public Drop[] drops;
}
