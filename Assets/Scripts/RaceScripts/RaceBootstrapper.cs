using System;
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
    //private Race
    
    public RaceBootstrapper(
        [Inject(Id = "CarContainer")] CarContainer carContainer,
        [Inject(Id = "CarStartPoint")] GameObject startPoint,
        [Inject(Id = "PlayerCamera")] GameObject playerCamera,
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
            if(_carContainer.Car != null)
            {
                for(int i = 0; i < _carContainer.CarsAmounts; i++)
                {   
                    var car = _container.InstantiatePrefab(
                        _carContainer.Car,
                        new Vector3(_startPoint.transform.position.x, _startPoint.transform.position.y, _startPoint.transform.position.z + 2.5f*i),
                        _startPoint.transform.rotation,
                        null
                    );

                    Debug.Log($"Car  spawned: {i}");
                    
                    if ( i == 0)
                    {
                        // check later
                        _raceController.SetPlayerCar(car.GetComponent<CarPhysicSystem>());

                        var camera = _container.InstantiatePrefab(_playerCamera,
                        _startPoint.transform.position,
                        _startPoint.transform.rotation,
                        null);

                        if(camera != null)
                        {
                            camera.GetComponent<CameraRotation>().TargetObject = car.transform;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("car was not spawned");
            } 
        }
        else
        {
            Debug.Log("Batch processing was turned on"); 
        }
        
    }
}