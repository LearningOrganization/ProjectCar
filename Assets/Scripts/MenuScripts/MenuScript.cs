using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuScript : MonoBehaviour
{
    [Inject] private StateMachine<IGameState> _fsm;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            _ = _fsm.Enter<LoadingState>().LoadScene<GamePlayState>(PlayingScene.UtahTrackScene,
            () => Debug.Log("UtahTrackScene was loaded"));
        });
    }
}