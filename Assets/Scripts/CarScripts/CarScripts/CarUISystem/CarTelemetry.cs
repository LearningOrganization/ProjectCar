[System.Serializable]
public struct CarTelemetry
{
    public float Speed;
    public float RPM;
    public float MinRPM;
    public float MaxRPM;

    public Gear Gear;

    public bool ABSActive;

    public float Throttle;
    public float Brake;
}

public enum Gear
{
    Reverse = -1,
    Neutral = 0,
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Fifth = 5,
    Sixth = 6
}