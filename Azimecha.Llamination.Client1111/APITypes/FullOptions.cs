using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111.APITypes {
    [JsonObject]
    internal class FullOptions {
        [JsonProperty("samples_save", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveAllImages;

        [JsonProperty("samples_format", NullValueHandling = NullValueHandling.Ignore)]
        public string SaveFormat;

        [JsonProperty("samples_filename_pattern", NullValueHandling = NullValueHandling.Ignore)]
        public string SavedFilenamePattern;

        [JsonProperty("grid_save", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveAllGrids;

        [JsonProperty("grid_format", NullValueHandling = NullValueHandling.Ignore)]
        public string GridSaveFormat;

        [JsonProperty("grid_extended_filename", NullValueHandling = NullValueHandling.Ignore)]
        public string UseExtendedGridFilename;

        [JsonProperty("grid_only_if_multiple", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoGridIfSingleImage;

        [JsonProperty("grid_prevent_empty_slots", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PreventEmptyGridSlots;

        [JsonProperty("n_rows", NullValueHandling = NullValueHandling.Ignore)]
        public int? GridRowCount;

        [JsonProperty("enable_pnginfo", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludePNGInfo;

        [JsonProperty("save_txt", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveParametersInTextFile;

        [JsonProperty("save_images_before_face_restoration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveCopyBeforeFaceRestore;

        [JsonProperty("save_images_before_highres_fix", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveCopyBeforeHiResFix;

        [JsonProperty("save_images_before_color_correction", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveCopyBeforeColorCorrection;

        [JsonProperty("jpeg_quality", NullValueHandling = NullValueHandling.Ignore)]
        public byte? JpegQualityPercent;

        [JsonProperty("export_for_4chan", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CreateDownsizedJPEGForLargeImages;

        [JsonProperty("use_original_name_batch", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseOriginalFilenameDuringBatch;

        [JsonProperty("use_upscaler_name_as_suffix", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AppendUpscalerName;

        [JsonProperty("save_selected_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveButtonSavesOnlyOne;

        [JsonProperty("do_not_add_watermark", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableWatermark;

        [JsonProperty("temp_dir", NullValueHandling = NullValueHandling.Ignore)]
        public string TempImageFolder;

        [JsonProperty("clean_temp_dir_at_start", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CleanTempImageFolderOnStartup;

        [JsonProperty("outdir_samples", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageSaveFolderOverride;

        [JsonProperty("outdir_txt2img_samples", NullValueHandling = NullValueHandling.Ignore)]
        public string TextToImageSaveFolder;

        [JsonProperty("outdir_img2img_samples", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageToImageSaveFolder;

        [JsonProperty("outdir_extras_samples", NullValueHandling = NullValueHandling.Ignore)]
        public string ExtrasSaveFolder;

        [JsonProperty("outdir_grids", NullValueHandling = NullValueHandling.Ignore)]
        public string GridSaveFolderOverride;

        [JsonProperty("outdir_txt2img_grids", NullValueHandling = NullValueHandling.Ignore)]
        public string TextToImageGridSaveFolder;

        [JsonProperty("outdir_img2img_grids", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageToImageGridSaveFolder;

        [JsonProperty("outdir_save", NullValueHandling = NullValueHandling.Ignore)]
        public string SaveButtonOutputFolder;

        [JsonProperty("save_to_dirs", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveImagesToSubdirectories;

        [JsonProperty("grid_save_to_dirs", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveGridsToSubdirectories;

        [JsonProperty("use_save_to_dirs_for_ui", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveButtonUsesSubdirectories;

        [JsonProperty("directores_filename_pattern", NullValueHandling = NullValueHandling.Ignore)]
        public string SubdirectoryNamePattern;

        [JsonProperty("directories_max_prompt_words", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxWordsForDirectoryNames;

        [JsonProperty("ESRGAN_tile", NullValueHandling = NullValueHandling.Ignore)]
        public int? ESRGANTileSize;

        [JsonProperty("ESRGAN_tile_overlap", NullValueHandling = NullValueHandling.Ignore)]
        public int? ESRGANTileOverlap;

        [JsonProperty("realesrgan_enabled_models", NullValueHandling = NullValueHandling.Ignore)]
        public string[] EnabledESRGANModels;

        [JsonProperty("upscaler_for_img2img", NullValueHandling = NullValueHandling.Ignore)]
        public string UpscalerForImageToImage;

        [JsonProperty("use_scale_latent_for_hires_fix", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseScaleLatentForHiResFix;

        [JsonProperty("ldsr_steps", NullValueHandling = NullValueHandling.Ignore)]
        public int? LDSRStepCount;

        [JsonProperty("ldsr_cached", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CacheLDSRModel;

        [JsonProperty("SWIN_tile", NullValueHandling = NullValueHandling.Ignore)]
        public int? SWINTileSize;

        [JsonProperty("SWIN_tile_overlap", NullValueHandling = NullValueHandling.Ignore)]
        public int? SWINTileOverlap;

        [JsonProperty("face_restoration_model", NullValueHandling = NullValueHandling.Ignore)]
        public string FaceRestorationModel;

        [JsonProperty("code_former_weight", NullValueHandling = NullValueHandling.Ignore)]
        public float? CodeFormerWeight;

        [JsonProperty("face_restoration_unload", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoUnloadFaceRestorationModel;

        [JsonProperty("memmon_poll_rate", NullValueHandling = NullValueHandling.Ignore)]
        public float? VRAMLoadPollRate;

        [JsonProperty("samples_log_stdout", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AlwaysPrintInfoToConsole;

        [JsonProperty("multiple_tqdm", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowFullJobProgressBar;

        [JsonProperty("unload_models_when_training", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UnloadModelsWhenTraining;

        [JsonProperty("pin_memory", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EnableLoaderMemoryPinning;

        [JsonProperty("save_optimizer_state", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveOptimizerTrainingState;

        // todo: add the rest (boring :eyeroll:)

        [JsonProperty("sd_model_checkpoint", NullValueHandling = NullValueHandling.Ignore)]
        public string DiffusionModelCheckpoint;

        [JsonProperty("sd_checkpoint_cache", NullValueHandling = NullValueHandling.Ignore)]
        public int? DiffusionCheckpointsToCache;

        [JsonProperty("sd_vae", NullValueHandling = NullValueHandling.Ignore)]
        public string DiffusionVAE;

        [JsonProperty("sd_vae_as_default", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreDiffusionVAEIfCheckpointHasOwn;

        [JsonProperty("sd_hypernetwork", NullValueHandling = NullValueHandling.Ignore)]
        public string DiffusionHypernetwork;

        [JsonProperty("sd_hypernetwork_strength", NullValueHandling = NullValueHandling.Ignore)]
        public float? DiffusionHypernetworkStrength;
    }
}
