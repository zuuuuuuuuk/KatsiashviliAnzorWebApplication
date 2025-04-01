namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IDateTimeParser
    {
        DateTime? Parse(string? dateTimeString);
    }
}
