using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class DateTimeParser : IDateTimeParser
    {




        public DateTime? Parse(string? dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            string[] formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "MM/dd/yyyy HH:mm" };

            if (DateTime.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out DateTime parsedDate))
            {
                // Assume the input is in local time and convert to UTC for storage
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Local).ToUniversalTime();
            }

            throw new Exception("Invalid date format. Use YYYY-MM-DD HH:mm.");
        }
    }
}
