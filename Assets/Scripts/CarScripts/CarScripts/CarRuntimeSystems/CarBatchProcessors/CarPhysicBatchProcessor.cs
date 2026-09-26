using Unity.Collections;
using Unity.Jobs;
using Unity.Splines.Examples;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Callbacks;
using UnityEngine;

public class CarPhysicBatchProcessor : MonoBehaviour
{
    // config data 
    public NativeArray<EngineBatchConfig> _engineBatchConfigs;
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
    public int _capacity;
    private PlayerInput _sharedPlayerInput;

    private AtmoEngineSimulation _atmoEngineSimulation;
    private BrakeSimulation _brakeSimulation;
    private SteeringSimulation _steeringSimulation;
    private TransmissionSimulation _transmissionSimulation;

    public void InitPhysicBatchProcessor(int capacity)
    {
        _capacity = capacity;
        _carAmount = 0;

        _engineBatchConfigs = new NativeArray<EngineBatchConfig>(capacity, Allocator.Persistent);
        _transmissionBatchConfigs = new NativeArray<TransmissionBatchConfig>(capacity, Allocator.Persistent);
        _brakeConfigs = new NativeArray<BrakeConfig>(capacity, Allocator.Persistent);
        _steeringConfigs = new NativeArray<SteeringConfig>(capacity, Allocator.Persistent);
        _carPhysicsData = new NativeArray<CarPhysicsData>(capacity, Allocator.Persistent);

        _rigidbodies = new Rigidbody[capacity];
        _steeringWheels = new WheelCollider[capacity * 2];
        _motorizedWheels = new WheelCollider[capacity * 2];
        _wheelMeshes = new Transform[capacity * 4];
    }

    public int RegisterCar(
        CarConfigData carConfigData,
        Rigidbody rigidbody,
        WheelCollider leftFront, WheelCollider rightFront,
        WheelCollider left, WheelCollider right,
        Transform lfMesh, Transform rfMesh, Transform lrMesh, Transform rrMesh
    )
    {

        if(_carAmount >= _capacity)
        {
            Debug.Log("Overloading of car amount");
            return -1;
        }

        int index = _carAmount ++;

        _engineBatchConfigs[index] = CarConfigConverter.ToBatchConfig(carConfigData.EngineConfig);
        _transmissionBatchConfigs[index] = CarConfigConverter.ToBatchConfig(carConfigData.TransmissionConfig);
        _brakeConfigs[index] = carConfigData.BreakConfig;
        _steeringConfigs[index] = carConfigData.SteeringConfig;

        var data = new CarPhysicsData();
        data.Vehicle.Mass = rigidbody.mass;
        data.Transmission.CurrentGear = 0;
        _carPhysicsData[index] = data;

        _rigidbodies[index] = rigidbody;
        _steeringWheels[index * 2 + 0] = leftFront;
        _steeringWheels[index * 2 + 1] = rightFront;
        _motorizedWheels[index * 2 + 0] = left;
        _motorizedWheels[index * 2 + 1] = right;
        _wheelMeshes[index * 4 + 0] = lfMesh;
        _wheelMeshes[index * 4 + 1] = rfMesh;
        _wheelMeshes[index * 4 + 2] = lrMesh;
        _wheelMeshes[index * 4 + 3] = rrMesh;
        return index;
    }

    // public int RegisterCar(
    //     CarConfigData carConfigData,
    //     Rigidbody rigidbody,
    //     WheelCollider leftFront, WheelCollider rightFront,
    //     WheelCollider rl, WheelCollider rr,
    //     WheelCollider fl, WheelCollider fr,
    //     Transform lfMesh, Transform rfMesh, Transform lrMesh, Transform rrMesh
    // )
    // {

    //     if(_carAmount >= _capacity)
    //     {
    //         Debug.Log("Overloading");
    //         return -1;
    //     }

    //     int index = _carAmount ++;

