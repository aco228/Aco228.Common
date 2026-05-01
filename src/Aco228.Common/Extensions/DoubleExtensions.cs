namespace Aco228.Common.Extensions;

public static class DoubleExtensions
{
    public static string ToPreciseString(this double input)
        => $"{input:0.0000}";
    
    public static string ToDoubleString(this int input, string extension = "")
        => input < 10 ? $"0{input}{extension}" :  $"{input}{extension}";

    public static string ToDoubleString(this double? input, string extension = "")
        => input.HasValue == false ? string.Empty : input.Value.ToDoubleString();
    
    public static string ToDoubleString(this double input, string extension = "")
        => double.IsNaN(input) || double.IsInfinity(input) 
            ? "0" + extension 
            : (int)input == input ? ((int)input).ToString() + extension : $"{input:0.00}" + extension;
}