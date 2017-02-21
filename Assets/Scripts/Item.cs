using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item  {

    public string name;
    public Sprite sprite;
    public int maxStack = 64;
    public enum Type
    {
        Item, Block, Weapon, Tool
    }
    public Type type;

    public class ItemStack
    {
        public Item item;
        public int stackSize;

        public ItemStack(Item item, int stackSize)
        {
            this.item = item;
            this.stackSize = stackSize;
        }
    }
}