    //     _engineBatchConfigs[index] = CarConfigConverter.ToBatchConfig(carConfigData.EngineConfig);
    //     _transmissionBatchConfigs[index] = CarConfigConverter.ToBatchConfig(carConfigData.TransmissionConfig);
    //     _brakeConfigs[index] = carConfigData.BreakConfig;
    //     _steeringConfigs[index] = carConfigData.SteeringConfig;

    //     var data = new CarPhysicsData();
    //     data.Vehicle.Mass = rigidbody.mass;
    //     data.Transmission.CurrentGear = 0;
    //     _carPhysicsData[index] = data;

    //     _rigidbodies[index] = rigidbody;
    //     _steeringWheels[index * 2 + 0] = leftFront;
    //     _steeringWheels[index * 2 + 1] = rightFront;
    //     _motorizedWheels[index * 4 + 0] = rl;
    //     _motorizedWheels[index * 4 + 1] = rr;
    //     _motorizedWheels[index * 4 + 2] = fl;
    //     _motorizedWheels[index * 4 + 3] = fr;
    //     _wheelMeshes[index * 4 + 0] = lfMesh;
    //     _wheelMeshes[index * 4 + 1] = rfMesh;
    //     _wheelMeshes[index * 4 + 2] = lrMesh;
    //     _wheelMeshes[index * 4 + 3] = rrMesh;
    //     return index;
    // }


    private void RenderWheels(int i)
    {
        SetRenderWheel(_steeringWheels[i * 2 + 0], _wheelMeshes[i * 4 + 0]);
        SetRenderWheel(_steeringWheels[i * 2 + 1], _wheelMeshes[i * 4 + 1]);
        SetRenderWheel(_motorizedWheels[i * 2 + 0], _wheelMeshes[i * 4 + 2]);
        SetRenderWheel(_motorizedWheels[i * 2 + 1], _wheelMeshes[i * 4 + 3]);
    }
    
    private void SetRenderWheel(WheelCollider collider, Transform mesh)
    {
        if (collider == null || mesh == null) return;
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        mesh.position = position;
        mesh.rotation = rotation;
    }
    
    private void ApplySteering(int i, ref CarPhysicsData data)
    {
        for (int w = 0; w < 2; w++)
        {
            var wheel = _steeringWheels[i * 2 + w];
            if (wheel != null) wheel.steerAngle = data.Steering.CurrentSteeringAngle;
        }
    }

    private void ApplyBrakes(int i, ref CarPhysicsData carPhysicsData)
    {
        float frontTorque = carPhysicsData.Brake.FrontBrakeTorque;
        float rearTorque  = carPhysicsData.Brake.RearBrakeTorque;

        for(int w = 0; w < 2; w ++)
        {
            var sw = _steeringWheels[ i * 2 + w];
            if(sw != null) sw.brakeTorque = frontTorque;

            var mw = _motorizedWheels[ i * 2 + w];
            if(mw != null) mw.brakeTorque = rearTorque;
        }
    }

    private void ReadWheelData(int i, ref CarPhysicsData carPhysicsData)
    {
        var lr = _motorizedWheels[i * 2 + 0];
        var rr = _motorizedWheels[i * 2 + 1];

        float totalRPM = 0f;
        int validateWheels = 0;

        if (lr != null) { totalRPM += lr.rpm; validateWheels++; }
        if (rr != null) { totalRPM += rr.rpm; validateWheels++; }

        carPhysicsData.WheelData.GeneralWheelsRPM = validateWheels > 0 ? totalRPM / validateWheels : 0f;
    }

    private void UpdateVehicleSpeed(int i, ref CarPhysicsData data)
    {
        Vector3 localVelocity = _rigidbodies[i].transform.InverseTransformDirection(_rigidbodies[i].linearVelocity);
        data.Vehicle.SpeedMS = localVelocity.z;
        data.Vehicle.SpeedKmH = data.Vehicle.SpeedMS * 3.6f;
    }

