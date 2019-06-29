using System;
using System.Windows.Forms;

namespace SpotifySharper.Injector
{
    public class Main
    {
        // we use a default injection method name in order to execute our code in the remote process
#if SHARP_NEEDLE

        [STAThread]
#endif
        public static int Run(string arg)
        {
            Console.WriteLine("Hello world from the injected process!");

            MessageBox.Show("C#/.Net dll payload example");
            MessageBox.Show(arg);
            return 0;
        }
    }
}