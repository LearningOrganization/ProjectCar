
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
        
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}