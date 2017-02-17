using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public Image playerInventory;
    public bool isInventoryOpen = false;
    public Image[] slots;
    public GameObject slotPrefab;
    public Image grayOut;

    private Inventory playerInventoryScript;
    private GameObject cursorIcon;
    private Item.ItemStack cursorStack;

    private void Start()
    {
        cursorIcon = (GameObject) GameObject.Instantiate(slotPrefab,Vector3.zero,Quaternion.identity);
        cursorIcon.transform.SetParent(gameObject.transform);
    }

    private void Update()
    {
        if (playerInventoryScript == null)
        {
            playerInventoryScript = GameObject.FindWithTag("player").GetComponent<Inventory>();
        }

        RenderCursorStack();

        //TODO drag and drop when button  held down
        if (isInventoryOpen)
        {           
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (isMouseOverSlot(i))
                    {
                        if (cursorStack == null)
                        {
                            if (playerInventoryScript.itemStacks[i] != null)
                            {
                                cursorStack = playerInventoryScript.itemStacks[i];
                                playerInventoryScript.itemStacks[i] = null;
                            }
                        }
                        else
                        {
                            playerInventoryScript.itemStacks[i] = cursorStack;
                            cursorStack = null;
                        }
                    }
                }
            }

            RenderSlots();
        }
    }


    public void ShowInventory(bool value)
    {
        grayOut.color = value ? new Color(0, 0, 0, 0.75f) : new Color(0, 0, 0, 0);
        playerInventory.gameObject.SetActive(value);
        isInventoryOpen = value;
    }

    private void RenderCursorStack()
    {
        if (cursorStack != null)
        {
            
            cursorIcon.transform.position = Input.mousePosition;
            if (cursorIcon.GetComponent<Image>().color.a != 1)
            {
                cursorIcon.GetComponent<Image>().color = new Color(1,1,1,1);
                cursorIcon.GetComponent<Image>().sprite = cursorStack.item.sprite;
                cursorIcon.transform.GetChild(0).GetComponent<Text>().text = cursorStack.stackSize.ToString();
            }
        }
        else
        {
            
            if (cursorIcon.GetComponent<Image>().color.a != 0)
            {
                cursorIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                cursorIcon.GetComponent<Image>().sprite = null;
                cursorIcon.transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
       
    }

    public bool isMouseOverSlot(int slotIndex)
    {
        RectTransform rt = slots[slotIndex].GetComponent<RectTransform>();
        if (Input.mousePosition.x > rt.position.x - rt.sizeDelta.x * 1.5f && Input.mousePosition.x < rt.position.x + rt.sizeDelta.x * 1.5f )
        {
            if (Input.mousePosition.y > rt.position.y - rt.sizeDelta.y * 1.5f && Input.mousePosition.y < rt.position.y + rt.sizeDelta.y * 1.5f)
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
                    slots[i].color = new Color(1,1,1,1);
                    slots[i].sprite = itemStack.item.sprite;
                    slots[i].transform.GetChild(0).GetComponent<Text>().text = itemStack.stackSize.ToString();
            }
            else
            {
                    slots[i].color = new Color(1, 1, 1, 0);
                    slots[i].sprite = null;
                    slots[i].transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
    }

}
