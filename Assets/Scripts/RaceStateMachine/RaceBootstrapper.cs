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
    
    public RaceBootstrapper(
        [Inject(Id = "CarPrefab")] GameObject carPrefab,
        [Inject(Id = "CarStartPoint")] GameObject startPoint,
        [Inject(Id = "PlayerCamera")] GameObject playerCamera,
        StateMachine<IRaceState> RaceStateMachine,
        DiContainer container)
    {
        _carPrefab = carPrefab;
        _startPoint = startPoint;
        _playerCamera = playerCamera;
        _raceStateMachine = RaceStateMachine;
        _container = container;
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

        Debug.Log("Player car spawned");

        AttachCamera(car);
    }

    private void AttachCamera(GameObject car)
    {
        
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