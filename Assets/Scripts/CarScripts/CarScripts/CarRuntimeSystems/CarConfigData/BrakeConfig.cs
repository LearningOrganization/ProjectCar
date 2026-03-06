using UnityEngine;

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