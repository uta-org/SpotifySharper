using System;
using SharpNeedle;
using SpotifySharper.Lib;
using System.Text;
using WatsonTcp;
using static SpotifySharper.Lib.SpotifyConsts;

using System.Drawing;
using Colorful;
using Newtonsoft.Json;
using SpotifySharper.Lib.Model;
using Console = Colorful.Console;

namespace SpotifySharper.Shell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Start Socket Server Initialization
            WatsonTcpServer server = new WatsonTcpServer(SERVER, Port)
            {
                ClientConnected = ClientConnected,
                ClientDisconnected = ClientDisconnected,
                MessageReceived = MessageReceived,
                Debug = false
            };

            server.Start();
            // End Socket Server Initialization

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
                Console.WriteLine($"Successfully injected into Spotify (PID: {spotifyProcess?.Id} | Title: {spotifyProcess?.MainWindowTitle})!", Color.Lime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex, Color.Red);
            }
            // Injection End

            Console.Read();
        }

        private static bool ClientConnected(string ipPort)
        {
            Console.WriteLine("Client connected: " + ipPort);
            return true;
        }

        private static bool ClientDisconnected(string ipPort)
        {
            Console.WriteLine("Client disconnected: " + ipPort);
            return true;
        }

        private static bool MessageReceived(string ipPort, byte[] data)
        {
            string msg = "";

            if (data != null && data.Length > 0)
                msg = Encoding.UTF8.GetString(data);

            // Console.WriteLine(msg, Color.Aqua);

            object @object;
            if (Extensions.IsBasicJson(msg))
            {
                string[] lines = msg.GetLines();
                TypeAdviser currentAdviser;

                // Console.WriteLine($"Length: {lines.Length}");

                string line0 = lines[0],
                       line1 = lines[1];
                if (!(lines.Length == 2 && Extensions.IsJson(line0) && Extensions.IsJson(line1)))
                {
                    Console.WriteLine($"Couldn't complete resolving advised type! (Line0: '{lines[0]}' || Line1: '{lines[1]}')", Color.Red);
                    return false;
                }

                try
                {
                    currentAdviser = JsonConvert.DeserializeObject<TypeAdviser>(line0);
                    @object = JsonConvert.DeserializeObject(line1, currentAdviser.ResolveType());

                    if (@object is ColoredMessage)
                    {
                        string caption = $"[Client #{ipPort}]: {{0}}";
                        var coloredMsg = @object as ColoredMessage;
                        Console.WriteLineFormatted(caption, coloredMsg.Color, Console.ForegroundColor, coloredMsg.Message);

                        return true;
                    }

                    Console.WriteLine($"Unsupported type: '{line1}' to deserialize!", Color.Red);
                    return false;

                    // Console.WriteLine($"Deserialized adviser of type: '{m_CurrentAdviser.TypeName}'!", Color.Lime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex, Color.Red);
                    return false;
                }
            }

            Console.WriteLine($"[Client #{ipPort}]: {msg}");
            return true;
        }
    }
}