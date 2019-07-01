using System;
using System.Drawing;

namespace SpotifySharper.Lib.Model
{
    [Serializable]
    public class ColoredMessage
    {
        public Color Color { get; set; }
        public string Message { get; set; }

        private ColoredMessage()
        {
        }

        public ColoredMessage(Color color, string msg)
        {
            Color = color;
            Message = msg;
        }
    }
}