using UnityEngine;

[System.Serializable]
public class TransmissionSimulation
{
    private float _shiftTimer = 0f;

    public void UpdatePhysics(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        UpdateClutch(ref data, ref transmissionConfig);

        UpdateGearRatio(ref data, ref transmissionConfig);

        CalculateTransmissionTorque(ref data, ref transmissionConfig);
    }

    private void UpdateClutch(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if(_shiftTimer > 0f)
        {
            _shiftTimer -= data.DeltaTime;
            float progress = 1f - (_shiftTimer / transmissionConfig.ShiftTime);
            data.Transmission.ClutchEngagement = Mathf.SmoothStep(0f, 1f, progress);
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

        float totalRatio = Mathf.Abs(data.Transmission.CurrentTotalGearRatio);
        // torque which transmission gets from engine
        float engineTorqueToWheels = data.Engine.EngineTorque * data.Transmission.ClutchEngagement;

        float transmittedTorque = engineTorqueToWheels * transmissionConfig.TransmissionEfficiency * totalRatio;
        
        transmittedTorque -= data.Engine.EngineBraking * totalRatio;

        if(data.Transmission.CurrentGear == -1)
        {
            transmittedTorque = -Mathf.Abs(transmittedTorque);
        }
        
        data.Transmission.TransmissionTorque = transmittedTorque;
    }

    // shifting funcs
    public void ShiftUp(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.CurrentGear < transmissionConfig.ForwardGearRatios.Length && _shiftTimer <= 0f)
        {
            data.Transmission.CurrentGear++;
            _shiftTimer = transmissionConfig.ShiftTime;
        }
    }
    public void ShiftDown(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (data.Transmission.CurrentGear >= 1 && _shiftTimer <= 0f)
        {
            data.Transmission.CurrentGear--;
            _shiftTimer = transmissionConfig.ShiftTime;
        }
    }
    public void ShiftToReverse(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if (_shiftTimer > 0f)
            return;
        if (data.Transmission.CurrentGear == -1)
            return;
        
        float speedKmh = data.Vehicle.SpeedKmH;
        float absSpeed = Mathf.Abs(speedKmh);

        // you can not reverse car on big speed (you can easily brake your transmission)
        if(absSpeed > transmissionConfig.ReverseMaxSpeed)
        {
            // add sound of braking gears
            return;
        }

        data.Transmission.CurrentGear = -1; // Reverse

        _shiftTimer = transmissionConfig.ShiftTime;
    }
    public void ShiftToNeutral(ref CarPhysicsData data, ref TransmissionConfig transmissionConfig)
    {
        if(_shiftTimer > 0f)
            return;
        
        data.Transmission.CurrentGear = 0;

        _shiftTimer = transmissionConfig.ShiftTime;
    }
}
