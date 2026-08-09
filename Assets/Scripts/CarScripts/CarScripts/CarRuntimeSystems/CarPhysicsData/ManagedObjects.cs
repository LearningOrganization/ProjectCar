using UnityEngine;

// this class of special heap managed objects, which i can not use in dod paradigm
[System.Serializable]
public class ManagedObjects
{
    
    public WheelCollider FRCollider;
    public WheelCollider FLCollider;
    public WheelCollider RRCollider;
    public WheelCollider RLCollider;

    public Rigidbody rigidbody;

    public AnimationCurve TorqueCurve; 
}
