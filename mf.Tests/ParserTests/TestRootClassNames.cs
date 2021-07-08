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

        [Fact]
        void If_Item_Has_NestedItem_NestedMicroformatsFlag_Is_Set()
        {
            string test = @"<article class='h-entry'>
  <div class='u-like-of h-cite'>
    <p>I really like <a class='p-name u-url' href='http://microformats.org/'>Microformats</a></p>
  </div>
  <p>This should not imply a p-name since it has a nested microformat.</p>
</article>";
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://localhost"));

            Assert.Single(doc.Items);
            Assert.True(doc.Items.First().HasNestedMicroformats);

        }

    }
}
