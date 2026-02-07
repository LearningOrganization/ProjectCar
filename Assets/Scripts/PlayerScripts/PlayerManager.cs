using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public GameObject Player;
    public PlayerInputControls Input { get; private set; }

    void Awake()
    {
        // should be replaced by DI
        Instance = this;
        Input = GetComponent<PlayerInputControls>();
    }
}

