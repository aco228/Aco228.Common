namespace Aco228.Common.Helpers;

public static class DoubleHelper
{
    public static double GetDifference(double firstValue, double secondValue)
    {
        return firstValue - secondValue;
    }
    
    public static double Round(this double value)
        => Math.Round(value, 2);
    
    public static double SanitizeDouble(this double value) =>
        double.IsFinite(value) ? value.Round() : 0;

    public static double? SanitizeDoubleNull(this double? value) =>
        value.HasValue == false ? null : value.Value.SanitizeDouble();

    public static double Between(this double value, double min, double max)
    {
        if (value < min) value = min;
        if(value > max) value = max;
        return value;
    }
    
    public static double Normalize(params double?[] ratios)
    {
        var enumerator = ratios.Where(x => x.HasValue  && !double.IsNaN(x.Value));
        return enumerator.Sum(x => x.Value) / enumerator.Count() * 1.0;
    }
    
    public static double Random(double minValue, double maxValue)
    {
        double sample = new Random().NextDouble();
        return (maxValue * sample) + (minValue * (1d - sample));
    }

    public static double Multiply(this double input, double multiply)
        => input * multiply;

    public static double TryUpdateStringDouble(string input, double defaultValue)
    {
        var result = double.TryParse(input, out var res) ? res : defaultValue;
        if (result == 0)
            return defaultValue;

        if (result < defaultValue)
            return defaultValue;
        
        return result;
    }

    public static int TryUpdateStringInt(string input, int defaultValue)
    {
        var result = int.TryParse(input, out var res) ? res : defaultValue;
        if (result == 0)
            return defaultValue;

        if (result < defaultValue)
            return defaultValue;
        
        return result;
    }
    
}