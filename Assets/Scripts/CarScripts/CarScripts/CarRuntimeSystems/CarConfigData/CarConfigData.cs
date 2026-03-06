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
