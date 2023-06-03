using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111.APITypes {
    [JsonObject]
    internal class TextToImageResponse {
        [JsonProperty("images")]
        public byte[][] CompressedImages;

        [JsonProperty("parameters")]
        public TextToImageParams Parameters;

        [JsonProperty("info")]
        public string InfoString;
    }
}
