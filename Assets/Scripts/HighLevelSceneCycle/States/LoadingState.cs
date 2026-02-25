
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Zenject;

public class LoadingState : IGameState
{
    private ISceneLoader _sceneLoader;
    private StateMachine<IGameState> _fsm;
    
    [Inject]
    public void Construct(ISceneLoader sceneLoader, StateMachine<IGameState> gameStateMachine)
    {
        _sceneLoader = sceneLoader;
        _fsm = gameStateMachine;
    }

    public async Task LoadScene<T>(SceneId playingScene, Action action = null) where T : IGameState
    {
        UnityEngine.Debug.Log("Loading playing scene");
        await _sceneLoader.LoadScene<T>(playingScene, action);
    }

    public void Enter()
    {
        UnityEngine.Debug.Log("LoadingState is running");
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