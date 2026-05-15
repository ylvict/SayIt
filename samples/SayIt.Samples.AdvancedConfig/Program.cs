using System;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.AdvancedConfig
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = args.Length > 0 ? args[0]
                : "Welcome to SayIt! You can control rate, pitch, and volume.";

            var normal = new SayItConfig()
                .WithVoice("en-US-JennyNeural");

            var fast = normal.WithRate("+50%");

            var slowDeep = normal
                .WithRate("-30%")
                .WithPitch("-4st");

            var whisper = normal
                .WithRate("-20%")
                .WithVolume("30%");

            Console.WriteLine("1. Normal rate -> normal.mp3");
            await SayIt.SaveAsync(text, "normal.mp3", normal);

            Console.WriteLine("2. Fast (+50%) -> fast.mp3");
            await SayIt.SaveAsync(text, "fast.mp3", fast);

            Console.WriteLine("3. Slow & deep (-30%, -4st) -> slow_deep.mp3");
            await SayIt.SaveAsync(text, "slow_deep.mp3", slowDeep);

            Console.WriteLine("4. Whisper (-20%, 30% volume) -> whisper.mp3");
            await SayIt.SaveAsync(text, "whisper.mp3", whisper);

            Console.WriteLine("All done! Compare the files.");
        }
    }
}
