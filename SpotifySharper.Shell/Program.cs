using System;
using SharpNeedle;
using SpotifySharper.Lib;

using System.Drawing;
using Console = Colorful.Console;

namespace SpotifySharper.Shell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Injection Begin
            var spotifyProcess = Extensions.SpotifyProcess;
            PayloadInjector injector = new PayloadInjector(spotifyProcess,
                Environment.CurrentDirectory,
                "SharpDomain.dll",
                Environment.CurrentDirectory,
                "SpotifySharper.Injector.dll",
                string.Empty);

            try
            {
                injector.InjectAndForget();
                Console.WriteLine($"Successfully injected into Spotify (PID: {spotifyProcess?.Id} | Title: {spotifyProcess?.MainWindowTitle})", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex, Color.Red);
            }
            // Injection End

            Console.Read();
        }
    }
}