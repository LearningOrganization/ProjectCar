using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;

public struct TorqueCurvePoint
{
    public float RPM;
    public float Torque;

    public TorqueCurvePoint(float rpm, float torque)
    {
        RPM = rpm;
        Torque = torque;
    }
}

[Serializable]
public struct TorqueCurve
{
    public FixedList128Bytes<TorqueCurvePoint> Points;
    public int Length => Points.Length;

    public float Evaluate (float currentRPM)
    {
        int count = Points.Length;

        if(count == 0)
            return 0f;
        
        if(count == 1)
            return Points[0].Torque;

        if(currentRPM < Points[0].RPM)
            return 0;

        //Above curve
        if (currentRPM >= Points[count - 1].RPM)
            return Points[count - 1].Torque;
        
        int index = FindSegment(currentRPM);

        TorqueCurvePoint a = Points[index];
        TorqueCurvePoint b = Points[index + 1];

        float range = b.RPM - a.RPM;

        if(range <= 0f)
            return a.Torque;

        float t = (currentRPM - a.RPM) / range;

        return math.lerp(a.Torque, b.Torque, t);
    }

    private int FindSegment(float rpm)
    {
        int count = Points.Length;
        int result = -1;

        for (int i = 0; i < count - 1; i++)
        {
            bool match = (rpm < Points[i + 1].RPM) & (result == -1);
            result = math.select(result, i, match);
        }

        return result == -1 ? count - 2 : result;
    }
}
