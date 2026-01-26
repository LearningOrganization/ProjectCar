using UnityEngine;

[System.Serializable]
public struct CarPhysicData
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
    public float SteeringAngle;

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
    public void UpdatePhysics(ref CarPhysicData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class TransmissionSimulation
{
    public void UpdatePhysics(ref CarPhysicData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class BreakingSimulation
{
    public void UpdatePhysics(ref CarPhysicData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class SteeringSimulation
{
    [Header("Steer wheels")]
    public WheelCollider[] SteeringWheels;

    [Header("Steering params")]
    public float SteeringRange = 30f;
    public float SteerSpeed = 180f;
    public float SteeringRangeAtMaxSpeed = 5f;

    public void UpdatePhysics(ref CarPhysicData data, ref PlayerInput playerInput)
    {
        
    }
}

