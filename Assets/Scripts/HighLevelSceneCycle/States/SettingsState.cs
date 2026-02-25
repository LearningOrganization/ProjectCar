
public class SettingsState : IGameState
{
    public void Enter()
    {
        UnityEngine.Debug.Log("SettingsState runs");
    }

    public void Exit()
    {
        UnityEngine.Debug.Log("SettingsState exit");
    }
}