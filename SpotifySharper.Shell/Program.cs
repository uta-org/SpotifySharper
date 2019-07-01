#define ONLY_PID

//using System;
//using System.Runtime.InteropServices;

using System;
using System.Diagnostics;
using System.Linq;
using ES.ManagedInjector;
using SharpNeedle;
using SpotifySharper.Injector;
using SpotifySharper.Lib;

using System.Drawing;
using Console = Colorful.Console;

namespace SpotifySharper.Shell
{
    internal class Program
    {
        // donut -f loader.dll -c TestClass -m RunProcess -p notepad.exe
        private static void Main(string[] args)
        {
            //Console.WriteLine($"Is 64 bits?: {Is64Bit(processes[0]?.Handle)}");

            // int? pid = Extensions.FindProcessId();

#if SHARP_NEEDLE
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
                Console.WriteLine($"Successfully injected into Spotify (PID: {spotifyProcess?.Id} | Title: {spotifyProcess?.MainWindowTitle})", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex, Color.Red);
            }
#else
#if !ONLY_PID
            if (pid.HasValue)
            {
                InjectionResult result = Inject(pid.Value);
                Console.WriteLine($"Injection Result: {result}");
            }
            else
            {
                Console.WriteLine($"Couldn't find any Spotify process opened!", Color.Red);
            }
#else
            Console.WriteLine($"Spotify ID: {(pid.HasValue ? pid.Value.ToString() : "NULL")}");
#endif
#endif

            // Console.WriteLine($"Spotify ID: {FindProcessId()}");
            Console.Read();
        }

        private static InjectionResult Inject(int pid)
        {
            var injector = new ES.ManagedInjector.Injector(pid, typeof(Main).Assembly);
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