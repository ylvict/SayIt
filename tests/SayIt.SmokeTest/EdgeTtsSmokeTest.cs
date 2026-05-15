using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace SayIt.SmokeTest
{
    public class EdgeTtsSmokeTest
    {
        [Fact]
        public async Task SynthesizeShortText_ReturnsNonEmptyMp3()
        {
            EnsureNetwork();

            var audio = await SayIt.StreamAsync("Hello world, this is a smoke test.");

            Assert.NotNull(audio);
            Assert.True(audio.Length > 1000, $"Audio too small: {audio.Length} bytes");

            var header = new byte[4];
            var pos = audio.Position;
            audio.Position = 0;
            audio.ReadExactly(header);
            audio.Position = pos;

            Assert.Equal(0xFF, header[0]);
            Assert.True(header[1] >= 0xE0, $"Not a valid MP3 sync header: {header[1]:X2}");
        }

        [Fact]
        public async Task SynthesizeChineseText_ReturnsNonEmptyAudio()
        {
            EnsureNetwork();

            var audio = await SayIt.StreamAsync("你好世界，这是一个测试。");

            Assert.NotNull(audio);
            Assert.True(audio.Length > 1000, $"Audio too small: {audio.Length} bytes");
        }

        [Fact]
        public async Task StreamToFile_WritesValidFile()
        {
            EnsureNetwork();

            var path = Path.GetTempFileName() + ".mp3";
            try
            {
                await SayIt.SaveAsync("Test file output.", path);

                var file = new FileInfo(path);
                Assert.True(file.Exists);
                Assert.True(file.Length > 1000, $"File too small: {file.Length} bytes");
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public async Task ListVoices_ReturnsNonEmptyList()
        {
            EnsureNetwork();

            var voices = await SayIt.ListVoicesAsync();

            Assert.NotNull(voices);
            Assert.True(voices.Count > 10, $"Too few voices: {voices.Count}");

            var zhVoices = voices.Where(v => v.Locale.StartsWith("zh-")).ToList();
            Assert.True(zhVoices.Count > 0, "No Chinese voices found");
        }

        [Fact]
        public async Task CustomConfig_AppliesVoiceAndRate()
        {
            EnsureNetwork();

            var config = new SayItConfig()
                .WithVoice("en-US-JennyNeural")
                .WithRate("+50%");

            var audio = await SayIt.StreamAsync("Fast Jenny voice.", config);

            Assert.NotNull(audio);
            Assert.True(audio.Length > 1000);
        }

        [Fact]
        public async Task SynthesizeLongText_ReturnsAudio()
        {
            EnsureNetwork();

            var longText = string.Join(" ",
                Enumerable.Repeat("This is a sentence to test longer text synthesis.", 20));

            var audio = await SayIt.StreamAsync(longText);

            Assert.NotNull(audio);
            Assert.True(audio.Length > 5000, $"Audio too small for long text: {audio.Length} bytes");
        }

        [Fact]
        public async Task MultipleConcurrentRequests_AllSucceed()
        {
            EnsureNetwork();

            var tasks = Enumerable.Range(0, 5).Select(i =>
                SayIt.StreamAsync($"Concurrent request number {i}."));

            var results = await Task.WhenAll(tasks);

            Assert.All(results, r => Assert.True(r.Length > 1000));
        }

        private static void EnsureNetwork()
        {
            try
            {
                using var tcp = new TcpClient();
                tcp.Connect("speech.platform.bing.com", 443);
            }
            catch
            {
                Assert.True(false, "This test requires network access to speech.platform.bing.com:443");
            }
        }
    }
}
