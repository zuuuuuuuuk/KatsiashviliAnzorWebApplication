using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class DateTimeParser : IDateTimeParser
    {


        public DateTimeParser() { }

        public DateTime? Parse(string? dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                throw new ArgumentException("Date string cannot be empty.");
            }

            DateTime parsedDate;
            string[] formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "MM/dd/yyyy HH:mm" };

            if (DateTime.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out parsedDate))
            {
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            throw new Exception("invalid date format. use YYYY-MM-DD HH-mm.");

        }
    }
}
