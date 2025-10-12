namespace Aco228.Common.Extensions;

public static class DateTimeExtensions
{
    public static string GetDateKey(this DateTime date) 
        => $"{date.Year}{date.Month.WithZeroPrefix()}{date.Day.WithZeroPrefix()}";

    public static int GetDayIndex(this DateTime date)
        => int.Parse(date.ToString("yyyyMMdd"));
    
    
    public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    
    public static long ToUnixTimestampSeconds(this DateTime dateTime)
        => new DateTimeOffset(dateTime).ToUnixTimeSeconds();

    public static DateTime GetDateFromDateKey(string dateKey)
    {
        if (string.IsNullOrEmpty(dateKey))
            return DateTime.Now;
        
        if(dateKey.Length == 6)
            return new DateTime(
                year: (2000 + int.Parse(dateKey.Substring(0, 2))),
                month: int.Parse(dateKey.Substring(2, 2)),
                day: int.Parse(dateKey.Substring(4, 2)));
        
        if(dateKey.Length == 8)
            return new DateTime(
                year: (int.Parse(dateKey.Substring(0, 4))),
                month: int.Parse(dateKey.Substring(4, 2)),
                day: int.Parse(dateKey.Substring(6, 2)));
        
        return DateTime.MinValue;
    }
    
    public static bool IsSameDayAs(this DateTime datetime, DateTime compareDate)
        => datetime.Year == compareDate.Year && datetime.Month == compareDate.Month && datetime.Day == compareDate.Day;

    public static bool IsLargerThan(this DateTime dateTime, TimeSpan? timeSpan) => timeSpan == null ? false : DateTime.Now - dateTime > timeSpan;
    public static bool IsLessThan(this DateTime dateTime, TimeSpan? timeSpan) => timeSpan == null ? false : DateTime.Now - dateTime < timeSpan;

    public static DateTime Copy(this DateTime dateTime)
        => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Microsecond);

    public static DateTime Yesterday(this DateTime dateTime)
        => dateTime.AddDays(-1);

    public static double GetHoursDifference(this DateTime dateTime) => (DateTime.Now - dateTime).TotalHours;
    public static double GetHoursDifference(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.Now - dateTime.Value).TotalHours;
    
    public static double GetHoursDifferenceUTC(this DateTime dateTime) => (DateTime.UtcNow - dateTime).TotalHours;
    public static double GetHoursDifferenceUTC(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.UtcNow - dateTime.Value).TotalHours;
    
    public static double GetMinutesDifference(this DateTime dateTime) => (DateTime.Now - dateTime).TotalMinutes;
    public static double GetMinutesDifference(this DateTime? dateTime) => dateTime == null ? 0 :  (DateTime.Now - dateTime.Value).TotalMinutes;
    
    public static double GetMinutesDifferenceUTC(this DateTime dateTime) => (DateTime.UtcNow - dateTime).TotalMinutes;
    public static double GetMinutesDifferenceUTC(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.UtcNow - dateTime.Value).TotalMinutes;
    
    public static double GetDaysDifference(this DateTime dateTime) => (DateTime.Now - dateTime).TotalDays;
    public static double GetDaysDifference(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.Now - dateTime.Value).TotalDays;
    
    public static double GetDaysDifferenceUtc(this DateTime dateTime) => (DateTime.UtcNow - dateTime).TotalDays;
    public static double GetDaysDifferenceUtc(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.UtcNow - dateTime.Value).TotalDays;
    
    public static double GetSecondsDifference(this DateTime dateTime) => (DateTime.Now - dateTime).TotalSeconds;
    public static double GetSecondsDifference(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.Now - dateTime.Value).TotalSeconds;
    
    public static double GetSecondsDifferenceUtc(this DateTime dateTime) => (DateTime.UtcNow - dateTime).TotalSeconds;
    public static double GetSecondsDifferenceUtc(this DateTime? dateTime) => dateTime == null ? 0 : (DateTime.UtcNow - dateTime.Value).TotalSeconds;
    
    public static double CompareDaysWith(this DateTime dateTime, DateTime compareWith)
        => (compareWith - dateTime).TotalDays;

    public static string GetMonthName(this DateTime datetime) => GetMonthName(datetime.Month);
    public static string GetMonthName(int monthInt)
    {
        return monthInt switch 
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "Avgust",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
        };
    }
}