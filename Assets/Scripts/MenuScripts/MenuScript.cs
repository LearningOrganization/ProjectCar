using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuScript : MonoBehaviour
{
    [Inject] private StateMachine<IGameState> _fsm;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            _ = _fsm.Enter<LoadingState>().LoadAndEnterPlayingScene<GamePlayState>(PlayingScene.UtahTrackScene,
            () => Debug.Log("UtahTrackScene was loaded"));
        });
    }
}