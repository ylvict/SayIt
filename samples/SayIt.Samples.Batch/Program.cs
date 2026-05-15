using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.Batch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sentences = new[]
            {
                "First sentence of the batch.",
                "Second sentence follows right after.",
                "Third one is here to demonstrate concurrency.",
                "And the fourth to wrap things up nicely."
            };

            var sw = Stopwatch.StartNew();

            var tasks = new Task[sentences.Length];
            for (int i = 0; i < sentences.Length; i++)
            {
                var index = i;
                tasks[i] = Task.Run(async () =>
                {
                    var path = $"batch_{index + 1}.mp3";
                    Console.WriteLine($"Starting #{index + 1}");
                    await SayIt.SaveAsync(sentences[index], path);
                    Console.WriteLine($"Finished #{index + 1}: {path}");
                });
            }

            await Task.WhenAll(tasks);

            sw.Stop();
            Console.WriteLine($"All {sentences.Length} files generated in {sw.Elapsed.TotalSeconds:F1}s");
        }
    }
}
