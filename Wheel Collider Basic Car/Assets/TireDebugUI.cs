using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TireDebugUI : MonoBehaviour
{
    public TMP_Text slipValueText;
    public WheelCollider wheelCollider;

    WheelHit hit = new WheelHit();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wheelCollider.GetGroundHit(out hit))
        {
            slipValueText.text = hit.forwardSlip.ToString("F1");
        }

        //slipValueText.text = wheelCollider.rpm.ToString("F0");
    }
}
