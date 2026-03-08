using UnityEngine;

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