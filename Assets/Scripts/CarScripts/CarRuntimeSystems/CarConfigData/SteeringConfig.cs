using UnityEngine;

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