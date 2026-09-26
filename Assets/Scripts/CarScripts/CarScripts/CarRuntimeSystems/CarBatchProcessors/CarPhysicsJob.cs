using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct CarPhysicsJob : IJobParallelFor
{
    public NativeArray<CarPhysicsData> PhysicsData;

    [ReadOnly] public NativeArray<EngineBatchConfig> EngineConfigs;
    [ReadOnly] public NativeArray<TransmissionBatchConfig> TransmissionConfigs;
    [ReadOnly] public NativeArray<BrakeConfig> BrakeConfigs;
    [ReadOnly] public NativeArray<SteeringConfig> SteeringConfigs;

    [ReadOnly] public PlayerInput SharedInput;
    public float DeltaTime;

    public void Execute(int i)
    {
        var data = PhysicsData[i]; 

        var steerCfg = SteeringConfigs[i];
        var transCfg = TransmissionConfigs[i];
        var engineCfg = EngineConfigs[i];
        var brakeCfg = BrakeConfigs[i];

        var input = SharedInput; 
        float dt = DeltaTime;

        // maybe it should be replaced
        var steering = new SteeringSimulation();
        var transmission = new TransmissionSimulation();
        var engine = new AtmoEngineSimulation();
        var brake = new BrakeSimulation();

        steering.UpdatePhysics(ref data.Vehicle, ref data.Steering, ref steerCfg, ref input, ref dt);
        transmission.UpdatePhysics(ref data.Transmission, ref data.Engine, ref transCfg, ref dt);
        engine.UpdatePhysics(ref data.Engine, ref data.Transmission, ref data.WheelData, ref engineCfg, ref input, ref dt);
        brake.UpdatePhysics(ref data.Brake, ref brakeCfg, ref input, ref dt);

        PhysicsData[i] = data;
    }
}
