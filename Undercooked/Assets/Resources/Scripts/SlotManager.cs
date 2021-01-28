using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public GameManager GM;   
    private Sprite[] slotSprites;

    private void Start()
    {
        GM = GM.GetComponent<GameManager>();

        slotSprites = Resources.LoadAll<Sprite>("Sprites/UI/FoodUI");
    }

    private void Update()
    {
        SelectSlot();
        SlotAnimation();
        ScrollInventory();
    }

    // Scrolling Through Inventory Slots
    void ScrollInventory()
    {

        if (Input.GetKeyDown("tab") && !Input.GetButton("Use"))
        {
            {
                GM.NextSlot();
            }
        }

        if (Input.GetKeyDown("left shift") && !Input.GetButton("Use"))
        {
            {
                GM.PreviousSlot();
            }
        }
    }

    // A function to change inventory slots
    // slotSprites 0 is the unselected sprite, 1 is the selected sprite
    void SelectSlot()
    {
        transform.GetChild(GM.GetCurrentSlotIdx()).GetComponent<Image>().sprite = slotSprites[1];
        if(GM.GetCurrentSlotIdx() != 0)
        {
            transform.GetChild(GM.GetCurrentSlotIdx() - 1).GetComponent<Image>().sprite = slotSprites[0];
        }
        else
        {
            transform.GetChild(GM.slotCount - 1).GetComponent<Image>().sprite = slotSprites[0];
        }
    }

    //// Adds a food image into inventory slot
    //public void UpdateSlotUI()
    //{
    //    transform.GetChild(GM.GetCurrentSlotIdx()).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    //    transform.GetChild(GM.GetCurrentSlotIdx()).GetChild(0).GetComponent<Image>().sprite = GM.GetFoodSprite();
    //}

    public void SlotAnimation()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Animator>().SetInteger("SlotNumber", GM.GetCurrentSlotIdx());
            child.GetComponent<Animator>().SetBool("IsChopping", GM.isChopping);

            // If the animation ends, replace the image with the cut image
            if (ChildAnimationDonePlaying(child, "FoodUICut"))
            {
                
                if (GetFoodObj().IsCuttable()) // make sure the food is cuttable first
                {
                    string foodName = RemoveItem();
                    AddItem(foodName + "_cut"); // add the cut version to the inventory
                }
                

            }
        }
    }

    // Auxillary function for getting when the animation ends
    bool ChildAnimationDonePlaying(Transform child, string stateName)
    {
        return (child.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(stateName) && 
                child.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && 
                !child.GetComponent<Animator>().IsInTransition(0));
    }

    // Refreshes the inventory UI
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < GM.slots.Length; i++)
        {
            if (GM.slots[i] == null)
            {
                transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
            } else {
                transform.GetChild(GM.GetCurrentSlotIdx()).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                transform.GetChild(GM.GetCurrentSlotIdx()).GetChild(0).GetComponent<Image>().sprite = GM.GetFoodSprite();
            }
        }
    }


    // Inventory methods
    public bool AddItem(string name)
    {
        if (GM.GetCurrentSlotString() == null)
        {
            GM.SetCurrentSlotString(name);
            UpdateInventoryUI();
            return true;
        }
        else
        {
            return false;
        }

    }

    public string RemoveItem()
    {
        if (GM.GetCurrentSlotString() != null)
        {
            string removed_item = GM.GetCurrentSlotString();
            GM.EmptyCurrentSlot();
            return removed_item;
        }
        else
        {
            return null;
        }

    }

    public string GetItem()
    {
        if (GM.GetCurrentSlotString() != null)
        {
            return GM.GetCurrentSlotString();
        }
        else
        {
            return null;
        }
    }

    public Food GetFoodObj()
    {
        if (GM.GetCurrentSlotString() != null)
        {
            return GM.GetCurrentSlotFood();
        }
        else
        {
            return null;
        }
    }

}