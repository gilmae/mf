using AngleSharp.Html.Parser;
using Xunit;
namespace mf.Tests.ParserTests
{
    public class TestGetValueClassValue
    {
        [Fact]
        void ShouldOnlyReturnTextWithinValueClassTags()
        {
            string test = @"<span class='tel'>
  <span class='type'>Home</span>:
  <span class='value'>+1.415.555.1212</span>
</span>";

            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var text = node.ParseValueClassPattern();

            Assert.Equal("+1.415.555.1212", text);

        }

        [Fact]
        void Should_ParseMultipleSiblingValueClasses()
        {
            var test = @"<span class='tel'>
  <span class='type'>Home</span>:
  <span class='value'>+44</span> (0) <span class='value'>1223 123 123</span>
</span>";


        var _parser = new HtmlParser(new HtmlParserOptions
        {
            IsNotConsumingCharacterReferences = true,
        });
        var node = _parser.ParseDocument(test).DocumentElement;

        var text = node.ParseValueClassPattern();

        Assert.Equal("+441223 123 123", text);
        }

        [Fact]
        void Should_NotParseNestedValueClasses()
        {
            string test = @"<p class='description'>
  <span class='value'>
    <em class='value'>Puppies Rule!</em>
    <strong>But kittens are better!</strong>
 </span>
</p>";

            var _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            var node = _parser.ParseDocument(test).DocumentElement;

            var actual = node.ParseValueClassPattern();
            string expected = "<em class=\"value\">Puppies Rule!</em><strong>But kittens are better!</strong>";
            Assert.Equal(expected, actual);
        }
    }
}
