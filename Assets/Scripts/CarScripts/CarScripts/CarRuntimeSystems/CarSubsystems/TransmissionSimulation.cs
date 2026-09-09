using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[System.Serializable]
public struct TransmissionSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref TransmissionBatchConfig transmissionBatchConfig)
    {
        UpdateClutch(ref data, ref transmissionBatchConfig);

        UpdateGearRatio(ref data, ref transmissionBatchConfig);

        CalculateTransmissionTorque(ref data, ref transmissionBatchConfig);
    }

    private void UpdateClutch(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if(data.Transmission.ShiftTimer > 0f)
        {
            data.Transmission.ShiftTimer -= data.DeltaTime;
            float progress = 1f - (data.Transmission.ShiftTimer / TransmissionBatchConfig.ShiftTime);
            data.Transmission.ClutchEngagement = math.smoothstep(0f, 1f, progress);
        }
        else
        {
            data.Transmission.ClutchEngagement = 1f;
        }
    }
    private void UpdateGearRatio(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if(data.Transmission.CurrentGear == 0)
        {
            data.Transmission.CurrentGearRatio = 0f;
        }
        else if (data.Transmission.CurrentGear == -1)
        {
            data.Transmission.CurrentGearRatio = TransmissionBatchConfig.ReverseRatio[0];
        }
        else if(data.Transmission.CurrentGear > 0 && data.Transmission.CurrentGear <= TransmissionBatchConfig.ForwardGearRatios.Length)
        {
            data.Transmission.CurrentGearRatio = TransmissionBatchConfig.ForwardGearRatios[data.Transmission.CurrentGear - 1];
        }

        data.Transmission.CurrentTotalGearRatio = data.Transmission.CurrentGearRatio * TransmissionBatchConfig.FinalDriveRatio;
    }
    private void CalculateTransmissionTorque(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (data.Transmission.CurrentGear == 0)
        {
            data.Transmission.TransmissionTorque = 0f;
            return;
        }

        float totalRatio = math.abs(data.Transmission.CurrentTotalGearRatio);
        // torque which transmission gets from engine
        float engineTorqueToWheels = data.Engine.EngineTorque * data.Transmission.ClutchEngagement;

        float transmittedTorque = engineTorqueToWheels * TransmissionBatchConfig.TransmissionEfficiency * totalRatio;
        
        transmittedTorque -= data.Engine.EngineBraking * totalRatio;

        if(data.Transmission.CurrentGear == -1)
        {
            transmittedTorque = -math.abs(transmittedTorque);
        }
        
        data.Transmission.TransmissionTorque = transmittedTorque;
    }

    // shifting funcs
    public void ShiftUp(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (data.Transmission.CurrentGear < TransmissionBatchConfig.ForwardGearRatios.Length && data.Transmission.ShiftTimer <= 0f)
        {
            data.Transmission.CurrentGear++;
            data.Transmission.ShiftTimer = TransmissionBatchConfig.ShiftTime;
        }
    }
    public void ShiftDown(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (data.Transmission.CurrentGear >= 1 && data.Transmission.ShiftTimer <= 0f)
        {
            data.Transmission.CurrentGear--;
            data.Transmission.ShiftTimer = TransmissionBatchConfig.ShiftTime;
        }
    }
    public void ShiftToReverse(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (data.Transmission.ShiftTimer > 0f)
            return;
        if (data.Transmission.CurrentGear == -1)
            return;
        
        float speedKmh = data.Vehicle.SpeedKmH;
        float absSpeed = math.abs(speedKmh);

        // player can not reverse car on big speed (you can easily brake your transmission)
        if(absSpeed > TransmissionBatchConfig.ReverseMaxSpeed)
        {
            // add sound of braking gears
            return;
        }

        data.Transmission.CurrentGear = -1; // Reverse

        data.Transmission.ShiftTimer = TransmissionBatchConfig.ShiftTime;
    }
    public void ShiftToNeutral(ref CarPhysicsData data, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if(data.Transmission.ShiftTimer > 0f)
            return;
        
        data.Transmission.CurrentGear = 0;

        data.Transmission.ShiftTimer = TransmissionBatchConfig.ShiftTime;
    }
}
