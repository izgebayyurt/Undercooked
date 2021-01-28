using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++){
            Button btn = levelButtons[i].GetComponent<Button>();
            btn.onClick.AddListener(() => StartLevel(i));
        }

    }

    void StartLevel(int idx)
    {
        SceneManager.LoadScene("Level" + idx);
    }

}
