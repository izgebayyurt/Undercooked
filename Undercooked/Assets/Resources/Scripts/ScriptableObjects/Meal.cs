using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Meal : ScriptableObject
{
    public Food[] ingredients;
    public new string name;
    public Sprite artwork;

    public string GetName()
    {
        return name;
    }

    public Sprite GetSprite()
    {
        return artwork;
    }

}
