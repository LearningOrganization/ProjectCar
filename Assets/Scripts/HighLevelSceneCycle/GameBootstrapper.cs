

using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Zenject;

public class GameBootstrapper : MonoBehaviour
{
    
    private StateMachine<IGameState> _fsm;

    [Inject]
    public void Construct(StateMachine<IGameState> gameStateMachine)
    {
        _fsm = gameStateMachine;
    }

    void Start()
    {
        _fsm.Enter<MainMenuState>();
    }

}