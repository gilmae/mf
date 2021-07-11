using System;
using Xunit;
namespace mf.Tests.DateTimeZoneTests
{
    public class TestDateTimeZone
    {

        [Fact]
        void If_NoDataSet_Then_HsNoValuesSet()
        {
            DateTimeZone dtz = new DateTimeZone();
            Assert.False(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.False(dtz.hasTz);
        }

        [Fact]
        void Set_Date_Should_SetYearMonthAndDay()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2000, 1, 2);

            Assert.True(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.False(dtz.hasTz);

            Assert.Equal(2000, dtz.dt.Year);
            Assert.Equal(1, dtz.dt.Month);
            Assert.Equal(2, dtz.dt.Day);
        }

        [Fact]
        void Set_Multiple_Dates_Retains_First_Date()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2000, 1, 2);
            dtz.SetDate(2010, 11, 12);

            Assert.Equal(2000, dtz.dt.Year);
            Assert.Equal(1, dtz.dt.Month);
            Assert.Equal(2, dtz.dt.Day);
        }

        [Fact]
        void Set_Time_With_NullSeconds_Should_SetHourAndMinute()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTime(10, 11, null);

            Assert.False(dtz.hasDate);
            Assert.True(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.False(dtz.hasTz);

            Assert.Equal(10, dtz.dt.Hour);
            Assert.Equal(11, dtz.dt.Minute);
            Assert.Equal(0, dtz.dt.Second);
        }

        [Fact]
        void Set_Time_With_Seconds_Should_SetHourAndMinute()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTime(10, 11, 12);

            Assert.False(dtz.hasDate);
            Assert.True(dtz.hasTime);
            Assert.True(dtz.hasSeconds);
            Assert.False(dtz.hasTz);

            Assert.Equal(10, dtz.dt.Hour);
            Assert.Equal(11, dtz.dt.Minute);
            Assert.Equal(12, dtz.dt.Second);
        }

        [Fact]
        void Set_Multiple_Times_With_Null_Seconds_Should_Retain_First_Time()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTime(10, 11, null);
            dtz.SetTime(1, 1, null);

            Assert.Equal(10, dtz.dt.Hour);
            Assert.Equal(11, dtz.dt.Minute);
            Assert.Equal(0, dtz.dt.Second);
        }

        [Fact]
        void Set_Multiple_Times_With_Seconds_Should_Retain_First_Time()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTime(10, 11, 3);
            dtz.SetTime(1, 1, 4);

            Assert.Equal(10, dtz.dt.Hour);
            Assert.Equal(11, dtz.dt.Minute);
            Assert.Equal(3, dtz.dt.Second);
        }

        [Fact]
        void Set_Timezone_Should_SetTimezone()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTimezone(10, 11);

            Assert.False(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.True(dtz.hasTz);

            Assert.Equal(10, dtz.tz.Hours);
            Assert.Equal(11, dtz.tz.Minutes);
        }

        [Fact]
        void Set_Multiple_Timezones_Should_RetainFirst()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTimezone(10, 11);
            dtz.SetTimezone(1, 1, true);

            Assert.Equal(10, dtz.tz.Hours);
            Assert.Equal(11, dtz.tz.Minutes);
        }

        [Fact]
        void Set_West_Timezone_Should_SetTimezone()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTimezone(10, 11, true);

            Assert.False(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.True(dtz.hasTz);

            Assert.Equal(-10, dtz.tz.Hours);
            Assert.Equal(-11, dtz.tz.Minutes);
        }

        [Fact]
        void Set_String_Timezone_Should_SetTimezone()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTimezone("10:11");

            Assert.False(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.True(dtz.hasTz);

            Assert.Equal(10, dtz.tz.Hours);
            Assert.Equal(11, dtz.tz.Minutes);
        }

        [Fact]
        void Set_String_West_Timezone_Should_SetTimezone()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTimezone("-10:11");

            Assert.False(dtz.hasDate);
            Assert.False(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.True(dtz.hasTz);

            Assert.Equal(-10, dtz.tz.Hours);
            Assert.Equal(-11, dtz.tz.Minutes);
        }

        [Fact]
        void If_Time_Set_And_Date_NotSet_ToString_Returns_Empty()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetTime(10, 11, 12);

            Assert.Equal("", dtz.ToString());
        }

        [Fact]
        void If_Date_Set_ToString_Returns_yyyyMMdd()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2010, 11, 12);

            Assert.Equal("2010-11-12", dtz.ToString());
        }

        [Fact]
        void If_HoursAndMinutes_Set_ToString_Returns_yyyyMMdd_HH_mm()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, null);

            Assert.Equal("2001-02-03T04:05", dtz.ToString());
        }

        [Fact]
        void If_HoursAndMinutesAndSeconds_Set_ToString_Returns_yyyyMMdd_HH_mm_ss()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, 6);

            Assert.Equal("2001-02-03T04:05:06", dtz.ToString());
        }

        [Fact]
        void If_PMHour_Set_ToString_Returns_TwentyFourHourTime()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(14, 5, 6);

            Assert.Equal("2001-02-03T14:05:06", dtz.ToString());
        }

        [Fact]
        void If_Timezone_Set_And_Seconds_Not_ToString_Returns_yyyyMMdd_HH_mm_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(14, 5, null);
            dtz.SetTimezone(6, 7);

            Assert.Equal("2001-02-03T14:05+0607", dtz.ToString());
        }

        [Fact]
        void If_Timezone_Set__ToString_Returns_yyyyMMdd_HH_mm_ss_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, 6);
            dtz.SetTimezone(7, 8);

            Assert.Equal("2001-02-03T04:05:06+0708", dtz.ToString());
        }


        [Fact]
        void If_Timezone_West_Of_UTC_Set_ToString_Returns_Negative_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, 6);
            dtz.SetTimezone(7, 8, true);

            Assert.Equal("2001-02-03T04:05:06-0708", dtz.ToString());
        }

        [Fact]
        void If_Timezone_Set_To_Zeros_ToString_Returns_yyyyMMdd_HH_mm_Z()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(14, 5, null);
            dtz.SetTimezone(0, 0);

            Assert.Equal("2001-02-03T14:05Z", dtz.ToString());
        }

        [Fact]
        void If_String_Timezone_Set_And_Seconds_Not_ToString_Returns_yyyyMMdd_HH_mm_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(14, 5, null);
            dtz.SetTimezone("06:07");

            Assert.Equal("2001-02-03T14:05+0607", dtz.ToString());
        }

        [Fact]
        void If_String_Timezone_Set__ToString_Returns_yyyyMMdd_HH_mm_ss_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, 6);
            dtz.SetTimezone("07:08");

            Assert.Equal("2001-02-03T04:05:06+0708", dtz.ToString());
        }


        [Fact]
        void If_String_Timezone_West_Of_UTC_Set_ToString_Returns_Negative_zzz()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(4, 5, 6);
            dtz.SetTimezone("-07:08");

            Assert.Equal("2001-02-03T04:05:06-0708", dtz.ToString());
        }

        [Fact]
        void If_String_Timezone_Set_To_Zeros_ToString_Returns_yyyyMMdd_HH_mm_Z()
        {
            DateTimeZone dtz = new DateTimeZone();
            dtz.SetDate(2001, 2, 3);
            dtz.SetTime(14, 5, null);
            dtz.SetTimezone("00:00");

            Assert.Equal("2001-02-03T14:05Z", dtz.ToString());
        }


    }
}
