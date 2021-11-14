/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using System;

    /// <summary>
    /// Helper class to handle logging out information of varying severity.
    /// </summary>
    internal static class Log
    {
        public static void Info(string info)
        {
            Console.WriteLine($"INFO: {info}");
        }

        public static void Warning(string warning)
        {
            Console.WriteLine($"WARNING: {warning}");
        }

        public static void Error(string error)
        {
            Console.WriteLine($"ERROR: {error}");
        }
    }
}