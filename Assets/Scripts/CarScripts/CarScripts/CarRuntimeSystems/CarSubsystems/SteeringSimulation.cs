using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class SteeringSimulation
{
  
    private float _currentSteerAngle = 0f;
    
    public void UpdatePhysics(ref CarPhysicsData data, ref SteeringConfig steeringConfig, ref PlayerInput playerInput)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = Mathf.Clamp01(data.Vehicle.SpeedKmH / steeringConfig.MaxSpeedForSteering);
        float currentMaxAngle = Mathf.Lerp(steeringConfig.SteeringRangeAtZeroSpeed, steeringConfig.SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = Mathf.Abs(steeringInput) > 0.01f ? steeringConfig.SteerSpeed : steeringConfig.ReturnSpeed;
        _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, targetAngle, speed * data.DeltaTime);
        
        data.Steering.CurrentSteeringAngle = _currentSteerAngle;
    }

}
