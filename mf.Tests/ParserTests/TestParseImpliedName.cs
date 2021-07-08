using System;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseImpliedName
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParseImpliedName()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://test.com");
        }

        [Fact]
        void If_No_SpecialCases_Should_ReturnText()
        {
            string test = @"<article class='h-entry'>
    <p> This should imply a p-name </p>
     </ article > ";

            var node = _parser.ParseDocument(test).GetElementsByTagName("article").First();

            string expectedValue = "This should imply a p-name";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_Img_Should_ReturnAlt()
        {
            string test = @"<img class='h-entry' alt='foo' />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("img").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_Area_Should_ReturnAlt()
        {
            string test = @"<area class='h-entry' alt='foo' />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("area").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_Abbr_Should_ReturnTitle()
        {
            string test = @"<abbr class='h-entry' title='foo' />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("abbr").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_RootClass_Should_ReturnText()
        {
            string test = @"<p class='h-entry'><img class='h-entry' alt='foo' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foobar";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Img_Should_ReturnChildAlt()
        {
            string test = @"<p class='h-entry'><img alt='foo' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Area_Should_ReturnChildAlt()
        {
            string test = @"<p class='h-entry'><area alt='foo' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Abbr_Should_ReturnChildTitle()
        {
            string test = @"<p class='h-entry'><abbr title='foo' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Img_Should_ReturnGrandchildAlt()
        {
            string test = @"<p class='h-entry'><span><img alt='foo' />bar</span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Area_Should_ReturnGrandchildAlt()
        {
            string test = @"<p class='h-entry'><span><area alt='foo' />bar</span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Abbr_Should_ReturnGrandchildTitle()
        {
            string test = @"<p class='h-entry'><span><abbr title='foo' />bar</span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foo";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_RootClass_Should_ReturnText()
        {
            string test = @"<p class='h-entry'><span><img alt='foo' class='h-entry'/>bar</span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "foobar";
            string actualValue = node.ParseImpliedName(_baseUri);

            Assert.Equal(expectedValue, actualValue);
        }
    }
}
