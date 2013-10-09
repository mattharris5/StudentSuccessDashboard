using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestDriver_GUI
{
    public partial class RunDialog : Form
    {
        public RunDialog()
        {
            InitializeComponent();
        }

        private void RunDialog_Load(object sender, EventArgs e)
        {
            testName.Text = "LoLZ";
        }

    }
}
