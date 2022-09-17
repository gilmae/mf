using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using mf;

namespace mf.Tests.VocabularyTests {

    [Vocab (MustBeType = new [] { "h-entry" })]
    public record Bookmark {
        [Property (Name = "bookmark-of")]
        public string[] Bookmarks { get; set; }

        [TypeConverter (typeof (FileConverter))]
        [Property(Name = "file")]
        public File[] Files { get; set; }
    }

    public record File {
        public string Value { get; set; }
        public string Alt { get; set; }
    }

    public class FileConverter : TypeConverter {
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof (object[]);
        }

        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof (File[]);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            var orig = value as object[];
            var dest = new File[orig.Length];

            for (int i = 0; i < orig.Length; i++) {
                File f = new File ();
                if (orig[i].GetType () == typeof (string)) {
                    f.Value = (string) orig[i];
                } else if (orig[i].GetType () == typeof (JsonElement)) {
                    if (((JsonElement) orig[i]).ValueKind == JsonValueKind.String) {
                        f.Value = ((JsonElement) orig[i]).GetString ();
                    }
                } else {
                    dynamic o = (dynamic) orig[i];
                    f.Value = o.value;
                    f.Alt = o.alt;
                }
                dest[i] = f;
            }
            return dest;
        }
    }
}