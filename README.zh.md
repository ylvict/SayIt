# SayIt

零依赖 .NET 文字转语音 SDK，基于 Microsoft Edge 的免费在线语音服务。
无需 API Key、无需注册、无外部依赖。

> [**English Docs → README.md**](README.md)

[![NuGet](https://img.shields.io/nuget/v/SayIt)](https://www.nuget.org/packages/SayIt)
[![build](https://github.com/ylvict/SayIt/actions/workflows/ci.yml/badge.svg)](https://github.com/ylvict/SayIt/actions/workflows/ci.yml)

```shell
dotnet add package SayIt
```

## 🚀 快速开始

```csharp
using SayIt;

// 一行代码 — 文本转文件
await SayIt.SaveAsync("你好世界", "hello.mp3");

// 文本转音频流
await using var audio = await SayIt.StreamAsync("Hello world");

// 指定音色
await SayIt.SaveAsync("Bonjour", "greeting.mp3",
    new SayItConfig().WithVoice(VoiceId.FrFRDeniseNeural));
```

## ✨ 特性

- **零配置** — 无需 API Key、无需注册、无需账号
- **零外部依赖** — 仅使用 `System.Net.WebSockets` 和 `System.Text.Json`
- **400+ 神经音色**，覆盖 100+ 语言
- **跨平台** — .NET Standard 2.1
- **流式输出** — 输出到文件、流或管道
- **流式输入** — 以 `IAsyncEnumerable<string>` 形式喂入文本（如 LLM 流式输出）
- **可调节** — 控制语速、音高、音量、输出格式

## 📖 API

### ⚡ 静态 API（一行式）

| 方法 | 说明 |
|------|------|
| `SayIt.SaveAsync(text, path, config?, ct?)` | 合成文本到音频文件 |
| `SayIt.StreamAsync(text, config?, ct?)` | 合成文本到音频 `Stream` |
| `SayIt.StreamAsync(IAsyncEnumerable<string>, config?, ct?)` | 流式文本 → 音频流 |
| `SayIt.ListVoicesAsync(ct?)` | 列出所有可用音色 |

### 🔧 实例 API（可复用配置）

```csharp
var speaker = new SayItSpeaker(
    new SayItConfig()
        .WithVoice(VoiceId.ZhCNXiaoxiaoNeural)
        .WithRate("+20%")
        .WithPitch("-2st"));

await speaker.SaveAsync("你好", "hello.mp3");
await speaker.SaveAsync("世界", "world.mp3");
await speaker.StreamToAsync("嗨", httpResponse.Body);
await foreach (var chunk in speaker.StreamChunksAsync("测试"))
    // 处理每个音频块
```

### ⚙️ 配置

```csharp
var config = new SayItConfig()
    .WithVoice(VoiceId.ZhCNXiaoxiaoNeural) // 音色名称
    .WithRate("+30%")                        // 语速
    .WithPitch("-4st")                        // 音高
    .WithVolume("150%")                       // 音量
    .WithFormat(OutputFormat.Webm_24Khz_16Bit_Opus)
    .WithTimeout(60);                           // 超时秒数
```

### 🎤 内置音色

`VoiceId` 提供编译期安全的音色选择，覆盖 20 种语言的 65+ 个常用音色：

```csharp
var config = new SayItConfig()
    .WithVoice(VoiceId.ZhCNXiaoxiaoNeural)  // 中文（普通话）
    .WithVoice(VoiceId.EnUSJennyNeural)     // 英语（美国）
    .WithVoice(VoiceId.JaJPNanamiNeural)    // 日语
    .WithVoice(VoiceId.FrFRDeniseNeural);   // 法语
```

也支持通过字符串动态指定音色（例如从 `ListVoicesAsync()` 获取）：

```csharp
var voice = (await SayIt.ListVoicesAsync()).First();
var config = new SayItConfig().WithVoice(voice.ShortName);
```

> 完整 400+ 音色列表请参阅 [微软官方 TTS 音色文档](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts) 或运行时调用 `SayIt.ListVoicesAsync()`。

### 🎵 输出格式

- `OutputFormat.Mp3_24Khz_96Kbps`（默认）
- `OutputFormat.Mp3_24Khz_48Kbps`
- `OutputFormat.Mp3_48Khz_96Kbps`
- `OutputFormat.Webm_24Khz_16Bit_Opus`
- `OutputFormat.Pcm_16Khz_16Bit` / `Pcm_24Khz_16Bit` / `Pcm_48Khz_16Bit`
- `OutputFormat.Wav_16Khz_16Bit` / `Wav_24Khz_16Bit` / `Wav_48Khz_16Bit`

## 📁 示例项目

| 示例 | 说明 |
|------|------|
| [`SayIt.Samples.File`](samples/SayIt.Samples.File) | 基础文本 → 文件 |
| [`SayIt.Samples.Stream`](samples/SayIt.Samples.Stream) | 文本 → 音频流 |
| [`SayIt.Samples.StreamingInput`](samples/SayIt.Samples.StreamingInput) | `IAsyncEnumerable<string>` → 音频 |
| [`SayIt.Samples.VoiceList`](samples/SayIt.Samples.VoiceList) | 查询和筛选音色 |
| [`SayIt.Samples.AdvancedConfig`](samples/SayIt.Samples.AdvancedConfig) | 语速/音高/音量预设 |
| [`SayIt.Samples.Batch`](samples/SayIt.Samples.Batch) | 并发批量合成 |
| [`SayIt.Samples.Playback`](samples/SayIt.Samples.Playback) | 用 NAudio 播放音频（外部依赖） |

## 🔍 工作原理

SayIt 使用与 Microsoft Edge "朗读" 功能相同的 WebSocket API。
它通过模拟 Edge 浏览器的 User-Agent 连接到 `speech.platform.bing.com`，发送 SSML，
实时接收 MP3 音频块。无需 Azure 订阅或 API Key。

## 📄 许可

[MIT](LICENSE)
