using UnityEngine;
using Zenject;

public class RacePrepareState : IRaceState
{
    private readonly RaceController _controller;
    public RacePrepareState(RaceController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("Prepare state running");
        _controller.StartRacePreparing();
    }

    public void Exit()
    {
        Debug.Log("Prepare state was finished");
    }
}