using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCore.Structures;
using Grimoire.Structures;
using Grimoire.GUI;
using Grimoire.Functions;

namespace Grimoire.Functions
{
    public class Commands
    {
        protected delegate void CommandAction(Command command);
        protected Dictionary<string, CommandAction> cmdList;

        public Commands()
        {
            cmdList = new Dictionary<string, CommandAction>();

            cmdList.Add("about", showAbout);
            cmdList.Add("clear", clear);
            cmdList.Add("load", load);
            cmdList.Add("hash", hash);
            cmdList.Add("help", showHelp);
            cmdList.Add("configure", configure);
            cmdList.Add("compare", compare);
            cmdList.Add("gui", showGUI);
            cmdList.Add("export", exportFile);
            cmdList.Add("insert", insertFile);
            cmdList.Add("delete", deleteFile);
            cmdList.Add("rebuild", rebuild);
            cmdList.Add("search", searchFile);
            cmdList.Add("shrink", shrink);
            cmdList.Add("set", set);
        }

        public Commands(ref FileStream inList) { }

        #region Commands Class Methods

        public Command CreateCommand(string text)
        {
            Command cmd = new Command();
            string[] cmdBlocks = text.Split(' ');

            cmd.Name = cmdBlocks[0];

            if (cmdBlocks.Length == 2)
            {
                if (cmdBlocks[1].Contains('-'))
                {
                    cmd.Flags = cmdBlocks[1].Contains(',') ? cmdBlocks[1].Split(',').ToList<string>() : new List<string> { cmdBlocks[1] };
                }
                else { cmd.Variable = cmdBlocks[1]; }
            }
            else if (cmdBlocks.Length == 3)
            {
                if (cmdBlocks[1].Contains('-'))
                {
                    cmd.Flags = cmdBlocks[1].Contains(',') ? cmdBlocks[1].Split(',').ToList<string>() : new List<string> { cmdBlocks[1] };
                }

                if (cmdBlocks[0] == "set")
                {
                    cmd.Variable = string.Format("{0} {1}", cmdBlocks[1], cmdBlocks[2]);
                }
                else { cmd.Variable = cmdBlocks[2]; }
            }

            return cmd;
        }

        public void Execute(Command command)
        {
            if (!cmdList.ContainsKey(command.Name)) { return; }

            cmdList[command.Name].Invoke(command);
        }
        #endregion

        #region Action Methods

        private void clear(Command command) { Program.FileManager.Clear(); }

        private void load(Command command)
        {
            string clientPath = OPT.Instance.GetString("client.path");
            if (!string.IsNullOrEmpty(clientPath)) { Program.FileManager.Load(); }
        }

        private void showAbout(Command command)
        {
            Console.WriteLine("Grimoire (or the book of spells) is a truly handy tool for any Rappelz developer, powered by DataCore v3.0.1.0 it can make quick work of all your Rappelz Client editing needs!");
        }

        private void hash(Command command)
        {
            Console.WriteLine("Please provide the location of the files to be hashed by dragging them onto the File Receiver interface.");
            List<string> paths = Input.Paths;

            if (paths != null)
            {
                List<string> newPaths = new List<string>();

                Console.WriteLine("Please review the following files:", paths.Count);
                foreach (string path in paths)
                {
                    string fileName = Path.GetFileName(path);
                    string filePath = Path.GetDirectoryName(path);
                    bool encoded = Program.FileManager.IsEncoded(fileName);
                    string newName = encoded ? Program.FileManager.DecodeName(fileName) : Program.FileManager.EncodeName(fileName);
                    string newPath = string.Format(@"{0}\{1}", filePath, newName);
                    newPaths.Add(newPath);
                    Console.WriteLine("\t- [Current] {0} [New] {1}", fileName, newName);
                }

                bool hash = OPT.Instance.GetBool("auto.hash");
                if (!hash)
                {
                    Console.WriteLine("Would you like to hash/unhash these files?");
                    hash = Input.YesNo;
                }

                if (hash)
                {
                    Console.Write("Hashing/Unhashing {0} files...", newPaths.Count);
                    ProgressBar pb = new ProgressBar();

                    for (int i = 0; i < newPaths.Count; i++)
                    {
                        File.Move(paths[i], newPaths[i]);
                        pb.Report(i / newPaths.Count);
                    }

                    pb.Dispose();

                    Output.Write(new Message() { Lines = new List<string>() { "[OK]", "\t- Press Enter to continue." }, ForeColors = new List<ConsoleColor>() { ConsoleColor.Green } });
                    Console.ReadLine();
                }
            }
            else { Console.WriteLine("No files were specified!"); }
        }

