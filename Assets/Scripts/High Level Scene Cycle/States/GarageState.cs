

public class GarageState : IGameState
{
    public void Enter()
    {
        UnityEngine.Debug.Log("GarageState runs");
    }

    public void Exit()
    {
        UnityEngine.Debug.Log("GarageState exit");
    }
}