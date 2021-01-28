using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public GameManager GM;

    public List<Order> orders = new List<Order>(); // Pull orders to display from the Queue
    public Order[] possibleOrders; // Orders that can be added the Queue

    public static int maxOrderCount; // Maximum amount of orders that can appear
    public float timeBetweenOrders = 5; // Time that must pass between two orders

    bool actionHappening = false; // Unused for now

    /* Animation Fields */
    public float OrderAddUIBounciness; // Determines how bouncy the entrance of Order UI is
    public float OrderRemoveUIBounciness; // Determines how bouncy the exit of Order UI is
    public Vector3[] OrderUIPoisitons;

    // Animation curves
    [SerializeField] public AnimationCurve OrderAddAnim;
    [SerializeField] public AnimationCurve OrderRemoveAnim;

    void Start()
    {
        // Initialize values
        maxOrderCount = transform.childCount;
        OrderUIPoisitons = new Vector3[maxOrderCount];
        InitializeOrderUIPositions();

        // Start adding orders
        InvokeRepeating("AddOrder", 2.0f, 5f);
    }

    void Update()
    {
        DecreaseTime();
        CheckTimeOut();
    }

    void AddOrder()
    {
        // if we have max orders, return
        if (orders.Count == maxOrderCount){
            return;
        }

        // Pick a random possible order
        Order order = Instantiate(possibleOrders[Random.Range(0, possibleOrders.Length)]);

        // Set the index of the order and activate it
        order.SetOrderIndex(orders.Count);
        order.SetActive(true);

        // Set the timer
        order.SetTimeLeft(order.GetTotalTime());
        GetComponent<Animator>().SetFloat("Speed" + order.GetOrderIndex().ToString(), order.GetTimeMultiplier());
        GetComponent<Animator>().SetFloat("Time" + order.GetOrderIndex().ToString(), order.GetTotalTime());

        // Play animation
        GetComponent<Animator>().Play("Timer", order.GetOrderIndex());

        StartCoroutine(EnterAnimation(order.GetOrderIndex(), order));

        // Add it to the order list
        orders.Add(order);

        // UI animation
        UpdateUI();


        //RearrangeTimers();
    }

    public IEnumerator RemoveOrder(int orderIndex)
    {
        //if(actionHappening){
        //    yield return new WaitForSeconds(0.01f);
        //}

        if (orders[orderIndex].GetTimeLeft() <= 0){
            orders[orderIndex].SetActive(false);
        }

        yield return StartCoroutine(ExitAnimation(orderIndex));

        for (int i = orderIndex; i < orders.Count; i++)
        {
            orders[i].SetOrderIndex(orders[i].GetOrderIndex() - 1);
        }
        // Remove the order from the list
        // orders.RemoveAt(orderIndex);
        orders.RemoveAt(orderIndex);

        UpdateUI();
        RearrangeTimers(orderIndex);
        //actionHappening = false;
    }

    // Checks whether the time bar ended or not.
    public void CheckTimeOut()
    {

        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].GetTimeLeft() <= 0 && orders[i].GetActive())
            {

                // Decrease the score, then remove the order
                GM.ChangeScore(-orders[i].GetPrice() / 2);

                StartCoroutine(RemoveOrder(i));
                break;
            }
        }
    }

    public void DecreaseTime()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            float timeLeft = orders[i].GetTimeLeft() - Time.deltaTime;
            orders[i].SetTimeLeft(timeLeft);
            GetComponent<Animator>().SetFloat("Time" + i.ToString(), timeLeft);
        }
    }

    void UpdateUI()
    {

        // Update the sprites of the orders
        for (int i = 0; i < orders.Count; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = orders[i].GetSprite();
        }

        // Make the sprites of non-existing orders transparent
        for (int i = orders.Count; i < maxOrderCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = null;
            transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

    }

    // Rearranges the positions of the timers
    public void RearrangeTimers(int removeIndex)
    {

        for (int i = removeIndex; i < orders.Count; i++)
        {
            if (i == maxOrderCount - 1){
                GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), 0);
                GetComponent<Animator>().SetFloat("Time" + (i).ToString(), 0);
                GetComponent<Animator>().Play("OrderEnter", i);
                continue;
            }

            float normalizedTime = GetComponent<Animator>().GetCurrentAnimatorStateInfo(i+1).normalizedTime;

            GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), orders[i].GetTimeMultiplier());
            GetComponent<Animator>().SetFloat("Time" + (i).ToString(), orders[i].GetTimeLeft());
            GetComponent<Animator>().Play("Timer", i, normalizedTime);
        }

        //for (int i = orders.Count - 1; i > removeIndex; i--)
        //{
        //    float time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).normalizedTime;

        //    GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), orders[i].GetTimeMultiplier());
        //    GetComponent<Animator>().SetFloat("Time" + (i).ToString(), orders[i].GetTotalTime() - orders[i].GetTotalTime() * (time % 1));
        //    GetComponent<Animator>().Play("Timer", i - 1, time % 1);
        //}

        for (int i = orders.Count; i < maxOrderCount; i++)
        {
            GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), 0);
            GetComponent<Animator>().SetFloat("Time" + (i).ToString(), 0);
            GetComponent<Animator>().Play("OrderEnter", i);
        }
    }

    IEnumerator EnterAnimation(int orderIndex, Order order)
    {
        //if (actionHappening)
        //{
        //    yield return new WaitForSeconds(0.1f);
        //}

        transform.GetChild(orderIndex).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        transform.GetChild(orderIndex).GetComponent<Image>().sprite = order.GetSprite();

        float startTime = OrderAddAnim[0].time;
        float endTime = OrderAddAnim[OrderAddAnim.length - 1].time;

        // Starting position of the orders
        float startPosX = OrderUIPoisitons[orderIndex].x;
        float startPosY = OrderUIPoisitons[orderIndex].y;

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



        //actionHappening = false;
    }

    IEnumerator ExitAnimation(int orderIndex)
    {

        actionHappening = true;

        float startTime = OrderRemoveAnim[0].time;
        float endTime = OrderRemoveAnim[OrderRemoveAnim.length - 1].time;

        float startPosX = OrderUIPoisitons[orderIndex].x;
        float startPosY = OrderUIPoisitons[orderIndex].y;


        // Bounce Animation
        for (float t = startTime; t < endTime; t += Time.deltaTime)
        {

            float value = OrderRemoveAnim.Evaluate(t); // Get the value according to the animation curve

            transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition =
                 new Vector3(startPosX + value * OrderRemoveUIBounciness,
                             startPosY);

            yield return null;
        }

        // Debug.Log("Before: " + startPosX + " " + startPosY + " " + orderIndex);

        // Put the order back where it was once the animation ends
        transform.GetChild(orderIndex).GetComponent<RectTransform>().anchoredPosition =
                new Vector3(startPosX,
                            startPosY);

        // Debug.Log("After: " + startPosX + " " + startPosY + " " + orderIndex);

        actionHappening = false;
    }

    // Accesses all the Order UI anchored locations and saves them
    void InitializeOrderUIPositions(){
        for (int i = 0; i < maxOrderCount; i++){
            Vector3 startPos = transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            OrderUIPoisitons[i] = startPos;
        }
    }


}
        









    //void Start()
    //{
    //    InvokeRepeating("AddOrder", 2.0f, 5f);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    DecreaseTime();
    //    CheckTimeOut();
    //}

    //void UpdateTimers()
    //{

    //    // Update the sprites of the orders
    //    for (int i = 0; i < orders.Count; i++)
    //    {
    //        transform.GetChild(i).GetComponent<Image>().sprite = orders[i].GetSprite();
    //    }

    //    // Make the sprites of non-existing orders transparent
    //    for (int i = orders.Count; i < maxOrderCount; i++)
    //    {
    //        transform.GetChild(i).GetComponent<Image>().sprite = null;
    //        transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    //    }

    //    // Rearrange the timers
    //    RearrangeTimers();
    //}

    //void UpdateUI(){

    //    // Update the sprites of the orders
    //    for (int i = 0; i < orders.Count; i++)
    //    {
    //        transform.GetChild(i).GetComponent<Image>().sprite = orders[i].GetSprite();
    //    }

    //    // Make the sprites of non-existing orders transparent
    //    for (int i = orders.Count; i < maxOrderCount; i++)
    //    {
    //        transform.GetChild(i).GetComponent<Image>().sprite = null;
    //        transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    //    }

    //    // Rearrange the timers
    //    RearrangeTimers();
    //}

    //// Rearranges the positions of the timers
    //public void RearrangeTimers()
    //{

    //    for (int i = 0; i < orders.Count; i++)
    //    {
    //        float time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).normalizedTime;

    //        GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), orders[i].GetTimeMultiplier());
    //        GetComponent<Animator>().SetFloat("Time" + (i).ToString(), orders[i].GetTimeLeft());
    //        // GetComponent<Animator>().Play("Timer", i - 1, time % 1);
    //    }

    //    //for (int i = orders.Count - 1; i > removeIndex; i--)
    //    //{
    //    //    float time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).normalizedTime;

    //    //    GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), orders[i].GetTimeMultiplier());
    //    //    GetComponent<Animator>().SetFloat("Time" + (i).ToString(), orders[i].GetTotalTime() - orders[i].GetTotalTime() * (time % 1));
    //    //    GetComponent<Animator>().Play("Timer", i - 1, time % 1);
    //    //}

    //    for (int i = orders.Count; i < maxOrderCount; i++)
    //    {
    //        GetComponent<Animator>().SetFloat("Speed" + (i).ToString(), 0);
    //        GetComponent<Animator>().SetFloat("Time" + (i).ToString(), 0);
    //        GetComponent<Animator>().Play("OrderEnter", i - 1);
    //    }


    //}



    //void AddOrder()
    //{

    //    //actionHappening = true; // if we started adding, do not add/remove another order

    //    // Pick a random possible order
    //    Order order = possibleOrders[Random.Range(0, possibleOrders.Length)];

    //    // Add it to the order list
    //    orders.Add(order);

    //    // UI animation
    //    StartCoroutine(EnterAnimation(orders.Count - 1, order));

    //    // Set the timer
    //    order.SetTimeLeft(order.GetTotalTime());
    //    GetComponent<Animator>().SetFloat("Speed" + (orders.Count-1).ToString(), order.GetTimeMultiplier());
    //    GetComponent<Animator>().SetFloat("Time" + (orders.Count-1).ToString(), order.GetTotalTime());
    //}

















    //// Adds an order to the list
    ////void AddOrder()
    ////{

    ////    actionHappening = true; // if we started adding, do not add/remove another order

    ////    // Pick a random possible order
    ////    Order order = possibleOrders[Random.Range(0, possibleOrders.Length)];

    ////    // Add it to the order list
    ////    orders.Add(order);

    ////    // UI animation
    ////    StartCoroutine(EnterAnimation(orders.Count - 1, order));

    ////    // Set the timer
    ////    GetComponent<Animator>().SetFloat("Speed" + orders.Count.ToString(), order.GetTimeMultiplier());
    ////    GetComponent<Animator>().SetFloat("Time" + orders.Count.ToString(), order.GetTotalTime());

    ////}

    //// UI animation after adding an item. Order index determines the slot to be updated.


    //public IEnumerator RemoveOrder(int orderIndex)
    //{
    //    if(actionHappening){
    //        yield return new WaitForSeconds(0.01f);
    //    }

    //    yield return StartCoroutine(ExitAnimation(orderIndex));



    //    // Remove the order from the list
    //    orders.RemoveAt(orderIndex);
    //    actionHappening = false;
    //}




    //// Checks whether the time bar ended or not.
    //public void CheckTimeOut()
    //{
    //    for (int i = 0; i < orders.Count; i++)
    //    {
    //        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(i).IsName("OrderExit")){
    //            // Debug.Log("TimeOut: " + i);

    //            // Decrease the score, then remove the order
    //            GM.score -= orders[i].GetPrice()/2;

    //            GetComponent<Animator>().Play("OrderEnter", i);
    //            StartCoroutine(RemoveOrder(i));
    //            break;
    //        }

    //    }
    //}

    //public void DecreaseTime()
    //{
    //    for (int i = 0; i < orders.Count; i++)
    //    {
    //        float timeLeft = orders[i].GetTimeLeft() - Time.deltaTime;
    //        orders[i].SetTimeLeft(timeLeft);
    //        GetComponent<Animator>().SetFloat("Time" + i.ToString(), timeLeft);
    //    }
    //}

