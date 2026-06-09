using Unity.Mathematics;
using UnityEngine;

// TODO: make burst compilable
[System.Serializable]
public struct SteeringSimulation
{ 
    public void UpdatePhysics(ref CarPhysicsData data, ref SteeringConfig steeringConfig, ref PlayerInput playerInput)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = math.clamp(data.Vehicle.SpeedKmH / steeringConfig.MaxSpeedForSteering, 0, 1);
        float currentMaxAngle = math.lerp(steeringConfig.SteeringRangeAtZeroSpeed, steeringConfig.SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = math.abs(steeringInput) > 0.01f ? steeringConfig.SteerSpeed : steeringConfig.ReturnSpeed;
        data.Steering.CurrentSteeringAngle = Mathf.MoveTowards(data.Steering.CurrentSteeringAngle, targetAngle, speed * data.DeltaTime); // Mathf.MoveTowards should be replaced by auxiliary method in additional method
        
    }

}
