using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightController : MonoBehaviour {

	// Use this for initialization
	MeshRenderer renderer;
	public float emission = 1;
	public float obstacle = 20;
	void Start () {
		renderer = gameObject.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.SetFloat("_EmissionColorMul", emission);
		renderer.material.SetFloat("_ObstacleMul", obstacle);
	}
}
