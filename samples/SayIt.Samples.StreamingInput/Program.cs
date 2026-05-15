using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SayIt;

namespace SayIt.Samples.StreamingInput
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Streaming input demo: simulating LLM token output");
            Console.WriteLine();

            var audio = await SayIt.StreamAsync(SimulateLlmOutput());

            await using var file = File.Create("streaming_output.mp3");
            audio.Position = 0;
            await audio.CopyToAsync(file);

            Console.WriteLine("Done: streaming_output.mp3");
        }

        static async IAsyncEnumerable<string> SimulateLlmOutput()
        {
            var tokens = new[]
            {
                "Hello! ",
                "This is a streaming input demo. ",
                "Each sentence is synthesized separately ",
                "and combined into a single audio file. ",
                "SayIt handles each chunk sequentially."
            };

            foreach (var token in tokens)
            {
                Console.WriteLine($"  Input chunk: \"{token}\"");
                yield return token;
                await Task.Delay(100);
            }
        }
    }
}
