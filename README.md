# SayIt

Zero-dependency .NET text-to-speech SDK powered by Microsoft Edge's free online voices.
No API keys, no registration, no external dependencies.

> [**中文文档 → README.zh.md**](README.zh.md)

```xml
<PackageReference Include="SayIt" Version="1.0.0" />
```

## Quick Start

```csharp
using SayIt;

// One line — text to file
await SayIt.SaveAsync("你好世界", "hello.mp3");

// Text to audio stream
await using var audio = await SayIt.StreamAsync("Hello world");

// Specify voice
await SayIt.SaveAsync("Bonjour", "greeting.mp3",
    new SayItConfig().WithVoice("fr-FR-DeniseNeural"));
```

## Features

- **Zero configuration** — no API keys, no sign-up, no account
- **Zero external dependencies** — only uses `System.Net.WebSockets` and `System.Text.Json`
- **400+ neural voices** across 100+ languages
- **Cross-platform** — .NET Standard 2.1
- **Streaming** — output to file, stream, or pipe
- **Streaming input** — feed text as `IAsyncEnumerable<string>` (e.g. LLM tokens)
- **Adjustable** — control rate, pitch, volume, output format

## API

### Static API (one-liners)

| Method | Description |
|--------|-------------|
| `SayIt.SaveAsync(text, path, config?, ct?)` | Synthesize text to audio file |
| `SayIt.StreamAsync(text, config?, ct?)` | Synthesize text to audio `Stream` |
| `SayIt.StreamAsync(IAsyncEnumerable<string>, config?, ct?)` | Stream text chunks → audio stream |
| `SayIt.ListVoicesAsync(ct?)` | List all available voices |

### Instance API (reusable config)

```csharp
var speaker = new SayItSpeaker(
    new SayItConfig()
        .WithVoice("en-US-JennyNeural")
        .WithRate("+20%")
        .WithPitch("-2st"));

await speaker.SaveAsync("Hello", "hello.mp3");
await speaker.SaveAsync("World", "world.mp3");
await speaker.StreamToAsync("Hi", httpResponse.Body);
await foreach (var chunk in speaker.StreamChunksAsync("Hi"))
    // process chunk
```

### Configuration

```csharp
var config = new SayItConfig()
    .WithVoice("zh-CN-XiaoxiaoNeural")     // Voice name
    .WithRate("+30%")                        // Speech rate
    .WithPitch("-4st")                        // Voice pitch
    .WithVolume("150%")                       // Volume
    .WithFormat(OutputFormat.Webm_24Khz_16Bit_Opus)
    .WithTimeout(60);                           // Timeout in seconds
```

### Output Formats

- `OutputFormat.Mp3_24Khz_96Kbps` (default)
- `OutputFormat.Mp3_24Khz_48Kbps`
- `OutputFormat.Mp3_48Khz_96Kbps`
- `OutputFormat.Webm_24Khz_16Bit_Opus`
- `OutputFormat.Pcm_16Khz_16Bit` / `Pcm_24Khz_16Bit` / `Pcm_48Khz_16Bit`
- `OutputFormat.Wav_16Khz_16Bit` / `Wav_24Khz_16Bit` / `Wav_48Khz_16Bit`

## Samples

| Sample | Description |
|--------|-------------|
| `SayIt.Samples.File` | Basic text → file |
| `SayIt.Samples.Stream` | Text → audio stream |
| `SayIt.Samples.StreamingInput` | `IAsyncEnumerable<string>` → audio |
| `SayIt.Samples.VoiceList` | Query and filter voices |
| `SayIt.Samples.AdvancedConfig` | Rate/pitch/volume presets |
| `SayIt.Samples.Batch` | Concurrent batch synthesis |
| `SayIt.Samples.Playback` | Play audio with NAudio (external) |

## How It Works

SayIt uses the same WebSocket API that powers Microsoft Edge's "Read Aloud" feature.
It connects to `speech.platform.bing.com` with an Edge-emulated User-Agent, sends SSML,
and receives MP3 audio chunks in real time. No Azure subscription or API key required.

## License

MIT
