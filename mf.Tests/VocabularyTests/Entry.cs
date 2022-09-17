using System;
using System.ComponentModel;
using System.Globalization;

namespace mf.Tests.VocabularyTests
{
    [Vocab(MustBeType = new[] {"h-entry"})]
    public record Entry
    {
        [Property(Name="name")]
        public object[] Name { get; set; }

        [Property(Name = "summary")]
        public string[] Summary { get; set; }

        [Property(Name = "content")]
        public string Content { get; set; }

        [Property(Name="likes")]
        public int Likes { get; set; }

        [Property(Name = "published")]
        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime[] Published { get; set; }

        [Property(Name = "updated")]
        public object[] Updated { get; set; }

        [Property(Name = "author")]
        public object[] Author { get; set; }

        [Property(Name = "category")]
        public object[] Category { get; set; }

        [Property(Name = "url")]
        public object[] Url { get; set; }

        [Property(Name = "uid")]
        public object[] Uid { get; set; }

        [Property(Name = "name")]
        public object[] Geo { get; set; }

        [Property(Name = "geo")]
        public object[] Latitude { get; set; }

        [Property(Name = "longitude")]
        public object[] Longitude { get; set; }
    }

    public class DateTimeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(object[]);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(DateTime[]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var orig = value as object[];
            var dest = new DateTime[orig.Length];

            for (int i = 0;i<orig.Length;i++)
            {
                dest[i] = DateTime.Parse(orig[i].ToString());
            }
            return dest;
        }
    }
}
