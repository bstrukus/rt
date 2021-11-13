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
        static public void Info(string info)
        {
            Console.WriteLine($"INFO: {info}");
        }

        static public void Warning(string warning)
        {
            Console.WriteLine($"WARNING: {warning}");
        }

        static public void Error(string error)
        {
            Console.WriteLine($"ERROR: {error}");
        }
    }
}