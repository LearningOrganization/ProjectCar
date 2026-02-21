
public class RaceCountdownState : IRaceState
{

    private readonly RaceController _controller;

    public RaceCountdownState(RaceController controller)
    {
        _controller = controller;
    }
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}