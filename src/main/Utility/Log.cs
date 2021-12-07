/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System;

namespace rt.Utility
{
    /// <summary>
    /// Helper class to handle logging out information of varying severity.
    /// </summary>
    internal static class Log
    {
        private const bool LogInfo = true;
        private const bool LogWarning = true;
        private const bool LogError = true;
        // #todo Create functions that log START when called and END at end of scope, possibly using "using" functionality

        public static void Info(string info)
        {
            if (LogInfo)
                Console.WriteLine($"INFO: {info}");
        }

        public static void Warning(string warning)
        {
            if (LogWarning)
                Console.WriteLine($"WARNING: {warning}");
        }

        public static void Error(string error)
        {
            if (LogError)
                Console.WriteLine($"ERROR: {error}");
        }

        public static void Write(string text)
        {
            Console.WriteLine(text);
        }

        public static void WriteInPlace(string text)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine(text);
        }
    }
}