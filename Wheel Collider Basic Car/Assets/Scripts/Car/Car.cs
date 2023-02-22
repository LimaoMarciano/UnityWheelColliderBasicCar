using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public enum DriveType { FWD, RWD, AWD };

    // Public variables
    public AnimationCurve powerCurve;
    public float engineBrakeCoefficient = 0.2f;
    public float driveTrainEfficiency = 0.75f;
    public float turnRadius = 10f;
    public float brakeForce = 2000f;
    [Range(0,1)]
    public float brakeBias = 0.54f;
    [Range(0,10)]
    public float[] gearRatios;
    [Range(0, 10)]
    public float finalDriveRatio = 4.1f;
    [Range(0, 10)]
    public float reverseGearRatio = 3.28f;
    public DriveType driveType;
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public Vector3 centerOfMass;
    public float dragCoeficcient = 0.35f;
    public float frontalArea = 2f;

    //Private variables
    private float engineRpm;
    private float torque;
    private float throttle = 0f;
    private int currentGear = 0;
    private float currentGearRatio;
    private bool isOnReverse;
    private List<WheelCollider> drivenWheels = new List<WheelCollider>();
    private Rigidbody rb;
    
    //AckermannSteeringVariables
    private float rearAxleTrack;
    private float wheelBase;

    private float frontLeftWheelFwdSlip;
    private float frontRightWheelFwdSlip;
    private float rearLeftWheelFwdSlip;
    private float rearRightWheelFwdSlip;

    // Getters
    public int CurrentGear { get { return currentGear; } }
    public float EngineRpm { get { return engineRpm; }}
    public float Torque { get { return torque; }}
    public float Throttle {
        get { return throttle; }
        set { throttle = Mathf.Clamp01(value); }
    }

    // Start is called before the first frame update
    void Start()
    {

        //Creates list with driven wheels. FWD = Front wheels; RWD = Rear wheels; AWD = All wheels
        drivenWheels.Clear();

        switch (driveType)
        {
            case DriveType.FWD:
                drivenWheels.Add(frontLeftWheel);
                drivenWheels.Add(frontRightWheel);
                break;
            case DriveType.RWD:
                drivenWheels.Add(rearLeftWheel);
                drivenWheels.Add(rearRightWheel);
                break;
            case DriveType.AWD:
                drivenWheels.Add(frontLeftWheel);
                drivenWheels.Add(frontRightWheel);
                drivenWheels.Add(rearLeftWheel);
                drivenWheels.Add(rearRightWheel);
                break;
        }

        //Calculates wheel base (distance between car's two axles) and rear axle track (distance between rear wheels).
        //This is necessary for Ackermann steering calculation
        Vector3 frontAxleMidPoint = (frontLeftWheel.transform.position + frontRightWheel.transform.position) / 2f;
        Vector3 rearAxleMidPoint = (rearLeftWheel.transform.position + rearRightWheel.transform.position) / 2f;
        wheelBase = Vector3.Distance(frontAxleMidPoint, rearAxleMidPoint);
        rearAxleTrack = Vector3.Distance(rearLeftWheel.transform.position, rearRightWheel.transform.position);
        
        SetGear(0);

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;

        //This is important for low speed/high torque situations. It's setting 8 substeps for speeds below 5m/s
        frontLeftWheel.ConfigureVehicleSubsteps(5, 8, 4);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //For figuring out engine rpm, we'll use the average rpm of the driven wheels
        float wheelRpmAvg = 0f;
        foreach (WheelCollider wheelCollider in drivenWheels)
        {
            wheelRpmAvg += wheelCollider.rpm;
        }
        
        wheelRpmAvg /= drivenWheels.Count;

        //Calculate more realistic drag force using formula D = 1/2 * p * v² * cD * A
        float speed = GetCurrentRigidbodySpeed();
        float dragForce = 0.5f * Mathf.Pow(speed, 2) * dragCoeficcient * frontalArea;
        rb.AddForce(-dragForce * rb.velocity.normalized);

        //Engine rpm will be the driven wheels rpm multiplied by the current gear ratio
        engineRpm = Mathf.Abs(wheelRpmAvg * currentGearRatio * finalDriveRatio);
  

        //Apply torque based on torque curve and transmission ratios or engine braking if no throttle
        if (throttle > 0)
            torque = powerCurve.Evaluate(engineRpm) * throttle * currentGearRatio * finalDriveRatio * driveTrainEfficiency;
        else
            torque = -engineBrakeCoefficient * engineRpm / 60;

        //Aproximation of a open differenctial. Will direct torque to the wheel with more slip
        frontLeftWheelFwdSlip = GetWheelColliderFwdSlip(frontLeftWheel);
        frontRightWheelFwdSlip = GetWheelColliderFwdSlip(frontRightWheel);
        rearLeftWheelFwdSlip = GetWheelColliderFwdSlip(rearLeftWheel);
        rearRightWheelFwdSlip = GetWheelColliderFwdSlip(rearRightWheel);

        float slipRatio = 0.5f;

        switch (driveType)
        {
            case DriveType.FWD:
                slipRatio = GetDifferentialTorqueSplit(frontLeftWheelFwdSlip, frontLeftWheelFwdSlip);
                frontLeftWheel.motorTorque = torque * (1f - slipRatio);
                frontRightWheel.motorTorque = torque * slipRatio;
                break;
            case DriveType.RWD:
                slipRatio = GetDifferentialTorqueSplit(rearLeftWheelFwdSlip, rearLeftWheelFwdSlip);
                rearLeftWheel.motorTorque = torque * (1f - slipRatio);
                rearRightWheel.motorTorque = torque * slipRatio;
                break;
            case DriveType.AWD:
                slipRatio = GetDifferentialTorqueSplit(frontLeftWheelFwdSlip, frontLeftWheelFwdSlip);
                frontLeftWheel.motorTorque = torque * (1f - slipRatio) * 0.5f;
                frontRightWheel.motorTorque = torque * slipRatio * 0.5f;

                slipRatio = GetDifferentialTorqueSplit(rearLeftWheelFwdSlip, rearLeftWheelFwdSlip);
                rearLeftWheel.motorTorque = torque * (1f - slipRatio) * 0.5f;
                rearRightWheel.motorTorque = torque * slipRatio * 0.5f;
                break;
        }

    }
    
    float GetDifferentialTorqueSplit (float leftWheelSlip, float rightWheelSlip)
    {
        float slipRatio;

        leftWheelSlip = Mathf.Abs(leftWheelSlip);
        rightWheelSlip = Mathf.Abs(rightWheelSlip);

        if (leftWheelSlip != 0f && rightWheelSlip != 0f)
            slipRatio = leftWheelSlip / (leftWheelSlip + rightWheelSlip);
        else
            slipRatio = 0.5f;

        return slipRatio;
    }

    float GetWheelColliderFwdSlip (WheelCollider wheel)
    {
        float slip = 1f;
        WheelHit hit;
        if (wheel.GetGroundHit(out hit))
        {
            slip = hit.forwardSlip;
        }

        return slip;
    }

    /// <summary>
    /// Calculate steering angle. Input value is in range between -1 (left) and +1 (right).
    /// The maximum turn radius is set on turnRadius variable.
    /// </summary>
    /// <param name="steeringInput">Value between -1 (left) and +1 (right)</param>
    Vector2 CalculateAckermannSteering(float steeringInput, float wheelBase, float turnRadius, float rearAxleTrack)
    {
        Vector2 steeringAngles = Vector2.zero;

        if (steeringInput > 0) //Turning right
        {
            steeringAngles.x = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearAxleTrack / 2))) * steeringInput;
            steeringAngles.y = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearAxleTrack / 2))) * steeringInput;
        }
        else if (steeringInput < 0) //Turning left
        {
            steeringAngles.x = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearAxleTrack / 2))) * steeringInput;
            steeringAngles.y = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearAxleTrack / 2))) * steeringInput;
        }

        return steeringAngles;
    }

    public void ApplySteering (float input)
    {
        Vector2 steeringAngles = CalculateAckermannSteering(input, wheelBase, turnRadius, rearAxleTrack);
        frontLeftWheel.steerAngle = steeringAngles.x;
        frontRightWheel.steerAngle = steeringAngles.y;
    }

    public void ApplyBrakeForce (float input)
    {
        frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = (brakeForce * brakeBias * input) / 2f;
        rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = (brakeForce * (1 - brakeBias) * input) / 2f;
    }


    /// <summary>
    /// Sets current transmission gear. If set to a negative value, it means is on reverse gear.
    /// The value is always clamped to the set number of gears
    /// </summary>
    /// <param name="gear"></param>
    public void SetGear (int gear)
    {
        if (gear < 0)
        {
            currentGearRatio = -reverseGearRatio;
            currentGear = -1;
            isOnReverse = true;
        }
        else
        {
            isOnReverse = false;
            currentGear = Mathf.Clamp(gear, 0, gearRatios.Length - 1);
            currentGearRatio = gearRatios[currentGear];
        }
    }

    public float GetCurrentRigidbodySpeed ()
    {
        return transform.InverseTransformDirection(rb.velocity).z;
    }

    private void OnDrawGizmos()
    {
        drawCenterOfMass();
    }

    // Gizmos drawing methods
    //========================================================================

    /// <summary>
    /// Draws the vehicle center of mass on Scene view
    /// </summary>
    private void drawCenterOfMass()
    {
        Vector3 pos = transform.position;
        pos += transform.right * centerOfMass.x;
        pos += transform.up * centerOfMass.y;
        pos += transform.forward * centerOfMass.z;

        drawGizmoAtPosition(pos, 0.5f, Color.yellow);
    }


    /// <summary>
    /// Draws a 3D cross gizmo at position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    private void drawGizmoAtPosition(Vector3 pos, float size, Color color)
    {
        float halfSize = size * 0.5f;
        Debug.DrawLine(new Vector3(pos.x + halfSize, pos.y, pos.z), new Vector3(pos.x - halfSize, pos.y, pos.z), color);
        Debug.DrawLine(new Vector3(pos.x, pos.y + halfSize, pos.z), new Vector3(pos.x, pos.y - halfSize, pos.z), color);
        Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z + halfSize), new Vector3(pos.x, pos.y, pos.z - halfSize), color);
    }
}
