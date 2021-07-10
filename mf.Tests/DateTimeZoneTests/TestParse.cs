using System;
using Xunit;
namespace mf.Tests.DateTimeZoneTests
{
    public class TestParse
    {



        [InlineData("2001-02-03", true, false, false, false)]
        [InlineData("2006-01-02T15:04:05-07:00", true, true, true, true)]
        [InlineData("2006-01-02T15:04:05-0700", true, true, true, true)]
        [InlineData("2006-01-02T15:04:05-07", true, true, true, true)]
        [InlineData("2006-01-02T15:04:05", true, true, true, false)]
        [InlineData("2006-01-02T15:04+07:00", true, true, false, true)]
        [InlineData("2006-01-02T15:04-07:00", true, true, false, true)]
        [InlineData("2006-01-02T15:04-0700", true, true, false, true)]
        [InlineData("2006-01-02T15:04-07", true, true, false, true)]
        [InlineData("2006-01-02T15:04", true, true, false, false)]
        [InlineData("2006-01-02T15:04Z", true, true, false, true)]
        [InlineData("2006-01-02T15:04:05Z", true, true, true, true)]
        [InlineData("2006-123", true, false, false, false)]
        [InlineData("15:04:05-07:00", false, true, true, true)]
        [InlineData("15:04:05-0700", false, true, true, true)]
        [InlineData("15:04:05-07", false, true, true, true)]
        [InlineData("15:04:05", false, true, true, false)]
        [InlineData("15:04+07:00", false, true, false, true)]
        [InlineData("15:04-07:00", false, true, false, true)]
        [InlineData("15:04-0700", false, true, false, true)]
        [InlineData("15:04-07", false, true, false, true)]
        [InlineData("15:04", false, true, false, false)]
        [InlineData("15:04Z", false, true, false, true)]
        [InlineData("15:04:05Z", false, true, true, true)]
        [InlineData("3:04:05AM", false, true, true, false)]
        [InlineData("3:04:05PM", false, true, true, false)]
        [InlineData("3:04AM", false, true, false, false)]
        [InlineData("3:04PM", false, true, false, false)]
        [InlineData("3AM", false, true, false, false)]
        [InlineData("3PM", false, true, false, false)]
        [InlineData("3A.M.", false, true, false, false)]
        [InlineData("3P.M.", false, true, false, false)]
        [InlineData("-10:01", false, false, false, true)]
        [InlineData("+10:01", false, false, false, true)]
        [InlineData("-1001", false, false, false, true)]
        [InlineData("+1001", false, false, false, true)]
        [InlineData("-10", false, false, false, true)]
        [InlineData("+10", false, false, false, true)]
        [InlineData("Z", false, false, false, true)]
        [Theory]
        void Test_String_Parsing(string test, bool expectDateSet, bool expectTimeSet, bool expectSecondsSet, bool expectTzSet)
        {
            var dtz = new DateTimeZone();
            dtz.Parse(test);

            Assert.Equal(expectDateSet, dtz.hasDate);
            Assert.Equal(expectTimeSet, dtz.hasTime);
            Assert.Equal(expectSecondsSet, dtz.hasSeconds);
            Assert.Equal(expectTzSet, dtz.hasTz);
        }

        void Multiple_Values_Progressively_Builds_Datetime()
        {
            var dtz = new DateTimeZone();
            dtz.Parse("2001-02-03");
            dtz.Parse("04:05");
            dtz.Parse("+06:07");

            Assert.True(dtz.hasDate);
            Assert.True(dtz.hasTime);
            Assert.False(dtz.hasSeconds);
            Assert.True(dtz.hasTz);
        }
    }
}
