using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CarUI : MonoBehaviour
{
    public Car targetCar;
    public TMP_Text currentGearText;
    public TMP_Text currentTorqueText;
    public TMP_Text currentSpeed;
    public UIFillBar rpmBar;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCar.CurrentGear >= 0)
            currentGearText.text = (targetCar.CurrentGear + 1).ToString();
        else
            currentGearText.text = "R";
        currentTorqueText.text = targetCar.Torque.ToString("F0");
        rpmBar.SetValue(targetCar.EngineRpm);
        currentSpeed.text = (targetCar.GetCurrentRigidbodySpeed() * 3.6f).ToString("F0");
    }
}
