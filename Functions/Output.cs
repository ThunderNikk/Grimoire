using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grimoire.Structures;

namespace Grimoire.Functions
{
    public static class Output
    {
        public static void Splash()
        {
            Write(new Message()
            {
                Lines = new List<string>()
                {
                    "============================================================================",
                    "============================= Grimoire v3 ==================================",
                    "============================================================================"
                },
                ForeColors = new List<ConsoleColor>()
                {
                    ConsoleColor.Green,
                    ConsoleColor.Green,
                    ConsoleColor.Green
                }
            });
        }

        public static void Write(Message message)
        {
            for (int idx = 0; idx < message.Lines.Count; idx++)
            {
                Console.ForegroundColor = message.UseForeColor(idx) ? message.ForeColors[idx] : ConsoleColor.White;
                Console.BackgroundColor = message.UseBackColor(idx) ? message.BackColors[idx] : ConsoleColor.Black;
                Console.WriteLine(message.Lines[idx]);
            }

            reset();
        }

        private static void reset()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
