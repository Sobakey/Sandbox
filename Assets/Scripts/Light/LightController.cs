using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightController : MonoBehaviour {

	// Use this for initialization
	MeshRenderer lightRenderer;
	public float emission = 1;
	public float obstacle = 20;
	void Start () {
		lightRenderer = gameObject.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		lightRenderer.material.SetFloat("_EmissionColorMul", emission);
		lightRenderer.material.SetFloat("_ObstacleMul", obstacle);
	}
}
