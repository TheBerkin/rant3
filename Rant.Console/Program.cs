using System;
using Rant;
using Rant.Vocabulary;

using System.Console;

using RantConsole.CmdLine;

namespace RantConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;            

            var file = GetProperty("file");
            var dicPath = GetProperty("dicpath");
            Title = "Rant Console" + (GetFlag("nsfw") ? " [NSFW]" : "");            

            var rant = new RantEngine(String.IsNullOrEmpty(dicPath) ? "dictionary" : dicPath, GetFlag("nsfw") ? NsfwFilter.Allow : NsfwFilter.Disallow);
            rant.Hooks.AddHook("load", hArgs => hArgs.Length != 1 ? "" : rant.DoFile(hArgs[0]));

            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    PrintOutput(rant.DoFile(file), true);
                    return;
                }
                catch (Exception ex)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(ex.Message);
                    ResetColor();
                }
            }

            while (true)
            {
                ForegroundColor = GetFlag("nsfw") ? ConsoleColor.Magenta : ConsoleColor.Yellow;
                Write("\u211d> "); // real number symbol
                ResetColor();

                var input = ReadLine();
#if DEBUG
                PrintOutput(rant.Do(input));
#else
                try
                {
                    PrintOutput(rant.Do(input));
                }
                catch (Exception e)
                {
                    ForegroundColor = ConsoleColor.Red;
                    if (e is RantException)
                    {
                        WriteLine(e.Message);
                    }
                    else
                    {
                        WriteLine(e.ToString()); // Print the whole stack trace if it isn't a syntax error
                    }                    
                    ResetColor();
                }
#endif
            }
        }

        static void PrintOutput(Output output, bool file = false)
        {
            foreach (var chan in output)
            {
                if (chan.Name != "main")
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("\{chan.Name} (\{chan.Visiblity}):");
                    ResetColor();
                }
                ForegroundColor = ConsoleColor.White;
                if (chan.Length > 0)
                {
                    WriteLine(chan.Value);
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGray;
                    if (!file) WriteLine("[Empty]");
                }
                ResetColor();
            }
        }
    }
}
