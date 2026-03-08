using UnityEngine;

[System.Serializable]
public struct BrakeData
{
    public float FrontBrakeTorque;
    public float RearBrakeTorque;
    public float HandbrakeEngagement;
    public bool IsFrontLocked;
    public bool IsRearLocked;
}
