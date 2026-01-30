using System;
using UnityEngine;

//
//
/// <summary>
///generate power P=w*N
/// </summary>
//
//

public class Engine : MonoBehaviour
{
    [Header("Engine Specifications")]
    public AnimationCurve TorqueCurve;
    public AnimationCurve PowerCurve;

    [Header("RPM Settings")]
    public float MinRPM;
    public float MaxRPM;
    public float IdleRPM;
    public float CurrentRPM;

    [Header("Torque Settings")]
    public float MaxTorque;
    public float CurrentTorque; // N/m

    [Header("Power Settings")]
    public float MaxPower;
    public float CurrentPower; // in Watt`s

    [Header("Engine Behavior")]
    public float ThrottleResponse;
    public float BasicEngineInertia = 1f;
    public float EngineBraking;

    public float InertiaMultiplier;

    private float _currentEngineInertia;

    [Header("Input")]
    [Range(0f, 1f)] public float ThrottleInput = 0f;

    private float _targetRPM;

    private void InitEngine()
    {
        var keys = TorqueCurve.keys;

        // init RPM
        MinRPM = keys[0].time;
        MaxRPM = keys[TorqueCurve.length - 1].time;

        IdleRPM = MinRPM;

        //add inertia
        _currentEngineInertia = BasicEngineInertia;

        CurrentRPM = IdleRPM + 1;
    }

    public void IncreaseEngineRPM()
    {
        _targetRPM = Mathf.Lerp(IdleRPM, MaxRPM, ThrottleInput);

        if (ThrottleInput > 0.01f)
        {
            float rpmDelta = (_targetRPM - CurrentRPM) / _currentEngineInertia;
            CurrentRPM += rpmDelta * ThrottleInput * Time.deltaTime;

            CurrentRPM = Mathf.Clamp(CurrentRPM, IdleRPM, MaxRPM);

            CurrentTorque = TorqueCurve.Evaluate(CurrentRPM);

            CurrentPower = PowerCurve.Evaluate(CurrentRPM);

        }
    }

    public void DecreaseEngineRPMOnNeutralGear()
    {
        if (ThrottleInput < 0.01f)
        {
            float brakingSpeed = EngineBraking / _currentEngineInertia;
            CurrentRPM = Mathf.MoveTowards(CurrentRPM, IdleRPM, brakingSpeed * Time.deltaTime);

            CurrentRPM = Mathf.Clamp(CurrentRPM, IdleRPM, MaxRPM);

            CurrentTorque = -(TorqueCurve.Evaluate(CurrentRPM) * 0.25f);

            CurrentPower = PowerCurve.Evaluate(CurrentRPM);
        }
    }

    public void SetMotorRPMFromTransmission(float transmissionRPM)
    {
        float rpmDelta = (transmissionRPM - CurrentRPM) / _currentEngineInertia;

        CurrentRPM += rpmDelta * Time.deltaTime;

        CurrentRPM = Mathf.Clamp(CurrentRPM, MinRPM, MaxRPM);

        CurrentTorque = TorqueCurve.Evaluate(CurrentRPM);

        if (ThrottleInput > 0.01f)
        {
            CurrentTorque = Math.Max(0f, CurrentTorque);
        }
        else
        {
            CurrentTorque = -Mathf.Abs(CurrentTorque * 0.25f);
        }

        CurrentPower = Mathf.Max(0f, PowerCurve.Evaluate(CurrentRPM));
    }

    private void Start()
    {
        InitEngine();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //UpdateRPM();
    }


}
