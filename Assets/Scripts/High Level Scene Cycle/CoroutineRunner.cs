

using Unity.VisualScripting;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance { get; private set;}

    public void Awake()
    {
        if(Instance != null)
        {
                Destroy(gameObject);
                return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}