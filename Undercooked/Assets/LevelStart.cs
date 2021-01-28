using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{

    public GameManager GM;

    // Update is called once per frame
    void Update()
    {
        if (!GM.gamePaused) // if the game starts, close the starting screen
        {
            gameObject.SetActive(false);
        }
    }
}
