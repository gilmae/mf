using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestPropertyNames
    {
        [Fact]
        public void FindsP_TypeProperties()
        {
            Parser p = new Parser();
            var doc = p.Parse("<article class=\"h-entry\"><h1 class=\"p-name\">Hello</h1></article>", new Uri("http://localhost"));

            Assert.True(doc.Items.First().Properties.ContainsKey("name"));
        }

    }
}
