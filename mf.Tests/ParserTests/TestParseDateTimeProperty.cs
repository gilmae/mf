using System;
using System.Linq;
using AngleSharp.Html.Parser;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseDateTimeProperty
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParseDateTimeProperty()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://example.com");
        }

        [Fact]
        void Should_Parse_ValueTitle_Pattern()
        {
            string test = @"    <p class='dt-start'>
      <span class='value-title' title='2013-03-14'> </span>
      March 14th 2013
    </p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            var expected = "2013-03-14";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Value_Pattern()
        {
            string test = @"    <p class='dt-start'>
        <time class='value' datetime='2013-06-25'>25 July</time>, from
        <span class='value'>07:00:00am 
    </span></p>   ";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            var expected = "2013-06-25T07:00:00";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Text()
        {
            string test = @"<p class='dt-start'>2013-07-02</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            var expected = "2013-07-02";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Time_DateTime()
        {
            string test = @"<time class='dt-start' datetime='2013-06-26'>26 June</time>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("time").First();

            var expected = "2013-06-26";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Ins_DateTime()
        {
            string test = @"<ins class='dt-start' datetime='2013-06-27'>Just added</ins>, ";

            var node = _parser.ParseDocument(test).GetElementsByTagName("ins").First();

            var expected = "2013-06-27";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Del_DateTime()
        {
            string test = @"<del class='dt-start' datetime='2013-06-28'>Removed</del>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("del").First();

            var expected = "2013-06-28";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Abbr_Title()
        {
            string test = @"<abbr class='dt-start' title='2013-06-29'>June 29</abbr> ";

            var node = _parser.ParseDocument(test).GetElementsByTagName("abbr").First();

            var expected = "2013-06-29";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Parse_Data_Value()
        {
            string test = @" <data class='dt-start' value='2013-07-01'></data>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("data").First();

            var expected = "2013-07-01";
            var actual = node.ParseDateTimeProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

    }
}
