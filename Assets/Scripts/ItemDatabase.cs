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
}
