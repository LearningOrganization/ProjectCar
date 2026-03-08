using System;
using UnityEngine;
using Zenject;

public class RaceBootstrapper : IInitializable
{
    private readonly GameObject _carPrefab;
    private readonly GameObject _startPoint;
    private readonly GameObject _playerCamera;
    private readonly StateMachine<IRaceState> _raceStateMachine;
    private readonly DiContainer _container;
    private RaceController _raceController;
    private CarUI _carUI;
    
    public RaceBootstrapper(
        [Inject(Id = "CarPrefab")] GameObject carPrefab,
        [Inject(Id = "CarStartPoint")] GameObject startPoint,
        [Inject(Id = "PlayerCamera")] GameObject playerCamera,
        StateMachine<IRaceState> RaceStateMachine,
        DiContainer container,
        RaceController raceController,
        CarUI carUI)
    {
        _carPrefab = carPrefab;
        _startPoint = startPoint;
        _playerCamera = playerCamera;
        _raceStateMachine = RaceStateMachine;
        _container = container;
        _raceController = raceController;
        _carUI = carUI;
    }
    public void Initialize()
    {
        Debug.Log("RaceBootstrapper Initialize");

        SpawnPlayerCar();

        _raceStateMachine.Enter<RacePrepareState>();
    }

    private void SpawnPlayerCar()
    {
        var car = _container.InstantiatePrefab(
            _carPrefab,
            _startPoint.transform.position,
            _startPoint.transform.rotation,
            null);
        
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
}