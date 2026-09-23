using Unity.Collections;
using Unity.Splines.Examples;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CarPhysicBatchProcessor : MonoBehaviour
{
    // config data 
    private NativeArray<EngineBatchConfig> _engineBatchConfigs;
    private NativeArray<TransmissionBatchConfig> _transmissionBatchConfigs;
    private NativeArray<BrakeConfig> _brakeConfigs;
    private NativeArray<SteeringConfig> _steeringConfigs;
    // curr physic data
    private NativeArray<CarPhysicsData> _carPhysicsData;

    // manage objectts, never burst compiled
    private Rigidbody[] _rigidbodies;
    private WheelCollider[] _steeringWheels;
    private WheelCollider[] _motorizedWheels;
    private Transform[] _wheelMeshes;


    private int _carAmount; 
    private int _capacity;
    private PlayerInput _sharedPlayerInput;

    private AtmoEngineSimulation _atmoEngineSimulation;
    private BrakeSimulation _brakeSimulation;
    private SteeringSimulation _steeringSimulation;
    private TransmissionSimulation _transmissionSimulation;



    private void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        for(int i = 0; i < _carAmount; i++)
        {
            //ref CarPhysicsData currCarPhysicsData = ref _carsPhysicsData.ExtractElementRef(i);

            //ref CarConfigData currCarConfigData = ref _carsConfigData.ExtractElementRef(i);


            // calculations

            //_steeringSimulation.UpdatePhysics(ref _carsPhysicsData[i], ref , ref );

            // forces applying (i have to reduce heap managed code hotpath calls    )
        }
    }

    public void InitPhysicBatchProcessor(int capacity)
    {
       _capacity = capacity;
       _carAmount = 0;

       



    }
}
