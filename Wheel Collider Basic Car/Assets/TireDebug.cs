using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireDebug : MonoBehaviour
{
    public Car targetCar;
    public TireDebugUI FL;
    public TireDebugUI FR;
    public TireDebugUI RL;
    public TireDebugUI RR;
    
    // Start is called before the first frame update
    void Start()
    {
        FL.wheelCollider = targetCar.frontLeftWheel;
        FR.wheelCollider = targetCar.frontRightWheel;
        RL.wheelCollider = targetCar.rearLeftWheel;
        RR.wheelCollider = targetCar.rearRightWheel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
