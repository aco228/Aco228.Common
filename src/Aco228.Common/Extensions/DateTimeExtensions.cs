namespace Aco228.Common.Extensions;

public static class DateTimeExtensions
{
    #region Date Key Operations
    
    public static string GetDateKey(this DateTime date) 
        => $"{date.Year}{date.Month.WithZeroPrefix()}{date.Day.WithZeroPrefix()}";

    public static int GetDayIndex(this DateTime date)
        => int.Parse(date.ToString("yyyyMMdd"));
    
    public static DateTime GetDateFromDateKey(string dateKey)
    {
        if (string.IsNullOrEmpty(dateKey) || (dateKey.Length != 6 && dateKey.Length != 8))
            return dateKey == null ? DateTime.Now : DateTime.MinValue;
        
        int year = dateKey.Length == 6 
            ? 2000 + int.Parse(dateKey.Substring(0, 2))
            : int.Parse(dateKey.Substring(0, 4));
        
        int month = int.Parse(dateKey.Substring(dateKey.Length == 6 ? 2 : 4, 2));
        int day = int.Parse(dateKey.Substring(dateKey.Length == 6 ? 4 : 6, 2));
        
        return new DateTime(year, month, day);
    }
    
    #endregion

    #region Unix Timestamp Operations
    
    public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    
    public static long ToUnixTimestampSeconds(this DateTime dateTime)
        => new DateTimeOffset(dateTime).ToUnixTimeSeconds();

    public static DateTime ToDateTimeUtc(this long timestampMs)
        => DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime;

    public static DateTime ToDateTimeSecondsUtc(this long timestampMs)
        => DateTimeOffset.FromUnixTimeSeconds(timestampMs).UtcDateTime;

