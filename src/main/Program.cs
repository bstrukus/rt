/*
 * Copyright Ben Strukus
 */

using main;
using System;

namespace rt
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            PixelBuffer pb = new PixelBuffer(256, 256);
            pb.Fill();
            pb.Save("test.bmp");
        }
    }
}