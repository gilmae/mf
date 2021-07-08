using System;
using System.Linq;
using AngleSharp.Html.Parser;
using Xunit;
namespace mf.Tests.ParserTests
{
    public class TestParseImpliedUrl
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParseImpliedUrl()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://test.com");
        }

        [Fact]
        void If_Anchor_Should_ReturnHref()
        {
            string test = @"<a class='h-cite' href='http://google.com'>Google</a>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("a").First();

            string expected = "http://google.com";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Area_Should_ReturnHref()
        {
            string test = @"<area class='h-cite' href='http://google.com'>Google</area>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("area").First();

            string expected = "http://google.com";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Url_Is_Relative_Should_ReturnAsAbsolute()
        {
            string test = @"<area class='h-cite' href='test.html'>Google</area>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("area").First();

            string expected = "http://test.com/test.html";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_RootClass_Should_ReturnEmptyString()
        {
            string test = @"<p class='h-entry'><area class='h-cite' href='test.html'>Google</area></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Anchor_Should_ReturnHref()
        {
            string test = @"<p class='h-entry'><a href='test.html'>Google</a></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "http://test.com/test.html";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Area_Should_ReturnHref()
        {
            string test = @"<p class='h-entry'><area href='http://google.com'>Google</area></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "http://google.com";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Anchor_Should_ReturnHref()
        {
            string test = @"<p class='h-entry'><span><a href='http://google.com'>Google</a></span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "http://google.com";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Area_Should_ReturnHref()
        {
            string test = @"<p class='h-entry'><span><area href='test.html'>Google</area></span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "http://test.com/test.html";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_RootClass_Should_ReturnEmpty()
        {
            string test = @"<p class='h-entry'><span><area class='h-cite' href='test.html'>Google</area></span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expected = "";
            string actual = node.ParseImpliedUrl(_baseUri);

            Assert.Equal(expected, actual);
        }
    }
}
