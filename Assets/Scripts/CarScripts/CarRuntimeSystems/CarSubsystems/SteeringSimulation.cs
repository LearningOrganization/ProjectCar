using Unity.Burst;
using Unity.Mathematics;

[System.Serializable]
[BurstCompile]

public struct SteeringSimulation
{ 
    public void UpdatePhysics(ref VehicleData vehicleData, ref SteeringData steeringData, ref SteeringConfig steeringConfig, ref PlayerInput playerInput, ref float DeltaTime)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = math.clamp(vehicleData.SpeedKmH / steeringConfig.MaxSpeedForSteering, 0, 1);
        float currentMaxAngle = math.lerp(steeringConfig.SteeringRangeAtZeroSpeed, steeringConfig.SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = math.select(steeringConfig.ReturnSpeed, steeringConfig.SteerSpeed, math.abs(steeringInput) > 0.01f);
        steeringData.CurrentSteeringAngle = AuxiliaryMathf.MoveTowards(steeringData.CurrentSteeringAngle, targetAngle, speed * DeltaTime); 
    }

}
