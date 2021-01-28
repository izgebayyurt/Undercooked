using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    public bool isServing;
    bool triggerStay;

    public OrderManager OM;
    public GameManager GM;

    public bool Serve()
    {
        // if there is a order
        if (OM.orders.Count > 0)
        {
            // loop over all the orders
            for (int i = 0; i < OM.orders.Count; i++)
            {

                // turn the ui slots into an inventory list which we can loop on
                List<string> inventory = new List<string>(GM.slots);
  
                // if the order is bigger than the inventory, move on.
                if (OM.orders[i].GetIngredients().Count > inventory.Count)
                {
                    continue;
                }

                // If we can find a suitable order, we are going to remove it
                bool removed = true;



                for (int j = 0; j < OM.orders[i].GetIngredients().Count; j++)
                   
                {
                    removed = inventory.Remove(OM.orders[i].GetIngredients()[j]);

                    //Debug.Log(removed);

                    if (!removed)
                    {
                        break;
                    }
                }

                if (!removed)
                {
                    continue;
                }
                else
                {
                    // Remove the order i
                     StartCoroutine(OM.RemoveOrder(i));

                    // Add the score
                    GM.ChangeScore(OM.orders[i].GetPrice());

                    isServing = true; // Animate it

                    // Remove the order's ingredients from the inventory
                    RemoveIngredients(i);

                    // Update inventory UI
                    GM.UpdateInventoryUI();

                    return true;
                }
              
            }
            
        }

        return false;
    }



    // Start is called before the first frame update
    void Start()
    {
        OM = OM.GetComponent<OrderManager>();
        GM = GM.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isServing)
        {
            GetComponent<Animator>().SetBool("Serving", true);
            
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Cashier_Order") &&
                GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
                !GetComponent<Animator>().IsInTransition(0))
            {
                GetComponent<Animator>().SetBool("Serving", false);
                isServing = false;
            }
        }

        if (Input.GetButtonDown("Use") && triggerStay && !isServing)
        {
            Serve();
        }
    }


    // OnTriggerStay is buggy so I am using this instead.
    private void OnTriggerEnter2D(Collider2D collision)
    {

        triggerStay = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        triggerStay = false;
    }

    private void RemoveIngredients(int i){
        // Remove ingredients from inventory
        for (int j = 0; j < OM.orders[i].GetIngredients().Count; j++)
        {
            for (int k = 0; k < GM.slots.Length; k++)
            {
                if (OM.orders[i].GetIngredients()[j] == GM.slots[k])
                {
                    GM.slots[k] = null;
                    break;
                }
            }
        }
    }

}
