using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    Task LoadNonPlayingScene<T>(NonPlayingScene sceneID, Action onLoaded = null) where T : IGameState;
    Task LoadPlayingScene<T>(PlayingScene sceneID, Action onLoaded = null) where T : IGameState;
}