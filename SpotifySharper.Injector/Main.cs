using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using SpotifySharper.Injector.Tools;
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
            // Init queue
            ObjectQueue = new ConcurrentQueue<object>();

            // Start job
            Task.Factory.StartNew(SpotifyPatchAds.PatchAds);

            // Start socket
            InitSocketClient();
        }

        private static void InitSocketClient()
        {
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
            => ObjectQueue.Enqueue(new ColoredMessage(color, msg));

        public static void SendMessage(object obj, Color color)
            => SendMessage(obj.ToString(), color);

        private static bool ServerDisconnected()
        {
            // Console.WriteLine("Server disconnected");
            return true;
        }
    }
}