using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    [SerializeField] private AtmoEngineSimulation EngineSimulation;
    [SerializeField] private TransmissionSimulation TransmissionSimulation;
    [SerializeField] private BrakeSimulation BrakeSimulation;
    [SerializeField] private SteeringSimulation SteeringSimulation;

    [Header("Steer wheels")]
    [SerializeField] private WheelCollider[] SteeringWheels;
    [Header("Motorized wheels")]
    [SerializeField] private WheelCollider[] MotorizedWheels;
    [Header("Wheels mesh")]
    [SerializeField] private Transform LeftFrontWheelMesh;
    [SerializeField] private Transform RightFrontWheelMesh;
    [SerializeField] private Transform LeftRearWheelMesh;
    [SerializeField] private Transform RightRearWheelMesh;

    public CarConfigData CarConfigData;

    [Header("Debug")]
    [SerializeField] private bool ShowDebugInfo = false;

    public InputPermissions CurrentPermissions = InputPermissions.All;

    [HideInInspector]
    public event Action<CarTelemetry> OnTelemetryUpdated; 

    private CarPhysicsData _carPhysicsData;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    private bool disableCarEngineTorque = false;
    private float _uiTimer;
    private float _uiUpdateStep = 0.05f; // 20 hz upadte

    public float GetCurrentCarSpeed()
    {
        return _carPhysicsData.Vehicle.SpeedKmH;
    }

    public void DisableCarEngineTorque()
    {
       _carPhysicsData.Transmission.CurrentGear = 0;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _carPhysicsData.Vehicle.Mass = _rb.mass;
        _carPhysicsData.Transmission.CurrentGear = 0;

    }

    void Start()
    {
        EngineSimulation.Init(ref CarConfigData.EngineConfig);
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput = PlayerManager.Instance.Input.PlayerInput;
        
        ApplyInputPermissions();

        UpdateGear();

        RenderdWheels();
    }

    void FixedUpdate()
    {
        _carPhysicsData.DeltaTime = Time.fixedDeltaTime;
        _carPhysicsData.Vehicle.Velocity = _rb.linearVelocity;

        ReadWheelData();
        UpdateVehicleSpeed();
        
        SteeringSimulation.UpdatePhysics(ref _carPhysicsData, ref CarConfigData.SteeringConfig, ref _playerInput);

        //wheels logic

        // transmission 
        TransmissionSimulation.UpdatePhysics(ref _carPhysicsData, ref CarConfigData.TransmissionConfig);

        // engine 
        EngineSimulation.UpdatePhysics(ref _carPhysicsData, ref CarConfigData.EngineConfig, ref _playerInput);

        // brakes 
        BrakeSimulation.UpdatePhysics(ref _carPhysicsData, ref CarConfigData.BreakConfig, ref _playerInput);

        ApplyBrakes();

        ApplyTorqueToWheels();

        //application of all forces 
        ApplySteering();

        _uiTimer += Time.deltaTime;

        if(_uiTimer > _uiUpdateStep)
        {
            _uiTimer = 0f;
            OnTelemetryUpdated?.Invoke(new CarTelemetry
            {
                Speed = _carPhysicsData.Vehicle.SpeedKmH,
                RPM = _carPhysicsData.Engine.EngineRPM,
                MinRPM = 0,
                MaxRPM = CarConfigData.EngineConfig.MaxRPM,
                Gear = (Gear)_carPhysicsData.Transmission.CurrentGear,
                
            });
        }

        if(ShowDebugInfo)
        {
            ShowDebug();
        }
    }

    private void ApplySteering()
    {
        foreach(WheelCollider wheel in SteeringWheels)
        {
            if(wheel != null)
            {
                wheel.steerAngle = _carPhysicsData.Steering.CurrentSteeringAngle;
            }
        }
    }

    private void UpdateGear()
    {
        if (!CurrentPermissions.HasFlag(InputPermissions.GearShift))
            return;

        _playerInput.ShiftCommand = PlayerManager.Instance.Input.ConsumeShiftCommand();

        switch ( _playerInput.ShiftCommand)
        {
            case GearShiftCommand.Up:
                TransmissionSimulation.ShiftUp(ref _carPhysicsData, ref CarConfigData.TransmissionConfig);
                break;

            // case GearShiftCommand.Down:
            //     TransmissionSimulation.ShiftDown(ref _carPhysicsData, ref CarConfigData.TransmissionConfig);
            //     break;

            case GearShiftCommand.Down:
            if (_carPhysicsData.Vehicle.SpeedKmH < CarConfigData.TransmissionConfig.ReverseMaxSpeed)
            {
                if (_carPhysicsData.Transmission.CurrentGear == 0)
                {
                    _carPhysicsData.Transmission.CurrentGear = -1;
                }
                else
                {
                    TransmissionSimulation.ShiftDown(ref _carPhysicsData, ref CarConfigData.TransmissionConfig);
                }
            }
            else
            {
                TransmissionSimulation.ShiftDown(ref _carPhysicsData, ref CarConfigData.TransmissionConfig);
            }
            break;
        }
    }

    private void ApplyTorqueToWheels()
    {
        float totalTorque = _carPhysicsData.Transmission.TransmissionTorque;
        bool isAlmostStopped = Mathf.Abs(_carPhysicsData.Vehicle.SpeedKmH) < 0.3f;
        bool inDrive = _carPhysicsData.Transmission.CurrentGear > 0;

        bool inForwardGear = _carPhysicsData.Transmission.CurrentGear > 0;

        if (inForwardGear && _carPhysicsData.Vehicle.SpeedMS < -0.5f && _playerInput.ThrottleInput < 0.01f)
        {
            totalTorque = 0f;
        }

        foreach(var wheel in MotorizedWheels)
        {

            if(wheel == null) continue;
            float appliedTorque = totalTorque / MotorizedWheels.Length;
            WheelHit hit;
            wheel.GetGroundHit(out hit);

            float forwardSlip = Mathf.Abs(hit.forwardSlip);
            Debug.Log($"Slip: {forwardSlip}");
            if(forwardSlip < 0.1f) 
            {
                

                if(_carPhysicsData.Transmission.CurrentGear > 1)
                {
                    // I have to solve this shit 
                    appliedTorque *= _carPhysicsData.Transmission.CurrentGear ;   
                    //appliedTorque += 5000; 
                    //hit.forwardSlip = 0.2f;
                }
            }

            wheel.motorTorque = appliedTorque;
        }
    }
    
    private void ReadWheelData()
    {
       if(MotorizedWheels.Length == 0)
        {
            _carPhysicsData.WheelData.GeneralWheelsRPM = 0f;
            return;
        }

        float totalRPM = 0f;
        int validWheels = 0;

        foreach (var wheel in MotorizedWheels)
        {
            if (wheel != null)
            {
                totalRPM += wheel.rpm;
                validWheels++;
            }
        }
        _carPhysicsData.WheelData.GeneralWheelsRPM = validWheels > 0 ? totalRPM / validWheels : 0f;
    }

    private void ApplyBrakes()
    {
        float frontTorque = _carPhysicsData.Brake.FrontBrakeTorque;
        float rearTorque  = _carPhysicsData.Brake.RearBrakeTorque;

        foreach (var wheel in SteeringWheels)
        {
            if (wheel != null)
                wheel.brakeTorque = frontTorque;
        }

        foreach (var wheel in MotorizedWheels)
        {
            if (wheel != null)
                wheel.brakeTorque = rearTorque;
        }
    }

    private void UpdateVehicleSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);
        _carPhysicsData.Vehicle.SpeedMS = localVelocity.z;
        _carPhysicsData.Vehicle.SpeedKmH = _carPhysicsData.Vehicle.SpeedMS * 3.6f;
    }

    private void ApplyInputPermissions()
    {
        // Steering
        if (!CurrentPermissions.HasFlag(InputPermissions.Steering))
        {
            _playerInput.WheelsRotatingInput = Vector2.zero;
        }

        // Driving
        if (!CurrentPermissions.HasFlag(InputPermissions.Driving))
        {
            _playerInput.ThrottleInput = 0f;
            _playerInput.BrakeInput = 0f;
            _playerInput.Handbrake = false;
        }

        // Camera
        if (!CurrentPermissions.HasFlag(InputPermissions.Camera))
        {
            _playerInput.Look = Vector2.zero;
        }

        // // Gear shifting
        // if (!CurrentPermissions.HasFlag(InputPermissions.GearShift))
        // {
        //     _playerInput.ShiftCommand = GearShiftCommand.None;
        // }
    }
    private void ShowDebug()
    {
        string gearName = _carPhysicsData.Transmission.CurrentGear switch
        {
            -1 => "R",
            0 => "N",
            _ => _carPhysicsData.Transmission.CurrentGear.ToString()
        };
        
        Debug.Log($"RPM: {_carPhysicsData.Engine.EngineRPM:F0} | " +
                  $"Gear: {gearName} | " +
                  $"Speed: {_carPhysicsData.Vehicle.SpeedKmH:F1} km/h | " +
                  $"Torque: {_carPhysicsData.Engine.EngineTorque:F0} Nm | " +
                  $"WheelsRPM: {_carPhysicsData.WheelData.GeneralWheelsRPM:F0} | " +
                  $"Clutch: {_carPhysicsData.Transmission.ClutchEngagement:F2} | " +
                  $"TransTorque: {_carPhysicsData.Transmission.TransmissionTorque:F0} Nm");
    }

    private void RenderdWheels()
    {
        SetRenderWheel(SteeringWheels[0], LeftFrontWheelMesh);
        SetRenderWheel(SteeringWheels[1], RightFrontWheelMesh);
        SetRenderWheel(MotorizedWheels[0], LeftRearWheelMesh);
        SetRenderWheel(MotorizedWheels[1], RightRearWheelMesh);
    }
    private void SetRenderWheel(WheelCollider collider, Transform mesh)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        mesh.position = position;
        mesh.rotation = rotation;
    }
}
