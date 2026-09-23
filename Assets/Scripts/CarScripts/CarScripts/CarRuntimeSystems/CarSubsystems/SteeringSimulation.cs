using Unity.Mathematics;

[System.Serializable]
public struct SteeringSimulation
{ 
    public void UpdatePhysics(ref VehicleData vehicleData, ref SteeringData steeringData, ref SteeringConfig steeringConfig, ref PlayerInput playerInput, ref float DeltaTime)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = math.clamp(vehicleData.SpeedKmH / steeringConfig.MaxSpeedForSteering, 0, 1);
        float currentMaxAngle = math.lerp(steeringConfig.SteeringRangeAtZeroSpeed, steeringConfig.SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = math.abs(steeringInput) > 0.01f ? steeringConfig.SteerSpeed : steeringConfig.ReturnSpeed;
        steeringData.CurrentSteeringAngle = AuxiliaryMathf.MoveTowards(steeringData.CurrentSteeringAngle, targetAngle, speed * DeltaTime); 
    }

}
