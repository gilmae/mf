using System;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestNestedMicroformats
    {
        [Fact]
        void If_NestedMicroformat_Found_And_Not_Also_A_Property_Add_To_Children()
        {
            string test = @"<article id='foo'  class='h-entry'>
  <div class='h-cite'>
    <p>I really like <a class='p-name u-url' href='http://microformats.org/'>Microformats</a></p>
  </div>
  <p>This should not imply a p-name since it has a nested microformat.</p>
</article>";
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://localhost"));

            Assert.Single(doc.Items);
            Assert.Single(doc.Items[0].Children);
        }

        [Fact]
        void If_NestedMicroformat_Found_And_Also_A_Property_Add_To_Properties()
        {
            string test = @"<article id='foo'  class='h-entry'>
  <div class='u-like-of h-cite'>
    <p>I really like <a class='p-name u-url' href='http://microformats.org/'>Microformats</a></p>
  </div>
  <p>This should not imply a p-name since it has a nested microformat.</p>
</article>";
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://localhost"));

            Assert.Single(doc.Items);
            Assert.Empty(doc.Items[0].Children);
            Assert.Single(doc.Items[0].Properties["like-of"]);
            Assert.IsType<Microformat>(doc.Items[0].Properties["like-of"][0]);
        }
    }
}
