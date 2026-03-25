namespace Aco228.Common.Helpers;

public static class PercentageHelper
{
    public static int AddPercentage(this int input, double percentage)
    {
        var change = (int) Math.Ceiling(input * (percentage / 100.0));
        return input + change;
    }
}