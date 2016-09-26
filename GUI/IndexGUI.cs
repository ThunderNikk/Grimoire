using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCore.Structures;

namespace Grimoire.GUI
{
    public partial class IndexGUI : Form
    {
        public IndexGUI()
        {
            InitializeComponent();
        }

        public void loadIndex(ref List<IndexEntry> index)
        {
            indexGrid.DataSource = index;
        }
    }
}
