using Unity.Mathematics; // - replace Mathf functions by math 
using UnityEngine;


// TODO: make burst compilable
[System.Serializable]
public struct AtmoEngineSimulation
{
 
    public void Init(ref EngineData engineData, ref EngineBatchConfig engineConfig)
    {
        engineData.CurrentRPM = engineConfig.IdleRPM;
    }

    public void UpdatePhysics(ref CarPhysicsData data, ref EngineBatchConfig engineConfig,  ref PlayerInput input)
    {
        float dt = data.DeltaTime;
        float throttle = math.clamp(input.ThrottleInput, 0, 1);

        // ================= IDLE CONTROL =================
        if (data.Engine.CurrentRPM < engineConfig.IdleRPM)
            throttle = math.max(throttle, engineConfig.IdleThrottleBoost);

        // ================= BASE TORQUE ==================
        float baseTorque = engineConfig.TorqueCurve.Evaluate(data.Engine.CurrentRPM); // replace by interpolation of auxilary class
        float engineTorque = baseTorque * throttle;

        // ================= ENGINE LOSSES ================
        float engineBraking = math.pow(1f - throttle, 2f) * engineConfig.BackTorque;

        // ================= REV LIMITER ==================
        if (engineConfig.UseRevLimiter)
            engineTorque = ApplyRevLimiter(engineTorque, ref engineConfig, ref data);

        // ================= DRIVETRAIN STATE =============
        bool drivetrainConnected =
            data.Transmission.CurrentGear != 0 &&
            data.Transmission.ClutchEngagement > 0.01f &&
            math.abs(data.Transmission.CurrentTotalGearRatio) > 0.01f;

        // ================= TARGET RPM ===================
        float targetRPM = data.Engine.CurrentRPM;

        if (drivetrainConnected)
        {
            //targetRPM = Mathf.Abs(data.WheelData.GeneralWheelsRPM * data.Transmission.CurrentTotalGearRatio);
            targetRPM = data.WheelData.GeneralWheelsRPM * data.Transmission.CurrentTotalGearRatio;
            targetRPM = math.max(targetRPM, engineConfig.IdleRPM);
        }

        // ================= RPM DYNAMICS =================
        float rpmDelta = 0f;

        if (drivetrainConnected)
        {
            float rpmError = targetRPM - data.Engine.CurrentRPM;
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

        rpmDelta += engineTorque / math.max(0.001f, inertia);
        rpmDelta -= engineBraking / math.max(0.001f, inertia);

        // ================= APPLY RPM ====================
        data.Engine.CurrentRPM += rpmDelta * dt;

        if (data.Engine.CurrentRPM < engineConfig.IdleRPM && throttle <= engineConfig.IdleThrottleBoost + 0.01f)
            data.Engine.CurrentRPM = math.lerp(data.Engine.CurrentRPM, engineConfig.IdleRPM, dt * 5f);

        data.Engine.CurrentRPM = math.clamp(data.Engine.CurrentRPM, engineConfig.MinRPM, engineConfig.MaxRPM);

        // ================= TORQUE OUTPUT ================
        float torqueToTransmission = 0f;

        if (drivetrainConnected)
            torqueToTransmission = engineTorque * data.Transmission.ClutchEngagement;

        // ================= WRITE DATA ===================
        data.Engine.EngineRPM = data.Engine.CurrentRPM;
        data.Engine.EngineTorque = torqueToTransmission;
        data.Engine.EngineBraking = engineBraking;
        data.Engine.EngineInertia = inertia;
    }

    private float ApplyRevLimiter(float requestedTorque, ref EngineBatchConfig engineConfig, ref CarPhysicsData data)
    {
        float rpm = data.Engine.CurrentRPM;

        if (rpm < engineConfig.RevLimiterRPM)
        {
            data.Engine.RevLimiterTimer = 0f;
            return requestedTorque;
        }

        float overrev = rpm - engineConfig.RevLimiterRPM;

        switch (engineConfig.limiterType)
        {
            case RevLimiterType.HardCut:
                return 0f;
            case RevLimiterType.SoftCut:
            {
                float softCutRange = 200f;
                float reduction = math.saturate(overrev / softCutRange);
                return requestedTorque * (1f - reduction);
            }
            case RevLimiterType.Ignition:
            {
                float dt = data.DeltaTime;

                data.Engine.RevLimiterTimer += dt;

                float cutFrequency = 0.05f; // 20 Hz
                float half = cutFrequency * 0.5f;

                if (data.Engine.RevLimiterTimer >= cutFrequency)
                    data.Engine.RevLimiterTimer = 0f;

                return (data.Engine.RevLimiterTimer < half)
                    ? 0f
                    : requestedTorque;
            }
            default:
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
