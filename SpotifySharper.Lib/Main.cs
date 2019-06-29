using System;

namespace SpotifySharper.Lib
{
    public class Main
    {
        // we use a default injection method name in order to execute our code in the remote process
        private static void Inject()
        {
            Console.WriteLine("Hello world from the injected process!");
        }
    }
}