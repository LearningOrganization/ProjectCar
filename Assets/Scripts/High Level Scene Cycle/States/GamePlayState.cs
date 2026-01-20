

public class GamePlayState : IGameState
{
    public void Enter()
    {
        UnityEngine.Debug.Log("GamePlayState runs");
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}