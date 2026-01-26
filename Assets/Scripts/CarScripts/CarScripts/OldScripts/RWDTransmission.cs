// using System;
// using System.Collections;
// using System.ComponentModel;
// using System.Numerics;
// using System.Runtime.CompilerServices;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.Rendering;

// //
// //  
// //
// // 
// public class RWDTransmission : MonoBehaviour
// {
//     [Header("Transmission components")]
//     public Engine Engine;
//     public WheelControl[] Wheels;

//     [Header("Gear specs")]
//     public float[] GearRatios = { 0.0f, 3.214f, 1.925f, 1.302f, 1.000f, 0.752f };
//     public float FinalGearRatio = 4.111f;
//     public float TransmissionEfficiency = 0.85f;

//     [Header("Settings")]
//     public bool IsAutomatic = true;
//     public float UpshiftRPM;
//     public float DownshiftRPM;
//     public float ShiftingTime = 0.1f;

//     [Header("Wheels")]
//     public float WheelRadius; // meters

//     [Header("Current state")]
//     public int CurrentGear = 0;
//     public float SpeedMS;
//     public float SpeedKmH;
//     public float WheelRPM;
//     public float PrimaryShaftRPM;

//     public float ShiftCooldown = 10f;
//     private float _lastShiftTime = -999f;
//     private bool _isShifting;

//     // Inputs
//     private float _throttleInput;
//     private bool _shiftUpInput;
//     private bool _shiftDownInput;
//     private Rigidbody _rigidbody;
//     private PlayerInputControls _input;

//     void Start()
//     {
//         _input = PlayerManager.Instance.Input;
//         _rigidbody = GetComponent<Rigidbody>();
//         WheelRadius = Wheels[0].WheelCollider.radius;
//     }

//     void FixedUpdate()
//     {
//         ReadInput();

//         SpeedMS = (float)Math.Round(_rigidbody.linearVelocity.magnitude, 4, MidpointRounding.AwayFromZero);
//         SpeedKmH = (float)(SpeedMS * 3.6);

//         Engine.ThrottleInput = _throttleInput;

//         CalcRPM();

//         if (!_isShifting)
//         {
//             if (CurrentGear > 0)
//             {

//                 if (_throttleInput > 0.01f)
//                 {
//                     Engine.IncreaseEngineRPM();
//                 }
//                 else if (_throttleInput < 0.01f)
//                 {
//                     //Engine.DecreaseEngineRPMOnNeutralGear();
//                     Engine.SetMotorRPMFromTransmission(PrimaryShaftRPM);
//                 }
//             }
//             else
//             {

//                 if (_throttleInput > 0.01f)
//                 {
//                     Engine.IncreaseEngineRPM();
//                 }
//             }
//         }
//         else
//         {
//             if (_throttleInput > 0.01f)
//                 Engine.IncreaseEngineRPM();
//             else
//                 Engine.DecreaseEngineRPMOnNeutralGear();
//         }

//         if (IsAutomatic)
//             HandleAutomaticTransmission();
//         else
//             HandleManualShifting();

//         ApplyTorqueToWheels();
//     }

//     private void ReadInput()
//     {
//         //_throttleInput = _input.PlayerInput.AccelerationAndBrake.y;
//         // _shiftUpInput = Input.GetKeyDown(KeyCode.E);
//         // _shiftDownInput = Input.GetKeyDown(KeyCode.Q);
//     }

//     private void HandleAutomaticTransmission()
//     {
//         if (Engine.CurrentRPM <= DownshiftRPM && CurrentGear > 1)
//         {
//             float maxSpeedForLowerGear = CalculateMaxSpeedForGear(CurrentGear - 1);
//             if (SpeedKmH < maxSpeedForLowerGear * 0.8f)
//             {
//                 StartCoroutine(ShiftDowntWithDelay(CurrentGear - 1));
//             }
//         }

//         if (_isShifting || Time.time - _lastShiftTime < ShiftCooldown)
//             return;

//         if (Engine.CurrentRPM >= UpshiftRPM && CurrentGear < GearRatios.Length - 1 && _throttleInput > 0.1f)
//         {
//             StartCoroutine(ShiftUptWithDelay(CurrentGear + 1));
//         }

//     }

//     private void HandleManualShifting()
//     {
//         if (_isShifting) return;

//         if (_shiftUpInput && CurrentGear < GearRatios.Length - 1)
//         {
//             StartCoroutine(ShiftUptWithDelay(CurrentGear + 1));
//         }
//         else if (_shiftDownInput && CurrentGear > 0)
//         {
//             StartCoroutine(ShiftDowntWithDelay(CurrentGear - 1));
//         }
//     }

//     private IEnumerator ShiftUptWithDelay(int newGear)
//     {
//         _isShifting = true;
//         _lastShiftTime = Time.time;

//         yield return new WaitForSeconds(ShiftingTime);

//         CurrentGear = Mathf.Clamp(newGear, 0, GearRatios.Length - 1);
//         // set engine rpm   
//         Engine.SetMotorRPMFromTransmission(PrimaryShaftRPM);

//         //Debug.Log(Engine.CurrentRPM + " " + CurrentGear + " " + PrimaryShaftRPM + " " + WheelRPM);

//         _isShifting = false;

//     }

//     private IEnumerator ShiftDowntWithDelay(int newGear)
//     {
//         _isShifting = true;
//         _lastShiftTime = Time.time;

//         yield return new WaitForSeconds(ShiftingTime);

//         CurrentGear = Mathf.Clamp(newGear, 0, GearRatios.Length - 1);

//         // set engine rpm   
//         Engine.SetMotorRPMFromTransmission(PrimaryShaftRPM);

//         _isShifting = false;

//     }

//     private float CalculateMaxSpeedForGear(int gear)
//     {
//         if (gear <= 0 || gear >= GearRatios.Length) return 0f;

//         float wheelCircumference = 2 * Mathf.PI * WheelRadius;
//         float maxWheelRPM = Engine.MaxRPM / (GearRatios[gear] * FinalGearRatio);
//         float maxSpeedMS = (maxWheelRPM * wheelCircumference) / 60f;

//         return maxSpeedMS * 3.6f;
//     }

//     private void CalcRPM()
//     {
//         float totalRPM = 0f;
//         int motorizedWheels = 0;

//         foreach (var wheel in Wheels)
//         {
//             if (wheel.Motorized)
//             {
//                 totalRPM += Mathf.Abs(wheel.WheelCollider.rpm);
//                 motorizedWheels++;
//             }
//         }

//         if (motorizedWheels > 0)
//             WheelRPM = totalRPM / motorizedWheels;
//         else
//             WheelRPM = 0f;

//         if (CurrentGear > 0 && CurrentGear < GearRatios.Length)
//         {
//             PrimaryShaftRPM = WheelRPM * FinalGearRatio * GearRatios[CurrentGear];
//         }
//         else
//         {
//             PrimaryShaftRPM = 0f;
//         }

//         //Debug.Log(PrimaryShaftRPM);
//     }

//     private void ApplyTorqueToWheels()
//     {
//         if (!_isShifting)
//         {
//             float totalRatio = GearRatios[CurrentGear] * FinalGearRatio;
//             float wheelTorque = Engine.CurrentTorque * TransmissionEfficiency * totalRatio;

//             foreach (var wheel in Wheels)
//                 if (wheel.Motorized)
//                     wheel.WheelCollider.motorTorque = wheelTorque / 2;
//         }
//         else
//         {
//             foreach (var wheel in Wheels)
//                 if (wheel.Motorized)
//                     wheel.WheelCollider.motorTorque = 0;
//         }
//     }
// }