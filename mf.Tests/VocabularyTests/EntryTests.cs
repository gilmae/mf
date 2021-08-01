using System;
using mf.vocabularies;
using Xunit;

namespace mf.Tests.VocabularyTests
{
    public class EntryTests
    {
        [Fact]
        public void IfNotA_Hentry_Throws_Exception()
        {
            Microformat mf = new Microformat { Type = new[] { "h-cite" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new[] { "test" } } } };
            Assert.Throws<ArgumentException>(() => mf.AsEntry());
        }

        [Fact]
        public void EntryHasName()
        {
            Microformat mf = new Microformat { Type = new[] { "h-entry" }, Properties = new System.Collections.Generic.Dictionary<string, object[]> { { "name", new[] { "test" } } } };

            Entry v = mf.AsEntry();

            Assert.Equal(new[] { "test" }, v.Name);
        }
    }
}
