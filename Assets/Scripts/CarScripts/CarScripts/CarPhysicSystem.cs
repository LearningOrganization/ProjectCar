using System.Data.Common;
using UnityEngine;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    [SerializeField] private AtmoEngineSimulation EngineSimulation;
    [SerializeField] private TransmissionSimulation TransmissionSimulation;
    [SerializeField] private BreakingSimulation BreakingSimulation;
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

    [Header("Debug")]
    [SerializeField] private bool ShowDebugInfo = false;

    public InputPermissions CurrentPermissions = InputPermissions.All;
    
    private CarPhysicsData _carPhysicsData;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    private bool disableCarEngineTorque = false;

    public float GetCurrentCarSpeed()
    {
        return _carPhysicsData.SpeedKmH;
    }

    public void DisableCarEngineTorque()
    {
       _carPhysicsData.CurrentGear = 0;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _carPhysicsData.Mass = _rb.mass;
        _carPhysicsData.CurrentGear = 0;

        _carPhysicsData.WheelInertia = MotorizedWheels[0].mass *
        MotorizedWheels[0].radius * MotorizedWheels[0].radius * 2.0f;
    }

    void Start()
    {
        EngineSimulation.Init();
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
        _carPhysicsData.Velocity = _rb.linearVelocity;

        ReadWheelData();
        UpdateVehicleSpeed();
        

        SteeringSimulation.UpdatePhysics(ref _carPhysicsData, ref _playerInput);

        // brakes 

        //wheels logic

        // transmission 
        TransmissionSimulation.UpdatePhysics(ref _carPhysicsData);

        // engine 
        EngineSimulation.UpdatePhysics(ref _carPhysicsData, ref _playerInput);

        ApplyBrakes();

        ApplyTorqueToWheels();

        //application of all forces 
        ApplySteering();


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
                wheel.steerAngle = _carPhysicsData.CurrentSteeringAngle;
            }
        }
    }

    private void UpdateGear()
    {
        if(_playerInput.ShiftUpRequested)
        {
            TransmissionSimulation.ShiftUp(ref _carPhysicsData);
            PlayerManager.Instance.Input.ConsumeShiftInputs();
        }
        else if(_playerInput.ShiftDownRequested)
        {
            TransmissionSimulation.ShiftDown(ref _carPhysicsData);

            if(_carPhysicsData.CurrentGear == 0)
                TransmissionSimulation.ShiftToReverse(ref _carPhysicsData);

            PlayerManager.Instance.Input.ConsumeShiftInputs();
        }
    }

    private void ApplyTorqueToWheels()
    {
        float totalTorque = _carPhysicsData.TransmissionTorque;
        bool isAlmostStopped = Mathf.Abs(_carPhysicsData.SpeedKmH) < 1f;
        bool inDrive = _carPhysicsData.CurrentGear > 0;

        foreach(var wheel in MotorizedWheels)
        {

            if(wheel == null) continue;
            float appliedTorque = totalTorque / MotorizedWheels.Length;
            WheelHit hit;
            wheel.GetGroundHit(out hit);

            float forwardSlip = Mathf.Abs(hit.forwardSlip);
            
            if(forwardSlip < 0.1f) 
            {
                if(_carPhysicsData.CurrentGear > 1)
                {
                    appliedTorque *= _carPhysicsData.CurrentGear * 2;    
                }
            }

            wheel.motorTorque = appliedTorque;
        }
    }
    private void ReadWheelData()
    {
       if(MotorizedWheels.Length == 0)
        {
            _carPhysicsData.GeneralWheelsRPM = 0f;
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
        _carPhysicsData.GeneralWheelsRPM = validWheels > 0 ? totalRPM / validWheels : 0f;
    }

    private void ApplyBrakes()
    {
        // should be implemented in fuure
    }

    private void UpdateVehicleSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);
        _carPhysicsData.SpeedMS = localVelocity.z;
        _carPhysicsData.SpeedKmH = _carPhysicsData.SpeedMS * 3.6f;
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

    // Gear shifting
    if (!CurrentPermissions.HasFlag(InputPermissions.GearShift))
    {
        _playerInput.ShiftUpRequested = false;
        _playerInput.ShiftDownRequested = false;
    }
}
    private void ShowDebug()
    {
        string gearName = _carPhysicsData.CurrentGear switch
        {
            -1 => "R",
            0 => "N",
            _ => _carPhysicsData.CurrentGear.ToString()
        };
        
        Debug.Log($"RPM: {_carPhysicsData.EngineRPM:F0} | " +
                  $"Gear: {gearName} | " +
                  $"Speed: {_carPhysicsData.SpeedKmH:F1} km/h | " +
                  $"Torque: {_carPhysicsData.EngineTorque:F0} Nm | " +
                  $"WheelsRPM: {_carPhysicsData.GeneralWheelsRPM:F0} | " +
                  $"Clutch: {_carPhysicsData.ClutchEngagement:F2} | " +
                  $"TransTorque: {_carPhysicsData.TransmissionTorque:F0} Nm");
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
