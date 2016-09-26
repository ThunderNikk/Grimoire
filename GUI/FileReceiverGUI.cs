using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCore;

namespace Grimoire.GUI
{
    public partial class FileReceiverGUI : Form
    {
        public List<string> FileList = new List<string>();

        public FileReceiverGUI()
        {
            InitializeComponent();
        }

        private void FileReceiverGUI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void FileReceiverGUI_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            FileList = files.ToList<string>();

            this.Hide();
        }

        private void FileReceiverGUI_Load(object sender, EventArgs e)
        {

        }

        private void FileReceiverGUI_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
