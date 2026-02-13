// using System;
// using System.Collections;
// using UnityEngine;

// public class SteeringControl : MonoBehaviour
// {
//     [Header("Steer wheels")]
//     public WheelControl[] SteeringWheels;

//     public float SteeringRange = 30f;
//     public float SteerSpeed = 5f;
//     public float SteeringRangeAtMaxSpeed = 5f;

//     private RWDTransmission transmission;
//     private float _currentSteerRange;
//     private float _wheelsRotating;
//     private PlayerInputControls _input;



//     public void Steering(WheelControl wheel)
//     {
//         _currentSteerRange = Mathf.Lerp(SteeringRange, SteeringRangeAtMaxSpeed, transmission.SpeedKmH/350);

//         float targetSteerAngle = _wheelsRotating * _currentSteerRange;

//         wheel.CurrentSteerAngle = Mathf.MoveTowards(
//             wheel.CurrentSteerAngle,
//             targetSteerAngle,
//             SteerSpeed * Time.deltaTime
//         );

//         wheel.WheelCollider.steerAngle = wheel.CurrentSteerAngle;
//     }

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     private void Start()
//     {
//         _input = PlayerManager.Instance.Input;
//         _wheelsRotating = _input.PlayerInput.WheelsRotating.x;
//         transmission = this.GetComponent<RWDTransmission>();
//     }

//     // Update is called once per frame
//     private void Update()
//     {

//     }

//     private void FixedUpdate()
//     {
//         _wheelsRotating = _input.PlayerInput.WheelsRotating.x;

//         foreach (WheelControl wheel in SteeringWheels)
//         {
//             if (wheel.Steerable)
//             {
//                 Steering(wheel);
//             }
//         } 
//     }


// }
