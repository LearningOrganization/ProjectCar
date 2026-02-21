using UnityEngine;
using Zenject;

public class RaceCountdownState : IRaceState
{

    private readonly RaceController _controller;

    [Inject]
    public RaceCountdownState(RaceController controller)
    {
        _controller = controller;
    }
    public void Enter()
    {
        Debug.Log("Countdown state running");
        _controller.StartRaceCountdown();
    }

    public void Exit()
    {
        Debug.Log("Countdown state was finished");
    }
}