using System;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseImpliedPhoto
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParseImpliedPhoto()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://test.com");
        }

        [Fact]
        void If_No_SpecialCases_Should_ReturnNull()
        {
            string test = @"<article class='h-entry'>
    <p> There's no implied photo </p>
     </ article > ";

            var node = _parser.ParseDocument(test).GetElementsByTagName("article").First();

            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.Null(actualValue);
        }

        [Fact]
        void If_ImgWithSrcAndAlt_Should_ReturnPhoto()
        {
            string test = @"<img class='h-entry' alt='foo' src='jane.jpeg' />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("img").First();

            Photo expectedValue = new Photo { Value = "http://test.com/jane.jpeg", Alt = "foo" };
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as Photo);
            Assert.True(expectedValue.AreEquivalent(actualValue as Photo));
        }

        [Fact]
        void If_ImgWithSrc_Should_ReturnString()
        {
            string test = @"<img class='h-entry' src='jane.jpeg' />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("img").First();

            string expectedValue = "http://test.com/jane.jpeg";
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as string);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_ImgWithoutSrc_Should_ReturnNull()
        {
            string test = @"<img class='h-entry' alt='foo'  />";

            var node = _parser.ParseDocument(test).GetElementsByTagName("img").First();

            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.Null(actualValue);
        }

        [Fact]
        void If_ObjectWithData_Should_ReturnString()
        {
            string test = @"<object class='h-card' data='jane.jpeg'>Jane Doe</object>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("object").First();

            string expectedValue = "http://test.com/jane.jpeg";
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as string);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_ObjectWithoutData_Should_ReturnNull()
        {
            string test = @"<object class='h-card' >Jane Doe</object>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("object").First();

            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.Null(actualValue);
        }


        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_RootClass_Should_ReturnNull()
        {
            string test = @"<p class='h-entry'><img class='h-entry' alt='foo' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.Null(actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Img_Should_ReturnPhoto()
        {
            string test = @"<p class='h-entry'><img alt='foo' src='jane.jpeg' />bar</p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            Photo expectedValue = new Photo { Value = "http://test.com/jane.jpeg", Alt = "foo" };
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as Photo);
            Assert.True(expectedValue.AreEquivalent(actualValue as Photo));
        }

        [Fact]
        void If_NothingOnParent_And_OnlyChild_Is_Object_Should_ReturnString()
        {
            string test = @"<p class='h-entry'><object data='jane.jpeg'>Jane Doe</object></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "http://test.com/jane.jpeg";
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as string);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Img_Should_ReturnPhoto()
        {
            string test = @"<p class='h-entry'><span><img alt='foo' src='jane.jpeg' />bar</span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            Photo expectedValue = new Photo { Value = "http://test.com/jane.jpeg", Alt = "foo" };
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as Photo);
            Assert.True(expectedValue.AreEquivalent(actualValue as Photo));
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_Object_Should_ReturnString()
        {
            string test = @"<p class='h-entry'><span><object data='jane.jpeg'>Jane Doe</object></span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            string expectedValue = "http://test.com/jane.jpeg";
            object actualValue = node.ParseImpliedPhoto(_baseUri);

            Assert.NotNull(actualValue);
            Assert.NotNull(actualValue as string);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        void If_NothingOnParent_And_Child_And_OnlyChilds_OnlyChild_Is_RootClass_Should_ReturnNull()
        {
            string test = @"<p class='h-entry'><span><object data='jane.jpeg' class='h-entry'>Jane Doe</object></span></p>";

            var node = _parser.ParseDocument(test).GetElementsByTagName("p").First();

            object actualValue = null;

            Assert.Null(actualValue);
        }
    }
}
