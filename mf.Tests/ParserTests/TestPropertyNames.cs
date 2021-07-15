using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestPropertyNames
    {
        [Fact]
        void If_Id_IsPresent_Set_ItemId()
        {
            Parser p = new Parser();
            var doc = p.Parse("<article id=\"AnArticle\" class=\"h-entry\"><h1 class=\"p-name\">Hello</h1></article>", new Uri("http://localhost"));

            Assert.Equal("AnArticle", doc.Items.First().Id);

        }

        [Fact]
        public void Finds_P_Properties()
        {
            Parser p = new Parser();
            var doc = p.Parse("<article class=\"h-entry\"><h1 class=\"p-name\">Hello</h1></article>", new Uri("http://localhost"));

            Assert.True(doc.Items.First().Properties.ContainsKey("name"));
        }

        [Fact]
        public void Finds_U_Properties()
        {
            string test = @"<div class='h-entry'>
    <p class='p-name'>microformats.org at 7</p>


    <p class='u-url'>
      <span class='value-title' title='http://microformats.org/'> </span>
      Article permalink
    </p>
    <p class='u-url'>
        <span class='value'>http://microformats.org/</span> -
        <span class='value'>2012/06/25/microformats-org-at-7</span>
    </p>

    <p><a class='u-url' href='http://microformats.org/2012/06/25/microformats-org-at-7'>Article permalink</a></p>

    <img src='images/logo.gif' alt='company logos' usemap='#logomap' />
    <map name='logomap'>
        <area class='u-url' shape='rect' coords='0,0,82,126' href='http://microformats.org/' alt='microformats.org' />
    </map>

    <img class='u-photo' src='images/logo.gif' alt='company logos' />

    <video class='u-photo' poster='posterimage.jpg'>
        Sorry, your browser doesn't support embedded videos.
    </video>

    <object class='u-url' data='http://microformats.org/wiki/microformats2-parsing'></object>

    <abbr class='u-url' title='http://microformats.org/wiki/value-class-pattern'>value-class-pattern</abbr>
    <data class='u-url' value='http://microformats.org/wiki/'></data>
    <p class='u-url'>http://microformats.org/discuss</p>
</div>";
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://example.com"));

            Assert.Single(doc.Items);
            var item = doc.Items.First();

            Assert.True(item.Properties.ContainsKey("url"));
            Assert.Equal(8, item.Properties["url"].Length);

            Assert.Equal(new[]
            {
                "http://microformats.org/",
          "http://microformats.org/2012/06/25/microformats-org-at-7",
          "http://microformats.org/2012/06/25/microformats-org-at-7",
          "http://microformats.org/",
          "http://microformats.org/wiki/microformats2-parsing",
          "http://microformats.org/wiki/value-class-pattern",
          "http://microformats.org/wiki/",
          "http://microformats.org/discuss"
            }, item.Properties["url"]);

            Assert.True(item.Properties.ContainsKey("photo"));
            Assert.Equal(2, item.Properties["photo"].Length);

            Assert.True(new Photo { Alt="company logos", Value= "http://example.com/images/logo.gif" }
                .AreEquivalent(item.Properties["photo"].First() as Photo));

            Assert.Equal("http://example.com/posterimage.jpg", item.Properties["photo"].Last());
        }

        [Fact]
        public void If_ItemHasNoExplicitName_Should_CheckForImpliedName()
        {
            string test = @"<article class='h-entry'>
  <p>This should imply a p-name</p>
</article>

<article class='h-entry'>
  <div class='p-content'><p>This should not imply a p-name since it has an p-* property.</p></div>
</article>

<article class='h-entry'>
  <div class='e-content'><p>This should not imply a p-name since it has an e-* property.</p></div>
</article>
<article class='h-entry'>
  <div class='u-like-of h-cite'>
    <p>I really like <a class='p-name u-url' href='http://microformats.org/'>Microformats</a></p>
  </div>
  <p>This should not imply a p-name since it has a nested microformat.</p>
</article>";
            Parser p = new Parser();
            var doc = p.Parse(test, new Uri("http://localhost"));

            Assert.Equal(4, doc.Items.Count());
            Assert.True(doc.Items.First().Properties.ContainsKey("name"));
            Assert.Equal("This should imply a p-name", doc.Items.First().Properties["name"].Single());
            Assert.False(doc.Items[1].Properties.ContainsKey("name"));
            Assert.False(doc.Items[2].Properties.ContainsKey("name"));

            Assert.False(doc.Items[3].Properties.ContainsKey("name"));


        }

    }
}
