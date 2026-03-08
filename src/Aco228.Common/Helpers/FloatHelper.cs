namespace Aco228.Common.Helpers;

public static class FloatHelper
{
    public static float GetDifference(float firstValue, float secondValue)
    {
        return firstValue - secondValue;
    }

    public static float Random(float minValue, float maxValue)
    {
        float sample = new Random().NextSingle();
        return (maxValue * sample) + (minValue * (1f - sample));
    }

    public static T RandomChance<T>(params Func<T>[] choices)
    {
        var num = Random(0, choices.Length);
        return choices[(int)num]();
    }

    public static float Multiply(this float input, float multiply)
        => input * multiply;
}