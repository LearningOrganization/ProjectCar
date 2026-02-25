using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class AtmoEngineSimulation
{
    [Header("Engine Specifications")]
    [SerializeField] private AnimationCurve TorqueCurve;
    [SerializeField] private float MinRPM = 1000;
    [SerializeField] private float MaxRPM = 7250 ;
    [SerializeField] private float IdleRPM = 1200 ;
    [SerializeField] private float EngineInertiaAccel = 0.1f;
    [SerializeField] private float EngineInertiaDecel = 0.1f;

    [Header("Engine Dynamics")]
    [SerializeField] private float BackTorque = 100f;  // reverse torque & engine losses
    [SerializeField] private float IdleThrottleBoost = 0.12f;

    [Header("Engine-Wheel Coupling")]
    [SerializeField] private float CouplingStrength = 5f;

    [Header("Rev Limiter")]
    [SerializeField] private bool UseRevLimiter = true;
    [SerializeField] private float RevLimiterRPM = 7000; 
    [SerializeField] private RevLimiterType limiterType = RevLimiterType.SoftCut;
    
    [SerializeField] private float _currentRPM;
    private bool _isRevLimiterActive = false;
    private float _revLimiterTimer = 0f;
    
    public enum RevLimiterType
    {
        HardCut,
        SoftCut,
        Ignition 
    }

    public void Init()
    {
        _currentRPM = IdleRPM;
    }

    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput input)
    {
        float dt = data.DeltaTime;
        float throttle = Mathf.Clamp01(input.ThrottleInput);

        // ================= IDLE CONTROL =================
        if (_currentRPM < IdleRPM)
            throttle = Mathf.Max(throttle, IdleThrottleBoost);

        // ================= BASE TORQUE ==================
        float baseTorque = TorqueCurve.Evaluate(_currentRPM);
        float engineTorque = baseTorque * throttle;

        // ================= ENGINE LOSSES ================
        float engineBraking = Mathf.Pow(1f - throttle, 2f) * BackTorque;

        // ================= REV LIMITER ==================
        if (UseRevLimiter)
            engineTorque = ApplyRevLimiter(engineTorque, ref data);

        // ================= DRIVETRAIN STATE =============
        bool drivetrainConnected =
            data.CurrentGear != 0 &&
            data.ClutchEngagement > 0.01f &&
            Mathf.Abs(data.CurrentTotalGearRatio) > 0.01f;

        // ================= TARGET RPM ===================
        float targetRPM = _currentRPM;

        if (drivetrainConnected)
        {
            targetRPM = Mathf.Abs(data.GeneralWheelsRPM * data.CurrentTotalGearRatio);
            targetRPM = Mathf.Max(targetRPM, IdleRPM);
        }

        // ================= RPM DYNAMICS =================
        float rpmDelta = 0f;

        if (drivetrainConnected)
        {
            float rpmError = targetRPM - _currentRPM;
            rpmDelta += rpmError * CouplingStrength * data.ClutchEngagement;
        }

        float netTorque = engineTorque - engineBraking;

        float inertia;
        if (drivetrainConnected)
        {
            inertia = netTorque >= 0f ? EngineInertiaAccel : EngineInertiaDecel;
        }
        else
        {
            inertia = 0.06f;
        }

        rpmDelta += netTorque / Mathf.Max(0.001f, inertia);

        // ================= APPLY RPM ====================
        _currentRPM += rpmDelta * dt;

        if (_currentRPM < IdleRPM && throttle <= IdleThrottleBoost + 0.01f)
            _currentRPM = Mathf.Lerp(_currentRPM, IdleRPM, dt * 5f);

        _currentRPM = Mathf.Clamp(_currentRPM, MinRPM, MaxRPM);

        // ================= TORQUE OUTPUT ================
        float torqueToTransmission = 0f;

        if (drivetrainConnected)
            torqueToTransmission = engineTorque * data.ClutchEngagement;

        // ================= WRITE DATA ===================
        data.EngineRPM = _currentRPM;
        data.EngineTorque = torqueToTransmission;
        data.EngineBraking = engineBraking;
        data.EngineInertia = inertia;
    }

    private float ApplyRevLimiter(float requestedTorque, ref CarPhysicsData data)
    {
        if (_currentRPM >= RevLimiterRPM)
        {
            _isRevLimiterActive = true;
                
            switch (limiterType)
            {
                case RevLimiterType.HardCut:
                    return 0;
                    
                case RevLimiterType.SoftCut:
                    float softCutRange = 200f; 
                    float overrev = _currentRPM - RevLimiterRPM;
                    float reduction = Mathf.Clamp01(overrev / softCutRange);
                    return requestedTorque * (1f - reduction);
                    
                    case RevLimiterType.Ignition:
                        _revLimiterTimer += data.DeltaTime;
                        
                        float cutFrequency = 0.05f; // 20 Hz
                        if (_revLimiterTimer >= cutFrequency)
                        {
                            _revLimiterTimer = 0f;
                            return (_revLimiterTimer < cutFrequency / 2f) ? 0f : requestedTorque;
                        }
                        return 0f;
                    
                    default:
                        return 0f;
                }
            }
            else
            {
                _isRevLimiterActive = false;
                _revLimiterTimer = 0f;
                return requestedTorque;
            }
        }
    

    }


