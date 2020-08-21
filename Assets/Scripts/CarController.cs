using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; 
    public float maxMotorTorque; 
    public float maxSteeringAngle;
    public float maxBrakeTorque;

    public float CurrentAcceleration
    {
        get => m_currentAcceleration;
        set => m_currentAcceleration = Mathf.Clamp(value, -1f, 1f);
    }

    public float CurrentBrakeTorque
    {
        get => m_currentBrakeTorque;
        set
        {
            m_currentBrakeTorque = value <= 0.8f ? 0f : value;
        } 
    }

    public float CurrentSteeringAngle
    {
        get => m_currentSteeringAngle;
        set => m_currentSteeringAngle = Mathf.Clamp(value,-1f,1f);
    }

    private float m_currentSteeringAngle = 0f;
    private float m_currentAcceleration = 0f;
    private float m_currentBrakeTorque = 0f;

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * m_currentAcceleration;
        float steering = maxSteeringAngle * m_currentSteeringAngle;
        float brake = maxBrakeTorque * m_currentBrakeTorque;
       // Debug.Log(motor + "," + steering + "," + brake);
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
        }
    }
}
[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; 
    public bool steering;
    public bool brake;
}