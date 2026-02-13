using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class TransmissionSimulation
{
    [Header("Gear specs")]
    [SerializeField] private float[] ForwardGearRatios = { 3.214f, 1.925f, 1.302f, 1.000f, 0.752f };
    [SerializeField] private float[] ReverseRatio =  {-3.2f};
    [SerializeField] private float FinalDriveRatio = 4.111f;
    [SerializeField] private float TransmissionEfficiency = 0.90f;
        
    [Header("Shift Settings")]
    [SerializeField] private float ShiftTime = 0.5f;
    [SerializeField] private float ReverseMaxSpeed = 2.0f; //km/h

    private float _shiftTimer = 0f;


    public void UpdatePhysics(ref CarPhysicsData data)
    {
        UpdateClutch(ref data);

        UpdateGearRatio(ref data);

        CalculateTransmissionTorque(ref data);

        //Debug.Log($"Gear: {data.CurrentGear}; Gear ratio: {data.CurrentGearRatio}; ClutchEngagement: {data.ClutchEngagement}; TransmissionTorque: {data.TransmissionTorque}");
    }

    private void UpdateClutch(ref CarPhysicsData data)
    {
        if(_shiftTimer > 0f)
        {
            _shiftTimer -= data.DeltaTime;
            float progress = 1f - (_shiftTimer / ShiftTime);
            data.ClutchEngagement = Mathf.SmoothStep(0f, 1f, progress);
        }
        else
        {
            data.ClutchEngagement = 1f;
        }
    }
    private void UpdateGearRatio(ref CarPhysicsData data)
    {
        if(data.CurrentGear == 0)
        {
            data.CurrentGearRatio = 0f;
        }
        else if (data.CurrentGear == -1)
        {
            data.CurrentGearRatio = ReverseRatio[0];
        }
        else if(data.CurrentGear > 0 && data.CurrentGear <= ForwardGearRatios.Length)
        {
            data.CurrentGearRatio = ForwardGearRatios[data.CurrentGear - 1];
        }

        data.CurrentTotalGearRatio = data.CurrentGearRatio * FinalDriveRatio;
    }
    private void CalculateTransmissionTorque(ref CarPhysicsData data)
    {
        if (data.CurrentGear == 0)
        {
            data.TransmissionTorque = 0f;
            return;
        }

        float totalRatio = Mathf.Abs(data.CurrentTotalGearRatio);
        // torque which transmission gets from engine
        float engineTorqueToWheels = data.EngineTorque * data.ClutchEngagement;

        float transmittedTorque = engineTorqueToWheels * TransmissionEfficiency * totalRatio;
        
        transmittedTorque -= data.EngineBraking * totalRatio;

        if(data.CurrentGear == -1)
        {
            transmittedTorque = -Mathf.Abs(transmittedTorque);
        }
        
        data.TransmissionTorque = transmittedTorque;
    }

    // shifting funcs
    public void ShiftUp(ref CarPhysicsData data)
    {
        if (data.CurrentGear < ForwardGearRatios.Length && _shiftTimer <= 0f)
        {
            data.CurrentGear++;
            _shiftTimer = ShiftTime;
        }
    }
    public void ShiftDown(ref CarPhysicsData data)
    {
        if (data.CurrentGear >= 1 && _shiftTimer <= 0f)
        {
            data.CurrentGear--;
            _shiftTimer = ShiftTime;
        }
    }
    public void ShiftToReverse(ref CarPhysicsData data)
    {
        if (_shiftTimer > 0f)
            return;
        if (data.CurrentGear == -1)
            return;
        
        float speedKmh = data.SpeedKmH;
        float absSpeed = Mathf.Abs(speedKmh);

        // you can not reverse car on big speed (you can easily brake your transmission)
        if(absSpeed > ReverseMaxSpeed)
        {
            // add sound of braking gears
            return;
        }

        data.CurrentGear = -1; // Reverse

        _shiftTimer = ShiftTime;
    }
    public void ShiftToNeutral(ref CarPhysicsData data)
    {
        if(_shiftTimer > 0f)
            return;
        
        data.CurrentGear = 0;

        _shiftTimer = ShiftTime;
    }
}
