using System.Windows.Data;
using System.Globalization;

namespace DeLong_Desktop.ApiService.Helpers;

public class UtcToUzbekistanTimeConverter : IValueConverter
{
    public static readonly UtcToUzbekistanTimeConverter Instance = new UtcToUzbekistanTimeConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // DateTimeOffset bo‘lsa
        if (value is DateTimeOffset dateTimeOffset)
        {
            var uzbekistanTime = dateTimeOffset.UtcDateTime.AddHours(5); // UTC+5
            return uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
        }
        // DateTime bo‘lsa
        else if (value is DateTime dateTime)
        {
            var uzbekistanTime = dateTime.AddHours(5); // UTC+5
            return uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
        }
        // Agar string bo‘lsa, parse qilib ko‘rish
        else if (value is string dateString && DateTime.TryParse(dateString, out var parsedDateTime))
        {
            var uzbekistanTime = parsedDateTime.AddHours(5);
            return uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
        }

        // Agar hech biri mos kelmassa, xuddi shunday qaytar
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}