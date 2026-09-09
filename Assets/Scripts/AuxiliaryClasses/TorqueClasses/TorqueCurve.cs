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
    public FixedList512Bytes<TorqueCurvePoint> Points;
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
        int head = 0;
        int tail = Points.Length - 1;

        while (head <= tail)
        {
            int middle = (head + tail) >> 1; // fast divide for 2

            if(Points[middle].RPM <= rpm)
                head = middle + 1;
            else 
                tail = middle - 1;
        }

        return tail;

    }
}
