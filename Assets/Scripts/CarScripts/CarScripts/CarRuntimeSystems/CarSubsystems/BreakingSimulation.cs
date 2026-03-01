using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

[System.Serializable]
public class BrakeSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref BrakeConfig brakeConfig, ref PlayerInput input)
    {
        UpdateHandBrake(ref data, ref brakeConfig, ref input);
        UpdateServiceBrake(ref data, ref brakeConfig, ref input);
        CheckWheelsLock(ref data, ref brakeConfig);
    }

    private void CheckWheelsLock(ref CarPhysicsData data, ref BrakeConfig brakeConfig)
    {
        data.Brake.IsFrontLocked = data.Brake.FrontBrakeTorque >= brakeConfig.MaxBrakeTorque * brakeConfig.FrontBrakeBias;
        
        float rearServiceBrake = data.Brake.RearBrakeTorque - 
                                data.Brake.HandbrakeEngagement * brakeConfig.HandbrakeTorque;
        data.Brake.IsRearLocked = rearServiceBrake >= brakeConfig.MaxBrakeTorque * (1f - brakeConfig.FrontBrakeBias);
    }

    private void UpdateServiceBrake(ref CarPhysicsData data, ref BrakeConfig brakeConfig, ref PlayerInput input)
    {
        float brakeInput = input.BrakeInput;
        if (brakeConfig.UseABS)
        {
            brakeInput = ApplyABS(brakeInput, ref brakeConfig, ref data);
        }

        float totalTorque = brakeInput * brakeConfig.MaxBrakeTorque;
        data.Brake.FrontBrakeTorque = totalTorque * brakeConfig.FrontBrakeBias;

        float rearServiceBrake = totalTorque * (1f - brakeConfig.FrontBrakeBias);

        float engagement = data.Brake.HandbrakeEngagement;
        if (engagement < 0.05f) engagement = 0f;

        float handbrakeTorque = engagement * brakeConfig.HandbrakeTorque;
        data.Brake.RearBrakeTorque = Mathf.Max(rearServiceBrake, handbrakeTorque);
    }

    private float ApplyABS(float brakeInput, ref BrakeConfig breakConfig, ref CarPhysicsData data)
    {
        if(data.Brake.IsFrontLocked || data.Brake.IsRearLocked)
        {
            brakeInput *= 0.6f;
        }
        return brakeInput;
    }

    private void UpdateHandBrake(ref CarPhysicsData data, ref BrakeConfig breakConfig, ref PlayerInput input)
    {
        float target = input.Handbrake ? 1f : 0f;

        float releaseSpeed = breakConfig.HandbrakeEngagement;
        if (!input.Handbrake && input.ThrottleInput > 0.1f)
        {
            releaseSpeed *= 4f;
        }

        data.Brake.HandbrakeEngagement = Mathf.MoveTowards(
            data.Brake.HandbrakeEngagement,
            target,
            releaseSpeed * data.DeltaTime);
    }
}