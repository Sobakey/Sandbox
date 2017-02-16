using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //TODO fix replacement stacks items on slots
    public int slotCount = 3 * 9;
    public Item.ItemStack[] itemStacks;

    private ItemDatabase itemDatabase;

    private void Start()
    {
        itemDatabase = GameObject.Find("GameManager").GetComponent<ItemDatabase>();
        itemStacks = new Item.ItemStack[slotCount];
        //itemStacks[0] = new Item.ItemStack(itemDatabase.FindItem("Dirt Block"), 25);
        //itemStacks[1] = new Item.ItemStack(itemDatabase.FindItem("Stone Block"), 64);
    }

    public void AddItem(string name, int count)
    {
        Item.ItemStack existingStack = FindExistingStack(name, count);

        if (existingStack != null)
        {
            existingStack.stackSize += count;
        }
        else
        {
            if (FindFirstAvailableSlot()>=0)
            {
                int availableSlot = FindFirstAvailableSlot();
                itemStacks[availableSlot] = new Item.ItemStack(itemDatabase.FindItem(name), count);
          
            }
            else
            {
                Debug.Log("No place");
            }
        }
    }

    public void RemoveItem(string name, int count)
    {
        Item.ItemStack existingStack = FindExistingStack(name, count);

        if (existingStack != null)
        {
            if (existingStack.stackSize - count >= 1)
            {
               existingStack.stackSize -= count;
            }
            else
            {
                existingStack = null ;
            }
        }
    }

   private Item.ItemStack FindExistingStack(string name)
    {
        foreach (Item.ItemStack i in itemStacks)
        {
            if (i != null)
            {
                if (i.item.name == name)
                {
                    return i;
                }
            }
        }

        return null;
    }

    private Item.ItemStack FindExistingStack(string name, int count)
    {
        foreach (Item.ItemStack i in itemStacks)
        {
            if (i != null)
            {
                if (i.item.name == name)
                {
                    if (i.stackSize + count <= i.item.maxStack)
                    {
                        return i;
                    }
                }
            }
        }

        return null;
    }

    private int FindFirstAvailableSlot()
    {
        for (int i = 0; i < itemStacks.Length; i++)
        {
            if (itemStacks[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

}
