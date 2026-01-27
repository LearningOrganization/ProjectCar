using UnityEngine;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    [SerializeField] private EngineSimulation EngineSimulation;
    [SerializeField] private TransmissionSimulation TransmissionSimulation;
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
       
    }

    void FixedUpdate()
    {
        _playerInput = PlayerManager.Instance.Input.PlayerInput;

        _carPhysicsData.DeltaTime = Time.deltaTime;
        _carPhysicsData.Velocity = _rb.linearVelocity;

        SteeringSimulation.UpdatePhysics(ref _carPhysicsData, ref _playerInput);

        // brakes 

        // wheels and transmission 

        // engine 

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

    private void ShowDebug()
    {
         Debug.Log($"RPM: {_carPhysicsData.EngineRPM:F0} | " +
                  $"Gear: {_carPhysicsData.CurrentGear} | " +
                  $"Speed: {_carPhysicsData.SpeedKmH:F1} km/h | " +
                  $"Torque: {_carPhysicsData.EngineTorque:F0} Nm");
    }
}
