using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class AtmoEngineSimulation
{
    private float _currentRPM;
    private bool _isRevLimiterActive = false;
    private float _revLimiterTimer = 0f;
    
    public void Init(ref EngineConfig engineConfig)
    {
        _currentRPM = engineConfig.IdleRPM;
    }

    public void UpdatePhysics(ref CarPhysicsData data, ref EngineConfig engineConfig,  ref PlayerInput input)
    {
        float dt = data.DeltaTime;
        float throttle = Mathf.Clamp01(input.ThrottleInput);

        // ================= IDLE CONTROL =================
        if (_currentRPM < engineConfig.IdleRPM)
            throttle = Mathf.Max(throttle, engineConfig.IdleThrottleBoost);

        // ================= BASE TORQUE ==================
        float baseTorque = engineConfig.TorqueCurve.Evaluate(_currentRPM);
        float engineTorque = baseTorque * throttle;

        // ================= ENGINE LOSSES ================
        float engineBraking = Mathf.Pow(1f - throttle, 2f) * engineConfig.BackTorque;

        // ================= REV LIMITER ==================
        if (engineConfig.UseRevLimiter)
            engineTorque = ApplyRevLimiter(engineTorque, ref engineConfig, ref data);

        // ================= DRIVETRAIN STATE =============
        bool drivetrainConnected =
            data.Transmission.CurrentGear != 0 &&
            data.Transmission.ClutchEngagement > 0.01f &&
            Mathf.Abs(data.Transmission.CurrentTotalGearRatio) > 0.01f;

        // ================= TARGET RPM ===================
        float targetRPM = _currentRPM;

        if (drivetrainConnected)
        {
            //targetRPM = Mathf.Abs(data.WheelData.GeneralWheelsRPM * data.Transmission.CurrentTotalGearRatio);
            targetRPM = data.WheelData.GeneralWheelsRPM * data.Transmission.CurrentTotalGearRatio;
            targetRPM = Mathf.Max(targetRPM, engineConfig.IdleRPM);
        }

        // ================= RPM DYNAMICS =================
        float rpmDelta = 0f;

        if (drivetrainConnected)
        {
            float rpmError = targetRPM - _currentRPM;
            rpmDelta += rpmError * engineConfig.CouplingStrength * data.Transmission.ClutchEngagement;
        }

        bool isAccelerating = engineTorque > engineBraking;
        float inertia;

        if(drivetrainConnected)
        {
            inertia = isAccelerating ? engineConfig.EngineInertiaAccel : engineConfig.EngineInertiaDecel;
        }
        else
        {
            inertia = isAccelerating ? engineConfig.EngineInertiaAccel : engineConfig.EngineInertiaDecel;
        }

        rpmDelta += engineTorque / Mathf.Max(0.001f, inertia);
        rpmDelta -= engineBraking / Mathf.Max(0.001f, inertia);
        // float netTorque = engineTorque - engineBraking;

        // float inertia;
        // if (drivetrainConnected)
        // {
        //     inertia = netTorque >= 0f ? engineConfig.EngineInertiaAccel : engineConfig.EngineInertiaDecel;
        // }
        // else
        // {
        //     inertia = 0.06f;
        // }

        // rpmDelta += netTorque / Mathf.Max(0.001f, inertia);

        // ================= APPLY RPM ====================
        _currentRPM += rpmDelta * dt;

        if (_currentRPM < engineConfig.IdleRPM && throttle <= engineConfig.IdleThrottleBoost + 0.01f)
            _currentRPM = Mathf.Lerp(_currentRPM, engineConfig.IdleRPM, dt * 5f);

        _currentRPM = Mathf.Clamp(_currentRPM, engineConfig.MinRPM, engineConfig.MaxRPM);

        // ================= TORQUE OUTPUT ================
        float torqueToTransmission = 0f;

        if (drivetrainConnected)
            torqueToTransmission = engineTorque * data.Transmission.ClutchEngagement;

        // ================= WRITE DATA ===================
        data.Engine.EngineRPM = _currentRPM;
        data.Engine.EngineTorque = torqueToTransmission;
        data.Engine.EngineBraking = engineBraking;
        data.Engine.EngineInertia = inertia;
    }

    private float ApplyRevLimiter(float requestedTorque, ref EngineConfig engineConfig, ref CarPhysicsData data)
    {
        if (_currentRPM >= engineConfig.RevLimiterRPM)
        {
            _isRevLimiterActive = true;
                
            switch (engineConfig.limiterType)
            {
                case RevLimiterType.HardCut:
                    return 0;
                    
                case RevLimiterType.SoftCut:
                    float softCutRange = 200f; 
                    float overrev = _currentRPM - engineConfig.RevLimiterRPM;
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

public enum RevLimiterType
{
    SoftCut,
    Ignition ,
    HardCut
}
