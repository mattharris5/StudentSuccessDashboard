using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestDriver_GUI;

namespace DBUpdate
{
    public partial class frmDBUpdate : Form
    {
        public frmDBUpdate()
        {
            InitializeComponent();
        }

        private void frmDBUpdate_load(object sender, EventArgs e)
        {
            string tstname;
            TestDriverGUI tstDriver;

            tstDriver = new TestDriverGUI();

            tstname = tstDriver.tstName;
            lblTestName.Text = lblTestName.Text + tstname;
        }

        
    }
}
