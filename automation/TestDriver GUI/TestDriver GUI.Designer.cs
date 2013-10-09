namespace TestDriver_GUI
{
    partial class TestDriverGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestDriverGUI));
            this.btnPlay = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lstAvailableTest = new System.Windows.Forms.ListBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDeselect = new System.Windows.Forms.Button();
            this.lstSelectedTest = new System.Windows.Forms.ListBox();
            this.lstTestSuite = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.btnSlctAllAvailable = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDeslctAllAvailable = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSuiteRunning = new System.Windows.Forms.Label();
            this.lstCaseSelect = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rdoChrome = new System.Windows.Forms.RadioButton();
            this.rdoFirefox = new System.Windows.Forms.RadioButton();
            this.slctBrowser = new System.Windows.Forms.GroupBox();
            this.rdoSafari = new System.Windows.Forms.RadioButton();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.txtTestRunning = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.envSelector = new System.Windows.Forms.ComboBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnStepExtract = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblIteration = new System.Windows.Forms.Label();
            this.cmbIteration = new System.Windows.Forms.ComboBox();
            this.btnResetForm = new System.Windows.Forms.Button();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.lblIter = new System.Windows.Forms.Label();
            this.lblIterCount = new System.Windows.Forms.Label();
            this.lblTstRun = new System.Windows.Forms.Label();
            this.lblTestFail = new System.Windows.Forms.Label();
            this.lblTestPass = new System.Windows.Forms.Label();
            this.bxRunStats = new System.Windows.Forms.GroupBox();
            this.bxDBConsole = new System.Windows.Forms.GroupBox();
            this.chkGenExcel = new System.Windows.Forms.CheckBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProductBox = new System.Windows.Forms.ComboBox();
            this.slctBrowser.SuspendLayout();
            this.bxRunStats.SuspendLayout();
            this.bxDBConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlay
            // 
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lstAvailableTest
            // 
            this.lstAvailableTest.FormattingEnabled = true;
            resources.ApplyResources(this.lstAvailableTest, "lstAvailableTest");
            this.lstAvailableTest.Name = "lstAvailableTest";
            this.lstAvailableTest.SelectedIndexChanged += new System.EventHandler(this.lstAvailableTest_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            resources.ApplyResources(this.btnSelect, "btnSelect");
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeselect
            // 
            resources.ApplyResources(this.btnDeselect, "btnDeselect");
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // lstSelectedTest
            // 
            this.lstSelectedTest.FormattingEnabled = true;
            resources.ApplyResources(this.lstSelectedTest, "lstSelectedTest");
            this.lstSelectedTest.Name = "lstSelectedTest";
            this.lstSelectedTest.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedTest.SelectedIndexChanged += new System.EventHandler(this.lstSelectedTest_SelectedIndexChanged);
            // 
            // lstTestSuite
            // 
            this.lstTestSuite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstTestSuite.FormattingEnabled = true;
            resources.ApplyResources(this.lstTestSuite, "lstTestSuite");
            this.lstTestSuite.Name = "lstTestSuite";
            this.lstTestSuite.SelectedIndexChanged += new System.EventHandler(this.lstTestSuite_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblTimer
            // 
            resources.ApplyResources(this.lblTimer, "lblTimer");
            this.lblTimer.Name = "lblTimer";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnSlctAllAvailable
            // 
            resources.ApplyResources(this.btnSlctAllAvailable, "btnSlctAllAvailable");
            this.btnSlctAllAvailable.Name = "btnSlctAllAvailable";
            this.btnSlctAllAvailable.TabStop = false;
            this.btnSlctAllAvailable.UseVisualStyleBackColor = true;
            this.btnSlctAllAvailable.Click += new System.EventHandler(this.slctAllAvailable_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnDeslctAllAvailable
            // 
            resources.ApplyResources(this.btnDeslctAllAvailable, "btnDeslctAllAvailable");
            this.btnDeslctAllAvailable.Name = "btnDeslctAllAvailable";
            this.btnDeslctAllAvailable.UseVisualStyleBackColor = true;
            this.btnDeslctAllAvailable.Click += new System.EventHandler(this.slctAllSelected_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtSuiteRunning
            // 
            resources.ApplyResources(this.txtSuiteRunning, "txtSuiteRunning");
            this.txtSuiteRunning.Name = "txtSuiteRunning";
            // 
            // lstCaseSelect
            // 
            this.lstCaseSelect.FormattingEnabled = true;
            resources.ApplyResources(this.lstCaseSelect, "lstCaseSelect");
            this.lstCaseSelect.Name = "lstCaseSelect";
            this.lstCaseSelect.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstCaseSelect.SelectedIndexChanged += new System.EventHandler(this.lstCaseSelect_SelectedIndexChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // rdoChrome
            // 
            resources.ApplyResources(this.rdoChrome, "rdoChrome");
            this.rdoChrome.Name = "rdoChrome";
            this.rdoChrome.UseVisualStyleBackColor = true;
            // 
            // rdoFirefox
            // 
            resources.ApplyResources(this.rdoFirefox, "rdoFirefox");
            this.rdoFirefox.Checked = true;
            this.rdoFirefox.Name = "rdoFirefox";
            this.rdoFirefox.TabStop = true;
            this.rdoFirefox.UseVisualStyleBackColor = true;
            // 
            // slctBrowser
            // 
            this.slctBrowser.Controls.Add(this.rdoSafari);
            this.slctBrowser.Controls.Add(this.rdoFirefox);
            this.slctBrowser.Controls.Add(this.rdoChrome);
            resources.ApplyResources(this.slctBrowser, "slctBrowser");
            this.slctBrowser.Name = "slctBrowser";
            this.slctBrowser.TabStop = false;
            // 
            // rdoSafari
            // 
            resources.ApplyResources(this.rdoSafari, "rdoSafari");
            this.rdoSafari.Name = "rdoSafari";
            this.rdoSafari.TabStop = true;
            this.rdoSafari.UseVisualStyleBackColor = true;
            // 
            // txtTestRunning
            // 
            resources.ApplyResources(this.txtTestRunning, "txtTestRunning");
            this.txtTestRunning.Name = "txtTestRunning";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // envSelector
            // 
            this.envSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.envSelector.ForeColor = System.Drawing.SystemColors.WindowText;
            this.envSelector.FormattingEnabled = true;
            resources.ApplyResources(this.envSelector, "envSelector");
            this.envSelector.Name = "envSelector";
            this.envSelector.TextChanged += new System.EventHandler(this.envSelector_Changed);
            // 
            // btnExtract
            // 
            resources.ApplyResources(this.btnExtract, "btnExtract");
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // btnStepExtract
            // 
            resources.ApplyResources(this.btnStepExtract, "btnStepExtract");
            this.btnStepExtract.Name = "btnStepExtract";
            this.btnStepExtract.UseVisualStyleBackColor = true;
            this.btnStepExtract.Click += new System.EventHandler(this.btnStepExtract_Click);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblIteration
            // 
            resources.ApplyResources(this.lblIteration, "lblIteration");
            this.lblIteration.Name = "lblIteration";
            // 
            // cmbIteration
            // 
            this.cmbIteration.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmbIteration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIteration.FormattingEnabled = true;
            this.cmbIteration.Items.AddRange(new object[] {
            resources.GetString("cmbIteration.Items"),
            resources.GetString("cmbIteration.Items1"),
            resources.GetString("cmbIteration.Items2"),
            resources.GetString("cmbIteration.Items3"),
            resources.GetString("cmbIteration.Items4"),
            resources.GetString("cmbIteration.Items5"),
            resources.GetString("cmbIteration.Items6"),
            resources.GetString("cmbIteration.Items7"),
            resources.GetString("cmbIteration.Items8"),
            resources.GetString("cmbIteration.Items9"),
            resources.GetString("cmbIteration.Items10"),
            resources.GetString("cmbIteration.Items11"),
            resources.GetString("cmbIteration.Items12"),
            resources.GetString("cmbIteration.Items13"),
            resources.GetString("cmbIteration.Items14")});
            resources.ApplyResources(this.cmbIteration, "cmbIteration");
            this.cmbIteration.Name = "cmbIteration";
            this.cmbIteration.Tag = "1";
            // 
            // btnResetForm
            // 
            resources.ApplyResources(this.btnResetForm, "btnResetForm");
            this.btnResetForm.Name = "btnResetForm";
            this.btnResetForm.UseVisualStyleBackColor = true;
            this.btnResetForm.Click += new System.EventHandler(this.btnResetForm_Click);
            // 
            // lblIter
            // 
            resources.ApplyResources(this.lblIter, "lblIter");
            this.lblIter.Name = "lblIter";
            // 
            // lblIterCount
            // 
            resources.ApplyResources(this.lblIterCount, "lblIterCount");
            this.lblIterCount.Name = "lblIterCount";
            // 
            // lblTstRun
            // 
            resources.ApplyResources(this.lblTstRun, "lblTstRun");
            this.lblTstRun.Name = "lblTstRun";
            // 
            // lblTestFail
            // 
            resources.ApplyResources(this.lblTestFail, "lblTestFail");
            this.lblTestFail.Name = "lblTestFail";
            // 
            // lblTestPass
            // 
            resources.ApplyResources(this.lblTestPass, "lblTestPass");
            this.lblTestPass.Name = "lblTestPass";
            // 
            // bxRunStats
            // 
            this.bxRunStats.Controls.Add(this.label11);
            this.bxRunStats.Controls.Add(this.lblTestFail);
            this.bxRunStats.Controls.Add(this.lblTestPass);
            this.bxRunStats.Controls.Add(this.lblTstRun);
            this.bxRunStats.Controls.Add(this.label12);
            this.bxRunStats.Controls.Add(this.label13);
            this.bxRunStats.Controls.Add(this.label4);
            this.bxRunStats.Controls.Add(this.lblTimer);
            resources.ApplyResources(this.bxRunStats, "bxRunStats");
            this.bxRunStats.Name = "bxRunStats";
            this.bxRunStats.TabStop = false;
            // 
            // bxDBConsole
            // 
            this.bxDBConsole.Controls.Add(this.btnStepExtract);
            this.bxDBConsole.Controls.Add(this.btnUpdate);
            this.bxDBConsole.Controls.Add(this.btnExtract);
            resources.ApplyResources(this.bxDBConsole, "bxDBConsole");
            this.bxDBConsole.Name = "bxDBConsole";
            this.bxDBConsole.TabStop = false;
            // 
            // chkGenExcel
            // 
            resources.ApplyResources(this.chkGenExcel, "chkGenExcel");
            this.chkGenExcel.Name = "chkGenExcel";
            this.chkGenExcel.UseVisualStyleBackColor = true;
            // 
            // lblProduct
            // 
            resources.ApplyResources(this.lblProduct, "lblProduct");
            this.lblProduct.Name = "lblProduct";
            // 
            // cmbProductBox
            // 
            this.cmbProductBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbProductBox, "cmbProductBox");
            this.cmbProductBox.FormattingEnabled = true;
            this.cmbProductBox.Items.AddRange(new object[] {
            resources.GetString("cmbProductBox.Items")});
            this.cmbProductBox.Name = "cmbProductBox";
            this.cmbProductBox.SelectedIndexChanged += new System.EventHandler(this.cmbProductBox_Changed);
            // 
            // TestDriverGUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbProductBox);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.chkGenExcel);
            this.Controls.Add(this.bxDBConsole);
            this.Controls.Add(this.bxRunStats);
            this.Controls.Add(this.lblIterCount);
            this.Controls.Add(this.lblIter);
            this.Controls.Add(this.btnResetForm);
            this.Controls.Add(this.cmbIteration);
            this.Controls.Add(this.lblIteration);
            this.Controls.Add(this.envSelector);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtTestRunning);
            this.Controls.Add(this.slctBrowser);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lstCaseSelect);
            this.Controls.Add(this.txtSuiteRunning);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnDeslctAllAvailable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSlctAllAvailable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstTestSuite);
            this.Controls.Add(this.lstSelectedTest);
            this.Controls.Add(this.btnDeselect);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lstAvailableTest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPlay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TestDriverGUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.slctBrowser.ResumeLayout(false);
            this.slctBrowser.PerformLayout();
            this.bxRunStats.ResumeLayout(false);
            this.bxRunStats.PerformLayout();
            this.bxDBConsole.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstAvailableTest;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDeselect;
        private System.Windows.Forms.ListBox lstSelectedTest;
        private System.Windows.Forms.ComboBox lstTestSuite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSlctAllAvailable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDeslctAllAvailable;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label txtSuiteRunning;
        private System.Windows.Forms.ListBox lstCaseSelect;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rdoChrome;
        private System.Windows.Forms.RadioButton rdoFirefox;
        private System.Windows.Forms.GroupBox slctBrowser;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Label txtTestRunning;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox envSelector;
        #endregion
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnStepExtract;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblIteration;
        private System.Windows.Forms.ComboBox cmbIteration;
        private System.Windows.Forms.Button btnResetForm;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Label lblIter;
        private System.Windows.Forms.Label lblIterCount;
        private System.Windows.Forms.Label lblTstRun;
        private System.Windows.Forms.Label lblTestFail;
        private System.Windows.Forms.Label lblTestPass;
        private System.Windows.Forms.GroupBox bxRunStats;
        private System.Windows.Forms.GroupBox bxDBConsole;
        private System.Windows.Forms.CheckBox chkGenExcel;
        private System.Windows.Forms.RadioButton rdoSafari;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cmbProductBox;
    }
}

