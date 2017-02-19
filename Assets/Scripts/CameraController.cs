using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {



    public GameObject player;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (player == null)
        {

            player = GameObject.FindWithTag("player");
          
        }
        //Камера должна быть всегда самой ближней по оси Z
        transform.position = player.transform.position - Vector3.forward;
       
	}
}
