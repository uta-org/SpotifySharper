using System;
using SharpNeedle;
using SpotifySharper.Lib;
using System.Text;
using WatsonTcp;
using static SpotifySharper.Lib.SpotifyConsts;

using System.Drawing;
using Newtonsoft.Json;
using SpotifySharper.Lib.Model;
using Console = Colorful.Console;

namespace SpotifySharper.Shell
{
    internal class Program
    {
        private static TypeAdviser m_CurrentAdviser;

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

        //static void Main(string[] args)
        //{
        //    //bool runForever = true;
        //    //while (runForever)
        //    //{
        //    //    Console.Write("Command [q cls list send]: ");
        //    //    string userInput = Console.ReadLine();
        //    //    if (String.IsNullOrEmpty(userInput)) continue;

        //    //    List<string> clients;
        //    //    string ipPort;

        //    //    switch (userInput)
        //    //    {
        //    //        case "q":
        //    //            runForever = false;
        //    //            break;
        //    //        case "cls":
        //    //            Console.Clear();
        //    //            break;
        //    //        case "list":
        //    //            clients = server.ListClients();
        //    //            if (clients != null && clients.Count > 0)
        //    //            {
        //    //                Console.WriteLine("Clients");
        //    //                foreach (string curr in clients) Console.WriteLine("  " + curr);
        //    //            }
        //    //            else Console.WriteLine("None");
        //    //            break;
        //    //        case "send":
        //    //            Console.Write("IP:Port: ");
        //    //            ipPort = Console.ReadLine();
        //    //            Console.Write("Data: ");
        //    //            userInput = Console.ReadLine();
        //    //            if (String.IsNullOrEmpty(userInput)) break;
        //    //            server.Send(ipPort, Encoding.UTF8.GetBytes(userInput));
        //    //            break;
        //    //    }
        //    //}
        //}

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

            object @object;
            if (Extensions.IsJson(msg))
            {
                bool isAdviserDeserialized = false;
                try
                {
                    m_CurrentAdviser = JsonConvert.DeserializeObject<TypeAdviser>(msg);
                    isAdviserDeserialized = true;

                    Console.WriteLine($"Deserialized adviser of type: '{m_CurrentAdviser.TypeName}'!", Color.Lime);
                }
                catch (Exception ex)
                {
                    // This isn't a TypeAdviser object instance
                    // ... Ignore ...

                    Console.WriteLine(ex, Color.Red);
                }

                if (m_CurrentAdviser != null && !isAdviserDeserialized)
                {
                    @object = JsonConvert.DeserializeObject(msg, m_CurrentAdviser.ResolveType());

                    if (@object is ColoredMessage)
                    {
                        var coloredMsg = @object as ColoredMessage;
                        Console.WriteLine($"[Client #{ipPort}]: {coloredMsg.Message}", coloredMsg.Color);

                        return true;
                    }
                }
            }
            else
                Console.WriteLine($"[Client #{ipPort}]: {msg}");

            return true;
        }
    }
}