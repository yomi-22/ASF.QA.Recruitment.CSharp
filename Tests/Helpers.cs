using Application;
using System;
using System.Globalization;
using System.Net.Http.Json;

namespace Tests
{
    public class Helpers
    {

        readonly HttpClient client = new();
        public void RunApp()
        {
            var entryPoint = typeof(Program).Assembly.EntryPoint!;
            entryPoint.Invoke(null, new object[] { Array.Empty<string>() });
        }

        public string CapturedStdOut(Action callback)
        {
            TextWriter originalStdOut = Console.Out;

            using var newStdOut = new StringWriter();
            Console.SetOut(newStdOut);

            callback.Invoke();
            var capturedOutput = newStdOut.ToString();

            Console.SetOut(originalStdOut);

            return capturedOutput;
        }

        public string UKTime()
        {
            var ukDateTime = DateTime.Now;

            return ukDateTime.ToString("dddd dd MMMM yyyy HH:mm:ss");
            }

        public string CanadaTime()
        {
            var canadaDateResponse = client.GetFromJsonAsync<WorldTimeAPIResponse>("http://worldtimeapi.org/api/timezone/America/Toronto")?.Result;

            DateTimeOffset canadaDateTime = DateTimeOffset.ParseExact(canadaDateResponse.datetime, "yyyy-MM-dd'T'HH:mm:ss.FFFFFFzzz", CultureInfo.InvariantCulture);
            string dateTime = canadaDateTime.ToString("dddd dd MMMM yyyy HH:mm:ss");
            return dateTime;
        }

        public string CanadaAndUKTimeDifference()
        {
            var canadaDateResponse = client.GetFromJsonAsync<WorldTimeAPIResponse>("http://worldtimeapi.org/api/timezone/America/Toronto").Result;
            var canadaDateTime = DateTimeOffset.ParseExact(canadaDateResponse.datetime, "yyyy-MM-dd'T'HH:mm:ss.FFFFFFzzz", CultureInfo.InvariantCulture);

            var ukDateTime = DateTime.Now;
            double timeDifference;

            if (ukDateTime > canadaDateTime.DateTime)
            {
                timeDifference = Math.Round(ukDateTime.Subtract(canadaDateTime.DateTime).TotalMinutes, 0);
                return $"{timeDifference}m";
            }
            else
            {
                timeDifference = Math.Round(canadaDateTime.Subtract(ukDateTime).TotalMinutes, 0);
                return $"{timeDifference}m";
            }
        }

        public bool ValidateDateTime(string dateTimeFormat, string date)
        {
            bool isValidDate;
            isValidDate = DateTime.TryParseExact(date, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
          result: out DateTime parsedDate);
            if (isValidDate)
            {
                return true;
            }
            return false;
        }
    }
}
