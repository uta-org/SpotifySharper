using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using WatsonTcp;
using static SpotifySharper.Lib.SpotifyConsts;

namespace SpotifySharper.Injector
{
    using Extensions;
    using Lib;
    using Lib.Model;

    public class Main
    {
        public static ConcurrentQueue<object> ObjectQueue { get; private set; }

        // we use a default injection method name in order to execute our code in the remote process
        [STAThread]
        public static void Run()
        {
            ObjectQueue = new ConcurrentQueue<object>();

            //Console.WriteLine("Hello world from the injected process!");
            // MessageBox.Show("Test");

            // SpotifyPatchAds.PatchAds();

            // Task.Factory.StartNew(InitSocketClient);

            InitSocketClient();
        }

        private static void InitSocketClient()
        {
            // MessageBox.Show("Test");

            WatsonTcpClient client = new WatsonTcpClient(SERVER, Port)
            {
                ServerConnected = ServerConnected,
                ServerDisconnected = ServerDisconnected,
                MessageReceived = MessageReceived,
                Debug = false
            };

            client.Start();

            bool runForever = true;
            do
            {
                // Client.Send(DateTime.Now.ToString());

                //Console.Write("Command [q cls send auth]: ");
                //string userInput = Console.ReadLine();
                //if (string.IsNullOrEmpty(userInput)) continue;

                //switch (userInput)
                //{
                //    case "q":
                //        runForever = false;
                //        break;

                //    case "cls":
                //        Console.Clear();
                //        break;

                //    case "send":
                //        Console.Write("Data: ");
                //        userInput = Console.ReadLine();
                //        if (String.IsNullOrEmpty(userInput)) break;
                //        client.Send(Encoding.UTF8.GetBytes(userInput));
                //        break;

                //    case "auth":
                //        Console.Write("Preshared key: ");
                //        userInput = Console.ReadLine();
                //        if (String.IsNullOrEmpty(userInput)) break;
                //        client.Authenticate(userInput);
                //        break;
                //}

                if (ObjectQueue.Count > 0 && ObjectQueue.TryDequeue(out var @object))
                {
                    if (@object is ColoredMessage)
                    {
                        var coloredMsg = @object as ColoredMessage;
                        client.Send(coloredMsg.Message, coloredMsg.Color);
                    }
                }

                Thread.Sleep(100);
            }
            while (runForever);
        }

        private static bool MessageReceived(byte[] data)
        {
            // Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(data));
            return true;
        }

        private static bool ServerConnected()
        {
            SendMessage("Client connected to server successfully! (Hello world from the other side!)", Color.Lime);
            return true;
        }

        public static void SendMessage(string msg)
            => SendMessage(msg, Console.ForegroundColor.ToColor());

        public static void SendMessage(string msg, Color color)
        {
            ObjectQueue.Enqueue(new ColoredMessage(color, msg));
        }

        private static bool ServerDisconnected()
        {
            // Console.WriteLine("Server disconnected");
            return true;
        }
    }
}