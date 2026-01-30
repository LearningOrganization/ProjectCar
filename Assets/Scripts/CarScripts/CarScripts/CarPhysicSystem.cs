using UnityEngine;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    [SerializeField] private EngineSimulation EngineSimulation;
    [SerializeField] private TransmissionSimulation TransmissionSimulation;
    [SerializeField] private TorqueDistributionSimulation TorqueDistributionSimulation;
    [SerializeField] private BreakingSimulation BreakingSimulation;
    [SerializeField] private SteeringSimulation SteeringSimulation;

    [Header("Steer wheels")]
    [SerializeField] private WheelCollider[] SteeringWheels;
    [Header("Motorized wheels")]
    [SerializeField] private WheelCollider[] MotorizedWheels;

    [Header("Debug")]
    [SerializeField] private bool ShowDebugInfo = false;
    
    private CarPhysicsData _carPhysicsData;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _carPhysicsData.Mass = _rb.mass;
        _carPhysicsData.CurrentGear = 0;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput = PlayerManager.Instance.Input.PlayerInput;
        UpdateGear();
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

        ApplyTorqueToWheels();

        //brakes

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

        if (MotorizedWheels[0] != null)
            MotorizedWheels[0].motorTorque = totalTorque / 2f;
        if (MotorizedWheels[1] != null)
            MotorizedWheels[1].motorTorque = totalTorque / 2f;

         _carPhysicsData.GeneralWheelsTorque = totalTorque;
    }

    private void ReadWheelData()
    {
        float leftRPM = 0f;
        float rightRPM = 0f;

        leftRPM = MotorizedWheels[0] != null ? MotorizedWheels[0].rpm : 0f;

        rightRPM = MotorizedWheels[1] != null ? MotorizedWheels[1].rpm : 0f;

        _carPhysicsData.GeneralWheelsRPM = (leftRPM + rightRPM) / 2f;
    }

    private void ApplyBrakes()
    {
        
    }

    private void UpdateVehicleSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);
        _carPhysicsData.SpeedMS = localVelocity.z;
        _carPhysicsData.SpeedKmH = _carPhysicsData.SpeedMS * 3.6f;
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
}
