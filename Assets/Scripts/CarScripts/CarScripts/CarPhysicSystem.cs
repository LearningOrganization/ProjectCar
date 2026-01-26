using UnityEngine;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    public EngineSimulation EngineSimulation;
    public TransmissionSimulation TransmissionSimulation;
    public  BreakingSimulation BreakingSimulation;
    public SteeringSimulation steeringControl;

    [Header("Debug")]
    public bool ShowDebugInfo = false;

    private CarPhysicsData _carPhysicsData;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _carPhysicsData.Mass = _rb.mass;
        _carPhysicsData.CurrentGear = 0;

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        _carPhysicsData.DeltaTime = Time.deltaTime;
        _carPhysicsData.Velocity = _rb.linearVelocity;

        // brakes 

        // wheels and transmission 

        // engine 

        // steering

        if(ShowDebugInfo)
        {
            ShowDebug();
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
