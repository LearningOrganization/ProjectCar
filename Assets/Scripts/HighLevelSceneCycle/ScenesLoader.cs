using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : ISceneLoader
{
    private static NonPlayingScene _loadingScene = NonPlayingScene.LoadingScene;
    [Inject] private StateMachine<IGameState> _gameState;

    public Task LoadNonPlayingScene<T>(NonPlayingScene sceneID, Action onLoaded = null) where T : IGameState
    {
        return AsyncSceneLoad(
            sceneID.ToString(),
            () =>
            {
                _gameState.Enter<T>();
                onLoaded?.Invoke();
            }
        );
    }

    public Task LoadPlayingScene<T>(PlayingScene sceneID, Action onLoaded = null) where T : IGameState
    {
        return AsyncSceneLoad(
            sceneID.ToString(),
            () =>
            {
                _gameState.Enter<T>();
                onLoaded?.Invoke();
            }
        );
    }

    private async Task AsyncSceneLoad(string sceneName, Action afterActivation = null)
    {
        // load loading scene
        await LoadSingleAsync(_loadingScene.ToString());

        // load target scene
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            await Task.Yield();

        // scene activation
        op.allowSceneActivation = true;

        while (!op.isDone)
            await Task.Yield();

        afterActivation?.Invoke();
    }

    private async Task LoadSingleAsync(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone)
            await Task.Yield();
    }
}

    

    

