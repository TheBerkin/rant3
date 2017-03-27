using System;
using System.IO;
using System.Reflection;

using Rant.Tools.Packer;

using static System.Console;

namespace Rant.Tools
{
    internal class Program
    {
        public static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name.ToLowerInvariant();

        private static void Main(string[] args)
        {
            if (CmdLine.Flag("version"))
            {
                WriteLine($"Rant {typeof(RantEngine).Assembly.GetName().Version}");
                return; 
            }
#if !DEBUG
            try
            {
#endif
                Command.Run(CmdLine.Command);
#if !DEBUG
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex.Message);
                ResetColor();
                Environment.Exit(1);
            }
#endif
        }
    }
}