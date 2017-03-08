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

	private System.Random rnd;
	public Block()
	{
		rnd = new System.Random();
	}

    public Sprite GetSprite(){
        return sprites[rnd.Next(0, sprites.Length - 1)];
    }
}
