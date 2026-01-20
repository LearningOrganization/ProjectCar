
using Zenject;

public class LoadingState : IGameState
{
    private ISceneLoader _sceneLoader;
    private GameStateMachine _fsm;

    [Inject]
    public void Construct(ISceneLoader sceneLoader, GameStateMachine gameStateMachine)
    {
        _sceneLoader = sceneLoader;
        _fsm = gameStateMachine;
    }


    public void Enter()
    {
        // should be replaced and adapted for all game and non-game scenes
        _sceneLoader.AsyncSceneLoad(Scene.UtahTrackScene, ()=>
        {
            _fsm.Enter<GamePlayState>();
        });
    }

    public void Exit()
    {
        
    }
}