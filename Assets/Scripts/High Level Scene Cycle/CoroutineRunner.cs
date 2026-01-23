

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[CoroutineRunner]");
                _instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public Coroutine Run(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}