using System;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestParseRels
    {

        string test = @"<a rel='author' href='http://example.com/a'>author a</a>
<a rel='author' href='http://example.com/b'>author b</a>
<a rel='in-reply-to' href='http://example.com/1'>post 1</a>
<a rel='in-reply-to' href='http://example.com/2'>post 2</a>
<a rel='alternate home'
   href='http://example.com/fr'
   media='handheld'
   hreflang='fr'>French mobile homepage</a>";

        [Fact]
        void Should_Find_Rels()
        {
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://example.com"));

            Assert.Equal(4, doc.Rels.Count);
            Assert.Equal(2, doc.Rels["author"].Count);
            Assert.Equal(2, doc.Rels["in-reply-to"].Count);
            Assert.Single(doc.Rels["home"]);
            Assert.Single(doc.Rels["alternate"]);

            Assert.Equal(5, doc.Rel_Urls.Count);

            Assert.Equal("handheld", doc.Rel_Urls["http://example.com/fr"]["media"]);
            Assert.Equal("fr", doc.Rel_Urls["http://example.com/fr"]["hreflang"]);
            Assert.Equal("French mobile homepage", doc.Rel_Urls["http://example.com/fr"]["text"]);

        }
    }
}
