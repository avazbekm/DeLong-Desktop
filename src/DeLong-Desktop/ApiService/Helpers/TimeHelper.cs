namespace DeLong_Desktop.ApiService.Helpers;

public static class TimeHelper
{
    private static readonly TimeZoneInfo UzbekistanTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent");

    public static DateTime ConvertToUzbekistanTime(DateTimeOffset utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.UtcDateTime, UzbekistanTimeZone);
    }
}
