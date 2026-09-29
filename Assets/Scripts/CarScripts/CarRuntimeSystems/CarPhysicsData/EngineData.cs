using UnityEngine;

[System.Serializable]
public struct EngineData
{
    public float EngineRPM;
    public float EngineTorque;
    public float EngineBraking;
    public float EngineInertia;

    public float CurrentRPM;
    public float RevLimiterTimer;

}
