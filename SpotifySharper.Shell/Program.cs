//using System;
//using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Linq;
using ES.ManagedInjector;
using SpotifySharper.Lib;

using System.Drawing;
using Console = Colorful.Console;

namespace SpotifySharper.Shell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine($"Is 64 bits?: {Is64Bit(processes[0]?.Handle)}");

            int? pid = FindProcessId();

            if (pid.HasValue)
            {
                InjectionResult result = Inject(pid.Value);
                Console.WriteLine($"Injection Result: {result}");
            }
            else
            {
                Console.WriteLine($"Couldn't find any Spotify process opened!", Color.Red);
            }

            // Console.WriteLine($"Spotify ID: {FindProcessId()}");
            Console.Read();
        }

        private static int? FindProcessId()
        {
            var processes = Process.GetProcessesByName("spotify");
            var proc = processes.FirstOrDefault(process =>
            {
                string cli = process.GetCommandLine();
                // Console.WriteLine($"ID: {process}\nCLI: {cli}");

                return cli.Contains("--type=renderer");
            });

            return proc?.Id;
        }

        private static InjectionResult Inject(int pid)
        {
            var injector = new Injector(pid, typeof(Main).Assembly);
            return injector.Inject();
        }

        //[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        //public static bool Is64Bit(IntPtr? processHandle)
        //{
        //    if (!processHandle.HasValue)
        //        return false;

        //    bool retVal;

        //    // Process.GetCurrentProcess().Handle
        //    IsWow64Process(processHandle.Value, out retVal);

        //    return retVal;
        //}
    }
}