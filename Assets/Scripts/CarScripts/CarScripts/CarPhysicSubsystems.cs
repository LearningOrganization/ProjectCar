using System;
using UnityEngine;

[System.Serializable]
public struct CarPhysicsData
{
    // ===== ENGINE DATA =====
    public float EngineRPM;
    public float EngineTorque;
    public float EngineBraking;

    // ===== TRANSMISSION DATA =====
    public int CurrentGear;
    public float CurrentGearRatio;
    public float FinalDriveRatio;
    public float ClutchEngagement;

    // ===== WHEEL DATA =====
    public float WheelRPM;
    public float WheelTorque;
    
    // ===== BRAKE DATA =====
    public float BrakeTorque;

    // ===== STEERING DATA =====
    public float CurrentSteeringAngle ; 
    // public float TargetSteerAngle; // target steering angle
    // public float MaxSteerAngle; // at current speed

    // ===== VEHICLE DATA =====
    public float SpeedKmH;
    public float SpeedMS;
    public Vector3 Velocity;
    public float Mass;

    // ===== AERODYNAMICS ===== i hope for future

    // ===== SYSTEM =====
    public float DeltaTime;
}

[System.Serializable]
public class EngineSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class TransmissionSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class BreakingSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class SteeringSimulation
{
    [Header("Steering Range")]
    [SerializeField] private float SteeringRangeAtZeroSpeed = 35f;
    [SerializeField] private float SteeringRangeAtMaxSpeed = 5f;
    [SerializeField] private float MaxSpeedForSteering = 200f; 
    
    [Header("Steering Response")]
    [SerializeField] private float SteerSpeed = 180f; 
    [Tooltip("Returning to center obviously faster")]
    [SerializeField] private float ReturnSpeed = 240f;
    
    [Header("Deadzone (optional)")]
    [SerializeField] private float InputDeadzone = 0.1f;
    
    private float _currentSteerAngle = 0f;
    
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = Mathf.Clamp01(data.SpeedKmH / MaxSpeedForSteering);
        float currentMaxAngle = Mathf.Lerp(SteeringRangeAtZeroSpeed, SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = Mathf.Abs(steeringInput) > 0.01f ? SteerSpeed : ReturnSpeed;
        _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, targetAngle, speed * data.DeltaTime);
        
        data.CurrentSteeringAngle = _currentSteerAngle;
    }

}

