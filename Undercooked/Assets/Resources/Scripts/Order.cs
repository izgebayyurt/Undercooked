using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Order : ScriptableObject
{
    public float totalTime;
    public float timeLeft;

    public int price;
    public float tipMultiplier;

    public Sprite artwork;

    public List<string> ingredients; // Write the ingredients one by one in here

    public int GetPrice()
    {
        return (int)(price + timeLeft * tipMultiplier);
    }

    public int GetTip()
    {
        return (int)(timeLeft * tipMultiplier);
    }

    public Sprite GetSprite()
    {
        return artwork;
    }

    public List<string> GetIngredients()
    {
        return ingredients;
    }

    public float GetTimeMultiplier()
    {
        return 60f / totalTime;
    }

    public float GetTotalTime()
    {
        return totalTime;
    }
}
