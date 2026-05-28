using UnityEngine;

[System.Serializable]
public struct CarPhysicsData
{
    // === dynamic systems data ===
    public EngineData Engine;
    public TransmissionData Transmission;
    public WheelData WheelData; 
    public SteeringData Steering;
    public BrakeData Brake;
    public AerodynamicData Aerodynamic;
    public VehicleData Vehicle;
    public TurboData Turbo;
    
    public ManagedObjectsSctruct managedObjectsSctruct;

    public float DeltaTime;
}