using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Player
    public GameObject player;

    // Managers 
    public GameObject SlotManager;
    private SlotManager SM;
    public GameObject OrderManager;

    // Inventory Management
    private int currentSlot = 0;
    public int slotCount;

    // Strings of food names
    public string[] slots;

    // To set up the level, add the available ingredients to the foods list.
    public Food[] foods;
    private readonly Dictionary<string, Food> foodDict = new Dictionary<string, Food>();

    // Global Timer
    public float totalLevelTime;
    private float remainingLevelTime;

    public bool gamePaused;

    // Chopping action
    public bool isChopping;

    // Score / Cash
    public int score;
    public TextMeshProUGUI cashTextUI;
    private TMP_Text cashText;

    // UI Management for overlays etc.
    public GameObject LevelUI;
    private GameObject UI_HUD;
    private GameObject UI_Overlays;
    private GameObject UI_PauseMenu;
    private GameObject UI_LevelStart;
    private GameObject UI_LevelEnd;


    void Awake()
    {
        PauseGame(); // Start the level as paused

        // Get the UI elements
        UI_HUD = LevelUI.transform.Find("HUD").gameObject;
        UI_Overlays = LevelUI.transform.Find("Overlays").gameObject; 
        UI_PauseMenu = UI_Overlays.transform.Find("PauseMenu").gameObject;
        UI_LevelStart = UI_Overlays.transform.Find("LevelStart").gameObject;
        UI_LevelEnd = UI_Overlays.transform.Find("LevelEnd").gameObject;

        // Enable level start panel
        UI_LevelStart.SetActive(true);

        // Get the SlotManager
        SM = SlotManager.GetComponent<SlotManager>();
    }

    void Start()
    {
        SetupFoodDict();
        SetupSlots();

        // Get text mesh pro from the ui object
        cashText = cashTextUI.GetComponent<TMP_Text>();

        remainingLevelTime = totalLevelTime;
    }

    void Update()
    {
        UpdateGameTime();

        // Press any key to start the game
        if (UI_LevelStart.gameObject.activeSelf && Input.anyKey){
            UI_LevelStart.gameObject.SetActive(false);
            ResumeGame();
        }
    }

    // Getters & Setters
    public Food GetCurrentSlotFood() { return foodDict[slots[currentSlot]] ;}
    public void SetCurrentSlotString(string name) { slots[currentSlot] = name ;}
    public string GetCurrentSlotString() { return slots[currentSlot] ;}
    public void EmptyCurrentSlot() { slots[currentSlot] = null ;}
    public int GetCurrentSlotIdx() { return currentSlot ;}
    public float GetRemainingLevelTime() { return remainingLevelTime ;}
    public float GetTotalLevelTime() { return totalLevelTime ;}

    // Load food objects into a dictionary
    private void SetupFoodDict()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            foodDict.Add(foods[i].name, foods[i]);
        }
    }


    // Setup the slots as null
    private void SetupSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = null;
        }
    }

    // Move the current slot index by 1, wrap around if its on the edge
    public void NextSlot(){
        currentSlot++;
        currentSlot = currentSlot % slotCount;
    }

    public void PreviousSlot(){
        if (currentSlot != 0)
        {
            currentSlot--;
        }
        else
        {
            currentSlot = slotCount - 1;
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
        SM.UpdateInventoryUI();
    }

    // Changes the score, and calls the function that updates the UI
    public void ChangeScore(int s){
        score += s;
        UpdateScoreUI(s);
    }

    public void UpdateScoreUI(int s){
        cashText.text = score.ToString();
        cashText.color = s > 0 ? new Color32(20, 220, 20, 255) : new Color32(220, 20, 20, 255);
        StartCoroutine(CashUIAnimation(60));
    }

    // Fade effect on the cash text
    IEnumerator CashUIAnimation(int framesToFade){
        float r = cashText.color.r * 255;
        float g = cashText.color.g * 255;
        float b = cashText.color.b * 255;

        for (int i = 1; i < framesToFade+1; i++){
            byte new_r = (byte)(r + ((255 - r) / framesToFade) * i);
            byte new_g = (byte)(g + ((255 - g) / framesToFade) * i);
            byte new_b = (byte)(b + ((255 - b) / framesToFade) * i);

            cashText.color = new Color32(new_r, new_g, new_b, 255);
            yield return null;
        }
    }

    // Pause and resume game
    void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1;
    }

    void TogglePauseMenu()
    {
        if (UI_PauseMenu.gameObject.activeSelf)
        {
            UI_PauseMenu.gameObject.SetActive(false);
        }
        else
        {
            UI_PauseMenu.gameObject.SetActive(true);
        }
    }

    // Updates the game time, if it reaches zero takes action
    public void UpdateGameTime()
    {
        if (remainingLevelTime >= 0)
        {
            remainingLevelTime -= Time.deltaTime;
        }
        else
        {
            EndLevel();
        }
    }

    public void EndLevel()
    {
        // Pause game actions
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
        OrderManager.SetActive(false);
        UI_LevelEnd.SetActive(true); // Enable level end panel
    }


}
