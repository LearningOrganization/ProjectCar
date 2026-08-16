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

    public float Evaluate (float currentRPM)
    {
        int count = Points.Length;

        if(count == 0)
            return 0f;
        
        if(count == 1)
            return Points[0].Torque;

        if(currentRPM <= Points[0].RPM)
            return 0;
        
        for (int i = 0; i < count -1; i++)
        {
            TorqueCurvePoint a = Points[i];
            TorqueCurvePoint b = Points[i+1];

            if(currentRPM <= b.RPM)
            {
                return Interpolate(
                    currentRPM,
                    a.RPM,
                    a.Torque,
                    b.RPM,
                    b.Torque
                );
            }
        }

        return Points[count - 1].Torque;
    }

    private static float Interpolate(
        float x,
        float x1,
        float y1,
        float x2,
        float y2)
    {
        float t = (x - x1) / (x2 - x1);

        return y1 + (y2 - y1) * t;
    }
}