        private void showHelp(Command command)
        {
            Help.Instance.ExplainCommand(command.Name == "help" && string.IsNullOrEmpty(command.Variable) ? "help" : command.Variable);
        }

        private void configure(Command command) { Program.Configure(); }


        private void compare(Command command)
        {
            Console.WriteLine("Please provide the location of the files to be compared by dragging them onto the File Receiver interface.");
            using (FileReceiverGUI receiver = new FileReceiverGUI())
            {
                receiver.ShowDialog();

                if (receiver.FileList.Count > 0)
                {
                    List<string> fileList = receiver.FileList;

                    foreach (string file in fileList)
                    {
                        string fileName = Path.GetFileName(file);
                        if (Program.FileManager.CoreEncoded && !Program.FileManager.IsEncoded(fileName))
                        {
                            fileName = Program.FileManager.EncodeName(fileName);
                        }
                        else if (!Program.FileManager.CoreEncoded && Program.FileManager.IsEncoded(fileName))
                        {
                            fileName = Program.FileManager.DecodeName(fileName);
                        }

                        Console.WriteLine("Comparing the hash of local file {0} with stored copy...", fileName);

                        string sourceHash = Hash.GetSHA512Hash(file);

                        Console.WriteLine("\t- Source File Hash: {0}", sourceHash);

                        string storedHash = null;

                        List<IndexEntry> results = Program.FileManager.Search(fileName, 0);

                        if (results.Count > 0)
                        {
                            IndexEntry entry = results[0];
                            storedHash = Program.FileManager.SHA512(ref entry);

                            if (storedHash.Length > 0)
                            {
                                Console.WriteLine("\t- Stored File Hash: {0}", storedHash);

                                if (sourceHash == storedHash) { Console.WriteLine("\t\t- The two files match!"); }
                            }
                        }
                        else { Console.WriteLine("\t- NO MATCHES FOUND!"); }
                    }
                }
            }
        }

        private void showGUI(Command command)
        {
            throw new NotImplementedException();
        }

        private void insertFile(Command command)
        {
            Console.WriteLine("Please provide the location of the files to be inserted by dragging them onto the File Receiver interface.");

            List<string> fileList = Input.Paths;
            if (fileList != null)
            {
                Console.WriteLine("Please review the following files:");

                foreach (string filePath in fileList)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    string cryptedName = Program.FileManager.IsEncoded(fileInfo.Name) ? Program.FileManager.DecodeName(fileInfo.Name) : Program.FileManager.EncodeName(fileInfo.Name);

                    Console.WriteLine("\t Name: [Normal] {0} [Hashed] {1}\n\t Size: {2}\n\t Location: data.00{3}", cryptedName, fileInfo.Name, fileInfo.Length, Program.FileManager.GetID(fileInfo.Name));
                }

                Console.WriteLine("Would you like to write these files to the Data File System?");
                if (Input.YesNo)
                {
                    Console.WriteLine("Writing files to the Data File System...");

                    if (fileList.Count > 1)
                    {
                        foreach (string filePath in fileList) { Program.FileManager.Import(filePath); }
                    }
                    else
                    {
                        Console.WriteLine("Writing {0} to data.00{1}", Path.GetFileName(fileList[0]), Program.FileManager.GetID(Path.GetFileName(fileList[0])));
                        Program.FileManager.Import(fileList[0]);
                    }

                    Program.FileManager.Save();
                }
            }
            else { Console.WriteLine("No files were specified!"); }
        }

        private void exportFile(Command command)
        {
            List<IndexEntry> results = null;
            if (command.HasFlags)
            {
                if (command.Flags.Contains("-p")) { results = Program.FileManager.Search(command.Variable, 1); }
                else if (command.Flags.Contains("-a")) { results = Program.FileManager.Index; }
                else if (command.Flags.Contains("-e")) { results = Program.FileManager.Search(command.Variable, 2); }
            }
            else { results = Program.FileManager.Search(command.Variable, 0); }

            if (results != null && results.Count > 0) { Program.FileManager.Export(ref results); }
            else { Console.WriteLine("\t- There were no exportable results!"); }

        }

