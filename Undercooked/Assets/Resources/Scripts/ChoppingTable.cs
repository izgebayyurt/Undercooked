using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingTable : MonoBehaviour
{
    public Animator animator;
    public GameManager GM;
    public SlotManager SM;

    private void Start()
    {
        SM = SM.GetComponent<SlotManager>();
        GM = GM.GetComponent<GameManager>();
    }

    void Update()
    {
        animator.SetBool("IsChopping", GM.isChopping);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {   
        // by default, the player is not chopping
        GM.isChopping = false;

        // by default the player can move & chopping animation is not playing
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            collision.gameObject.GetComponent<PlayerMovement>().enabled = true;
            collision.gameObject.GetComponent<Animator>().SetBool("IsChopping", false);
        }

        if (Input.GetButton("Use") && // pressing the use button
            SM.GetItem() != null && // there is a food 
            SM.GetFoodObj().IsCuttable()) // the food is cuttable
        {

            GM.isChopping = true;

            // if the collision is happening with a player
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {   
                // cancel movement while chopping
                collision.gameObject.GetComponent<PlayerMovement>().enabled = false;

                // enable chopping animation
                collision.gameObject.GetComponent<Animator>().SetBool("IsChopping", true);
            }

        }

    }
    
}
