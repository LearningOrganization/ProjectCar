using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BackToMenuScript : MonoBehaviour
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _garageButton;

    [Inject] private GameStateMachine _fsm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _menuButton.onClick.AddListener(() =>
        {
            _fsm.Enter<LoadingState>().LoadAndEnterNonPlayingScene<MainMenuState>(NonPlayingScene.MainMenuScene, () => Debug.Log("BackToMainMenu"));
        });
        _garageButton.onClick.AddListener(() =>
        {
            _fsm.Enter<LoadingState>().LoadAndEnterNonPlayingScene<GarageState>(NonPlayingScene.GarageScene, () => Debug.Log("Welcome to your garage!"));
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
