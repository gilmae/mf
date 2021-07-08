using System;
using System.Collections.Generic;

namespace mf
{
    public record Item
    {
        public string[] Type { get; set; }
        public Dictionary<string, object[]> Properties { get; set; }
    }

    public record Photo
    {
        public string Alt { get; set; }
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

