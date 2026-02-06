using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public struct CarPhysicsData
{
    // ===== ENGINE DATA =====
    public float EngineRPM;
    public float EngineTorque;
    public float EngineBraking;
    public float EngineInertia;

    // ===== TRANSMISSION DATA =====
    public int CurrentGear;
    public float CurrentGearRatio;
    public float CurrentTotalGearRatio;
    public float ClutchEngagement;
    public float TransmissionTorque;

    // ===== WHEEL DATA =====
    public float GeneralWheelsRPM;
    public float GeneralWheelsTorque;
    public float WheelInertia;
    public float[] WheelAngularVelocities;
    
    // ===== BRAKE DATA =====
    public float BrakeTorque;

    // ===== STEERING DATA =====
    public float CurrentSteeringAngle ; 

    // ===== VEHICLE DATA =====
    public float SpeedKmH;
    public float SpeedMS;
    public Vector3 Velocity;
    public float Mass;

    // ===== AERODYNAMICS ===== i hope for future

    // ===== SYSTEM =====
    public float DeltaTime;
}


[System.Serializable]
public class EngineSimulation
{
    [Header("Engine Specifications")]
    [SerializeField] private AnimationCurve TorqueCurve;
    [SerializeField] private float MinRPM = 1000;
    [SerializeField] private float MaxRPM = 7250 ;
    [SerializeField] private float IdleRPM = 1200 ;
    [SerializeField] private float EngineInertiaAccel = 0.1f;
    [SerializeField] private float EngineInertiaDecel = 0.1f;

    [Header("Engine Dynamics")]
    [SerializeField] private float BackTorque = 100f;  // reverse torque & engine losses
    [SerializeField] private float IdleThrottleBoost = 0.12f;

    [Header("Engine-Wheel Coupling")]
    [SerializeField] private float CouplingStrength = 5f;

    [Header("Rev Limiter")]
    [SerializeField] private bool UseRevLimiter = true;
    [SerializeField] private float RevLimiterRPM = 7000; 
    [SerializeField] private RevLimiterType limiterType = RevLimiterType.SoftCut;
    
    [SerializeField] private float _currentRPM;
    private bool _isRevLimiterActive = false;
    private float _revLimiterTimer = 0f;
    
    public enum RevLimiterType
    {
        HardCut,
        SoftCut,
        Ignition 
    }

    public void Init()
    {
        _currentRPM = IdleRPM;
    }

    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput input)
    {
        float dt = data.DeltaTime;
        float throttle = Mathf.Clamp01(input.ThrottleInput);

        // ================= IDLE CONTROL =================
        if (_currentRPM < IdleRPM)
            throttle = Mathf.Max(throttle, IdleThrottleBoost);

        // ================= BASE TORQUE ==================
        float baseTorque = TorqueCurve.Evaluate(_currentRPM);
        float engineTorque = baseTorque * throttle;

        // ================= ENGINE LOSSES ================
        float engineBraking = Mathf.Pow(1f - throttle, 2f) * BackTorque;

        // ================= REV LIMITER ==================
        if (UseRevLimiter)
            engineTorque = ApplyRevLimiter(engineTorque, ref data);

        // ================= DRIVETRAIN STATE =============
        bool drivetrainConnected =
            data.CurrentGear != 0 &&
            data.ClutchEngagement > 0.01f &&
            Mathf.Abs(data.CurrentTotalGearRatio) > 0.01f;

        // ================= TARGET RPM ===================
        float targetRPM = _currentRPM;

        if (drivetrainConnected)
        {
            targetRPM = Mathf.Abs(data.GeneralWheelsRPM * data.CurrentTotalGearRatio);
        }

        // ================= RPM DYNAMICS =================
        float rpmDelta = 0f;

        if (drivetrainConnected)
        {
            float rpmError = targetRPM - _currentRPM;
            rpmDelta += rpmError * CouplingStrength * data.ClutchEngagement;
        }

        float netTorque = engineTorque - engineBraking;

        float inertia;
        if (drivetrainConnected)
        {
            inertia = netTorque >= 0f ? EngineInertiaAccel : EngineInertiaDecel;
        }
        else
        {
            inertia = 0.06f;
        }

        rpmDelta += netTorque / Mathf.Max(0.001f, inertia);

        // ================= APPLY RPM ====================
        _currentRPM += rpmDelta * dt;

        // RPM не должен падать ниже idle сам по себе
        if (_currentRPM < IdleRPM && throttle <= IdleThrottleBoost + 0.01f)
            _currentRPM = Mathf.Lerp(_currentRPM, IdleRPM, dt * 5f);

        _currentRPM = Mathf.Clamp(_currentRPM, MinRPM, MaxRPM);

        // ================= TORQUE OUTPUT ================
        float torqueToTransmission = 0f;

        if (drivetrainConnected)
            torqueToTransmission = engineTorque * data.ClutchEngagement;

        // ================= WRITE DATA ===================
        data.EngineRPM = _currentRPM;
        data.EngineTorque = torqueToTransmission;
        data.EngineBraking = engineBraking;
        data.EngineInertia = inertia;
    }


    private float ApplyRevLimiter(float requestedTorque, ref CarPhysicsData data)
    {
        if (_currentRPM >= RevLimiterRPM)
        {
            _isRevLimiterActive = true;
                
            switch (limiterType)
            {
                case RevLimiterType.HardCut:
                    return 0;
                    
                case RevLimiterType.SoftCut:
                    float softCutRange = 200f; 
                    float overrev = _currentRPM - RevLimiterRPM;
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
    private float _totalRatio = 0f;

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

[System.Serializable]
public class BreakingSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        
    }
}

[System.Serializable]
public class SteeringSimulation
{
    [Header("Steering Range")]
    [SerializeField] private float SteeringRangeAtZeroSpeed = 35f;
    [SerializeField] private float SteeringRangeAtMaxSpeed = 5f;
    [SerializeField] private float MaxSpeedForSteering = 200f; 
    
    [Header("Steering Response")]
    [SerializeField] private float SteerSpeed = 180f; 
    [Tooltip("Returning to center obviously faster")]
    [SerializeField] private float ReturnSpeed = 240f;
    
    [Header("Deadzone (optional)")]
    [SerializeField] private float InputDeadzone = 0.1f;
    
    private float _currentSteerAngle = 0f;
    
    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        float steeringInput = playerInput.WheelsRotatingInput.x;
        
        float speedFactor = Mathf.Clamp01(data.SpeedKmH / MaxSpeedForSteering);
        float currentMaxAngle = Mathf.Lerp(SteeringRangeAtZeroSpeed, SteeringRangeAtMaxSpeed, speedFactor);
    
        float targetAngle = steeringInput * currentMaxAngle;
        
        float speed = Mathf.Abs(steeringInput) > 0.01f ? SteerSpeed : ReturnSpeed;
        _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, targetAngle, speed * data.DeltaTime);
        
        data.CurrentSteeringAngle = _currentSteerAngle;
    }

}

[System.Serializable]
public class TorqueDistributionSimulation
{
    public void UpdatePhysics(ref CarPhysicsData data)
    {
        
    }
}