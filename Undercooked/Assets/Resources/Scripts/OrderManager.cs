using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public GameManager GM;

    public List<Order> orders = new List<Order>(); // Pull orders to display from the Queue
    public Order[] possibleOrders; // Orders that can be added the Queue

    public int maxOrderCount; // Maximum amount of orders that can appear
    public float timeBetweenOrders = 5; // Time that must pass between two orders

    public float OrderAddUIBounciness; // Determines how bouncy the entrance of Order UI is
    public float OrderRemoveUIBounciness; // Determines how bouncy the exit of Order UI is

    [SerializeField] private AnimationCurve OrderAddAnim;
    [SerializeField] private AnimationCurve OrderRemoveAnim;

    bool removedOrder = false;

    void Start()
    {
        StartCoroutine("AddOrder");
    }

    // Update is called once per frame
    void Update()
    {

        if (removedOrder)
        {
            StartCoroutine("AddOrder");
            removedOrder = false;
        }

        for(int i = 1; i <= orders.Count; i++)
        {
            GetComponent<Animator>().SetFloat("Time" + i.ToString(), GetComponent<Animator>().GetFloat("Time" + i.ToString()) - Time.deltaTime);
        }

        CheckTimeOut();
    }

    // Adds an order to the list
    IEnumerator AddOrder()
    {
        yield return new WaitForSeconds(timeBetweenOrders);

        // Pick a random possible order and add it
        Order order = possibleOrders[Random.Range(0, possibleOrders.Length)];
        orders.Add(order);
        AddOrderUI(orders.Count - 1, order);

        // UI animation for order adding
        StartCoroutine(UIEnterAnimation(orders.Count - 1));

        // Timer
        GetComponent<Animator>().SetFloat("Speed" + orders.Count.ToString(), order.GetTimeMultiplier());
        GetComponent<Animator>().SetFloat("Time" + orders.Count.ToString(), order.GetTotalTime());
        

        if (orders.Count < 4)
        {
            StopCoroutine("AddOrder");
            StartCoroutine("AddOrder");
        }
        
    }

    // Updates the UI after adding an item. Order index determines the slot to be updated.
    void AddOrderUI(int orderIndex, Order order)
    {
        transform.GetChild(orderIndex).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        transform.GetChild(orderIndex).GetComponent<Image>().sprite = order.GetSprite();
    }

    IEnumerator UIEnterAnimation(int orderIndex)
    {
        float startTime = OrderAddAnim[0].time;
        float endTime = OrderAddAnim[OrderAddAnim.length - 1].time;

        // Starting position of the orders
        float startPosX = transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition.x;
        float startPosY = transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition.y;

        // Make them bounce here
        for (float t = startTime; t < endTime; t += Time.deltaTime)
        {

           float value = OrderAddAnim.Evaluate(t); // Get the value according to the animation curve

           transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition = 
                new Vector3(startPosX + value * -OrderAddUIBounciness,
                            startPosY);

            yield return null;
        }

        transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition =
                new Vector3(startPosX,
                            startPosY);

    }

    public IEnumerator RemoveOrder(int orderIndex)
    {
        float startTime = OrderRemoveAnim[0].time;
        float endTime = OrderRemoveAnim[OrderRemoveAnim.length - 1].time;

        float startPosX = transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition.x;
        float startPosY = transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition.y;

        for (float t = startTime; t < endTime; t += Time.deltaTime)
        {

            float value = OrderRemoveAnim.Evaluate(t); // Get the value according to the animation curve

            transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition =
                 new Vector3(startPosX + value * OrderRemoveUIBounciness,
                             startPosY);
            yield return null;
        }

        transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition =
                new Vector3(startPosX,
                            startPosY);

        // Make the image transparent
        transform.GetChild(orders.Count - 1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

        // Rearrange the timers
        RearrangeTimers(orderIndex);

        // Remove the order from the list
        orders.RemoveAt(orderIndex);

        // Update the sprites of the orders
        for (int i = 0; i < orders.Count; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = orders[i].GetSprite();
        }

        removedOrder = true;
    }

    // Rearranges the positions of the timers
    public void RearrangeTimers(int removeIndex)
    {

        for (int i = orders.Count-1; i > removeIndex; i--)
        {
            float time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).normalizedTime;

            GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), orders[i].GetTimeMultiplier());
            GetComponent<Animator>().SetFloat("Time" + (i).ToString(), orders[i].GetTotalTime() - orders[i].GetTotalTime()* (time % 1));
            GetComponent<Animator>().Play("Timer", i-1, time%1);   
        }

        GetComponent<Animator>().SetFloat("Speed" + (orders.Count).ToString(), 0);
        GetComponent<Animator>().SetFloat("Time" + (orders.Count).ToString(), 0);
        GetComponent<Animator>().Play("OrderExit", orders.Count - 1);
    }

    // Checks whether the time bar ended or not.
    public void CheckTimeOut()
    {
        //for (int i = 0; i < orders.Count; i++)
        //{
        //    if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).IsName("OrderExit")){
        //        // Decrease the score, then remove the order
        //        GM.score -= orders[i].GetPrice()/2;
        //        StartCoroutine(RemoveOrder(i));
        //        break;
        //    }

//        }
    }

}
