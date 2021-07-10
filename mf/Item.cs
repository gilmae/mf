using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("mf.Tests")]
namespace mf
{
    public record Microformat
    {
        public string Id { get; set; }
        public string[] Type { get; set; }
        public Dictionary<string, object[]> Properties { get; set; }

        internal bool HasPProperties { get; set; }
        internal bool HasUProperties { get; set; }
        internal bool HasEProperties { get; set; }
        internal bool HasNestedMicroformats { get; set; }
    }

    public record Photo
    {
        public string Alt { get; set; }
        public string Value { get; set; }
    }

    public record EmbeddedMarkup
    {
        public string Html { get; set; }
        public string Value { get; set; }
    }

    public static class PhotoExtensions
    {
        public static bool AreEquivalent(this Photo photo, Photo other)
        {
            return photo.Alt == other.Alt && photo.Value == other.Value;
        }
    }
}

