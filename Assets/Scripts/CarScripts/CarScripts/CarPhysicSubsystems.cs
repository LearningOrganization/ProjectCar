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
    [SerializeField] private float MaxRPM = 8000 ;
    [SerializeField] private float IdleRPM = 1500 ;
    [SerializeField] private float EngineInertia = 0.3f;
    [SerializeField] private float EngineFriction = 15f;
    [SerializeField] private float MaxEngineBrakingTorque = 50f;

    [Header("Rev Limiter")]
    [SerializeField] private bool UseRevLimiter = true;
    [SerializeField] private float RevLimiterRPM = 7800; 
    [SerializeField] private RevLimiterType limiterType = RevLimiterType.HardCut;
    
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

    public void UpdatePhysics(ref CarPhysicsData data, ref PlayerInput playerInput)
    {
        float targetRPMFromWheels = CalculateRPMFromWheels(ref data);

        float maxTorque = TorqueCurve.Evaluate(_currentRPM);

        // calculation of required torque
        float requestedTorque  = maxTorque * Mathf.Clamp01(playerInput.ThrottleInput);

        // REV LIMITER
        if (UseRevLimiter)
        {
            requestedTorque = ApplyRevLimiter(requestedTorque, ref data);
        }

        //engine braking logic
        float engineBraking = 0f;
        if(playerInput.ThrottleInput < 0.1f && _currentRPM > IdleRPM)
        {
            engineBraking = Mathf.Lerp(0f, MaxEngineBrakingTorque,
            (_currentRPM - IdleRPM) / (MaxRPM - IdleRPM));
        }

        // engine RPM dependency from clutch
        if(data.CurrentGear == 0 || data.ClutchEngagement < 0.1f )
        {
            UpdateEngineRPMFreeRunning(ref data, requestedTorque, playerInput.ThrottleInput);
        }
        else if(data.ClutchEngagement > 0.9f)
        {
            _currentRPM = targetRPMFromWheels;
            _currentRPM = Mathf.Clamp(_currentRPM, IdleRPM, MaxRPM);
        }
        else
        {
            float previousRPM = _currentRPM;
            UpdateEngineRPMFreeRunning(ref data, requestedTorque, playerInput.ThrottleInput);
            float freeRPM = _currentRPM;
            
            _currentRPM = Mathf.Lerp(freeRPM, targetRPMFromWheels, data.ClutchEngagement);
            _currentRPM = Mathf.Clamp(_currentRPM, IdleRPM, MaxRPM);
        }

        data.EngineRPM = _currentRPM;
        data.EngineTorque = requestedTorque;
        data.EngineBraking = engineBraking;
        data.EngineInertia = EngineInertia;
    }

    private float ApplyRevLimiter(float requestedTorque, ref CarPhysicsData data)
    {
        if (_currentRPM >= RevLimiterRPM)
        {
            _isRevLimiterActive = true;
            
            switch (limiterType)
            {
                case RevLimiterType.HardCut:
                    return 0f;
                
                case RevLimiterType.SoftCut:
                    float overrev = _currentRPM - RevLimiterRPM;
                    float softCutRange = 200f; 
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

    private void UpdateEngineRPMFreeRunning(ref CarPhysicsData data, float requestedTorque, float throttle)
    {
        float friction = EngineFriction * (_currentRPM / MaxRPM);
        float netTorque = requestedTorque - friction;
        
        if (throttle < 0.1f)
        {
            if (_currentRPM > IdleRPM)
            {
                netTorque = -EngineFriction * 2f;
            }
            else
            {
                _currentRPM = IdleRPM;
                return;
            }
        }

        float angularAcceleration = netTorque / EngineInertia;
        float rpmChange = angularAcceleration * data.DeltaTime * 9.5493f;
        
        _currentRPM += rpmChange;
        _currentRPM = Mathf.Clamp(_currentRPM, IdleRPM, MaxRPM);
    }

    private float CalculateRPMFromWheels(ref CarPhysicsData data)
    {
        if(data.CurrentGear == 0 )
            return IdleRPM;

        float totalRatio = Math.Abs(data.CurrentTotalGearRatio);
        float targetRPM = data.GeneralWheelsRPM * totalRatio;

         return Mathf.Max(targetRPM, IdleRPM);
    }
}

[System.Serializable]
public class TransmissionSimulation
{
    [Header("Gear specs")]
    [SerializeField] private float[] ForwardGearRatios = { 3.214f, 1.925f, 1.302f, 1.000f, 0.752f };
    [SerializeField] private float[] ReverseRatio =  {-3.2f};
    [SerializeField] private float FinalGearRatio = 4.111f;
    [SerializeField] private float TransmissionEfficiency = 0.85f;
        
    [Header("Shift Settings")]
    [SerializeField] private float ShiftTime = 5f;
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
    }
    private void CalculateTransmissionTorque(ref CarPhysicsData data)
    {
        _totalRatio = data.CurrentGearRatio * FinalGearRatio * TransmissionEfficiency;

        data.CurrentTotalGearRatio = _totalRatio;

        data.TransmissionTorque = _totalRatio * data.EngineTorque * data.ClutchEngagement;
        
        data.TransmissionTorque -= data.EngineBraking * _totalRatio;
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