using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextGlow : MonoBehaviour
{
    private Material txt;
    private static float frequencyMuliptlier = 4f;
    private static float intensityMuliptlier = 0.1f;

    void Start()
    {
        txt = transform.GetComponent<TextMeshProUGUI>().fontMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        float dilate = (1 + Mathf.Cos(Time.unscaledTime * frequencyMuliptlier)) * intensityMuliptlier;
        txt.SetFloat(ShaderUtilities.ID_FaceDilate, dilate);
    }
}
