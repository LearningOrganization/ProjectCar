using Unity.Burst;
using Unity.Mathematics;

[System.Serializable]
[BurstCompile]
public struct BrakeSimulation
{
    public void UpdatePhysics(ref BrakeData brakeData, ref BrakeConfig brakeConfig, ref PlayerInput input, ref float deltaTime)
    {
        UpdateHandBrake(ref brakeData, ref brakeConfig, ref input, ref deltaTime);
        UpdateServiceBrake(ref brakeData, ref brakeConfig, ref input);
        CheckWheelsLock(ref brakeData, ref brakeConfig);
    }

    private void CheckWheelsLock(ref BrakeData brakeData, ref BrakeConfig brakeConfig)
    {
        brakeData.IsFrontLocked = brakeData.FrontBrakeTorque >= brakeConfig.MaxBrakeTorque * brakeConfig.FrontBrakeBias;
        
        float rearServiceBrake = brakeData.RearBrakeTorque - 
                                brakeData.HandbrakeEngagement * brakeConfig.HandbrakeTorque;
        brakeData.IsRearLocked = rearServiceBrake >= brakeConfig.MaxBrakeTorque * (1f - brakeConfig.FrontBrakeBias);
    }

    private void UpdateServiceBrake(ref BrakeData brakeData, ref BrakeConfig brakeConfig, ref PlayerInput input)
    {
        float brakeInput = input.BrakeInput;
        if (brakeConfig.UseABS)
        {
            brakeInput = ApplyABS(brakeInput, ref brakeData);
        }

        float totalTorque = brakeInput * brakeConfig.MaxBrakeTorque;
        brakeData.FrontBrakeTorque = totalTorque * brakeConfig.FrontBrakeBias;

        float rearServiceBrake = totalTorque * (1f - brakeConfig.FrontBrakeBias);

        float engagement = brakeData.HandbrakeEngagement;
        if (engagement < 0.05f) engagement = 0f;

        float handbrakeTorque = engagement * brakeConfig.HandbrakeTorque;
        brakeData.RearBrakeTorque = math.max(rearServiceBrake, handbrakeTorque);
    }

    private float ApplyABS(float brakeInput,  ref BrakeData brakeData)
    {
        if(brakeData.IsFrontLocked || brakeData.IsRearLocked)
        {
            brakeInput *= 0.6f; // yes this is magic number but it works 
        }
        return brakeInput;
    }

    private void UpdateHandBrake(ref BrakeData brakeData, ref BrakeConfig breakConfig, ref PlayerInput input, ref float deltaTime)
    {
        float target = input.Handbrake ? 1f : 0f;

        float releaseSpeed = breakConfig.HandbrakeEngagement;
        if (!input.Handbrake && input.ThrottleInput > 0.1f)
        {
            releaseSpeed *= 4f;
        }

        brakeData.HandbrakeEngagement = AuxiliaryMathf.MoveTowards(
            brakeData.HandbrakeEngagement,
            target,
            releaseSpeed * deltaTime);
    }
}