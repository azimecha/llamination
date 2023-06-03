using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111.APITypes {
    [JsonObject]
    public class SDModelItem {
        [JsonProperty("title")]
        public string Title;

        [JsonProperty("model_name")]
        public string ModelName;

        [JsonProperty("hash")]
        public string Hash;

        [JsonProperty("filename")]
        public string Filename;

        [JsonProperty("config")]
        public string ConfigFile;
    }
}
