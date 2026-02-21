

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
        // race states
        Container.Bind<RacePrepareState>().AsSingle();
        Container.Bind<RaceCountdownState>().AsSingle();
        Container.Bind<RaceInProgressState>().AsSingle();
        Container.Bind<RacePauseState>().AsSingle();
        Container.Bind<RaceDNFState>().AsSingle();
        Container.Bind<RaceFinishState>().AsSingle();
        Container.Bind<RaceResultState>().AsSingle();

        //Race FSM
        Container.Bind<StateMachine<IRaceState>>().AsSingle();

        //scene objects
        Container.BindInstance(_carPrefab).WithId("CarPrefab");
        Container.BindInstance(_playerCamera).WithId("PlayerCamera");
        Container.BindInstance(_startPoint).WithId("CarStartPoint");

        //Race bootastrapper
        Container.BindInterfacesAndSelfTo<RaceBootstrapper>().AsSingle();

        //Race Controller
        Container.Bind<RaceController>().FromComponentInHierarchy().AsSingle();
    }


}