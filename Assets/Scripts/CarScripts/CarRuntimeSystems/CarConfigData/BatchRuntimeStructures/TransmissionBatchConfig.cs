using Unity.Collections;

public struct TransmissionBatchConfig
{
    public FixedList128Bytes<float> ForwardGearRatios;
    public int ForwardGearsCount;
    public FixedList128Bytes<float> ReverseRatio;
    public float FinalDriveRatio;
    public float TransmissionEfficiency;
        
    public float ShiftTime;
    public float ReverseMaxSpeed; //km/h
}