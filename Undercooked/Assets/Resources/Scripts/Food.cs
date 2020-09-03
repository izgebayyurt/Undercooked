using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Food : ScriptableObject
{
    public new string name;

    public Sprite artwork;

    public bool cuttable;
    public bool cookable;
    public bool cut;
    public bool cooked;

    public bool visible;

    public bool IsCuttable()
    {
        return cuttable;
    }
    public bool IsCut()
    {
        return cut;
    }
    public bool IsCookable()
    {
        return cookable;
    }
    public bool IsCooked()
    {
        return cooked;
    }

    public string GetName()
    {
        return name;
    }

    public Sprite GetSprite()
    {
        return artwork;
    }

    public bool IsVisible()
    {
        return visible;
    }

    public void SetVisible(bool v)
    {
        visible = v;
    }
}

