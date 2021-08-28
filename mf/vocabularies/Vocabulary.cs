using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace mf
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class VocabAttribute : Attribute
    {
        public string[] MustBeType { get; set; }
    }

    public abstract class PropertyConvertor
    {
        public abstract T ConvertTo<T>(object[] data);
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {

    }

    public static class VocabularExtensions
    {
        public static T AsVocab<T>(this Microformat item)
         {
            Action<PropertyInfo, T, object > SetValue = (PropertyInfo pi, T vocab, object v) => {
                if (pi.PropertyType.IsArray || pi.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                {
                    if (v is IEnumerable)
                    {
                        pi.SetValue(vocab, v);
                    } else
                    {
                        pi.SetValue(vocab, new object[] { v });
                    }
                }
                else
                {
                    if (v is IEnumerable)
                    {
                        object v1 = null;
                        IEnumerator enumer = (v as IEnumerable).GetEnumerator();
                        if (enumer.MoveNext()) v1 = enumer.Current;
                        
                        pi.SetValue(vocab, v1);
                    }
                    else
                    {
                        pi.SetValue(vocab, v);
                    }
                    
                }
            };

            Type vocabType = typeof(T);

            var vocabAttribute = vocabType.GetCustomAttributes(true).SingleOrDefault(ac => ac is VocabAttribute) as VocabAttribute;

            if (vocabAttribute == null)
            {
                throw new ArgumentException("Missing VocabAttribute");
            }

            if (vocabAttribute.MustBeType.Any() && !vocabAttribute.MustBeType.Intersect(item.Type).Any())
            {
                throw new ArgumentException("Mismatched Vocabulary types");
            }

            T vocab = Activator.CreateInstance<T>();

            foreach (var prop in vocabType.GetProperties())
            {
                var propAttr = prop.GetCustomAttributes(true).SingleOrDefault(a => a is PropertyAttribute) as PropertyAttribute;
                if (propAttr != null)
                {
                    if ( item.Properties.TryGetValue(propAttr.Name, out var v))
                    {
                        var converterAttribute = prop.GetCustomAttributes(true).SingleOrDefault(a => a is TypeConverterAttribute) as TypeConverterAttribute;
                        if (converterAttribute != null)
                        {
                            var converterTypeName = converterAttribute.ConverterTypeName;
                            var converterType = Type.GetType(converterTypeName);
                            var converter = (TypeConverter)Activator.CreateInstance(converterType);

                            if (converter.CanConvertFrom(typeof(object[])) && converter.CanConvertTo(prop.PropertyType))
                            {
                                var convertedValues = converter.ConvertTo(v, prop.PropertyType);
                                SetValue(prop, vocab, convertedValues);
                            }
                        } else
                        {
                            SetValue(prop, vocab, v);
                        }
                    }
                }

                var idAttr = prop.GetCustomAttributes(true).SingleOrDefault(a => a is IdAttribute) as PropertyAttribute;
                if (idAttr != null)
                {
                    prop.SetValue(vocab, item.Id);
                }

            }
            return vocab;
        }
    }
}
