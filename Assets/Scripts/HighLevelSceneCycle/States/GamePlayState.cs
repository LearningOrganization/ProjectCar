

public class GamePlayState : IGameState
{
    public void Enter()
    {
        UnityEngine.Debug.Log("GamePlayState runs");
    }

    public void Exit()
    {
        UnityEngine.Debug.Log("GamePlayState exit");
    }
}