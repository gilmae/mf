using System.Collections.Generic;
namespace mf
{
    public record Document
    {
        public List<Microformat> Items { get; set; }
        public Dictionary<string, List<string>> Rels { get; set; }
        public Dictionary<string, Dictionary<string, object>> Rel_Urls { get; set; }

        public Document()
        {
            Items = new List<Microformat>();
            Rels = new Dictionary<string, List<string>>();
            Rel_Urls = new Dictionary<string, Dictionary<string, object>>();
        }
    }


}
