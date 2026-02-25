using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    Task LoadScene<T>(SceneId sceneId, Action onLoaded = null) where T : IGameState;
}