using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public Sprite sprite;
    public Camera main_camera;
    public int chunk_size=2;
    GameObject tiles;
    GameObject tile;


    // Use this for initialization
    void Start () {

      tiles = new GameObject("tiles");
       

        for (int x = 0; x < chunk_size; x++)
        { 
            for (int y = 0; y < chunk_size; y++)
            {
                
                tile = new GameObject(string.Format("tile x:{0}, y:{1}",x,y));
                tile.transform.position = new Vector2(x, y);
                tile.transform.parent = tiles.transform;
                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                tile.AddComponent<BoxCollider2D>();
                sr.sprite = sprite;
               
            }
           
        }
        main_camera.transform.position = new Vector3(chunk_size/2, chunk_size / 2,-10);
        main_camera.orthographicSize =  2+  chunk_size / 2;


       
    }
	
	// Update is called once per frame
	void Update () {
    
    }
}
