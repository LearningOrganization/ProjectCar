using System;
using Unity.Splines.Examples;
using UnityEngine;
using Zenject;

public class RaceBootstrapper : IInitializable
{
    private readonly GameObject _startPoint;
    private readonly GameObject _playerCamera;
    private readonly CarContainer _carContainer;
    private readonly StateMachine<IRaceState> _raceStateMachine;
    private readonly DiContainer _container;
    private RaceController _raceController;
    private CarPhysicBatchProcessor _carPhysicBatchProcessor;
    //private Race
    
    private const float Spacing = 6f;           
    private const float WallHeight = 5f;
    private const float WallThickness = 1f;
    private const float WallPadding = 4f;        
    private const float CameraMargin = 1.3f; 

    public RaceBootstrapper(
        [Inject(Id = "CarContainer")] CarContainer carContainer,
        [Inject(Id = "CarStartPoint")] GameObject startPoint,
        [Inject(Id = "PlayerCamera")] GameObject playerCamera,
        [Inject(Id = "CarPhysicBatchProcessor")] CarPhysicBatchProcessor carPhysicBatchProcessor,
        StateMachine<IRaceState> RaceStateMachine,
        DiContainer container,
        RaceController raceController,
        CarUI carUI)
    {
        _carContainer = carContainer;
        _startPoint = startPoint;
        _playerCamera = playerCamera;
        _raceStateMachine = RaceStateMachine;
        _container = container;
        _raceController = raceController;
        _carPhysicBatchProcessor = carPhysicBatchProcessor;
    }
    public void Initialize()
    {
        SpawnPlayerCar();

        _raceStateMachine.Enter<RacePrepareState>();

    }

    private void SpawnPlayerCar()
    {
        if(!_carContainer.IsBatchProcessing)
        {
            CreateOOPCars();
        }
        else
        {
            CreateCarPhysicBatchProcessor();
            //Debug.Log("Batch processing was turned on"); 
        }
        
    }

    // private void CreateCarPhysicBatchProcessor()
    // {
    //     var processor = _container.InstantiatePrefab(
    //         _carPhysicBatchProcessor,
    //         new Vector3(0, 0, 0), 
    //         Quaternion.identity, 
    //         null
    //     ).GetComponent<CarPhysicBatchProcessor>();

    //     processor.InitPhysicBatchProcessor(_carContainer.CarsAmounts);

    //     if(_carContainer.Car != null)
    //     {
    //         for(int i = 0; i < _carContainer.CarsAmounts; i++)
    //         {
    //             var car = _container.InstantiatePrefab(
    //                 _carContainer.Car,
    //                 new Vector3(_startPoint.transform.position.x, _startPoint.transform.position.y, _startPoint.transform.position.z + 2.5f*i),
    //                 _startPoint.transform.rotation,
    //                 null
    //             );

    //             var carLogic = car.GetComponent<CarPhysicSystem>();

    //             Debug.Log($"Car  spawned: {i}");

    //             processor.RegisterCar(
    //                 carLogic.CarConfigData,
    //                 car.GetComponent<Rigidbody>(),
    //                 carLogic.SteeringWheels[0], carLogic.SteeringWheels[1],
    //                 carLogic.MotorizedWheels[0], carLogic.MotorizedWheels[1],
    //                 carLogic.LeftFrontWheelMesh, carLogic.RightFrontWheelMesh,
    //                 carLogic.LeftRearWheelMesh, carLogic.RightRearWheelMesh
    //             );

    //             carLogic.enabled = false;

    //             Debug.Log($"Car  spawned: {i}");

    //             if( i == 0)
    //             {
    //                 //carLogic.enabled = true;
    //                 //_raceController.SetPlayerCar(carLogic);


    //                 var camera = _container.InstantiatePrefab(_playerCamera,
    //                 _startPoint.transform.position,
    //                 _startPoint.transform.rotation,
    //                 null);

    //                 if(camera != null)
    //                 {
    //                     camera.GetComponent<CameraRotation>().TargetObject = car.transform;
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Car container is empty");
    //     }     
    // }

    // private void CreateOOPCars()
    // {
    //     if(_carContainer.Car != null)
    //     {
    //         for(int i = 0; i < _carContainer.CarsAmounts; i++)
    //         {   
    //             var car = _container.InstantiatePrefab(
    //                 _carContainer.Car,
    //                 new Vector3(_startPoint.transform.position.x, _startPoint.transform.position.y, _startPoint.transform.position.z + 2.5f*i),
    //                 _startPoint.transform.rotation,
    //                 null
    //             );

    //             Debug.Log($"Car  spawned: {i}");
                
    //             if ( i == 0)
    //             {
    //                 _raceController.SetPlayerCar(car.GetComponent<CarPhysicSystem>());

    //                 var camera = _container.InstantiatePrefab(_playerCamera,
    //                 _startPoint.transform.position,
    //                 _startPoint.transform.rotation,
    //                 null);

    //                 if(camera != null)
    //                 {
    //                     camera.GetComponent<CameraRotation>().TargetObject = car.transform;
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("car was not spawned");
    //     } 
    // }

