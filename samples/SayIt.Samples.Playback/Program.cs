using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using SayIt;

namespace SayIt.Samples.Playback
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = args.Length > 0 ? args[0]
                : "Hello! This is SayIt playing audio through your speakers.";

            Console.WriteLine($"Synthesizing: \"{text}\"");

            await SayIt.SaveAsync(text, "playback_temp.mp3");

            Console.WriteLine("Playing...");
            using var reader = new AudioFileReader("playback_temp.mp3");
            using var output = new WaveOutEvent();
            output.Init(reader);
            output.Play();

            while (output.PlaybackState == PlaybackState.Playing)
                await Task.Delay(100);

            File.Delete("playback_temp.mp3");
            Console.WriteLine("Done.");
        }
    }
}
