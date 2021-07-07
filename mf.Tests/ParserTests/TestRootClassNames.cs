using System;
using System.Linq;
using mf;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestRootClassNames
    {
        [Fact]
        public void FindsRootClassNames()
        {
            Parser p = new Parser();
            var doc = p.Parse("<article class=\"h-entry\"><h1 class=\"p-name\">Hello</h1></article>", new Uri("http://localhost"));
            Assert.Single(doc.Items);
            Assert.Equal(new[] { "h-entry" }, doc.Items.First().Type);

        }

        [Fact]
        public void FindsMultipleRootClassNames()
        {
            Parser p = new Parser();
            var doc = p.Parse("<article class=\"h-entry h-card\"><h1 class=\"p-name\">Hello</h1></article>", new Uri("http://localhost"));
            Assert.Single(doc.Items);
            Assert.Equal(new[] { "h-entry", "h-card" }, doc.Items.First().Type);
        }

    }
}
