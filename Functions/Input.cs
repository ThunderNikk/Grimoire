using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Grimoire.GUI;

namespace Grimoire.Functions
{
    public static class Input
    {
        public static bool YesNo
        {
            get
            {
                bool outResponse = false;

                Console.WriteLine("Please type one of the following and press ENTER:\n\t- Yes [Y]\n\t- No [N]");

                string response = Console.ReadLine().ToLower();

                if (response.Contains("y") || response.Contains("yes")) { outResponse = true; }

                return outResponse;
            }
        }

        public static string Path
        {
            get
            {
                using (OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog() { DefaultExt = "000", Title = "Select your data.000" })
                {
                    ofd.ShowDialog();

                    return File.Exists(ofd.FileName) ? ofd.FileName : null;
                }
            }
        }

        public static List<string> Paths
        {
            get
            {
                using (FileReceiverGUI receiver = new FileReceiverGUI())
                {
                    receiver.ShowDialog();
                    return receiver.FileList.Count > 0 ? receiver.FileList : null;
                }
            }
        }

        public static string Directory
        {
            get
            {
                using (FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Please select your build path", ShowNewFolderButton = true })
                {
                    fbd.ShowDialog();
                    return System.IO.Directory.Exists(fbd.SelectedPath) ? fbd.SelectedPath : null;
                }
            }
        }
    }
}
