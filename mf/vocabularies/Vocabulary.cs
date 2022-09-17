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
            Action<PropertyInfo, T, object, TypeConverter> SetValue = (PropertyInfo pi, T vocab, object v, TypeConverter converter) =>
            {
                if (v == null)
                {
                    pi.SetValue(vocab, v);
                }
                else if (converter == null)
                {
                    pi.SetValue(vocab, v);
                }
                else if (pi.PropertyType.IsArray || pi.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                {
                    if (v is IEnumerable)
                    {
                        int length = new List<object>(((IEnumerable<object>)v)).Count;
                        Array targetArr = Array.CreateInstance(pi.PropertyType.GetElementType(), length);

                        IEnumerator enumerator = ((IEnumerable)v).GetEnumerator();
                        int i = 0;
                        while (enumerator.MoveNext())
                        {
                            targetArr.SetValue(converter.ConvertTo(enumerator.Current, pi.PropertyType.GetElementType()), i);
                            i += 1;
                        } 

                        pi.SetValue(vocab, targetArr);
                    }
                    else
                    {
                        Array targetArr = Array.CreateInstance(pi.PropertyType.GetElementType(), 1);
                        targetArr.SetValue(converter.ConvertTo(v, pi.PropertyType.GetElementType()), 0);
                        pi.SetValue(vocab, targetArr);
                    }
                }
                else
                {
                    if (v is IEnumerable)
                    {
                        object v1 = null;
                        IEnumerator enumer = (v as IEnumerable).GetEnumerator();
                        if (enumer.MoveNext()) v1 = enumer.Current;

                        pi.SetValue(vocab, converter.ConvertTo(v1, pi.PropertyType));
                    }
                    else
                    {
                        pi.SetValue(vocab, converter.ConvertTo(v, pi.PropertyType));
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
                                SetValue(prop, vocab, convertedValues, null);
                            }
                        } else
                        {
                            if (prop.PropertyType == typeof(object[]))
                            {
                                SetValue(prop, vocab, v, null);
                            }
                            else {
                                Type targetType = prop.PropertyType;
                                if (prop.PropertyType.IsArray || prop.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                                {
                                    targetType = prop.PropertyType.GetElementType();
                                    
                                }
                                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                                SetValue(prop, vocab, v, converter);
                            }
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
