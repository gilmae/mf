using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace mf
{
    public record Document
    {
        
        [JsonPropertyName("properties")]   
        public List<Microformat> Items { get; set; }
                
        [JsonPropertyName("rels")]   
        public Dictionary<string, List<string>> Rels { get; set; }
                
        [JsonPropertyName("rel-urls")]   
        public Dictionary<string, Dictionary<string, object>> Rel_Urls { get; set; }

        public Document()
        {
            Items = new List<Microformat>();
            Rels = new Dictionary<string, List<string>>();
            Rel_Urls = new Dictionary<string, Dictionary<string, object>>();
        }
    }


}
