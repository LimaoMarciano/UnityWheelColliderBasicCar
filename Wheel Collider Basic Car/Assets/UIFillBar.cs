using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFillBar : MonoBehaviour
{

    public Image fillImage;
    public TMP_Text text;
    public float maxValue = 100f;

    public void SetValue (float value)
    {
        fillImage.fillAmount = Mathf.Clamp01(value / maxValue);
        text.text = value.ToString("F0");
    }
}
