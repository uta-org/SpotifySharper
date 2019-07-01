using System.Drawing;

namespace SpotifySharper.Lib.Model
{
    public class ColoredMessage
    {
        public Color Color { get; }
        public string Message { get; }

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