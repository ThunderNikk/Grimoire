using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grimoire.Structures;

namespace Grimoire.Functions
{
    //TODO: Display possible list of commands on typing only help
    public class Help
    {
        protected Dictionary<string, string> commands = new Dictionary<string, string>();
        protected Dictionary<string, Dictionary<string, string>> commandFlags = new Dictionary<string, Dictionary<string, string>>();
        protected Dictionary<string, List<string>> commandExplains = new Dictionary<string, List<string>>();

        protected static Help instance;
        public static Help Instance { get { if (instance == null) { instance = new Help(); }  return instance; } }

        public Help()
        {
            addCommands();
            addCommandFlags();
            addCommandExplains();
        }

        protected void addCommands()
        {
            commands.Add("about", "Shows information regarding Grimoire");
            commands.Add("help", "Grimoire features an in-depth in-house help system that can answer most if not all of your needs! Simply type: help followed by a command name for information about that command.");
            commands.Add("hash", "Encodes or Decodes the name of a file or multiple files.\n\t- Using this command will either change a plain-text file-name to a hashed name the client will accept or the opposite.");
            commands.Add("commands", "Grimoire uses commands to command the DataCore engine, you will have to make properly formed commands in order to get anything done.");
            commands.Add("configure", "Grimoire uses several settings to assist in its operation.\n\t- Using this command will ask you several questions to configure Grimoire.");
            commands.Add("gui", "Grimoire while at its core is a Console Application it will soon have full GUI capabilities that relay through it's base Console layer.\n\t- Using this command will show the GUI for Grimoire.");
            commands.Add("export", "Exports or extracts a file or file'(s) from the Data.XXX File System.\n\t- Using this command will export a file or multiple files from the desired client's Data.XXX File System.");
            commands.Add("insert", "Inserts or writes a file or file'(s) to the Data.XXX File System.\n\t- Using this commands will write one or multiple files into the desired client's Data.XXX File System.");
            commands.Add("delete", "Deletes or erases a file or file'(s) from the Data.XXX File System by writing zeros over the file bytes in the Data.XXX File System and removes it's entry from the Data Index.\n\t- Using this command will remove one or more files from the desired clients Data.XXX File System.");
            commands.Add("search", "Searches the loaded data.000 for one or more files based on certain criteria.\n\t- Using this command will tell you if a file or files exist in the Data File System.");
            commands.Add("rebuild", "Rebuilds a data.xxx storage file.\n\t- Using this command will rebuild a data.xxx storage file without any blank spaces caused by client updates.");
            commands.Add("set", "Sets a new variable to an existing config.opt setting.\n\t- Using this command will change a stored config variable that Grimoire uses to determine some operations and behaviors.");
        }

        protected void addCommandFlags()
        {
            Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();

            #region Export Flags
            tmpDictionary.Add("-a", "All flag, using this flag will export all files in the client.");
            tmpDictionary.Add("-e", "Extension flag, using this flag will only export files of the given extension.");
            tmpDictionary.Add("-p", "Partial flag, using this flag will export all files containing the given search term.");
            commandFlags.Add("export", tmpDictionary);
            #endregion

            #region Search
            tmpDictionary = new Dictionary<string, string>();
            tmpDictionary.Add("-e", "Extension flag, using this flag will only search for files with the given extension.");
            tmpDictionary.Add("-p", "Partial flag, using this flag will only search for files whose name contains the given term.");
            commandFlags.Add("search", tmpDictionary);
            #endregion

            #region Delete Flags
            tmpDictionary = new Dictionary<string, string>();
            tmpDictionary.Add("-m", "Multiple flag, using this flag will allow multiple file names to be passed in using , as a delimiter.");
            commandFlags.Add("delete", tmpDictionary);
            #endregion
        }

        private void addCommandExplains()
        {
            List<string> tmpList = new List<string>();

            #region Export
            tmpList = new List<string>();
            tmpList.Add("Full Export: export-a");
            tmpList.Add("Extension Export: export -e jpg");
            tmpList.Add("Singular Export: export db_item(ascii),rdb");
            commandExplains.Add("export", tmpList);
            #endregion

            #region Hash
            tmpList = new List<string>();
            tmpList.Add("Basic Example: hash");
            commandExplains.Add("help", tmpList);
            #endregion

            #region Rebuild
            tmpList = new List<string>();
            tmpList.Add("Basic Example: rebuild 1");
            commandExplains.Add("rebuild", tmpList);
            #endregion

            #region Search
            tmpList = new List<string>();
            tmpList.Add("Single File Search: search db_item(ascii).rdb");
            tmpList.Add("Partial File Search: search -p db_");
            tmpList.Add("Extension Search: search -e rdb");
            commandExplains.Add("search", tmpList);
            #endregion

            #region Set
            tmpList = new List<string>();
            tmpList.Add("Basic Example: set auto.load 0");
            commandExplains.Add("set", tmpList);
            #endregion

            #region Delete
            tmpList = new List<string>();
            tmpList.Add("Single Delete: delete db_item(ascii).rdb");
            tmpList.Add("Multiple Delete: delete db_item(ascii).rdb, db_item.rdb");
            commandExplains.Add("delete", tmpList);
            #endregion
        }

        public void ExplainCommand(string name)
        {
            if (commands.ContainsKey(name))
            {
                Console.WriteLine("{0}\n\t{1}\n", name, commands[name]);
                Console.WriteLine("\tFlags:");
                if (commandFlags.ContainsKey(name))
                {
                    foreach (KeyValuePair<string, string> keyValue in commandFlags[name])
                    {
                        Console.WriteLine("\t\t{0}:\n\t\t- {1}", keyValue.Key, keyValue.Value);
                    }
                }
                else { Console.WriteLine("\t\t- This operation doesn't use flags."); }

                if (commandExplains.ContainsKey(name))
                {
                    Console.WriteLine("\tExamples:");
                    foreach (string example in commandExplains[name])
                    {
                        Console.WriteLine("\t\t- {0}", example);
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
