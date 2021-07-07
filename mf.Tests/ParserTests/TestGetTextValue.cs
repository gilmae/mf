using System;
using AngleSharp.Html.Parser;
using Xunit;
namespace mf.Tests.ParserTests
{
    public class TestGetTextValue
    {

        [Fact]
        public void Should_Ignore_Script()
        {
            string test = "<span class=\"p-name\">A post<script>x = 1</script><style>p {color: #fff};</style></span>";
            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var text = node.GetTextValue(new Uri("http://test.com"));

            Assert.Equal("A post", text);
        }

        [Fact]
        public void If_Img_Has_AltAttribute_Use_AltAttribute()
        {
            string test = "<span class=\"p-name\"><img alt=\"Test Image\" /></span>";
            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var actual = node.GetTextValue(new Uri("http://test.com"));
            var expected = "Test Image";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void If_Img_Has_SrcAttribute_Use_SrcAttribute()
        {
            string test = "<span class=\"p-name\"><img src=\"http://google.com/test.png\" /></span>";
            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var actual = node.GetTextValue(new Uri("http://test.com"));
            var expected = "http://google.com/test.png";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void If_Img_Has_SrcAndALtAttribute_Use_AltAttribute()
        {
            string test = "<span class=\"p-name\"><img alt=\"Test Image\" src=\"http://google.com/test.png\" /></span>";
            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var actual = node.GetTextValue(new Uri("http://test.com"));
            var expected = "Test Image";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void If_Img_Has_RelativeSrcAttribute_Make_SrcAbsolute()
        {
            string test = "<span class=\"p-name\"><img src=\"test.png\" /></span>";
            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var actual = node.GetTextValue(new Uri("http://test.com"));
            var expected = "http://test.com/test.png";

            Assert.Equal(expected, actual);
        }
    }
}
