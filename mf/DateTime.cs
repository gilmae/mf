using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("mf.Tests")]
namespace mf
{
    public record DateTimeZone
    {
        internal DateTime dt { get; set; }
        internal TimeSpan tz { get; set; }
        
        internal bool hasDate { get; set; }
        internal bool hasTime { get; set; }
        internal bool hasTz { get; set; }
        internal bool hasSeconds { get; set; }

        public void SetDate(int year, int month, int day)
        {
            if (hasDate)
            {
                return;
            }

            dt = new DateTime(year, month, day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
            hasDate = true;
        }

        public void SetTime(int hour, int minute, int? seconds)
        {
            if (hasTime)
            {
                return;
            }
            dt = new DateTime(dt.Year, dt.Month, dt.Day, hour, minute, seconds ?? 0, 0, dt.Kind);
            hasTime = true;
            hasSeconds = seconds.HasValue;
        }

        public void SetTimezone(int hour, int minutes, bool westOfUTC = false)
        {
            if (hasTz)
            {
                return;
            }
            tz = new TimeSpan(Math.Abs(hour), Math.Abs(minutes), 0);
            if (westOfUTC)
            {
                tz *= -1;
            }
            hasTz = true;
        }

        public void SetTimezone(string zzz)
        {
            int[] parts = zzz.Split(':').Select(p => int.Parse(p)).ToArray();

            if (parts.Count() == 1)
            {
                SetTimezone(parts[0], 0, parts[0] < 1);
            }
            else if (parts.Count() == 2)
            {
                SetTimezone(parts[0], parts[1], parts[0] < 1);
            }
                

        }

        public override string ToString()
        {
            if (!hasDate)
            {
                return "";
            }

            if (!hasTime)
            {
                return dt.ToString("yyyy-MM-dd");
            }

            if (!hasTz)
            {
                if (hasSeconds)
                {
                    return dt.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                return dt.ToString("yyyy-MM-ddTHH:mm");
            }

            DateTimeOffset o = new DateTimeOffset(dt, tz);
            string value;
            if (hasSeconds)
            {
                value = o.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            else
            {
                value = o.ToString("yyyy-MM-ddTHH:mmzzz");
            }

            if (value.Substring(value.Length-3,1) ==":")
            {
                string minutes = value.Substring(value.Length - 2, 2);
                value = value.Substring(0, value.Length - 3) + minutes;
            }
            if (value.EndsWith("+0000"))
            {
                value = value.Remove(value.Length - 5, 5) + "Z";
            }
            return value;
        }
    }

    public static class DateTimeExtensions
    {
        struct datetimeFormats
        {
            internal string format { get; set; }
            internal bool hasSeconds { get; set; }
            internal bool hasTz { get; set; }

            public datetimeFormats(string format, bool hasSeconds, bool hasTz)
            {
                this.format = format;
                this.hasSeconds = hasSeconds;
                this.hasTz = hasTz;
            }
        };
        public static void Parse(this DateTimeZone dt, string s)
        {
            s = s.ToUpper();
            s.Replace(' ', 'T');
            s = new Regex(@"(A|P)\.?(M)\.?$").Replace(s, (Match match) =>
            {
                return $"{match.Groups[1]}{match.Groups[2]}";
            });

            datetimeFormats[] datetimeFormats = new datetimeFormats[]
            {
                new datetimeFormats("yyyy-MM-ddTHH:mm:sszzz", true,true ),
                new datetimeFormats("yyyy-MM-ddTHH:mmzzz", false,true ),

                new datetimeFormats("yyyy-MM-ddTHH:mm:sszz", true,true ),
                new datetimeFormats("yyyy-MM-ddTHH:mmzz", false,true ),

                new datetimeFormats("yyyy-MM-ddTHH:mm:ssz", true,true ),
                new datetimeFormats("yyyy-MM-ddTHH:mmz", false,true ),
                new datetimeFormats("yyyy-MM-ddTHH:mm:ssZ", true,true ),
                new datetimeFormats("yyyy-MM-ddTHH:mmZ", false,true ),

                new datetimeFormats("yyyy-MM-ddTHH:mm", false,false ),
                new datetimeFormats("yyyy-MM-ddTHH:mm:ss", true,false)
            };

            datetimeFormats[] timeFormats = new datetimeFormats[]
            {
                new datetimeFormats("HH:mm:sszzz", true,true ),
                new datetimeFormats("HH:mmzzz", false,true ),

                new datetimeFormats("HH:mm:sszz", true,true ),
                new datetimeFormats("HH:mmzz", false,true ),

                new datetimeFormats("HH:mm:ssz", true,true ),
                new datetimeFormats("HH:mmz", false,true ),
                new datetimeFormats("HH:mm:ssZ", true,true ),
                new datetimeFormats("HH:mmZ", false,true ),

                new datetimeFormats("HH:mm", false,false ),
                new datetimeFormats("HH:mm:ss", true,false),

                new datetimeFormats("HH:mm:sstt", true,false),
                new datetimeFormats("hh:mm:sstt", true,false),
                new datetimeFormats("htt", false,false ),
                new datetimeFormats("h:mmtt", false,false ),
                new datetimeFormats("h:mm:sstt", true,false),
                



            };

            datetimeFormats[] timeZoneFormats = new datetimeFormats[]
{
                new datetimeFormats("zzz", false,true ),
                new datetimeFormats("zz", false,true ),
                new datetimeFormats("Z", false,true ),
};

            DateTime d;

            // Parse Date Time formats
            foreach (var test in datetimeFormats)
            {
                if (DateTime.TryParseExact(s,test.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out  d))
                {
                    dt.SetDate(d.Year, d.Month, d.Day);
                    if (test.hasSeconds)
                    {
                        dt.SetTime(d.Hour, d.Minute, d.Second);
                    } else
                    {
                        dt.SetTime(d.Hour, d.Minute, null);
                    }

                    if (test.hasTz)
                    {
                        dt.SetTimezone(d.ToString("zzz"));
                    }

                    return;
                }
            }
            
            // Parse Date Only formats
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out  d))
            {
                dt.SetDate(d.Year, d.Month, d.Day);
                return;
            }

            // Including the weird arse Ordinal Date format
            var match = new Regex(@"(\d{4})-(\d{3})").Match(s);
            if (match != null && match.Success)
            {
                d = new DateTime(int.Parse(match.Groups[1].Value), 1, 1, 0, 0, 0);
                d.AddDays(int.Parse(match.Groups[2].Value) - 1);
                dt.SetDate(d.Year,d.Month,d.Day);
                return;
            }


            // Parse Time only formats
            foreach (var test in timeFormats)
            {
                if (DateTime.TryParseExact(s, test.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    if (test.hasSeconds)
                    {
                        dt.SetTime(d.Hour, d.Minute, d.Second);
                    }
                    else
                    {
                        dt.SetTime(d.Hour, d.Minute, null);
                    }

                    if (test.hasTz)
                    {
                        dt.SetTimezone(d.ToString("zzz"));
                    }

                    return;
                }
            }

            // Parse Timezone only formats
            foreach (var test in timeZoneFormats)
            {
                if (s == "Z")
                {
                    dt.SetTimezone(0, 0);
                    return;
                }

                if (DateTime.TryParseExact(s, test.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    if (test.hasTz)
                    {
                        dt.SetTimezone(d.ToString("zzz"));
                    }

                    return;
                }

                
            }


        }
    }
}
