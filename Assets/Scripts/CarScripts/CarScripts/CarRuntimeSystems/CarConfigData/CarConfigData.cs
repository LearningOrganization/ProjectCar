using UnityEngine;

[System.Serializable]
public struct CarConfigData
{
    public EngineConfig EngineConfig;
    public SteeringConfig SteeringConfig;
    public TransmissionConfig TransmissionConfig;
    public BrakeConfig BreakConfig;

    public ManagedObjectsSctruct ManagedObjectsSctruct;
    public bool IsTurbo;
}
