namespace VenomNamespace
{
    partial class Venom
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
            this.TB_Log = new System.Windows.Forms.TextBox();
            this.TB_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TB_Log
            // 
            this.TB_Log.Location = new System.Drawing.Point(87, 101);
            this.TB_Log.Name = "TB_Log";
            this.TB_Log.Size = new System.Drawing.Size(100, 20);
            this.TB_Log.TabIndex = 0;
            // 
            // TB_label
            // 
            this.TB_label.AutoSize = true;
            this.TB_label.Location = new System.Drawing.Point(84, 85);
            this.TB_label.Name = "TB_label";
            this.TB_label.Size = new System.Drawing.Size(91, 13);
            this.TB_label.TabIndex = 1;
            this.TB_label.Text = "First Log...to die...";
            this.TB_label.Click += new System.EventHandler(this.Label1_Click);
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.TB_label);
            this.Controls.Add(this.TB_Log);
            this.Name = "Venom";
            this.Text = "Venom";
            this.Load += new System.EventHandler(this.Venom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Log;
        private System.Windows.Forms.Label TB_label;
    }
}

