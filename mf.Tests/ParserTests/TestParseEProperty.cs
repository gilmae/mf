using System;
using AngleSharp.Html.Parser;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseEProperty
    {
        HtmlParser _parser;
        Uri _baseUri;
        public TestParseEProperty()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
            _baseUri = new Uri("http://example.com");
        }

        [Fact]
        void Ir_MarkupContainsRelativeUrls_ExpandThemForHtmlResult()
        {
            string test = @"<div id='tester' class='e-content'>
        <ul>
            <li><a href='http://www.w3.org/'>Should not change: http://www.w3.org/</a></li>
            <li><a href='http://example.com/'>Should not change: http://example.com/</a></li>
            <li><a href='test.html'>File relative: test.html = http://example.com/test.html</a></li>
            <li><a href='/test/test.html'>Directory relative: /test/test.html = http://example.com/test/test.html</a></li>
            <li><a href='/test.html'>Relative to root: /test.html = http://example.com/test.html</a></li>
        </ul>
        <img src='images/photo.gif' />
    </div>  ";

            var node = _parser.ParseDocument(test).GetElementById("tester");


            string expectedValue = "Should not change: http://www.w3.org/\n            Should not change: http://example.com/\n            File relative: test.html = http://example.com/test.html\n            Directory relative: /test/test.html = http://example.com/test/test.html\n            Relative to root: /test.html = http://example.com/test.html\n        \n         http://example.com/images/photo.gif";
            string expectedHtml = "<ul>\n            <li><a href=\"http://www.w3.org/\">Should not change: http://www.w3.org/</a></li>\n            <li><a href=\"http://example.com/\">Should not change: http://example.com/</a></li>\n            <li><a href=\"http://example.com/test.html\">File relative: test.html = http://example.com/test.html</a></li>\n            <li><a href=\"http://example.com/test/test.html\">Directory relative: /test/test.html = http://example.com/test/test.html</a></li>\n            <li><a href=\"http://example.com/test.html\">Relative to root: /test.html = http://example.com/test.html</a></li>\n        </ul>\n        <img src=\"http://example.com/images/photo.gif\">";
            (string actualValue, string actualHtml) = node.ParseEProperty(_baseUri);

            Assert.Equal(expectedValue, actualValue);
            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact]
        void If_MArkupIsHtmlEncoded_Then_DecodeForHtmlResult()
        {
            string test = @"<div class='h-entry'>
    <div id='tester' class='p-name e-content'>x&lt;y AT&amp;T &lt;b&gt;NotBold&lt;/b&gt; <b>Bold</b></div>
</div>";

            var node = _parser.ParseDocument(test).GetElementById("tester");


            string expectedValue = "x<y AT&T <b>NotBold</b> Bold";
            string expectedHtml = "x&lt;y AT&amp;T &lt;b&gt;NotBold&lt;/b&gt; <b>Bold</b>";
            (string actualValue, string actualHtml) = node.ParseEProperty(_baseUri);

            Assert.Equal(expectedValue, actualValue);
            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact]
        void If_Markup_CntainsEmbeddedProperty_Then_ResultContainsProperty()
        {
            string test = @"<div id='tester' class='e-content'>
        <p class='p-summary'>Last week the microformats.org community 
            celebrated its 7th birthday at a gathering hosted by Mozilla in 
            San Francisco and recognized accomplishments, challenges, and 
            opportunities.</p>

        <p>The microformats tagline “humans first, machines second” 
            forms the basis of many of our 
            <a href='http://microformats.org/wiki/principles'>principles</a>, and 
            in that regard, we’d like to recognize a few people and 
            thank them for their years of volunteer service </p>
    </div>  ";

            var node = _parser.ParseDocument(test).GetElementById("tester");

            string expectedValue = "Last week the microformats.org community \n            celebrated its 7th birthday at a gathering hosted by Mozilla in \n            San Francisco and recognized accomplishments, challenges, and \n            opportunities.\n\n        The microformats tagline “humans first, machines second” \n            forms the basis of many of our \n            principles, and \n            in that regard, we’d like to recognize a few people and \n            thank them for their years of volunteer service";
            string expectedHtml = "<p class=\"p-summary\">Last week the microformats.org community \n            celebrated its 7th birthday at a gathering hosted by Mozilla in \n            San Francisco and recognized accomplishments, challenges, and \n            opportunities.</p>\n\n        <p>The microformats tagline “humans first, machines second” \n            forms the basis of many of our \n            <a href=\"http://microformats.org/wiki/principles\">principles</a>, and \n            in that regard, we’d like to recognize a few people and \n            thank them for their years of volunteer service </p>";
            (string actualValue, string actualHtml) = node.ParseEProperty(_baseUri);

            Assert.Equal(expectedValue, actualValue);
            Assert.Equal(expectedHtml, actualHtml);
        }
    
    }
}
