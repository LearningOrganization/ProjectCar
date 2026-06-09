using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[System.Serializable]
public struct TransmissionSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        UpdateClutch(ref data, ref transmissionConfig);

        UpdateGearRatio(ref data, ref transmissionConfig);

        CalculateTransmissionTorque(ref data, ref transmissionConfig);
    }

    private void UpdateClutch(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if(data.Transmission.ShiftTimer > 0f)
        {
            data.Transmission.ShiftTimer -= data.DeltaTime;
            float progress = 1f - (data.Transmission.ShiftTimer / transmissionConfig.ShiftTime);
            data.Transmission.ClutchEngagement = math.smoothstep(0f, 1f, progress);
        }
        else
        {
            data.Transmission.ClutchEngagement = 1f;
        }
    }
    private void UpdateGearRatio(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if(data.Transmission.CurrentGear == 0)
        {
            data.Transmission.CurrentGearRatio = 0f;
        }
        else if (data.Transmission.CurrentGear == -1)
        {
            data.Transmission.CurrentGearRatio = transmissionConfig.ReverseRatio[0];
        }
        else if(data.Transmission.CurrentGear > 0 && data.Transmission.CurrentGear <= transmissionConfig.ForwardGearRatios.Length)
        {
            data.Transmission.CurrentGearRatio = transmissionConfig.ForwardGearRatios[data.Transmission.CurrentGear - 1];
        }

        data.Transmission.CurrentTotalGearRatio = data.Transmission.CurrentGearRatio * transmissionConfig.FinalDriveRatio;
    }
    private void CalculateTransmissionTorque(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.CurrentGear == 0)
        {
            data.Transmission.TransmissionTorque = 0f;
            return;
        }

        float totalRatio = math.abs(data.Transmission.CurrentTotalGearRatio);
        // torque which transmission gets from engine
        float engineTorqueToWheels = data.Engine.EngineTorque * data.Transmission.ClutchEngagement;

        float transmittedTorque = engineTorqueToWheels * transmissionConfig.TransmissionEfficiency * totalRatio;
        
        transmittedTorque -= data.Engine.EngineBraking * totalRatio;

        if(data.Transmission.CurrentGear == -1)
        {
            transmittedTorque = -math.abs(transmittedTorque);
        }
        
        data.Transmission.TransmissionTorque = transmittedTorque;
    }

    // shifting funcs
    public void ShiftUp(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.CurrentGear < transmissionConfig.ForwardGearRatios.Length && data.Transmission.ShiftTimer <= 0f)
        {
            data.Transmission.CurrentGear++;
            data.Transmission.ShiftTimer = transmissionConfig.ShiftTime;
        }
    }
    public void ShiftDown(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.CurrentGear >= 1 && data.Transmission.ShiftTimer <= 0f)
        {
            data.Transmission.CurrentGear--;
            data.Transmission.ShiftTimer = transmissionConfig.ShiftTime;
        }
    }
    public void ShiftToReverse(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.ShiftTimer > 0f)
            return;
        if (data.Transmission.CurrentGear == -1)
            return;
        
        float speedKmh = data.Vehicle.SpeedKmH;
        float absSpeed = math.abs(speedKmh);

        // player can not reverse car on big speed (you can easily brake your transmission)
        if(absSpeed > transmissionConfig.ReverseMaxSpeed)
        {
            // add sound of braking gears
            return;
        }

        data.Transmission.CurrentGear = -1; // Reverse

        data.Transmission.ShiftTimer = transmissionConfig.ShiftTime;
    }
    public void ShiftToNeutral(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if(data.Transmission.ShiftTimer > 0f)
            return;
        
        data.Transmission.CurrentGear = 0;

        data.Transmission.ShiftTimer = transmissionConfig.ShiftTime;
    }
}
