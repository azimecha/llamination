using Azimecha.Llamination.ImageGeneration;
using System;
using System.Collections.Generic;

namespace Azimecha.Llamination.Client1111 {
    public class Automatic1111ImageGenerator : IStableDiffusionProvider, IRemoteImageGenerator {
        private Newtonsoft.Json.JsonSerializer _serJSON = new Newtonsoft.Json.JsonSerializer();
        private ISimpleGetPostProvider _provGetPost;
        private string _strBaseURL, _strCurSampler;

        public Automatic1111ImageGenerator(string strBaseURL) : this(strBaseURL, new DefaultGetPostProvider()) { }

        public Automatic1111ImageGenerator(string strBaseURL, ISimpleGetPostProvider provGetPost) {
            _strBaseURL = strBaseURL;
            _provGetPost = provGetPost;
        }

        public IGeneratedImage GenerateImage(string strPrompt, string strAdditionalNegative = null, int nSeed = -1) {
            APITypes.TextToImageParams param = new APITypes.TextToImageParams() {
                Prompt = strPrompt,
                Seed = nSeed,
                Steps = Steps,
                CfgScale = ConfigScale,
                ImageWidth = Width,
                ImageHeight = Height,
                SamplerName = _strCurSampler ?? null
            };

            if (string.IsNullOrEmpty(NegativePrompt))
                param.NegativePrompt = strAdditionalNegative ?? string.Empty;
            else if (string.IsNullOrEmpty(strAdditionalNegative))
                param.NegativePrompt = NegativePrompt ?? string.Empty;
            else
                param.NegativePrompt = $"{NegativePrompt}, {strAdditionalNegative}";

            APITypes.TextToImageResponse resp = CallAPIEndpoint<APITypes.TextToImageParams, APITypes.TextToImageResponse>("/sdapi/v1/txt2img", param);
            return new GeneratedImage(resp, ImageReader);
        }

        private class GeneratedImage : IGeneratedImage {
            private uint[] _arrPixelData;
            private FileFormats.IImageReader _readerImage;

            public GeneratedImage(APITypes.TextToImageResponse resp, FileFormats.IImageReader reader) {
                CompressedData = resp.CompressedImages[0];
                Width = resp.Parameters.ImageWidth ?? -1;
                Height = resp.Parameters.ImageHeight ?? -1;
                _readerImage = reader;
            }

            public byte[] CompressedData { get; private set; }

            public uint[] PixelData {
                get {
                    if (_arrPixelData is null) {
                        using (System.IO.MemoryStream stm = new System.IO.MemoryStream(CompressedData))
                            _arrPixelData = _readerImage.ReadImage(stm).Pixels;
                    }

                    return _arrPixelData;
                }
            }

            public int Width { get; private set; }
            public int Height { get; private set; }

            public void Dispose() {
                CompressedData = null;
            }
        }

        private TResult QueryAPIEndpoint<TResult>(string strRelURL) {
            using (System.IO.Stream stmResult = _provGetPost.PerformGetRequest(_strBaseURL + strRelURL))
                return Deserialize<TResult>(stmResult);
        }

        private void CallAPIEndpoint<TRequest>(string strRelURL, TRequest val)
            => _provGetPost.PerformPostRequest(_strBaseURL + strRelURL, Serialize(val));

        private TResult CallAPIEndpoint<TRequest, TResult>(string strRelURL, TRequest val) {
            using (System.IO.Stream stmResult = _provGetPost.PerformPostRequest(_strBaseURL + strRelURL, Serialize(val)))
                return Deserialize<TResult>(stmResult);
        }

        private static readonly System.Text.UTF8Encoding ENCODING_UTF8_NO_BOM = new System.Text.UTF8Encoding(false);

        private byte[] Serialize<TObject>(TObject val) {
            using (System.IO.MemoryStream stmMem = new System.IO.MemoryStream())
            using (System.IO.TextWriter writerText = new System.IO.StreamWriter(stmMem, ENCODING_UTF8_NO_BOM)) {
                _serJSON.Serialize(writerText, val);
                writerText.Flush();
                return stmMem.ToArray();
            }
        }

        private TResult Deserialize<TResult>(System.IO.Stream stmData) {
            using (System.IO.TextReader readerText = new System.IO.StreamReader(stmData))
            using (Newtonsoft.Json.JsonReader readerJSON = new Newtonsoft.Json.JsonTextReader(readerText))
                return _serJSON.Deserialize<TResult>(readerJSON);
        }

        public int Steps { get; set; } = 80;
        public int ConfigScale { get; set; } = 11;

        public IEnumerable<ISamplerInfo> Samplers 
            => WrapSamplers(QueryAPIEndpoint<APITypes.SamplerItem[]>("/sdapi/v1/samplers"));

        private IEnumerable<ISamplerInfo> WrapSamplers(APITypes.SamplerItem[] arrSamplers) {
            foreach (APITypes.SamplerItem infSamp in arrSamplers)
                yield return new Sampler(this, infSamp.Name);
        }

        private class Sampler : ISamplerInfo {
            private Automatic1111ImageGenerator _gen;
            private string _strName;

            public Sampler(Automatic1111ImageGenerator gen, string strName) {
                _gen = gen;
                _strName = strName;
            }

            public string Name => _strName;

            public void Select() {
                _gen._strCurSampler = _strName;
            }
        }

        public IEnumerable<IRemoteModel> Models 
            => WrapModels(QueryAPIEndpoint<APITypes.SDModelItem[]>("/sdapi/v1/sd-models"));

        private IEnumerable<IRemoteModel> WrapModels(APITypes.SDModelItem[] arrModels) {
            foreach (APITypes.SDModelItem infModel in arrModels)
                yield return new Model(this, infModel);
        }

        private class Model : IRemoteModel {
            private Automatic1111ImageGenerator _gen;
            private string _strFilename;

            public Model(Automatic1111ImageGenerator gen, APITypes.SDModelItem infModel) {
                _gen = gen;
                _strFilename = infModel.Filename;

                Name = infModel.ModelName;
                DisplayName = infModel.Title;
                
                try {
                    Hash = Convert.FromBase64String(infModel.Hash);
                } catch (Exception) {
                    Hash = EMPTY_HASH;
                }
            }

            private static readonly byte[] EMPTY_HASH = new byte[0];

            public string Name { get; private set; }
            public string DisplayName { get; private set; }
            public byte[] Hash { get; private set; }

            // uniquely, this function directly changes the config option
            // switching models takes a long time, so we don't want to do it for each image
            public void Select() 
                => _gen.CallAPIEndpoint("/sdapi/v1/options", new APITypes.FullOptions() { DiffusionModelCheckpoint = DisplayName });
        }

        private const int STEPS_FULL = 100;

        public float Quality {
            get => (float)Steps / STEPS_FULL;
            set => Steps = (int)(value * STEPS_FULL);
        }

        private const int CFG_SCALE_FULL = 12;

        public float Accuracy {
            get => (float)ConfigScale / CFG_SCALE_FULL;
            set => ConfigScale = (int)(value * CFG_SCALE_FULL); 
        }

        public int Width { get; set; } = 512;
        public int Height { get; set; } = 512;

        public string NegativePrompt { get; set; }

        private static readonly FileFormats.IImageReader DEFAULT_IMAGE_READER = new FileFormats.SmartImageReader();
        public FileFormats.IImageReader ImageReader { get; set; } = DEFAULT_IMAGE_READER;

        public void Dispose() {
            _serJSON = null;
            _provGetPost = null;
            _strBaseURL = null;
            _strCurSampler = null;
        }
    }
}
