
using System;

public interface ISceneLoader
{
    void AsyncSceneLoad(Scene sceneID, Action onLoaded = null);
}