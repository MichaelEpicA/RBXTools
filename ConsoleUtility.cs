using System;
using System.Collections.Generic;
using System.Text;

namespace RBXTools
{
    static class ConsoleUtility
    {
        const char _block = '■';
        const string _back = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
        const string _twirl = "-\\|/";
        public static bool stillworking = false;
        public static void WriteProgressBar(int percent, bool update = false)
        {
            if (update)
                Console.Write(_back);
            Console.Write("[");
            var p = (int)((percent / 10f) + .5f);
            for (var i = 0; i < 10; ++i)
            {
                if (i >= p)
                    Console.Write(' ');
                else
                    Console.Write(_block);
            }
            Console.Write("] {0,3:##0}%", percent);
        }
        public static void WriteProgressBar(int percent, bool update = false, string stuff = "")
        {
            if (!stillworking)
            {
                stillworking = true;
            } else
            {
                return;
            }
            string __back = _back;
            for(int i = 0; i < stuff.Length; i++)
            {
                __back += '\b';
            }
            if (update)
            {
                Console.Write(__back);
                Console.Write('[');
            }
            var p = (int)((percent / 10f) + .5f);
            for (var i = 0; i < 10; ++i)
            {
                if (i >= p)
                    Console.Write(' ');
                else
                    Console.Write(_block);
            }
            Console.Write("] {0,3:##0}%", percent);
            Console.Write($" {stuff}");
            stillworking = false;
        }
        public static void WriteProgress(int progress, bool update = false)
        {
            if (update)
                Console.Write("\b\b\b\b\b");
            Console.Write($"{progress}%");
        }
    }
}
