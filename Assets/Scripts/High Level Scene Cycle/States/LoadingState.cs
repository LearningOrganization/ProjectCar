
using System;
using System.Diagnostics;
using Zenject;

public class LoadingState : IGameState
{
    private ISceneLoader _sceneLoader;
    private GameStateMachine _fsm;
    private IWalletService _walletService;

    [Inject]
    public void Construct(ISceneLoader sceneLoader, GameStateMachine gameStateMachine, IWalletService walletService)
    {
        _sceneLoader = sceneLoader;
        _fsm = gameStateMachine;
        _walletService = walletService;
    }


    public async void LoadAndEnterPlayingScene<T>(PlayingScene playingScene, Action action = null) where T : IGameState
    {
        UnityEngine.Debug.Log("Loading playing scene");
        await _sceneLoader.LoadPlayingScene<T>(playingScene, action);
    }

    public async void LoadAndEnterNonPlayingScene<T>(NonPlayingScene nonPlayingScene, Action action = null) where T : IGameState
    {
        UnityEngine.Debug.Log("Loading playing scene");
        await _sceneLoader.LoadNonPlayingScene<T>(nonPlayingScene, action);
    }

    public void Enter()
    {
        UnityEngine.Debug.Log("LoadingState is running");
        _walletService.TransactionTry(Currency.Cash, 1000);
        _walletService.TransactionTry(Currency.Tokens, 10);
        // should be replaced and adapted for all game and non-game scenes
        // _sceneLoader.AsyncSceneLoad(Scene.UtahTrackScene, ()=>
        // {
        //     _fsm.Enter<GamePlayState>();
        // });
    }

    public void Exit()
    {
        UnityEngine.Debug.Log("LoadingState exit");
    }
}