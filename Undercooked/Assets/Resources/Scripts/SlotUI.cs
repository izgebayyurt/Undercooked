using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
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

    }

    void SelectSlot()
    {
        transform.GetChild(GM.currentSlot).GetComponent<Image>().sprite = slotSprites[1];
        if(GM.currentSlot != 0)
        {
            transform.GetChild(GM.currentSlot - 1).GetComponent<Image>().sprite = slotSprites[0];
        }
        else
        {
            transform.GetChild(GM.slotCount - 1).GetComponent<Image>().sprite = slotSprites[0];
        }
    }

    // Adds a food image into inventory slot
    public void InsertFood()
    {
        transform.GetChild(GM.currentSlot).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        transform.GetChild(GM.currentSlot).GetChild(0).GetComponent<Image>().sprite = GM.GetFoodSprite();
    }

    public void SlotAnimation()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Animator>().SetInteger("SlotNumber", GM.currentSlot);
            child.GetComponent<Animator>().SetBool("IsChopping", GM.isChopping);

            // If the animation ends, replace the image with the cut image
            if (ChildAnimationDonePlaying(child, "FoodUICut"))
            {
                
                if (GM.GetFoodObj().IsCuttable()) // make sure the food is cuttable first
                {
                    string foodName = GM.RemoveItem();
                    GM.AddItem(foodName + "_cut");
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

    // Refreshes the UI
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < GM.slots.Length; i++)
        {
            if (GM.slots[i] == null)
            {
                transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
            }
        }
    }

}