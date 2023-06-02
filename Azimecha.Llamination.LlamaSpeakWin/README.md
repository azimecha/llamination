# Azimecha.Llamination.LlamaSpeakWin

This is an example text-to-speech program using SAPI, the Windows speech API. It accepts text from the console until reaching EOF (ctrl-Z), and writes the audio to a file path given as an argument.

The raw audio data is dumped in mono (1 channel) 16 kHz PCM format with 32-bit float samples. To convert it to a MP3, use this FFmpeg command:

```
ffmpeg -ar 16000 -ac 1 -f f32le -i <rawoutput.raw> -c:a libmp3lame <audiofile.mp3>
```
