[System.Serializable]
public struct CarTelemetry
{
    public float Speed;
    public float RPM;
    public float MinRPM;
    public float MaxRPM;

    public int Gear;

    public bool ABSActive;

    public float Throttle;
    public float Brake;
}