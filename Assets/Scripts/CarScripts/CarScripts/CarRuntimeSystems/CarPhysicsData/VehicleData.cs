using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct VehicleData
{
    public float SpeedKmH;
    public float SpeedMS;
    public Vector3 Velocity; // should be replaced by float3
    public float Mass;
}
