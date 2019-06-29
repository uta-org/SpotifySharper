#define ONLY_PID

//using System;
//using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Linq;
using ES.ManagedInjector;
using SharpNeedle;
using SpotifySharper.Injector;
using SpotifySharper.Lib;

#if !ONLY_PID
using System.Drawing;
using Console = Colorful.Console;
#else

using System;

#endif

namespace SpotifySharper.Shell
{
    internal class Program
    {
        // donut -f loader.dll -c TestClass -m RunProcess -p notepad.exe
        private static void Main(string[] args)
        {
            //Console.WriteLine($"Is 64 bits?: {Is64Bit(processes[0]?.Handle)}");

            int? pid = Extensions.FindProcessId();

#if SHARP_NEEDLE
            PayloadInjector injector = new PayloadInjector(Extensions.SpotifyProcess,
                Environment.CurrentDirectory,
                "SharpDomain.dll",
                Environment.CurrentDirectory,
                "SpotifySharper.Lib.dll",
                string.Empty);
            injector.InjectAndForget();
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