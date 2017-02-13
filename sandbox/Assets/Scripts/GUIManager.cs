using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public Image playerInventory;
    public bool isInventoryOpen = false;
    public Image[] slots;

    private Inventory playerInventoryScript;


    private void Update()
    {
        if (playerInventoryScript == null)
        {
            playerInventoryScript = GameObject.FindWithTag("player").GetComponent<Inventory>();
        }

        if (isInventoryOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (isMouseOverSlot(i))
                    {
                        if (playerInventoryScript.itemStacks[i] != null)
                        {

                        }
                    }
                }
            }

            RenderSlots();
        }
       

    }

    public void ShowInventory(bool value)
    {
        playerInventory.gameObject.SetActive(value);
        isInventoryOpen = value;
    }

    public bool isMouseOverSlot(int slotIndex)
    {
        RectTransform rt = slots[slotIndex].GetComponent<RectTransform>();
        if (Input.mousePosition.x > rt.position.x - rt.sizeDelta.x * 1.5f && Input.mousePosition.x < rt.position.x - rt.sizeDelta.x * 1.5f )
        {
            if (Input.mousePosition.y > rt.position.y - rt.sizeDelta.y * 1.5f && Input.mousePosition.y < rt.position.y - rt.sizeDelta.y * 1.5f)
            {
                return true;
            }
        }

        return false;
    }

    private void RenderSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Item.ItemStack itemStack = playerInventoryScript.itemStacks[i];
            if (itemStack != null)
            {
                if (slots[i].color.a != 1)
                {
                    slots[i].color = new Color(1,1,1,1);
                    slots[i].sprite = itemStack.item.sprite;
                    slots[i].transform.GetChild(0).GetComponent<Text>().text = itemStack.stackSize.ToString();
                }
            }
            else
            {
                if (slots[i].color.a != 0)
                {
                    slots[i].color = new Color(1, 1, 1, 0);
                    slots[i].sprite = null;
                    slots[i].transform.GetChild(0).GetComponent<Text>().text = "";
                }
            }
        }
    }

}
