using System;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using SpotifySharper.Lib.Model;
using WatsonTcp;

namespace SpotifySharper.Injector.Extensions
{
    public static class TcpExtensions
    {
        public static bool Send(this WatsonTcpClient client, string msg)
            => client.Send(Encoding.UTF8.GetBytes(msg));

        public static bool Send(this WatsonTcpClient client, string msg, Color color)
        {
            var coloredMsg = new ColoredMessage(color, msg);

            return client.Send(
                JsonConvert.SerializeObject(new TypeAdviser(coloredMsg.GetType().FullName))
                + Environment.NewLine +
                JsonConvert.SerializeObject(coloredMsg));
        }
    }
}