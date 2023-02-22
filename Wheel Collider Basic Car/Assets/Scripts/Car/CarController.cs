using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Car targetCar;
    
    public float maxRpmAllowedReverse = 200f;
    
    // Start is called before the first frame update
    void Start()
    {
        targetCar = GetComponent<Car>();
    }

    // Update is called once per frame
    void Update()
    {
        targetCar.Throttle = Input.GetAxis("Throttle");

        targetCar.ApplyBrakeForce (Input.GetAxis("Brake"));

        targetCar.ApplySteering(Input.GetAxis("Horizontal"));

       
        if (Input.GetButtonDown("IncreaseGear") && targetCar.CurrentGear < targetCar.gearRatios.Length - 1)
        {
            if (targetCar.CurrentGear < 0 && targetCar.EngineRpm > maxRpmAllowedReverse)
                return;
            
            targetCar.SetGear(targetCar.CurrentGear + 1);
        }

        if (Input.GetButtonDown("DecreaseGear") && targetCar.CurrentGear >= -1)
        { 
            if (targetCar.CurrentGear == 0 && targetCar.EngineRpm > maxRpmAllowedReverse)
                return;
        
            targetCar.SetGear(targetCar.CurrentGear - 1);
        }
    }
}