    public static DateTime ToDateTime(this long timestampMs)
        => DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).DateTime;

    public static DateTime FromUnixTimestampSeconds(this long timestampSeconds)
        => DateTimeOffset.FromUnixTimeSeconds(timestampSeconds).UtcDateTime;
    
    #endregion

    #region Date Comparison
    
    public static bool IsSameDayAs(this DateTime dateTime, DateTime compareDate)
        => dateTime.Year == compareDate.Year && dateTime.Month == compareDate.Month && dateTime.Day == compareDate.Day;

    public static bool IsLargerThan(this DateTime dateTime, TimeSpan? timeSpan) 
        => timeSpan != null && DateTime.Now - dateTime > timeSpan;
    
    public static bool IsLessThan(this DateTime dateTime, TimeSpan? timeSpan) 
        => timeSpan != null && DateTime.Now - dateTime < timeSpan;

    public static double CompareDaysWith(this DateTime dateTime, DateTime compareWith)
        => (compareWith - dateTime).TotalDays;
    
    #endregion

    #region Date Manipulation
    
    public static DateTime Copy(this DateTime dateTime)
        => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 
                       dateTime.Hour, dateTime.Minute, dateTime.Second, 
                       dateTime.Millisecond, dateTime.Microsecond);

    public static DateTime Yesterday(this DateTime dateTime)
        => dateTime.AddDays(-1);
    
    #endregion

    #region Time Difference Calculations
    
    private static double GetTimeDifference(DateTime dateTime, DateTime now, TimeUnit unit)
    {
        var difference = now - dateTime;
        return unit switch
        {
            TimeUnit.Seconds => difference.TotalSeconds,
            TimeUnit.Minutes => difference.TotalMinutes,
            TimeUnit.Hours => difference.TotalHours,
            TimeUnit.Days => difference.TotalDays,
            _ => 0
        };
    }

    // Main extension methods for DateTime
    public static double GetDifference(this DateTime dateTime, TimeUnit unit, bool utc = false)
        => GetTimeDifference(dateTime, utc ? DateTime.UtcNow : DateTime.Now, unit);

    public static double GetDifference(this DateTime? dateTime, TimeUnit unit, bool utc = false)
        => dateTime?.GetDifference(unit, utc) ?? 0;

    // Unix timestamp extension
    public static double GetDifference(this long timestampMs, TimeUnit unit, bool utc = false)
        => GetTimeDifference(ToDateTime(timestampMs), 
                            utc ? DateTime.UtcNow : DateTime.Now, unit);

    // Convenience methods for DateTime
    public static double GetSecondsDifference(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Seconds);
    
    public static double GetSecondsDifferenceUTC(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Seconds, utc: true);
    
    public static double GetMinutesDifference(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Minutes);
    
    public static double GetMinutesDifferenceUTC(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Minutes, utc: true);
    
    public static double GetHoursDifference(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Hours);
    
    public static double GetHoursDifferenceUTC(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Hours, utc: true);
    
    public static double GetDaysDifference(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Days);
    
    public static double GetDaysDifferenceUtc(this DateTime dateTime) 
        => dateTime.GetDifference(TimeUnit.Days, utc: true);

    // Convenience methods for nullable DateTime
    public static double GetSecondsDifference(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Seconds);
    
    public static double GetSecondsDifferenceUTC(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Seconds, utc: true);
    
    public static double GetMinutesDifference(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Minutes);
    
    public static double GetMinutesDifferenceUTC(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Minutes, utc: true);
    
    public static double GetHoursDifference(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Hours);
    
    public static double GetHoursDifferenceUTC(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Hours, utc: true);
    
    public static double GetDaysDifference(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Days);
    
    public static double GetDaysDifferenceUtc(this DateTime? dateTime) 
        => dateTime.GetDifference(TimeUnit.Days, utc: true);

    // Convenience methods for Unix timestamps
    public static double GetSecondsDifference(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Seconds);
    
    public static double GetSecondsDifferenceUTC(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Seconds, utc: true);
    
    public static double GetMinutesDifference(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Minutes);
    
    public static double GetMinutesDifferenceUTC(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Minutes, utc: true);
    
    public static double GetHoursDifference(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Hours);
    
    public static double GetHoursDifferenceUTC(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Hours, utc: true);
    
    public static double GetDaysDifference(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Days);
    
    public static double GetDaysDifferenceUTC(this long timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Days, utc: true);

    // Main extension method for nullable Unix timestamps
    public static double GetDifference(this long? timestampMs, TimeUnit unit, bool utc = false)
        => timestampMs?.GetDifference(unit, utc) ?? 0;

    // Convenience methods for nullable Unix timestamps
    public static double GetSecondsDifference(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Seconds);
    
    public static double GetSecondsDifferenceUTC(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Seconds, utc: true);
    
    public static double GetMinutesDifference(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Minutes);
    
    public static double GetMinutesDifferenceUTC(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Minutes, utc: true);
    
    public static double GetHoursDifference(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Hours);
    
    public static double GetHoursDifferenceUTC(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Hours, utc: true);
    
    public static double GetDaysDifference(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Days);
    
    public static double GetDaysDifferenceUTC(this long? timestampMs) 
        => timestampMs.GetDifference(TimeUnit.Days, utc: true);
    
    #endregion

    #region Formatting
    
    public static string GetMonthName(this DateTime dateTime) 
        => GetMonthName(dateTime.Month);
    
    public static string GetMonthName(int monthInt) => monthInt switch
    {
        1 => "January",
        2 => "February",
        3 => "March",
        4 => "April",
        5 => "May",
        6 => "June",
        7 => "July",
        8 => "August",
        9 => "September",
        10 => "October",
        11 => "November",
        12 => "December",
        _ => "Invalid Month"
    };
    
    #endregion

    public static (DateTime Date, string Key, long Unix) ConvertToKey(this DateTime dateTime)
    {
        var date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        var unix = new DateTimeOffset(date, TimeSpan.Zero).ToUnixTimeSeconds();;
        var key = date.ToString("yyyy-MM-dd");
        return (date, key, unix);
    }
}

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours,
    Days
}