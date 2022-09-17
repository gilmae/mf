using System;
using Xunit;

namespace mf.Tests.VocabularyTests {
    public class VocabularyTests {
        [Fact]
        public void If_MicroformatTypes_DoNotMatch_RequiredType_ThrowsArgumentException () {
            Microformat mf = new Microformat { Type = new [] { "h-cite" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new [] { "test" } } } };
            Assert.Throws<ArgumentException> (() => mf.AsVocab<Entry> ());
        }

        [Fact]
        public void Populates_Properties () {
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new [] { "test" } } } };

            Entry v = mf.AsVocab<Entry> ();

            Assert.Equal (new [] { "test" }, v.Name);
        }

        [Fact]
        public void Casts_ArrayOfString_Properties () {
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "summary", new [] { "test" } } } };

            Entry v = mf.AsVocab<Entry> ();

            Assert.Equal (new string[] { "test" }, v.Summary);
        }

        [Fact]
        public void Casts_ArrayOfDateTime_Properties () {
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "published", new [] { "2021-01-01T00:00:00" } } } };

            Entry v = mf.AsVocab<Entry> ();

            Assert.Equal (new DateTime[] { DateTime.Parse ("2021-01-01T00:00:00") }, v.Published);
        }

        [Fact]
        public void If_Property_IsScalar_Takes_FirstElement () {
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "content", new [] { "one", "two" } } } };

            Entry v = mf.AsVocab<Entry> ();

            Assert.Equal ("one", v.Content);
        }

        [Fact]
        public void If_Property_IsScalar_And_NoValuesInMicroformat_SetTo_Null () {
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "likes", new object[] { (object)0} } ,{ "content", new string[] { } } } };

            Entry v = mf.AsVocab<Entry> ();

            Assert.Equal (string.Empty, v.Content);
            Assert.Equal(0, v.Likes);
        } 

        [Fact]
        public void Casts_Single_String_Media_Types () {
            string expectedValue = "/tmp/1.tmp";
            Microformat mf = new Microformat { Type = new [] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "bookmark-of", new[] { "http://google.com" } }, { "file", new [] { expectedValue } } } };

            Bookmark b = mf.AsVocab<Bookmark> ();

            Assert.NotNull(b.Files);
            Assert.True (b.Files[0] is File);
            Assert.Equal (expectedValue, ((File) b.Files[0]).Value);

        }

        [Fact]
        public void Casts_Complex_Media_Types()
        {
            string expectedValue = "/tmp/1.tmp";
            string expectedAlt = "tmp";
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "bookmark-of", new[] { "http://google.com" } }, { "file", new[] { new { value = expectedValue, alt = expectedAlt } } } } };

            Bookmark b = mf.AsVocab<Bookmark>();

            Assert.NotNull(b.Files);
            Assert.True(b.Files[0] is File);
            Assert.Equal(expectedValue, ((File)b.Files[0]).Value);
            Assert.Equal(expectedAlt, ((File)b.Files[0]).Alt);

        }

        [Fact]
        public void Casts_Mixed_Media_Types()
        {
            string expectedValue = "/tmp/1.tmp";
            string expectedValue2 = "/tmp/1.tmp";
            string expectedAlt = "tmp";
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "bookmark-of", new object[] { "http://google.com" } }, { "file", new object[] { expectedValue, new { value = expectedValue2, alt = expectedAlt } } } } };

            Bookmark b = mf.AsVocab<Bookmark>();

            Assert.NotNull(b.Files);
            Assert.True(b.Files[0] is File);
            Assert.Equal(expectedValue, ((File)b.Files[0]).Value);

            Assert.True(b.Files[1] is File);
            Assert.Equal(expectedValue2, ((File)b.Files[1]).Value);
            Assert.Equal(expectedAlt, ((File)b.Files[1]).Alt);
        }
    }
}