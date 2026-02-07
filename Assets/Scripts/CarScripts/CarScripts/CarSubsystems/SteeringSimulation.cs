using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class SteeringSimulation
{
    [Header("Steering Range")]
    [SerializeField] private float SteeringRangeAtZeroSpeed = 35f;
    [SerializeField] private float SteeringRangeAtMaxSpeed = 1f;
    [SerializeField] private float MaxSpeedForSteering = 200f; 
    
    [Header("Steering Response")]
    [SerializeField] private float SteerSpeed = 180f; 
    [Tooltip("Returning to center obviously faster")]
    [SerializeField] private float ReturnSpeed = 240f;
    
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
