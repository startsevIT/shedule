namespace Domain;

public static class DateTimeExtensions
{
    public static string ToStringFormat(this DateTime date) =>
        $"{date.ToLongDateString()} {date.ToShortTimeString()}";
}