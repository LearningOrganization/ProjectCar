using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
{

    private static Scene _loadingScene = Scene.LoadingScene;

    public void AsyncSceneLoad(Scene sceneID, Action onLoaded = null)
    {
        SceneManager.LoadSceneAsync(_loadingScene.ToString())
            .completed += _ =>
            {
                LoadTargetScene(sceneID, onLoaded);
            };
    }

    private void LoadTargetScene(Scene sceneID, Action onLoaded = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID.ToString());
        operation.allowSceneActivation = false;

        CoroutineRunner.Instance.StartCoroutine(WaitForLoad(operation, onLoaded));
    }

    private System.Collections.IEnumerator WaitForLoad(AsyncOperation operation, Action onLoaded)
    {
      
        while (operation.progress < 0.9f)
        {
            // I can send progress in loading scene
            yield return null;
        }

        // small pause (not necessary)
        yield return null;

        operation.allowSceneActivation = true;

        onLoaded?.Invoke();
    }

}