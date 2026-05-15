using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SayIt.Internal;

namespace SayIt
{
    public sealed class SayItSpeaker
    {
        private readonly SayItConfig _config;

        public SayItSpeaker()
            : this(SayItConfig.Default)
        {
        }

        public SayItSpeaker(SayItConfig config)
        {
            _config = config ?? SayItConfig.Default;
        }

        public SayItSpeaker WithConfig(System.Func<SayItConfig, SayItConfig> configure)
        {
            return new SayItSpeaker(configure(_config));
        }

        public async Task<Stream> StreamAsync(
            string text, CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);
            var ssml = SsmlBuilder.Build(
                text, _config.VoiceName,
                _config.Rate, _config.Pitch, _config.Volume);

            using var conn = new WsConnection();
            await conn.ConnectAsync(ct);
            await conn.SendConfigAsync(_config.Format, ct);
            await conn.SendSsmlAsync(ssml, ct);

            var audio = await conn.ReceiveAllAudioAsync(ct);
            return new MemoryStream(audio);
        }

        public async Task StreamToAsync(
            string text, Stream destination, CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);
            var ssml = SsmlBuilder.Build(
                text, _config.VoiceName,
                _config.Rate, _config.Pitch, _config.Volume);

            using var conn = new WsConnection();
            await conn.ConnectAsync(ct);
            await conn.SendConfigAsync(_config.Format, ct);
            await conn.SendSsmlAsync(ssml, ct);

            await foreach (var chunk in conn.ReceiveAudioChunksAsync(ct))
                destination.Write(chunk, 0, chunk.Length);
        }

        public async IAsyncEnumerable<byte[]> StreamChunksAsync(
            string text,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);
            var ssml = SsmlBuilder.Build(
                text, _config.VoiceName,
                _config.Rate, _config.Pitch, _config.Volume);

            using var conn = new WsConnection();
            await conn.ConnectAsync(ct);
            await conn.SendConfigAsync(_config.Format, ct);
            await conn.SendSsmlAsync(ssml, ct);

            await foreach (var chunk in conn.ReceiveAudioChunksAsync(ct))
                yield return chunk;
        }

        public async IAsyncEnumerable<byte[]> StreamChunksAsync(
            IAsyncEnumerable<string> textStream,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);

            await foreach (var text in textStream.WithCancellation(ct))
            {
                var ssml = SsmlBuilder.Build(
                    text, _config.VoiceName,
                    _config.Rate, _config.Pitch, _config.Volume);

                using var conn = new WsConnection();
                await conn.ConnectAsync(ct);
                await conn.SendConfigAsync(_config.Format, ct);
                await conn.SendSsmlAsync(ssml, ct);

                await foreach (var chunk in conn.ReceiveAudioChunksAsync(ct))
                    yield return chunk;
            }
        }

        public Task<Stream> StreamAsync(
            IAsyncEnumerable<string> textStream, CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);
            return Task.Run(async () =>
            {
                var pipe = new MemoryStream();
                await foreach (var chunk in StreamChunksAsync(textStream, ct))
                    pipe.Write(chunk, 0, chunk.Length);
                pipe.Position = 0;
                return (Stream)pipe;
            }, ct);
        }

        public async Task SaveAsync(
            string text, string path, CancellationToken ct = default)
        {
            ct = ApplyTimeout(ct);
            var stream = await StreamAsync(text, ct);
            await using var file = File.Create(path);
            stream.Position = 0;
            await stream.CopyToAsync(file, 81920, ct);
        }

        private CancellationToken ApplyTimeout(CancellationToken ct)
        {
            if (ct != default)
                return ct;

            var timeoutCts = new CancellationTokenSource(_config.TimeoutSeconds * 1000);
            return timeoutCts.Token;
        }
    }
}
