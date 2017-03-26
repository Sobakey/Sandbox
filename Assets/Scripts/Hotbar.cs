using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{

    public int selectedSlot = 1;
    public Image[] slots;
    public Image selector;

    private Inventory playerInv;

    private void Update()
    {
        if (playerInv == null)
        {
            playerInv = GameObject.FindWithTag("player").GetComponent<Inventory>();
            return;
        }

        UpdateScroling();
        UpdateSelector();
        UpdateItems();

        #region управление анимацией худа
        if (Input.GetMouseButton(0))
        {
            transform.FindChild("LH").FindChild("LH_A").gameObject.SetActive(true);
        }
        else
        {
            transform.FindChild("LH").FindChild("LH_A").gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(1))
        {
            transform.FindChild("RH").FindChild("RH_A").gameObject.SetActive(true);
        }
        else
        {
            transform.FindChild("RH").FindChild("RH_A").gameObject.SetActive(false);
        }
       

        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            

            for (int i = 1; i <=  3; i++)
            {
                 int j = i;
                if (transform.FindChild("LH").GetChild(j).gameObject.activeInHierarchy)
                {
                    transform.FindChild("LH").GetChild(j).gameObject.SetActive(false);
                    if (i == 3) j = 0;
                    transform.FindChild("LH").GetChild(j + 1).gameObject.SetActive(true);
                    break;
                }             
            }
        }

        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftAlt))
        {

            for (int i = 1; i <= 3; i++)
            {
                int j = i;
                if (transform.FindChild("RH").GetChild(j).gameObject.activeInHierarchy)
                {
                    transform.FindChild("RH").GetChild(j).gameObject.SetActive(false);
                    if (i == 3) j = 0;
                    transform.FindChild("RH").GetChild(j + 1).gameObject.SetActive(true);
                    break;
                }
            }
        }

        #endregion
    }

    private void UpdateScroling()
    {
        selectedSlot += (int)Input.mouseScrollDelta.y;

        if (selectedSlot < 1)
        {
            selectedSlot = 2;
        }
        if (selectedSlot > 2)
        {
            selectedSlot = 1;
        }
    }

    private void UpdateSelector()
    {
        selector.rectTransform.localPosition = slots[selectedSlot - 1].rectTransform.localPosition;
        // Debug.Log(playerInv.itemStacks[selectedSlot - 1]);

        //if (playerInv.itemStacks[selectedSlot].item.type == Item.Type.Tool && playerInv.itemStacks[selectedSlot] != null)
        //{
        //    playerInv.gameObject.GetComponent<PlayerController>().HoldItem(slots[selectedSlot - 1].sprite, 1f);
        //}
        //else
        playerInv.gameObject.GetComponent<PlayerController>().HoldItem(slots[selectedSlot - 1].sprite, 0.8f);
    }

    private void UpdateItems()
    {
        for (int i = 0; i < 2; i++)
        {
            Image hotbarSlot = slots[1 - i];
            Item.ItemStack invSlot = playerInv.itemStacks[i + 34];
            if (invSlot != null)
            {
                hotbarSlot.sprite = invSlot.item.sprite;
                hotbarSlot.color = new Color(1, 1, 1, 1);
                if (invSlot.stackSize == 1 && invSlot.item.type == Item.Type.Tool)
                {
                    slots[i].transform.GetChild(0).GetComponent<Text>().text = "";
                }
                else
                    hotbarSlot.transform.FindChild("StackCountText").GetComponent<Text>().text = invSlot.stackSize.ToString();
            }
            else
            {
                hotbarSlot.sprite = null;
                hotbarSlot.color = new Color(1, 1, 1, 0);
                hotbarSlot.transform.FindChild("StackCountText").GetComponent<Text>().text = "";
            }
        }
    }

    public Item GetHeldItem()
    {
        int selSlot = 2 - selectedSlot;
        if (playerInv.itemStacks[selSlot + 26] != null)
        {
            return playerInv.itemStacks[selSlot + 26].item;
        }

        return null;
    }
}
