
using Zenject;

public class MainMenuState : IGameState
{
    private StateMachine<IGameState> _fsm;

    [Inject]
    public void Construct(StateMachine<IGameState> gameStateMachine)
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