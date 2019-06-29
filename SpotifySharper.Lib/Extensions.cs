using System.Diagnostics;
using System.Linq;
using System.Management;

namespace SpotifySharper.Lib
{
    public static class Extensions
    {
        public static Process SpotifyProcess
        {
            get
            {
                var processes = Process.GetProcessesByName("spotify");
                var proc = processes.FirstOrDefault(process =>
                {
                    string cli = process.GetCommandLine();
                    // Console.WriteLine($"ID: {process}\nCLI: {cli}");

                    // Return the one that doesn't not contain any arguments
                    return !cli.Contains("--");
                });
                return proc;
            }
        }

        public static int? FindProcessId()
        {
            var proc = SpotifyProcess;
            return proc?.Id;
        }

        public static string GetCommandLine(this Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }
    }
}