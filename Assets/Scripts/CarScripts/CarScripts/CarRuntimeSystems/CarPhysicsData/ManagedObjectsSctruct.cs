using UnityEngine;

// this structure of special heap managed objects, which i can not use in dod paradigm
public struct ManagedObjectsSctruct
{
    public WheelCollider[] SteeringWheels;
    public WheelCollider[] MotorizeWheels;
    public WheelCollider[] HandBreakableWheels;

    public Rigidbody rigidbody;
}
