namespace SymbIOT
{
    partial class LED
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.but = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // but
            // 
            this.but.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.but.Enabled = false;
            this.but.FlatAppearance.BorderSize = 0;
            this.but.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.but.Location = new System.Drawing.Point(0, 0);
            this.but.Margin = new System.Windows.Forms.Padding(0);
            this.but.Name = "but";
            this.but.Size = new System.Drawing.Size(15, 15);
            this.but.TabIndex = 0;
            this.but.UseVisualStyleBackColor = false;
            // 
            // LED
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but);
            this.Name = "LED";
            this.Size = new System.Drawing.Size(16, 16);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button but;
    }
}
