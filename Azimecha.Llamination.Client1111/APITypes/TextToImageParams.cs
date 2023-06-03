using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111.APITypes {
    [JsonObject]
    internal class TextToImageParams {
        [JsonProperty("enable_hr", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EnableHR = false;

        [JsonProperty("denoising_strength", NullValueHandling = NullValueHandling.Ignore)]
        public float? DenoisingStrength = 0;

        [JsonProperty("firstphase_width", NullValueHandling = NullValueHandling.Ignore)]
        public int? FirstPhaseWidth = 0;

        [JsonProperty("firstphase_height", NullValueHandling = NullValueHandling.Ignore)]
        public int? FirstPhaseHeight = 0;

        [JsonProperty("prompt")]
        public string Prompt;

        [JsonIgnore]
        public static readonly string[] NO_STYLES = new string[0];

        [JsonProperty("styles", NullValueHandling = NullValueHandling.Ignore)]
        public string[] StylesToUse = NO_STYLES;

        [JsonProperty("seed", NullValueHandling = NullValueHandling.Ignore)]
        public int? Seed = -1;

        [JsonProperty("subseed", NullValueHandling = NullValueHandling.Ignore)]
        public int? Subseed = -1;

        [JsonProperty("subseed_strength", NullValueHandling = NullValueHandling.Ignore)]
        public float? SubseedStrength = 0;

        [JsonProperty("seed_resize_from_h", NullValueHandling = NullValueHandling.Ignore)]
        public int? SeedResizeFromHeight = -1;

        [JsonProperty("seed_resize_from_w", NullValueHandling = NullValueHandling.Ignore)]
        public int? SeedResizeFromWidth = -1;

        [JsonProperty("sampler_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SamplerName = "Euler";

        [JsonProperty("batch_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchSize = 1;

        [JsonProperty("n_iter", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchCount = 1;

        [JsonProperty("steps", NullValueHandling = NullValueHandling.Ignore)]
        public int? Steps = 50;

        [JsonProperty("cfg_scale", NullValueHandling = NullValueHandling.Ignore)]
        public float? CfgScale = 7;

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageWidth = 512;

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageHeight = 512;

        [JsonProperty("restore_faces", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RestoreFaces = false;

        [JsonProperty("tiling", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseTiling = false;

        [JsonProperty("negative_prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string NegativePrompt = string.Empty;

        [JsonProperty("eta", NullValueHandling = NullValueHandling.Ignore)]
        public float? Eta;

        [JsonProperty("s_churn", NullValueHandling = NullValueHandling.Ignore)]
        public float? SChurn;

        [JsonProperty("s_tmax", NullValueHandling = NullValueHandling.Ignore)]
        public float? STMax;

        [JsonProperty("s_tmin", NullValueHandling = NullValueHandling.Ignore)]
        public float? STMin;

        [JsonProperty("s_noise", NullValueHandling = NullValueHandling.Ignore)]
        public float? SNoise;

        [JsonProperty("override_settings", NullValueHandling = NullValueHandling.Ignore)]
        public FullOptions SettingOverrides;

        [JsonProperty("override_settings_restore_afterwards", NullValueHandling = NullValueHandling.Ignore)]
        public bool RevertOverridesAutomatically = true;

        [JsonProperty("sampler_index", NullValueHandling = NullValueHandling.Ignore)]
        public string SamplerIndex; // not required
    }
}