        private void deleteFile(Command command)
        {
            List<IndexEntry> results = null;
            if (command.HasFlags)
            {
                if (command.Flags.Contains("-m")) { }
            }
            else { results = Program.FileManager.Search(command.Variable, 0); }

            Program.FileManager.Delete(ref results);
            Program.FileManager.Save();
        }

        private void rebuild(Command command)
        {
            string dataPath = null;

            try
            {
                int var = Convert.ToInt32(command.Variable);
                if (var >= 1 && var <= 9)
                {
                    Console.WriteLine("Are you sure you want to rebuild Data.00{0}", var);
                    if (Input.YesNo) { dataPath = Program.FileManager.Rebuild(var); }
                }
                else { Console.WriteLine("\t- You must enter a number between 1 and 8"); return; }

                if (!string.IsNullOrEmpty(dataPath)) { replaceFile(dataPath); }

                Program.FileManager.Save();

            }
            catch { Console.WriteLine("\t- You have not entered a number!"); }
        }

        private void searchFile(Command command)
        {
            Console.Write("Searching for {0}...", command.Variable);

            List<IndexEntry> results = null;
            if (command.HasFlags)
            {
                if (command.Flags.Contains("-p")) { results = Program.FileManager.Search(command.Variable, 1); }
                else if (command.Flags.Contains("-e"))
                {
                    if (command.Variable.Length >= 2) { results = Program.FileManager.Search(command.Variable, 2); }
                    else { Console.WriteLine("[FAIL]\n\t- You have not provided a proper extension! (e.g. rdb)"); return; }
                }
            }
            else { results = Program.FileManager.Search(command.Variable, 0); }

            if (results.Count > 1)
            {
                Console.WriteLine("[OK] ({0} results found)", results.Count);
                Console.WriteLine("Would you like to display the results?");
                if (Input.YesNo)
                {
                    foreach (IndexEntry entry in results) { Console.WriteLine("\t- Entry {0} located in data.00{1} with size {2}", entry.Name, entry.DataID, entry.Length); }
                }
            }
            else if (results.Count == 1)
            {
                Console.WriteLine("[FOUND]\n\t- Location: data.00{0}\n\t- Offset: {1}\n\t- Length: {2}", results[0].DataID, results[0].Offset, results[0].Length);
            }
        }

        private void shrink(Command command)
        {
            long originalSize = Program.FileManager.ClientSize;

            string dataPath = null;

            Console.WriteLine("Are you sure you want to rebuild the client? This may take some time.");
            if (Input.YesNo)
            {
                for (int i = 1; i < 9; i++)
                {
                    Console.WriteLine("Rebuilding data.00{0}...", i);
                    dataPath = Program.FileManager.Rebuild(i);

                    if (!string.IsNullOrEmpty(dataPath)) { replaceFile(dataPath); }
                }

                Program.FileManager.Save();

                long newSize = Program.FileManager.ClientSize;
                Console.WriteLine("\t- Client size reduced from: {0} to {1}", originalSize, newSize);
            }
        }

        private void set(Command command)
        {
            if (!string.IsNullOrEmpty(command.Variable))
            {
                string[] variableBlocks = command.Variable.Split(' ');

                if (variableBlocks.Length == 2)
                {
                    string settingName = variableBlocks[0];
                    string settingValue = null;

                    string type = OPT.Instance.GetType(settingName);
                    switch (type)
                    {
                        case "s":
                            settingValue = variableBlocks[1].ToString();
                            break;

                        case "n":
                            settingValue = Convert.ToInt32(variableBlocks[1]).ToString();
                            break;

                        case "b":
                            settingValue = Convert.ToBoolean(Convert.ToInt32(variableBlocks[1])).ToString();
                            break;
                    }

                    if (OPT.Instance.SettingExists(settingName) && !string.IsNullOrEmpty(settingValue))
                    {
                        OPT.Instance.Update(settingName, settingValue);
                        Console.WriteLine("Setting: {0} updated!\n\t- New Value: {1}", settingName, settingValue);
                        OPT.Instance.Write();
                    }
                }
            }
        }

        protected void replaceFile(string dataPath)
        {
            Console.Write("\t- Cleaning up...");
            File.Delete(dataPath);
            File.Move(string.Format(@"{0}_NEW", dataPath), dataPath);
            Output.Write(new Message() { Lines = new List<string>() { "[OK]" }, ForeColors = new List<ConsoleColor>() { ConsoleColor.Green } });
        }

        #endregion
    }
}
