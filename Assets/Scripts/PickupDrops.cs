using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDrops : MonoBehaviour {

    private Inventory connectedInventory;
    //private ItemDatabase itemDatabase;

	private void Start () {
        connectedInventory = transform.parent.GetComponent<Inventory>();
        //itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            connectedInventory.AddItem(other.name, 1);
            GameObject.Destroy(other.gameObject);
        }
    }
}
