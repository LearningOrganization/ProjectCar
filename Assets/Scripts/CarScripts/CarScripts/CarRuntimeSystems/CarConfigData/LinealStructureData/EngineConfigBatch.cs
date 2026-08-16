
public struct EngineConfigBatch
{

    public TorqueCurve TorqueCurve; // should be replaced by another system in future for clearer dod 
    public float MinRPM;
    public float MaxRPM ;
    public float IdleRPM;
    public float EngineInertiaAccel;
    public float EngineInertiaDecel;

    public float BackTorque;  // reverse torque & engine losses
    public float IdleThrottleBoost;

    public float CouplingStrength;

    public bool UseRevLimiter;
    public float RevLimiterRPM; 
    public RevLimiterType limiterType;
}