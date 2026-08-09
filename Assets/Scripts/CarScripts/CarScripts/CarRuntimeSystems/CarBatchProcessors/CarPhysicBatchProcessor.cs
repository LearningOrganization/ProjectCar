using Unity.Collections;
using Unity.Splines.Examples;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CarPhysicBatchProcessor : MonoBehaviour
{

    private NativeArray<CarPhysicsData> _carsPhysicsData;
    private NativeArray<CarConfigData>  _carsConfigData;
    private ManagedObjects[] _managedObjects;
    private int _carAmount; 

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
            ref CarPhysicsData currCarPhysicsData = ref _carsPhysicsData.ExtractElementRef(i);

            //ref CarConfigData currCarConfigData = ref _carsConfigData.ExtractElementRef(i);


            // calculations

            //_steeringSimulation.UpdatePhysics(ref _carsPhysicsData[i], ref , ref );

            // forces applying (i have to reduce heap managed code hotpath calls    )
        }
    }

    public void InitPhysicBatchProcessor()
    {
        
    }
}
