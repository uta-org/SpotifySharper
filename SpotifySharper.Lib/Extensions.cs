using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Console = Colorful.Console;

namespace SpotifySharper.Lib
{
    public static class Extensions
    {
        private static Process _spotifyProcess;

        public static Process SpotifyProcess
        {
            get
            {
                if (_spotifyProcess != null)
                    return _spotifyProcess;

                var processes = Process.GetProcessesByName("spotify");
                var proc = processes.FirstOrDefault(process =>
                {
                    string cli = process.GetCommandLine();
                    // Console.WriteLine($"ID: {process}\nCLI: {cli}");

                    // Return the one that doesn't not contain any arguments
                    return !cli.Contains("--");
                });

                _spotifyProcess = proc;
                return _spotifyProcess;
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

        public static bool IsJson(string strInput)
        {
            if (IsBasicJson(strInput)) //For array
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message, Color.Red);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex, Color.Red);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsBasicJson(string strInput)
        {
            strInput = strInput.Trim();
            return strInput.StartsWith("{") && strInput.EndsWith("}") || //For object
                   strInput.StartsWith("[") && strInput.EndsWith("]");
        }

        public static string[] GetLines(this string input)
        {
            return Regex.Split(input, @"\r?\n|\r");
        }

        public static Color ToColor(this ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return Color.Black;

                case ConsoleColor.Blue:
                    return Color.Blue;

                case ConsoleColor.Cyan:
                    return Color.Cyan;

                case ConsoleColor.DarkBlue:
                    return Color.DarkBlue;

                case ConsoleColor.DarkGray:
                    return Color.DarkGray;

                case ConsoleColor.DarkGreen:
                    return Color.DarkGreen;

                case ConsoleColor.DarkMagenta:
                    return Color.DarkMagenta;

                case ConsoleColor.DarkRed:
                    return Color.DarkRed;

                case ConsoleColor.DarkYellow:
                    return Color.FromArgb(255, 128, 128, 0);

                case ConsoleColor.Gray:
                    return Color.Gray;

                case ConsoleColor.Green:
                    return Color.Green;

                case ConsoleColor.Magenta:
                    return Color.Magenta;

                case ConsoleColor.Red:
                    return Color.Red;

                case ConsoleColor.White:
                    return Color.White;

                default:
                    return Color.Yellow;
            }
        }
    }
}