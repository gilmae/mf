using System;
using System.Text;
using System.Text.Json;

namespace mf.cli
{
    class Program
    {
        static void Main(string[] args)
        {
            string url;
            mf.Parser p = new mf.Parser();
            if (args.Length > 0)
            {
                url = args[0];
                var doc = p.Parse(new Uri(url));
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));

            }
        }
    }
}
