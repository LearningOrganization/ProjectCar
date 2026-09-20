using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[System.Serializable]
public struct TransmissionSimulation
{
    public void UpdatePhysics(ref TransmissionData transmissionData, ref EngineData engineData, ref TransmissionBatchConfig transmissionBatchConfig, ref float deltaTime)
    {
        UpdateClutch(ref transmissionData, ref transmissionBatchConfig, ref deltaTime);

        UpdateGearRatio(ref transmissionData, ref transmissionBatchConfig);

        CalculateTransmissionTorque(ref transmissionData, ref engineData, ref transmissionBatchConfig);
    }

    private void UpdateClutch(ref TransmissionData transmissionData, ref TransmissionBatchConfig TransmissionBatchConfig, ref float deltaTime)
    {
        if(transmissionData.ShiftTimer > 0f)
        {
            transmissionData.ShiftTimer -= deltaTime;
            float progress = 1f - (transmissionData.ShiftTimer / TransmissionBatchConfig.ShiftTime);
            transmissionData.ClutchEngagement = math.smoothstep(0f, 1f, progress);
        }
        else
        {
            transmissionData.ClutchEngagement = 1f;
        }
    }
    private void UpdateGearRatio(ref TransmissionData transmissionData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if(transmissionData.CurrentGear == 0)
        {
            transmissionData.CurrentGearRatio = 0f;
        }
        else if (transmissionData.CurrentGear == -1)
        {
            transmissionData.CurrentGearRatio = TransmissionBatchConfig.ReverseRatio[0];
        }
        else if(transmissionData.CurrentGear > 0 && transmissionData.CurrentGear <= TransmissionBatchConfig.ForwardGearRatios.Length)
        {
            transmissionData.CurrentGearRatio = TransmissionBatchConfig.ForwardGearRatios[transmissionData.CurrentGear - 1];
        }

        transmissionData.CurrentTotalGearRatio = transmissionData.CurrentGearRatio * TransmissionBatchConfig.FinalDriveRatio;
    }
    private void CalculateTransmissionTorque(ref TransmissionData transmissionData, ref EngineData engineData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (transmissionData.CurrentGear == 0)
        {
            transmissionData.TransmissionTorque = 0f;
            return;
        }

        float totalRatio = math.abs(transmissionData.CurrentTotalGearRatio);
        // torque which transmission gets from engine
        float engineTorqueToWheels = engineData.EngineTorque * transmissionData.ClutchEngagement;

        float transmittedTorque = engineTorqueToWheels * TransmissionBatchConfig.TransmissionEfficiency * totalRatio;
        
        transmittedTorque -= engineData.EngineBraking * totalRatio;

        if(transmissionData.CurrentGear == -1)
        {
            transmittedTorque = -math.abs(transmittedTorque);
        }
        
        transmissionData.TransmissionTorque = transmittedTorque;
    }

    // shifting funcs
    public void ShiftUp(ref TransmissionData transmissionData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (transmissionData.CurrentGear < TransmissionBatchConfig.ForwardGearRatios.Length && transmissionData.ShiftTimer <= 0f)
        {
            transmissionData.CurrentGear++;
            transmissionData.ShiftTimer = TransmissionBatchConfig.ShiftTime;
        }
    }
    public void ShiftDown(ref TransmissionData transmissionData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (transmissionData.CurrentGear >= 1 && transmissionData.ShiftTimer <= 0f)
        {
            transmissionData.CurrentGear--;
            transmissionData.ShiftTimer = TransmissionBatchConfig.ShiftTime;
        }
    }
    public void ShiftToReverse(ref TransmissionData transmissionData, ref VehicleData vehicleData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if (transmissionData.ShiftTimer > 0f)
            return;
        if (transmissionData.CurrentGear == -1)
            return;
        
        float speedKmh = vehicleData.SpeedKmH;
        float absSpeed = math.abs(speedKmh);

        // player can not reverse car on big speed (you can easily brake your transmission)
        if(absSpeed > TransmissionBatchConfig.ReverseMaxSpeed)
        {
            // add sound of braking gears
            return;
        }

        transmissionData.CurrentGear = -1; // Reverse

        transmissionData.ShiftTimer = TransmissionBatchConfig.ShiftTime;
    }
    public void ShiftToNeutral(ref TransmissionData transmissionData, ref TransmissionBatchConfig TransmissionBatchConfig)
    {
        if(transmissionData.ShiftTimer > 0f)
            return;
        
        transmissionData.CurrentGear = 0;

        transmissionData.ShiftTimer = TransmissionBatchConfig.ShiftTime;
    }
}
