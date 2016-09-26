using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grimoire.Functions;
using Grimoire.GUI;
using DataCore;
using DataCore.Structures;
using System.Windows.Forms;
using System.Text;
using Grimoire.Structures;

//TODO: Add Default Behaviors setting for launching by dragging file onto exe
namespace Grimoire
{
    class Program
    {
        static List<IndexEntry> index = new List<IndexEntry>();
        static OPT settings = OPT.Instance;
        public static Commands CMDManager = new Commands();
        public static FileIO FileManager = new FileIO();


        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("Loading settings from config.opt...");
            settings.Start();
            Console.WriteLine("[OK]\n\t- {0} settings loaded!", settings.Count);

            if (settings.GetString("client.path") == "0")
            {
                Console.WriteLine("This appears to be your first start! You must configure me!");
                Configure();
                settings.Write();
            }

            if (settings.GetBool("backups")) { Console.WriteLine("Reminder: Backups are currently on."); }


            Console.Write("Starting the File Manager...");
            FileManager.Start();
            Console.WriteLine("[OK]");

            if (settings.GetBool("auto.load")) { loadIndex(); }
            else
            {
                Console.WriteLine("Would you like to load your data.000 now?");
                if (Input.YesNo) { loadIndex(); }
            }

            wait();
        }

        public static void Configure()
        {
            Console.WriteLine("Grimoire v3 Configuration.\nPlease tell me where your client is by choosing an option below:\n\t- Manual [M] (type in the path to your clients data.000\n\t- Assisted [A] use file selection menu to find your data.000 path");

            switch (Console.ReadLine().ToLower())
            {
                case "":
                    Console.WriteLine("You haven't answered me! Wanna try that again?");
                    if (Input.YesNo) { Configure(); } else { return; }
                    break;

                case "m":
                    Console.WriteLine(@"Please type in the whole path of your clients data.000 and press enter please!\n\t- e.g. C:\Rappelz\data.000");
                    string directory = Console.ReadLine();
                    if (File.Exists(directory))
                    {
                        Console.WriteLine("Saving the directory to config.opt");
                        settings.Update("client.path", directory);
                    }
                    break;

                case "a":
                    Console.WriteLine("Please select the data.000 of your desired client!");
                    string filePath = Input.Path;
                    settings.Update("client.path", filePath);
                    break;
            }

            Console.WriteLine("Please tell me where you would like me to build any future files by choosing an option below:\n\t- Manual ([M] type in the path to your build folder)\n\t- Assisted ([A] use folder selection menu to find the path to your build folder)");

            switch (Console.ReadLine().ToLower())
            {
                case "":
                    Console.WriteLine("You haven't answered me! Wanna try that again?");
                    if (Input.YesNo) { Configure(); } else { return; }
                    break;

                case "m":
                    Console.WriteLine(@"Please type in the whole path of your build folder and press enter please!\n\t- e.g. C:\Rappel\new-files\");
                    string directory = Console.ReadLine();
                    if (Directory.Exists(directory)) { settings.Update("build.path", directory); }
                    break;

                case "a":
                    Console.WriteLine("Please select the folder you'd like to build files in!");
                    string folderDir = Input.Directory;
                    settings.Update("build.path", folderDir);
                    break;
            }

            Console.WriteLine("Ok, I'll remember that for you!");

            Console.WriteLine("Would you like to auto load the data.000 next time?");
            settings.Update("auto.load", Input.YesNo);
            Console.WriteLine("Ok, I'll remember that for you!");

            Console.WriteLine("Would you like to show the index manipulation interface on index load completed?");
            settings.Update("auto.show", Input.YesNo);
            Console.WriteLine("Ok, I'll remember that for you!");

            Console.WriteLine("Would you like to make backups of the data.000 and data.xxx when changes are made?\n\tWARNING! This will increase the time it takes to insert/update files!!!");
            settings.Update("backups", Input.YesNo);
            Console.WriteLine("Ok, I'll remember that for you!");
        }

        protected static void loadIndex() { FileManager.Load(); }

        protected static void wait()
        {
            Console.WriteLine("Waiting for command...");

            string cmdText = Console.ReadLine().ToLower();
            if (cmdText.Contains("exit")) { return; }

            CMDManager.Execute(CMDManager.CreateCommand(cmdText));

            wait();
        }
    }
}
