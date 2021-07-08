using System;
using System.Linq;
using AngleSharp.Html.Parser;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseUProperty
    {
        


        HtmlParser _parser;
        Uri _baseUri;
        public TestParseUProperty()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://test.com");
        }

        [Fact]
        void If_Not_SpecialCase_Should_ReturnText()
        {
            string test = @"<p class='u-url'>http://microformats.org/discuss</p>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();
            string expected = "http://microformats.org/discuss";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Data_Should_Return_ValueAttr()
        {
            string test = @"<data class='u-url' value='http://microformats.org/wiki/'></data>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("data").First();
            string expected = "http://microformats.org/wiki/";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Abbr_Should_Return_TitleAttr()
        {
            string test = @"<abbr class='u-url' title='http://microformats.org/wiki/value-class-pattern'>value-class-pattern</abbr>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("abbr").First();
            string expected = "http://microformats.org/wiki/value-class-pattern";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Object_Should_Return_DataAttr()
        {
            string test = @"<object class='u-url' data='http://microformats.org/wiki/microformats2-parsing'></object>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("object").First();
            string expected = "http://microformats.org/wiki/microformats2-parsing";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Video_Should_ReturnPoster()
        {
            string test = @"    <video class='u-photo' poster='posterimage.jpg'>
        Sorry, your browser doesn't support embedded videos.
    </video>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("video").First();
            string expected = "http://test.com/posterimage.jpg";
            string expectedName = "";
            (string actual, string actualName) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        void If_Area_WithHref_Should_ReturnHref()
        {
            string test = @"<map name='logomap'>
        <area class='u-url' shape='rect' coords='0,0,82,126' href='http://microformats.org/' alt='microformats.org' />
    </map>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("area").First();
            string expected = "http://microformats.org/";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_Img_WithoutHref_Should_ReturnSrcAndAlt()
        {
            string test = @"<img class='u-photo' src='images/logo.gif' alt='company logos' />";
            var node = _parser.ParseDocument(test).GetElementsByTagName("img").First();
            string expected = "http://test.com/images/logo.gif";
            string expectedName = "company logos";
            (string actual, string actualName) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        void If_AnchorWithHref_SHould_ReturnHref()
        {
            string test = @"<p><a class='u-url' href='http://microformats.org/2012/06/25/microformats-org-at-7'>Article permalink</a></p>";
            var node = _parser.ParseDocument(test).GetElementsByTagName("a").First();
            string expected = "http://microformats.org/2012/06/25/microformats-org-at-7";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_ValueTitle_Found_Return_ValueTitle()
        {
            string test = @"    <p class='u-url'>
      <span class='value-title' title='http://microformats.org/'> </span>
      Article permalink
    </p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();
            string expected = "http://microformats.org/";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

        [Fact]
        void If_MultipleValueClasses_Should_ReturnValueClassesCombined()
        {
            var test = @"    <p class='u-url'>
        <span class='value'>http://microformats.org/</span> -
        <span class='value'>2012/06/25/microformats-org-at-7</span>
    </p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();
            string expected = "http://microformats.org/2012/06/25/microformats-org-at-7";
            (string actual, string _) = node.ParseUProperty(_baseUri);

            Assert.Equal(expected, actual);
        }

    }
}
