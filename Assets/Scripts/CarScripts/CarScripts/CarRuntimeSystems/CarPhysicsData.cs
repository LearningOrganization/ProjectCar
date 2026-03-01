using UnityEngine;

[System.Serializable]
public struct CarPhysicsData
{
    // dynamic system data
    public EngineData Engine;
    public TransmissionData Transmission;
    public WheelData WheelData; 
    public SteeringData Steering;
    public BrakeData Brake;
    public AerodynamicData Aerodynamic;
    public VehicleData Vehicle;
    public TurboData Turbo;
    public float DeltaTime;
}

// === Dynamic Systems ===
[System.Serializable]
public struct EngineData
{
    public float EngineRPM;
    public float EngineTorque;
    public float EngineBraking;
    public float EngineInertia;
}
[System.Serializable]
public struct TransmissionData
{
    public int CurrentGear;
    public float CurrentGearRatio;
    public float CurrentTotalGearRatio;
    public float ClutchEngagement;
    public float TransmissionTorque; 
}
[System.Serializable]
public struct BrakeData
{
    public float FrontBrakeTorque;
    public float RearBrakeTorque;
    public float HandbrakeEngagement;
    public bool IsFrontLocked;
    public bool IsRearLocked;
}
[System.Serializable]
public struct SteeringData
{
    public float CurrentSteeringAngle; 
}
[System.Serializable]
public struct WheelData
{
    public float GeneralWheelsRPM;
}
[System.Serializable]
public struct AerodynamicData
{
    
}
[System.Serializable]
public struct VehicleData
{
    public float SpeedKmH;
    public float SpeedMS;
    public Vector3 Velocity;
    public float Mass;
}
[System.Serializable]
public struct TurboData
{
    
}








