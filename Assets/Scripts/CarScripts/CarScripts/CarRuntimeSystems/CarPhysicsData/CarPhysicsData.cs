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
    
    public  int managedObjectsIndex;
    public float DeltaTime;
}