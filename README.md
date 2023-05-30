# Llamination AI Library

This library allows .NET code to easily interface with llama.cpp and whisper.cpp, as well as other speech-to-text, text generation, and text-to-speech libraries. 

### Principles

This library is designed to:
 - Make high-level use of ML easy (you don't need to know how neurons work to use it)
 - Be at least as compatible as llama.cpp and whisper.cpp (you don't need windows 11 or a 4090 to use it)
 - Allow somewhat lower-level access for those who want it
 - Remain as fast as possible while still observing the previous points
 
## System requirements

.NET Framework 2.0+ and .NET Standard 2.0+ are both supported. (The second one includes .NET Core 3+ and .NET 5+).

Because SAPI is part of Windows, the SAPI library only works there. Everything else is cross platform.

## Usage instructions

Since AI moves fast, this library is intended to be used as a Git submodule. 

1. Add the submodule to your project. If you reject modern technology, unzip it to a folder in your solution folder.
2. Use *File* > *Add* > *Existing Project...* to add any projects you want to use to your solution.
3. Use *Project* > *Add Reference...* to reference the projects you want to use. Ehh if I have to tell you this you have bigger problems...
4. After you build, put compiled versions (.dll/.so) of libllama and libwhisper next to your binary.
5. Don't forget that you need a model lol

If you're using linux, figure out the commands yourself (this is the way of linux).
