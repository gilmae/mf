using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace mf.Tests.ParserTests
{
    public class TestPropertyNames
    {
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

    }
}
