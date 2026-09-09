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

        if(_carContainer.Car != null)
        {
            var car = _container.InstantiatePrefab(
                _carContainer.Car,
                _startPoint.transform.position,
                _startPoint.transform.rotation,
                null
            );

            _raceController.SetPlayerCar(car.GetComponent<CarPhysicSystem>());

            Debug.Log("Player car spawned");
            
            var camera = _container.InstantiatePrefab(_playerCamera,
            _startPoint.transform.position,
            _startPoint.transform.rotation,
            null);

            if(camera != null)
            {
                camera.GetComponent<CameraRotation>().TargetObject = car.transform;
            }
        }
        else
            Debug.Log("car was not spawned"); 
        
    }
}