/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using System;

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