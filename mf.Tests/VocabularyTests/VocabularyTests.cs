using System;
using Xunit;

namespace mf.Tests.VocabularyTests
{
    public class VocabularyTests
    {
        [Fact]
        public void If_MicroformatTypes_DoNotMatch_RequiredType_ThrowsArgumentException()
        {
            Microformat mf = new Microformat { Type = new[] { "h-cite" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new[] { "test" } } } };
            Assert.Throws<ArgumentException>(() => mf.AsVocab<Entry>());
        }

        [Fact]
        public void Populates_Properties()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new[] { "test" } } } };

            Entry v = mf.AsVocab<Entry>();

            Assert.Equal(new[] { "test" }, v.Name);
        }

        [Fact]
        public void Casts_ArrayOfString_Properties()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "summary", new[] { "test" } } } };

            Entry v = mf.AsVocab<Entry>();

            Assert.Equal(new string[] { "test" }, v.Summary);
        }

        [Fact]
        public void Casts_ArrayOfDateTime_Properties()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "published", new[] { "2021-01-01T00:00:00" } } } };

            Entry v = mf.AsVocab<Entry>();

            Assert.Equal(new DateTime[] { DateTime.Parse("2021-01-01T00:00:00") }, v.Published);
        }

        [Fact]
        public void If_Property_IsScalar_Takes_FirstElement()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "content", new[] { "one","two" } } } };

            Entry v = mf.AsVocab<Entry>();

            Assert.Equal("one", v.Content);
        }

        [Fact]
        public void If_Property_IsScalar_And_NoValuesInMicroformat_SetTo_Null ()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "content", new string[] {  } } } };

            Entry v = mf.AsVocab<Entry>();

            Assert.Equal(default(string), v.Content);
        }
    }
}
