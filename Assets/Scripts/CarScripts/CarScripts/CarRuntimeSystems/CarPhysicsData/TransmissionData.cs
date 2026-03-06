using UnityEngine;


[System.Serializable]
public struct TransmissionData
{
    public int CurrentGear;
    public float CurrentGearRatio;
    public float CurrentTotalGearRatio;
    public float ClutchEngagement;
    public float TransmissionTorque; 
}
