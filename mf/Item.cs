using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("mf.Tests")]
namespace mf
{
    public record Microformat
    {
        [JsonPropertyName("id")]        
        public string Id { get; set; }
        
        [JsonPropertyName("type")]   
        public string[] Type { get; set; }
        
        [JsonPropertyName("properties")]   
        public Dictionary<string, object[]> Properties { get; set; }
        
        [JsonPropertyName("children")]   
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
                    mf.Properties[name] = mf.Properties[name].Append(property);
                }
            }
        }


    }

    public record Photo
    {
                
        [JsonPropertyName("alt")]   
        public string Alt { get; set; }
                
        [JsonPropertyName("value")]   
        public string Value { get; set; }
    }

    public record EmbeddedMarkup
    {
                
        [JsonPropertyName("html")]   
        public string Html { get; set; }
                
        [JsonPropertyName("value")]   
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

