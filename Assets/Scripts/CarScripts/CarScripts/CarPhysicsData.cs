using System;
using UnityEngine;
using Zenject;

// should be decomposed on many structs
[System.Serializable]
public struct CarPhysicsData
{
    // ===== ENGINE DATA =====
    public float EngineRPM;
    public float EngineTorque;
    public float EngineBraking;
    public float EngineInertia;

    // ===== TRANSMISSION DATA =====
    public int CurrentGear;
    public float CurrentGearRatio;
    public float CurrentTotalGearRatio;
    public float ClutchEngagement;
    public float TransmissionTorque;

    // ===== WHEEL DATA =====
    public float GeneralWheelsRPM;
    //public float GeneralWheelsTorque;
    public float WheelInertia;
    
    // ===== BRAKE DATA =====
    public float BrakeTorque;

    // ===== STEERING DATA =====
    public float CurrentSteeringAngle ; 

    // ===== VEHICLE DATA =====
    public float SpeedKmH;
    public float SpeedMS;
    public Vector3 Velocity;
    public float Mass;

    // ===== AERODYNAMICS ===== i hope for future

    // ===== SYSTEM =====
    public float DeltaTime;
}
