using Unity.Collections;
using UnityEngine;

public static class CarConfigConverter
{
    public static TorqueCurve FromAnimationCurve(AnimationCurve curve)
    {
        TorqueCurve result = default;
        result.Points = new FixedList512Bytes<TorqueCurvePoint>();

        if (curve == null || curve.length == 0)
            return result;

        int maxCapacity = result.Points.Capacity;
        int keyCount = Mathf.Min(curve.length, maxCapacity);

        if (curve.length > maxCapacity)
        {
            Debug.LogWarning($"AnimationCurve contain {curve.length} keys, " +
                              $"TorqueCurve contain {maxCapacity}. " +
                              $"other keys will be discarded");
        }

        for (int i = 0; i < keyCount; i++)
        {
            Keyframe key = curve[i];
            result.Points.Add(new TorqueCurvePoint(key.time, key.value));
        }

        return result;
    }

    public static FixedList128Bytes<float> ConvertFloatArray(float[] source, string fieldName)
    {
        var list = new FixedList128Bytes<float>();

        if (source == null || source.Length == 0)
            return list;

        int maxCapacity = list.Capacity;
        int count = Mathf.Min(source.Length, maxCapacity);

        if (source.Length > maxCapacity)
        {
            Debug.LogWarning($"[TransmissionConfig] {fieldName} contain {source.Length} elements, " +
                              $"but {maxCapacity}. External elements were deleted");
        }

        for (int i = 0; i < count; i++)
        {
            list.Add(source[i]);
        }

        return list;
    }
}