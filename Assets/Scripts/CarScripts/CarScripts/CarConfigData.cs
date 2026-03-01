using UnityEngine;

[System.Serializable]
public struct CarConfigData
{
    public EngineConfig EngineConfig;
    public SteeringConfig SteeringConfig;
    public TransmissionConfig TransmissionConfig;
    public BrakeConfig BreakConfig;
    public bool IsTurbo;
}

[System.Serializable]
public struct EngineConfig
{
    [Header("Engine Specifications")]
    public AnimationCurve TorqueCurve; // should be replaced by another system in future for clearer dod 
    public float MinRPM;
    public float MaxRPM ;
    public float IdleRPM;
    public float EngineInertiaAccel;
    public float EngineInertiaDecel;

    [Header("Engine Dynamics")]
    public float BackTorque;  // reverse torque & engine losses
    public float IdleThrottleBoost;

    [Header("Engine-Wheel Coupling")]
    public float CouplingStrength;

    [Header("Rev Limiter")]
    public bool UseRevLimiter;
    public float RevLimiterRPM; 
    public RevLimiterType limiterType;
}

[System.Serializable]
public struct SteeringConfig
{
    [Header("Steering Range")]
    public float SteeringRangeAtZeroSpeed;
    public float SteeringRangeAtMaxSpeed;
    public float MaxSpeedForSteering; 
    [Header("Steering Response")]
    public float SteerSpeed; 
    [Tooltip("Returning to center obviously faster")]
    public float ReturnSpeed;
}

[System.Serializable]
public struct BrakeConfig
{
    [Header("Service Brake")]
    public float MaxBrakeTorque;
    [Range(0f, 1f)]
    public float FrontBrakeBias;
    
    [Header("Handbrake")]
    public float HandbrakeTorque;
    public float HandbrakeEngagement;

    [Header("ABS (simple)")]
    public bool UseABS;
    public float ABSSlipThreshold;
}

[System.Serializable]
public struct TransmissionConfig
{
    [Header("Gear specs")]
    public float[] ForwardGearRatios;
    public float[] ReverseRatio;
    public float FinalDriveRatio;
    public float TransmissionEfficiency;
        
    [Header("Shift Settings")]
    public float ShiftTime;
    public float ReverseMaxSpeed; //km/h

}

[System.Serializable]
public struct AerodynamicConfig
{
    
}