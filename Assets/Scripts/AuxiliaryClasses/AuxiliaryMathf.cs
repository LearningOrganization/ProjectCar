using Unity.Mathematics;

public struct AuxiliaryMathf
{
    public static float MoveTowards(float current, float target, float maxDelta)
    {
        float delta = target - current;
        float absDelta = math.abs(delta);
        float step = math.sign(delta) * maxDelta;
        return math.select(current + step, target, absDelta <= maxDelta);
    }
}