    private void ApplyTorqueToWheels(int i, ref CarPhysicsData data)
    {
        float totalTorque = data.Transmission.TransmissionTorque;
        bool inForwardGear = data.Transmission.CurrentGear > 0;

        if (inForwardGear && data.Vehicle.SpeedMS < -0.5f && _sharedPlayerInput.ThrottleInput < 0.01f)
            totalTorque = 0f;

        for (int w = 0; w < 2; w++)
        {
            var wheel = _motorizedWheels[i * 2 + w];
            if (wheel == null) continue;

            float appliedTorque = totalTorque / 2f;
            wheel.GetGroundHit(out WheelHit hit);
            float forwardSlip = Mathf.Abs(hit.forwardSlip);

            if (forwardSlip < 0.1f && data.Transmission.CurrentGear > 1)
                appliedTorque += 3000f; 

            wheel.motorTorque = appliedTorque;
        }
    }


    private void UpdateGear(ref CarPhysicsData data, ref TransmissionBatchConfig transCfg)
    {
        switch (_sharedPlayerInput.ShiftCommand)
        {
            case GearShiftCommand.Up:
                _transmissionSimulation.ShiftUp(ref data.Transmission, ref transCfg);
                break;

            case GearShiftCommand.Down:
                if (data.Vehicle.SpeedKmH < transCfg.ReverseMaxSpeed)
                {
                    if (data.Transmission.CurrentGear == 0)
                        data.Transmission.CurrentGear = -1;
                    else
                        _transmissionSimulation.ShiftDown(ref data.Transmission, ref transCfg);
                }
                else
                {
                    _transmissionSimulation.ShiftDown(ref data.Transmission, ref transCfg);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        _sharedPlayerInput = PlayerManager.Instance.Input.PlayerInput;

        for(int i = 0; i < _carAmount; i++)
        {
            ref var data = ref _carPhysicsData.ExtractElementRef(i);
            ref var transCfg = ref _transmissionBatchConfigs.ExtractElementRef(i);

            UpdateGear(ref data, ref transCfg);

            data.DeltaTime = Time.fixedDeltaTime;
            data.Vehicle.Velocity = _rigidbodies[i].linearVelocity;

            ReadWheelData(i, ref data);
            UpdateVehicleSpeed(i, ref data);
        }

            var job = new CarPhysicsJob
            {
                PhysicsData = _carPhysicsData,
                EngineConfigs = _engineBatchConfigs,
                TransmissionConfigs = _transmissionBatchConfigs,
                BrakeConfigs = _brakeConfigs,
                SteeringConfigs = _steeringConfigs,
                SharedInput = _sharedPlayerInput,
                DeltaTime = Time.fixedDeltaTime
            };

            JobHandle handle = job.Schedule(_carAmount, 32); // try another params for innerloopBatchCount param
            handle.Complete();

        for(int i = 0; i < _carAmount; i++)
        {
            ref var data = ref _carPhysicsData.ExtractElementRef(i);
            ApplyBrakes(i, ref data);
            ApplyTorqueToWheels(i, ref data);
            ApplySteering(i, ref data);
            RenderWheels(i);
        }
    }

    private void Start()
    {
        for(int i = 0; i < _carAmount; i++)
        {
            _atmoEngineSimulation.Init(ref _carPhysicsData.ExtractElementRef(i).Engine, ref _engineBatchConfigs.ExtractElementRef(i));

            _carPhysicsData.ExtractElementRef(i).Transmission.ShiftTimer = 0;
        }
    }

    private void OnDestroy()
    {
        if(_engineBatchConfigs.IsCreated) _engineBatchConfigs.Dispose();
        if(_transmissionBatchConfigs.IsCreated) _transmissionBatchConfigs.Dispose();
        if(_steeringConfigs.IsCreated) _steeringConfigs.Dispose();
        if(_brakeConfigs.IsCreated) _brakeConfigs.Dispose();
        if(_carPhysicsData.IsCreated) _carPhysicsData.Dispose();
    }
}
