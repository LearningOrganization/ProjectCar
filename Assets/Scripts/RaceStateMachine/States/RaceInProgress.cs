using UnityEngine;
using Zenject;

public class RaceInProgressState : IRaceState
{

    private readonly RaceController _controller;

    [Inject]
    public RaceInProgressState(RaceController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("Race in progress state running");
        _controller.StartRace();
    }

    public void Exit()
    {
        Debug.Log("Race in progress state was finished");
    }
}