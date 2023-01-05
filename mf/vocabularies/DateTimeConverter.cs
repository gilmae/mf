using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace mf
{
    public class DateTimeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(object[]);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var handledTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTime[]), typeof(DateTime?[]) };
            return handledTypes.Contains(destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var orig = value as object[];
            if (destinationType == typeof(DateTime[]))
            {
                var dest = new DateTime[orig.Length];

                for (int i = 0; i < orig.Length; i++)
                {
                    dest[i] = DateTime.Parse(orig[i].ToString());
                }
                return dest;
            }

            return DateTime.Parse(orig[0].ToString());
        }
    }
}