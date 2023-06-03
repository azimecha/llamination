using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111.APITypes {
    [JsonObject]
    internal class SamplerItem {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("aliases")]
        public string[] Aliases;

        [JsonProperty("options")]
        public Dictionary<string, string> Options = new Dictionary<string, string>();
    }
}
