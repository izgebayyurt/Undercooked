using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer : MonoBehaviour
{
    public GameManager GM;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        GM = GM.GetComponent<GameManager>();
        animator = transform.GetComponent<Animator>();
        animator.SetFloat("LevelTime", GM.GetTotalLevelTime());
        animator.SetFloat("TimerSpeed", 60/GM.GetTotalLevelTime());
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("LevelTime", GM.GetRemainingLevelTime());
    }
}
