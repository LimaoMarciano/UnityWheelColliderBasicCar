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

    public void SetDebugModeSideSlip ()
    {
        FL.wheelDebugType = TireDebugUI.WheelDebugType.SDW_SLIP;
        FR.wheelDebugType = TireDebugUI.WheelDebugType.SDW_SLIP;
        RL.wheelDebugType = TireDebugUI.WheelDebugType.SDW_SLIP;
        RR.wheelDebugType = TireDebugUI.WheelDebugType.SDW_SLIP;
    }

    public void SetDebugModeFwdSlip()
    {
        FL.wheelDebugType = TireDebugUI.WheelDebugType.FWD_SLIP;
        FR.wheelDebugType = TireDebugUI.WheelDebugType.FWD_SLIP;
        RL.wheelDebugType = TireDebugUI.WheelDebugType.FWD_SLIP;
        RR.wheelDebugType = TireDebugUI.WheelDebugType.FWD_SLIP;
    }

    public void SetDebugModeAllSlip()
    {
        FL.wheelDebugType = TireDebugUI.WheelDebugType.ALL_SLIP;
        FR.wheelDebugType = TireDebugUI.WheelDebugType.ALL_SLIP;
        RL.wheelDebugType = TireDebugUI.WheelDebugType.ALL_SLIP;
        RR.wheelDebugType = TireDebugUI.WheelDebugType.ALL_SLIP;
    }

    public void SetDebugModeForce()
    {
        FL.wheelDebugType = TireDebugUI.WheelDebugType.FORCE;
        FR.wheelDebugType = TireDebugUI.WheelDebugType.FORCE;
        RL.wheelDebugType = TireDebugUI.WheelDebugType.FORCE;
        RR.wheelDebugType = TireDebugUI.WheelDebugType.FORCE;
    }
}
