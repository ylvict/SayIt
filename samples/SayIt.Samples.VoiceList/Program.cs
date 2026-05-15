using System;
using System.Linq;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.VoiceList
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var voices = await SayIt.ListVoicesAsync();

            Console.WriteLine($"Available voices: {voices.Count}");

            var filter = args.Length > 0 ? args[0] : "zh-CN";

            var filtered = voices
                .Where(v => v.Locale.StartsWith(filter))
                .ToList();

            Console.WriteLine($"Voices matching locale '{filter}': {filtered.Count}");
            Console.WriteLine();

            foreach (var voice in filtered)
            {
                Console.WriteLine(
                    $"  {voice.ShortName,-40} {voice.Gender,-7} {voice.LocalName}");
            }
        }
    }
}
