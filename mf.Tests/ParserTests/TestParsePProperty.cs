using System;
using AngleSharp.Html.Parser;
using Xunit;
namespace mf.Tests.ParserTests
{
    public class TestParsePProperty
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParsePProperty()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://test.com");
        }

        [Fact]
        void If_AbbrTitle_Found_Should_UseTitle()
        {
            string test = "<abbr id='tester' class='p-name' title='Mister'>Mr</abbr>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mister";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_AbbrTitle_Missing_Should_UseText()
        {
            string test = "<abbr id='tester' class='p-name'>Mr</abbr>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mr";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_LinkTitle_Found_Should_UseTitle()
        {
            string test = "<link id='tester' class='p-name' title='Mister' />";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mister";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_LinkTitle_Missing_Should_BeEmpty()
        {
            string test = "<link id='tester' class='p-name' />";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_DataValue_Found_Should_UseValue()
        {
            string test = "<data id='tester' class='p-name' value='Mister'>Mr</data>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mister";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_DataValue_Missing_Should_UseText()
        {
            string test = "<data id='tester' class='p-name'>Mr</data>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mr";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_ImgAlt_Found_Should_UseAlt()
        {
            string test = "<img id='tester' class='p-name' alt='Mister'>Mr</img>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mister";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_ImgAlt_Missing_Should_BeEmpty()
        {
            string test = "<img id='tester' class='p-name'>Mr</img>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_AreaAlt_Found_Should_UseAlt()
        {
            string test = "<area id='tester' class='p-name' alt='Mister'>Mr</area>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "Mister";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_AreaAlt_Missing_Should_BeEmpty()
        {
            string test = "<area id='tester' class='p-name'>Mr</area>";
            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expected = "";
            string actual = node.ParsePProperty(_baseUri);

            Assert.Equal(expected, actual);
        }
    }
}
