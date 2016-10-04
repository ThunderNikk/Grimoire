using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grimoire.Structures
{
    public class Message
    {
        public List<string> Lines { get; set; }
        public List<ConsoleColor> ForeColors { get; set; }
        public List<ConsoleColor> BackColors { get; set; }
        public bool Break { get; set; }
        public int Breaks { get; set; }

        public bool UseForeColor(int index)
        {
            if (ForeColors != null)
            {
                if (index < ForeColors.Count)
                {
                    return ForeColors[index] >= 0;
                }
            }

            return false;
        }

        public bool UseBackColor(int index)
        {
            if (BackColors != null)
            {
                if (index < BackColors.Count)
                {
                    return BackColors[index] >= 0;
                }
            }

            return false;
        }
    }
}
