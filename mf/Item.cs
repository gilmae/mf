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
        public List<Microformat> Children { get; set; }

        internal bool HasPProperties { get; set; }
        internal bool HasUProperties { get; set; }
        internal bool HasEProperties { get; set; }
        internal bool HasNestedMicroformats { get; set; }

        public Microformat()
        {
            Properties = new Dictionary<string, object[]>();
            Children = new List<Microformat>();
        }
    }

    public static class MicroformatExtensions
    {
        public static void AddProperty(this Microformat mf, string name, object property)
        {
            if (mf.Properties == null)
            {
                mf.Properties = new Dictionary<string, object[]> {
                    { name, new object[] { property } }
                };
            }
            else
            {
                if (!mf.Properties.ContainsKey(name))
                {
                    mf.Properties[name] = new object[] { property };
                }
                else
                {
                    mf.Properties[name].Append(property);
                }

            }
        }
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

