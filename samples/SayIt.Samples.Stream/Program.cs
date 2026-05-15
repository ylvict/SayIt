using System;
using System.IO;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.Stream
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = args.Length > 0 ? args[0] : "Streaming audio from SayIt.";

            Console.WriteLine($"Streaming: \"{text}\"");

            await using var audio = await SayIt.StreamAsync(text);

            Console.WriteLine($"Received {audio.Length} bytes of audio");
            Console.WriteLine($"Format: {DetectFormat(audio)}");
        }

        static string DetectFormat(System.IO.Stream s)
        {
            var pos = s.Position;
            s.Position = 0;
            var buf = new byte[4];
            s.ReadExactly(buf);
            s.Position = pos;

            if (buf[0] == 0xFF && buf[1] == 0xFB) return "MP3";
            if (buf[0] == 0x52 && buf[1] == 0x49) return "WAV";
            if (buf[0] == 0x1A && buf[1] == 0x45) return "WebM";
            return "Unknown";
        }
    }
}
