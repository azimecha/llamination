# Azimecha.Llamination.LlamaListen

This is a sample speech recognition program. It takes a model path and input file path as arguments, and prints the recognized text sequences on standard output.

The input file needs to be mono (1 channel) PCM audio in 32-bit float format. For most models, the sample rate should be 16kHz. Use FFmpeg to convert your audio if needed:

```
ffmpeg -i <inputfile.wav> -ac 1 -ar 16000 -c:a pcm_f32le -f f32le <outputfile.raw>
```