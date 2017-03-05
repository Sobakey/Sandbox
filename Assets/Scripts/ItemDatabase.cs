using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {

    public List<Item> items;

    public Item FindItem(string name)
    {
        foreach (Item item in items)
        {
            if (item.name == name)
            {
                return item;
            }
        }

        return null;
    }

	private static ItemDatabase instance;

	public static ItemDatabase Instance
	{
		get { return instance ?? (GameObject.Find("GameManager").GetComponent<ItemDatabase>()); }
	}
}
