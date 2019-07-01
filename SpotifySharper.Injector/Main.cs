using System;
using System.Windows.Forms;

namespace SpotifySharper.Injector
{
    public class Main
    {
        // we use a default injection method name in order to execute our code in the remote process
        [STAThread]
        public static void Run()
        {
            //Console.WriteLine("Hello world from the injected process!");
            MessageBox.Show("Test");

            // SpotifyPatchAds.PatchAds();
        }
    }
}