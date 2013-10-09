namespace TestDriver_GUI
{
    partial class dbUpdater
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
            this.lblTestName = new System.Windows.Forms.Label();
            this.lblFuncName = new System.Windows.Forms.Label();
            this.cmbFuncName = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStpNumber = new System.Windows.Forms.Label();
            this.gpDataItems = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // lblTestName
            // 
            this.lblTestName.AutoSize = true;
            this.lblTestName.Location = new System.Drawing.Point(9, 20);
            this.lblTestName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTestName.Name = "lblTestName";
            this.lblTestName.Size = new System.Drawing.Size(65, 13);
            this.lblTestName.TabIndex = 2;
            this.lblTestName.Text = "Test Name: ";
            // 
            // lblFuncName
            // 
            this.lblFuncName.AutoSize = true;
            this.lblFuncName.Location = new System.Drawing.Point(9, 49);
            this.lblFuncName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFuncName.Name = "lblFuncName";
            this.lblFuncName.Size = new System.Drawing.Size(82, 13);
            this.lblFuncName.TabIndex = 3;
            this.lblFuncName.Text = "Function Name:";
            // 
            // cmbFuncName
            // 
            this.cmbFuncName.FormattingEnabled = true;
            this.cmbFuncName.Location = new System.Drawing.Point(94, 49);
            this.cmbFuncName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbFuncName.Name = "cmbFuncName";
            this.cmbFuncName.Size = new System.Drawing.Size(151, 21);
            this.cmbFuncName.TabIndex = 5;
            this.cmbFuncName.SelectedIndexChanged += new System.EventHandler(this.cmbFuncName_Selected_Changed);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(28, 103);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 20);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Update";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(159, 103);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 20);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStpNumber
            // 
            this.lblStpNumber.AutoSize = true;
            this.lblStpNumber.Location = new System.Drawing.Point(256, 20);
            this.lblStpNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStpNumber.Name = "lblStpNumber";
            this.lblStpNumber.Size = new System.Drawing.Size(94, 13);
            this.lblStpNumber.TabIndex = 9;
            this.lblStpNumber.Text = "Number Of Steps: ";
            // 
            // gpDataItems
            // 
            this.gpDataItems.Location = new System.Drawing.Point(259, 36);
            this.gpDataItems.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gpDataItems.Name = "gpDataItems";
            this.gpDataItems.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gpDataItems.Size = new System.Drawing.Size(1406, 88);
            this.gpDataItems.TabIndex = 10;
            this.gpDataItems.TabStop = false;
            this.gpDataItems.Text = "Database Items";
            this.gpDataItems.Visible = false;
            // 
            // dbUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1674, 132);
            this.Controls.Add(this.gpDataItems);
            this.Controls.Add(this.lblStpNumber);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbFuncName);
            this.Controls.Add(this.lblFuncName);
            this.Controls.Add(this.lblTestName);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "dbUpdater";
            this.Text = "Database Step Updater";
            this.Load += new System.EventHandler(this.dbUpdater_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTestName;
        private System.Windows.Forms.Label lblFuncName;
        private System.Windows.Forms.ComboBox cmbFuncName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStpNumber;
        private System.Windows.Forms.GroupBox gpDataItems;
    }
}