namespace Aco228.Common.Helpers;

public static class DoubleHelper
{
    public static double GetDifference(double firstValue, double secondValue)
    {
        return firstValue - secondValue;
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