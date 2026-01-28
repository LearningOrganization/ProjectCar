

using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Zenject;

public class GameBootstrapper : MonoBehaviour
{
    
    private GameStateMachine _fsm;

    [Inject]
    public void Construct(GameStateMachine gameStateMachine)
    {
        _fsm = gameStateMachine;
    }

    void Start()
    {
        _fsm.Enter<MainMenuState>();
    }

}