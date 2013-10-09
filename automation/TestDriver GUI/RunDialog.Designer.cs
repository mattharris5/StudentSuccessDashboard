namespace TestDriver_GUI
{
    partial class RunDialog
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
            this.testNum = new System.Windows.Forms.Label();
            this.testName = new System.Windows.Forms.Label();
            this.endTests = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // testNum
            // 
            this.testNum.AutoSize = true;
            this.testNum.Location = new System.Drawing.Point(12, 10);
            this.testNum.Name = "testNum";
            this.testNum.Size = new System.Drawing.Size(38, 13);
            this.testNum.TabIndex = 0;
            this.testNum.Text = "Test #";
            // 
            // testName
            // 
            this.testName.AutoSize = true;
            this.testName.Location = new System.Drawing.Point(12, 30);
            this.testName.Name = "testName";
            this.testName.Size = new System.Drawing.Size(59, 13);
            this.testName.TabIndex = 1;
            this.testName.Text = "Test Name";
            // 
            // endTests
            // 
            this.endTests.Location = new System.Drawing.Point(12, 50);
            this.endTests.Name = "endTests";
            this.endTests.Size = new System.Drawing.Size(260, 23);
            this.endTests.TabIndex = 2;
            this.endTests.Text = "End Tests";
            this.endTests.UseVisualStyleBackColor = true;
            // 
            // RunDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 79);
            this.Controls.Add(this.endTests);
            this.Controls.Add(this.testName);
            this.Controls.Add(this.testNum);
            this.Name = "RunDialog";
            this.Text = "RunDialog";
            this.Load += new System.EventHandler(this.RunDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label testNum;
        private System.Windows.Forms.Label testName;
        private System.Windows.Forms.Button endTests;
    }
}