    private int GetColumns(int amount) => Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(amount)));

    private Vector3 GetGridPosition(int index, int columns)
    {
        int row = index / columns;
        int col = index % columns;

        return new Vector3(
            _startPoint.transform.position.x + col * Spacing,
            _startPoint.transform.position.y,
            _startPoint.transform.position.z + row * Spacing
        );
    }

private void SetupBenchmarkCamera(int amount, int columns)
{
    int rows = Mathf.CeilToInt(amount / (float)columns);

    float gridWidth = (columns - 1) * Spacing;
    float gridDepth = (rows - 1) * Spacing;

    Vector3 center = _startPoint.transform.position + new Vector3(gridWidth / 2f, 0f, gridDepth / 2f);

    var cameraGO = _container.InstantiatePrefab(
        _playerCamera,
        center + Vector3.up * 80f, 
        Quaternion.Euler(90f, 0f, 0f),
        null
    );

    var cam = cameraGO.GetComponentInChildren<Camera>();
    if (cam == null)
    {
        Debug.LogWarning("Benchmark camera: no Camera component found on prefab.");
        return;
    }

    cam.orthographic = true;

    float aspect = cam.aspect > 0f ? cam.aspect : 16f / 9f;

    float visibleDepth = gridDepth + WallPadding * 2f;
    float visibleWidth = gridWidth + WallPadding * 2f;

    float sizeByDepth = visibleDepth / 2f;
    float sizeByWidth = (visibleWidth / 2f) / aspect;

    cam.orthographicSize = Mathf.Max(sizeByDepth, sizeByWidth, 1f) * CameraMargin;

    var followScript = cameraGO.GetComponent<CameraRotation>();
    if (followScript != null)
        followScript.enabled = false;
}

    private void SpawnWalls(int amount, int columns)
    {
        int rows = Mathf.CeilToInt(amount / (float)columns);

        float gridWidth = (columns - 1) * Spacing;
        float gridDepth = (rows - 1) * Spacing;

        Vector3 origin = _startPoint.transform.position;
        Vector3 center = origin + new Vector3(gridWidth / 2f, 0f, gridDepth / 2f);

        CreateWall(new Vector3(center.x, WallHeight / 2f, origin.z - WallPadding - WallThickness / 2f),
                new Vector3(gridWidth + WallPadding * 2f, WallHeight, WallThickness));

        CreateWall(new Vector3(center.x, WallHeight / 2f, origin.z + gridDepth + WallPadding + WallThickness / 2f),
                new Vector3(gridWidth + WallPadding * 2f, WallHeight, WallThickness));

        CreateWall(new Vector3(origin.x - WallPadding - WallThickness / 2f, WallHeight / 2f, center.z),
                new Vector3(WallThickness, WallHeight, gridDepth + WallPadding * 2f));

        CreateWall(new Vector3(origin.x + gridWidth + WallPadding + WallThickness / 2f, WallHeight / 2f, center.z),
                new Vector3(WallThickness, WallHeight, gridDepth + WallPadding * 2f));
    }

    private void CreateWall(Vector3 position, Vector3 scale)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "BenchmarkWall";
        wall.transform.position = position;
        wall.transform.localScale = scale;

        var renderer = wall.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = false;

        var col = wall.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    private void CreateOOPCars()
    {
        if (_carContainer.Car == null)
        {
            Debug.Log("car was not spawned");
            return;
        }

        int amount = _carContainer.CarsAmounts;
        int columns = GetColumns(amount);

        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = GetGridPosition(i, columns);

            var car = _container.InstantiatePrefab(
                _carContainer.Car,
                pos,
                _startPoint.transform.rotation,
                null
            );

            if (i == 0)
            {
                //_raceController.SetPlayerCar(car.GetComponent<CarPhysicSystem>());
            }
        }

        SpawnWalls(amount, columns);
        SetupBenchmarkCamera(amount, columns);
    }

    private void CreateCarPhysicBatchProcessor()
    {
        var processor = _container.InstantiatePrefab(
            _carPhysicBatchProcessor,
            Vector3.zero,
            Quaternion.identity,
            null
        ).GetComponent<CarPhysicBatchProcessor>();

        processor.InitPhysicBatchProcessor(_carContainer.CarsAmounts);

        if (_carContainer.Car == null)
        {
            Debug.Log("Car container is empty");
            return;
        }

        int amount = _carContainer.CarsAmounts;
        int columns = GetColumns(amount);

        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = GetGridPosition(i, columns);

            var car = _container.InstantiatePrefab(
                _carContainer.Car,
                pos,
                _startPoint.transform.rotation,
                null
            );

            var carLogic = car.GetComponent<CarPhysicSystem>();

            processor.RegisterCar(
                carLogic.CarConfigData,
                car.GetComponent<Rigidbody>(),
                carLogic.SteeringWheels[0], carLogic.SteeringWheels[1],
                carLogic.MotorizedWheels[0], carLogic.MotorizedWheels[1],
                carLogic.LeftFrontWheelMesh, carLogic.RightFrontWheelMesh,
                carLogic.LeftRearWheelMesh, carLogic.RightRearWheelMesh
            );

            UnityEngine.Object.Destroy(carLogic);
        }

        SpawnWalls(amount, columns);
        SetupBenchmarkCamera(amount, columns);
    }
}