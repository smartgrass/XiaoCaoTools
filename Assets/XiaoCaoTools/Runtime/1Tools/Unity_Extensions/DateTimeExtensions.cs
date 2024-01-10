using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
namespace GG.Extensions
{
public static class DateTimeExtensions
    {
        public static readonly DateTime UNIXTIME_ZERO_POINT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string ToOccurrenceSuffix(this int integer)
        {
            switch (integer % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }
            switch (integer % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }
        
        public static string ToString(this DateTime dateTime, string format, bool useExtendedSpecifiers)
        {
            return useExtendedSpecifiers 
                       ? dateTime.ToString(format)
                                 .Replace("nn", dateTime.Day.ToOccurrenceSuffix().ToLower())
                                 .Replace("NN", dateTime.Day.ToOccurrenceSuffix().ToUpper())
                       : dateTime.ToString(format);
            
        } 
        /// <summary>
        /// Converts a Unix timestamp (UTC timezone by definition) into a DateTime object
        /// </summary>
        /// <param name="value">An input of Unix timestamp in seconds or milliseconds format</param>
        /// <param name="localize">should output be localized or remain in UTC timezone?</param>
        /// <param name="isInMilliseconds">Is input in milliseconds or seconds?</param>
        /// <returns></returns>
        public static DateTime FromUnixtime(this long value, bool localize = false, bool isInMilliseconds = false)
        {
            DateTime result;

            if (isInMilliseconds)
            {
                result = UNIXTIME_ZERO_POINT.AddMilliseconds(value);
            }
            else
            {
                result = UNIXTIME_ZERO_POINT.AddSeconds(value);
            }

            if (localize)
                return result.ToLocalTime();
            else
                return result;
        }

        /// <summary>
        /// Converts a DateTime object into a Unix time stamp
        /// </summary>
        /// <param name="value">any DateTime object as input</param>
        /// <param name="isInMilliseconds">Should output be in milliseconds or seconds?</param>
        /// <returns></returns>
        public static long ToUnixtime(this DateTime value, bool isInMilliseconds = false)
        {
            if (isInMilliseconds)
            {
                return (long) value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalMilliseconds;
            }
            else
            {
                return (long) value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalSeconds;
            }
        }

        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            var modTicks = dt.Ticks % d.Ticks;
            var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    
        /// <summary>
    /// get the datetime of the start of the month
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    /// <remarks>http://stackoverflow.com/a/5002582/428061</remarks>
    public static System.DateTime StartOfMonth(this System.DateTime dt) =>
        new System.DateTime(dt.Year, dt.Month, 1);
    
        /// <summary>
    /// get datetime of the start of the year
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static System.DateTime StartOfYear(this System.DateTime dt) => 
         new System.DateTime(dt.Year, 1, 1);

        /// <summary>
        /// Gets the current week number for a specified DateTime object.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static int GetCurrentWeekNumber(this DateTime dateTime)
        {
            int weekOfYear = Thread.CurrentThread.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            return weekOfYear;
        }

        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }

        public static List<DateTime> AllDatesInMonth(this List<DateTime> dateTime, int month, int year)
        {
            DateTime firstOftargetMonth = new DateTime(year, month, 1);
            DateTime firstOfNextMonth = firstOftargetMonth.AddMonths(1);
            List<DateTime> allDates = new List<DateTime>();

            foreach (DateTime allDate in dateTime)
            {
                if (allDate.InRange(firstOftargetMonth, firstOfNextMonth))
                {
                    allDates.Add(allDate);
                }
            }

            return allDates;
        }

        public static IList<DateTime> GetRandomDates(DateTime startDate, DateTime maxDate, int range)
        {
            int[] randomResult = GetRandomNumbers(range).ToArray();

            double calculationValue = maxDate.Subtract(startDate).TotalMinutes / int.MaxValue;
            List<DateTime> dateResults = randomResult.Select(s => startDate.AddMinutes(s * calculationValue)).ToList();
            return dateResults;
        }

        static IEnumerable<int> GetRandomNumbers(int size)
        {
            byte[] data = new byte[4];
            using (RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider(data))
            {
                for (int i = 0; i < size; i++)
                {
                    rng.GetBytes(data);

                    int value = BitConverter.ToInt32(data, 0);
                    yield return value < 0 ? value * -1 : value;
                }
            }
        }

        public static DateTime ToDateTime(this string s, string format = "ddMMyyyy", string cultureString = "en-GB")
        {
            try
            {
                DateTime r = DateTime.ParseExact(s: s, format: format, provider: CultureInfo.GetCultureInfo(cultureString));
                return r;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (CultureNotFoundException)
            {
                throw; // Given Culture is not supported culture
            }
        }

        public static DateTime ToDateTime(this string s, string format, CultureInfo culture)
        {
            try
            {
                DateTime r = DateTime.ParseExact(s: s, format: format, provider: culture);
                return r;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (CultureNotFoundException)
            {
                throw; // Given Culture is not supported culture
            }

        }
    }
}
