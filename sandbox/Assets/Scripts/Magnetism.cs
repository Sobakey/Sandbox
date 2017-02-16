using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetism : MonoBehaviour {

    public Transform target;
    public float range = 5;
    public float strength = 9;

	private void Update () {
        if (InRange())
        {
            Attract();
        }
	}

    private bool InRange()
    {
        return Vector3.Distance(transform.position, target.position) <= range;
    }

    private void Attract()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
    }
}
