using UnityEngine;

public class CarPhysicSystem : MonoBehaviour
{
    [Header("Physic Components")]
    public EngineSimulation EngineSimulation;
    public TransmissionSimulation TransmissionSimulation;
    public  BreakingSimulation BreakingSimulation;
    public SteeringSimulation steeringControl;

    [Header("Debug")]
    public bool ShowDebugInfo = true;

    private CarPhysicData _carPhysicData;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _carPhysicData.Mass = _rb.mass;
        _carPhysicData.CurrentGear = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // input reading
        

    }

    void FixedUpdate()
    {
        
    }
}
