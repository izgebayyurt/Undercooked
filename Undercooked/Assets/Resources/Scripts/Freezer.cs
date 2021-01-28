using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{
    public GameManager GM;
    public SlotManager SM;

    public GameObject indicator;
    public GameObject food;

    public float delta;
    public bool indicatorMove;
    public bool foodMove;

    // Food sprite start & end points
    private Vector3 foodStart;
    private Vector3 foodEnd;

    // Indicator sprite start & end points
    private Vector3 indicatorStart;
    private Vector3 indicatorEnd;

    private float secondsForOneLength = 1f;

    private bool use = false;

    void Start()
    {
        SM = SM.GetComponent<SlotManager>();
        GM = GM.GetComponent<GameManager>();

        // Set the transforms of the indicator and food according to the position of the freezer
        indicator.transform.position = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        food.transform.position = transform.position + new Vector3(0.0f, 0.35f, 0.0f);

        foodStart = food.transform.position;
        foodEnd = food.transform.position + new Vector3(0.0f, delta, 0.0f);

        indicatorStart = indicator.transform.position;
        indicatorEnd = indicator.transform.position + new Vector3(0.0f, delta, 0.0f);
    }

    void Update()
    {
        if (foodMove)
        {
            food.transform.position = Vector3.Lerp(foodStart, foodEnd,
                Mathf.SmoothStep(0f, 1f,
                Mathf.PingPong(Time.time / secondsForOneLength, 1f)
            ));
        }
        

        if (indicatorMove)
        {
            indicator.transform.position = Vector3.Lerp(indicatorStart, indicatorEnd,
               Mathf.SmoothStep(0f, 1f,
               Mathf.PingPong(Time.time / secondsForOneLength, 1f)
            ));
        }

        use = Input.GetButton("Use");

        if (transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FreezerPickUp") &&
                transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            transform.GetComponent<Animator>().SetBool("IsPickingItem", false);
        }

    }

    private void OnTriggerStay2D(Collider2D col)
    {

        if (use && GM.slots[GM.GetCurrentSlotIdx()] == null)
        {
            SM.AddItem(food.GetComponent<SpriteRenderer>().sprite.name);
            transform.GetComponent<Animator>().SetBool("IsPickingItem", true);  
        }

        

    }
}

