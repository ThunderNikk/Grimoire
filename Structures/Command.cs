using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grimoire.Structures
{
    /// <summary>
    /// A structure containing information about a command
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Name of the command being executed
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Flags associated to the command being executed
        /// </summary>
        public List<string> Flags { get; set; }
        /// <summary>
        /// Any relevant variables to the command being executed
        /// </summary>
        public string Variable { get; set; }

        /// <summary>
        /// Determines if the current command contains any flags
        /// </summary>
        public bool HasFlags { get { return Flags != null ? Flags.Count > 0 : false; } }
    }  
}
