namespace Frontend
{
    partial class MainForm
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
            this.ExpressionBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.VarsBox = new System.Windows.Forms.TextBox();
            this.Execute = new System.Windows.Forms.Button();
            this.ImportBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Debug = new System.Windows.Forms.Button();
            this.DebugBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ExpressionBox
            // 
            this.ExpressionBox.Location = new System.Drawing.Point(53, 43);
            this.ExpressionBox.Multiline = true;
            this.ExpressionBox.Name = "ExpressionBox";
            this.ExpressionBox.Size = new System.Drawing.Size(803, 263);
            this.ExpressionBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Expression:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 320);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Variables (seperate with comma):";
            // 
            // VarsBox
            // 
            this.VarsBox.Location = new System.Drawing.Point(53, 336);
            this.VarsBox.Name = "VarsBox";
            this.VarsBox.Size = new System.Drawing.Size(340, 20);
            this.VarsBox.TabIndex = 3;
            // 
            // Execute
            // 
            this.Execute.Location = new System.Drawing.Point(53, 374);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(75, 23);
            this.Execute.TabIndex = 7;
            this.Execute.Text = "Execute";
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // ImportBtn
            // 
            this.ImportBtn.Location = new System.Drawing.Point(761, 322);
            this.ImportBtn.Name = "ImportBtn";
            this.ImportBtn.Size = new System.Drawing.Size(95, 23);
            this.ImportBtn.TabIndex = 8;
            this.ImportBtn.Text = "Import Textfile";
            this.ImportBtn.UseVisualStyleBackColor = true;
            this.ImportBtn.Click += new System.EventHandler(this.ImportBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Debug
            // 
            this.Debug.Location = new System.Drawing.Point(400, 374);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(75, 23);
            this.Debug.TabIndex = 9;
            this.Debug.Text = "Debug";
            this.Debug.UseVisualStyleBackColor = true;
            this.Debug.Click += new System.EventHandler(this.Debug_Click);
            // 
            // DebugBox
            // 
            this.DebugBox.Location = new System.Drawing.Point(482, 376);
            this.DebugBox.Name = "DebugBox";
            this.DebugBox.Size = new System.Drawing.Size(374, 20);
            this.DebugBox.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 432);
            this.Controls.Add(this.DebugBox);
            this.Controls.Add(this.Debug);
            this.Controls.Add(this.ImportBtn);
            this.Controls.Add(this.Execute);
            this.Controls.Add(this.VarsBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExpressionBox);
            this.Name = "MainForm";
            this.Text = "BDD Front End";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExpressionBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox VarsBox;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.Button ImportBtn;
        private System.Windows.Forms.OpenFileDialog openInputFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Debug;
        private System.Windows.Forms.TextBox DebugBox;

    }
}

