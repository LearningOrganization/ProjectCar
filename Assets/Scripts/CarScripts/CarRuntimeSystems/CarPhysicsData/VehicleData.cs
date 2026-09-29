using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct VehicleData
{
    public float SpeedKmH;
    public float SpeedMS;
    public float3 Velocity; // should be replaced by float3
    public float Mass;
}
