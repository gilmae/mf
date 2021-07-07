using System;
using System.Collections.Generic;

namespace mf
{
    public record Item
    {
        public string[] Type { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
