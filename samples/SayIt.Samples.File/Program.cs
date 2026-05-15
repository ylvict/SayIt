using System;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.File
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = args.Length > 0 ? args[0] : "你好世界，欢迎使用 SayIt！";
            var path = args.Length > 1 ? args[1] : "output.mp3";

            Console.WriteLine($"Synthesizing: \"{text}\" -> {path}");
            await SayIt.SaveAsync(text, path);
            Console.WriteLine($"Done: {path}");
        }
    }
}
