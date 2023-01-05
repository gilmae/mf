using System;
using System.IO;
using System.Text.Json;

namespace mf.cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();

            if (args.Length > 0)
            {
                if (Uri.IsWellFormedUriString(args[0], UriKind.Absolute) ){
                    var doc = p.Parse(new Uri(args[0]));
                    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
                } else if (File.Exists(args[0]))
                {
                    string html = File.ReadAllText(args[0]);
                    Uri uri = new Uri("http://test.org");
                    if (args.Length > 1 && Uri.IsWellFormedUriString(args[1], UriKind.RelativeOrAbsolute))
                    {
                        uri = new Uri(args[1]);
                    }
                    var doc = p.Parse(html,uri);
                    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions {DefaultIgnoreCondition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, WriteIndented = true,  }));

                }
            }
        }
    }
}
