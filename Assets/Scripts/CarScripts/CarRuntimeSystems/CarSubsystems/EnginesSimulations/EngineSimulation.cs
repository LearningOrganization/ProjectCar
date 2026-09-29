using Unity.Burst;
using Unity.Mathematics; // - replace Mathf functions by math 
using UnityEngine;


// TODO: make burst compilable
[System.Serializable]
[BurstCompile]
public struct AtmoEngineSimulation
{
 
    public void Init(ref EngineData engineData, ref EngineBatchConfig engineConfig)
    {
        engineData.CurrentRPM = engineConfig.IdleRPM;
    }

    public void UpdatePhysics(ref EngineData engineData, ref TransmissionData transmissionData, ref WheelData wheelData, ref EngineBatchConfig engineConfig,  ref PlayerInput input, ref float deltaTime)
    {
        float dt = deltaTime;
        float throttle = math.clamp(input.ThrottleInput, 0, 1);

        // ================= IDLE CONTROL =================
        if (engineData.CurrentRPM < engineConfig.IdleRPM)
            throttle = math.max(throttle, engineConfig.IdleThrottleBoost);

        // ================= BASE TORQUE ==================
        float baseTorque = engineConfig.TorqueCurve.Evaluate(engineData.CurrentRPM); // replace by interpolation of auxilary class
        float engineTorque = baseTorque * throttle;

        // ================= ENGINE LOSSES ================
        float engineBraking = math.pow(1f - throttle, 2f) * engineConfig.BackTorque;

        // ================= REV LIMITER ==================
        if (engineConfig.UseRevLimiter)
            engineTorque = ApplyRevLimiter(engineTorque, ref engineConfig, ref engineData, ref deltaTime);

        // ================= DRIVETRAIN STATE =============
        bool drivetrainConnected =
            transmissionData.CurrentGear != 0 &&
            transmissionData.ClutchEngagement > 0.01f &&
            math.abs(transmissionData.CurrentTotalGearRatio) > 0.01f;

        // ================= TARGET RPM ===================
        float targetRPM = engineData.CurrentRPM;

        if (drivetrainConnected)
        {
            //targetRPM = Mathf.Abs(data.WheelData.GeneralWheelsRPM * transmissionData.CurrentTotalGearRatio);
            targetRPM = wheelData.GeneralWheelsRPM * transmissionData.CurrentTotalGearRatio;
            targetRPM = math.max(targetRPM, engineConfig.IdleRPM);
        }

        // ================= RPM DYNAMICS =================
        float rpmDelta = 0f;

        if (drivetrainConnected)
        {
            float rpmError = targetRPM - engineData.CurrentRPM;
            rpmDelta += rpmError * engineConfig.CouplingStrength * transmissionData.ClutchEngagement;
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
        engineData.CurrentRPM += rpmDelta * dt;

        if (engineData.CurrentRPM < engineConfig.IdleRPM && throttle <= engineConfig.IdleThrottleBoost + 0.01f)
            engineData.CurrentRPM = math.lerp(engineData.CurrentRPM, engineConfig.IdleRPM, dt * 5f);

        engineData.CurrentRPM = math.clamp(engineData.CurrentRPM, engineConfig.MinRPM, engineConfig.MaxRPM);

        // ================= TORQUE OUTPUT ================
        float torqueToTransmission = 0f;

        if (drivetrainConnected)
            torqueToTransmission = engineTorque * transmissionData.ClutchEngagement;

        // ================= WRITE DATA ===================
        engineData.EngineRPM = engineData.CurrentRPM;
        engineData.EngineTorque = torqueToTransmission;
        engineData.EngineBraking = engineBraking;
        engineData.EngineInertia = inertia;
    }

    private float ApplyRevLimiter(float requestedTorque, ref EngineBatchConfig engineConfig, ref EngineData engineData, ref float deltaTime)
    {
        float rpm = engineData.CurrentRPM;

        if (rpm < engineConfig.RevLimiterRPM)
        {
            engineData.RevLimiterTimer = 0f;
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
                float dt = deltaTime;

                engineData.RevLimiterTimer += dt;

                float cutFrequency = 0.05f; // 20 Hz
                float half = cutFrequency * 0.5f;

                if (engineData.RevLimiterTimer >= cutFrequency)
                    engineData.RevLimiterTimer = 0f;

                return (engineData.RevLimiterTimer < half)
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
