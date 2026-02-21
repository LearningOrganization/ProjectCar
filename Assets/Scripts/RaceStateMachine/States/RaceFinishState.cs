using UnityEngine;

public class RaceFinishState : IRaceState
{
    private readonly RaceController _controller;

  
    public RaceFinishState(RaceController controller)
    {
        _controller = controller;
    }
    public void Enter()
    {
        Debug.Log("Race finish state running");
        _controller.FinishRace();
        // add some another logic
    }

    public void Exit()
    {
        Debug.Log("Race finish state was finished");
    }
}