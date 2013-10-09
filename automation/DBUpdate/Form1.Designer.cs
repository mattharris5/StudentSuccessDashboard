namespace DBUpdate
{
    partial class frmDBUpdate
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
            this.lblFuncName = new System.Windows.Forms.Label();
            this.lblTestName = new System.Windows.Forms.Label();
            this.lblStepArgs = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFuncName
            // 
            this.lblFuncName.AutoSize = true;
            this.lblFuncName.Location = new System.Drawing.Point(12, 60);
            this.lblFuncName.Name = "lblFuncName";
            this.lblFuncName.Size = new System.Drawing.Size(107, 17);
            this.lblFuncName.TabIndex = 0;
            this.lblFuncName.Text = "Function Name:";
            // 
            // lblTestName
            // 
            this.lblTestName.AutoSize = true;
            this.lblTestName.Location = new System.Drawing.Point(12, 24);
            this.lblTestName.Name = "lblTestName";
            this.lblTestName.Size = new System.Drawing.Size(85, 17);
            this.lblTestName.TabIndex = 1;
            this.lblTestName.Text = "Test Name: ";
            // 
            // lblStepArgs
            // 
            this.lblStepArgs.AutoSize = true;
            this.lblStepArgs.Location = new System.Drawing.Point(12, 96);
            this.lblStepArgs.Name = "lblStepArgs";
            this.lblStepArgs.Size = new System.Drawing.Size(102, 17);
            this.lblStepArgs.TabIndex = 2;
            this.lblStepArgs.Text = "Step Argument";
            // 
            // frmDBUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 255);
            this.Controls.Add(this.lblStepArgs);
            this.Controls.Add(this.lblTestName);
            this.Controls.Add(this.lblFuncName);
            this.Name = "frmDBUpdate";
            this.Text = "Database Step Updater";
            this.Load += new System.EventHandler(this.frmDBUpdate_load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFuncName;
        private System.Windows.Forms.Label lblTestName;
        private System.Windows.Forms.Label lblStepArgs;
    }
}

