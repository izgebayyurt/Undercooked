using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inventory Management
    public int currentSlot = 0;
    public int slotCount;

    // strings of food names
    public string[] slots;

    public Food[] foods;
    private readonly Dictionary<string, Food> foodDict = new Dictionary<string, Food>();

    public GameObject SlotManager;
    private SlotUI SlotUI;

    // Chopping
    public bool isChopping;

    // Score
    public int score;




    void Start()
    {
        // Setting up inventory
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = null;
        }

        // Load food objects into a dictionary
        for (int i = 0; i < foods.Length; i++)
        {
            foodDict.Add(foods[i].name, foods[i]);
        }

        SlotUI = SlotManager.GetComponent<SlotUI>();
    }

    void Update()
    {
        ScrollInventory();
    }

    void ScrollInventory()
    {
        // Scrolling Through Inventory Slots
        if (Input.GetKeyDown("tab") && !Input.GetButton("Use"))
        {
            {
                currentSlot++;
                currentSlot = currentSlot % slotCount;
            }
        }

        if (Input.GetKeyDown("left shift") && !Input.GetButton("Use"))
        {
            {
                if (currentSlot != 0)
                {
                    currentSlot--;
                }
                else
                {
                    currentSlot = slotCount - 1;
                }
       
            }
        }
    }

    // Inventory methods
    public bool AddItem(string name)
    {
        if (slots[currentSlot] == null)
        {
            slots[currentSlot] = name;
            SlotUI.InsertFood();
            return true;
        }
        else
        {
            return false;
        }

    }

    public string RemoveItem()
    {
        if (slots[currentSlot] != null)
        {
            string removed_item = slots[currentSlot];
            slots[currentSlot] = null;
            return removed_item;
        }
        else
        {
            return null;
        }

    }

    public string GetItem()
    {
        if (slots[currentSlot] != null)
        {
            return slots[currentSlot];
        }
        else
        {
            return null;
        }
    }

    public Food GetFoodObj()
    {
        if (slots[currentSlot] != null)
        {
            return foodDict[slots[currentSlot]];
        }
        else
        {
            return null;
        }
        
    }

    // Returns the sprite from the food object that is in the current slot
    public Sprite GetFoodSprite()
    {
        return foodDict[slots[currentSlot]].GetSprite();
    }

    // Updates the UI of inventory
    public void UpdateInventoryUI()
    {   
        SlotUI.UpdateInventoryUI();
    }

}
