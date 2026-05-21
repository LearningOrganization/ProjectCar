

using System;
using UnityEngine;
using Zenject;

public class RaceInstaller : MonoInstaller
{
    [SerializeField] private GameObject _carPrefab;
    [SerializeField] private GameObject _playerCamera; 
    [SerializeField] private GameObject _startPoint;

    public override void InstallBindings()
    {
        // --- Race States ---
        Container.Bind<RacePrepareState>().AsSingle();
        Container.Bind<RaceCountdownState>().AsSingle();
        Container.Bind<RaceInProgressState>().AsSingle();
        Container.Bind<RacePauseState>().AsSingle();
        Container.Bind<RaceDNFState>().AsSingle();
        Container.Bind<RaceFinishState>().AsSingle();
        Container.Bind<RaceResultState>().AsSingle();

        // --- Race FSM ---
        Container.Bind<StateMachine<IRaceState>>().AsSingle();

        // --- Scene Objects ---
        //Container.BindInstance(_carPrefab).WithId("CarPrefab"); // should be replaced bys more dynamic solution
        Container.BindInstance(_playerCamera).WithId("PlayerCamera");
        Container.BindInstance(_startPoint).WithId("CarStartPoint"); // also should be replaced to FromComponentInHierarchy as a list of start points

        // --- Race Bootastrapper ---
        Container.BindInterfacesAndSelfTo<RaceBootstrapper>().AsSingle();

        // --- Race Controller ---
        Container.Bind<RaceController>().FromComponentInHierarchy().AsSingle();

        // --- Race HUD ---
        Container.Bind<RaceUIController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CarUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CarPhysicSystem>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<FinishScript>().FromComponentInHierarchy().AsSingle();
    }


}