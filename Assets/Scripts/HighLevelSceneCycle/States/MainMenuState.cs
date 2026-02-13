

using Zenject;

public class MainMenuState : IGameState
{
    private GameStateMachine _fsm;

    [Inject]
    public void Construct(GameStateMachine gameStateMachine)
    {
        _fsm = gameStateMachine;
    }

    public void Enter()
    {
        //_fsm.Enter<LoadingState>();
        UnityEngine.Debug.Log("MainMenuState runs");
    }

    public void Exit()
    {
         UnityEngine.Debug.Log("MainMenuState exit");
    }
}