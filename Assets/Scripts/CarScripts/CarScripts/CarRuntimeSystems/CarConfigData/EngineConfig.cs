using UnityEngine;

[System.Serializable]
public struct EngineConfig
{
    [Header("Engine Specifications")]
    // replace by Vector2 array 
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