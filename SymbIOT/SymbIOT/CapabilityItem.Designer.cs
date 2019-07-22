namespace SymbIOT
{
    partial class CapabilityItem
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
            this.LBL_KeyName = new System.Windows.Forms.Label();
            this.CBO_Value = new System.Windows.Forms.ComboBox();
            this.NUD_Value = new System.Windows.Forms.NumericUpDown();
            this.TB_Value = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Value)).BeginInit();
            this.SuspendLayout();
            // 
            // LBL_KeyName
            // 
            this.LBL_KeyName.AutoSize = true;
            this.LBL_KeyName.Location = new System.Drawing.Point(8, 8);
            this.LBL_KeyName.Name = "LBL_KeyName";
            this.LBL_KeyName.Size = new System.Drawing.Size(53, 13);
            this.LBL_KeyName.TabIndex = 0;
            this.LBL_KeyName.Text = "KeyName";
            // 
            // CBO_Value
            // 
            this.CBO_Value.FormattingEnabled = true;
            this.CBO_Value.Location = new System.Drawing.Point(11, 24);
            this.CBO_Value.Name = "CBO_Value";
            this.CBO_Value.Size = new System.Drawing.Size(130, 21);
            this.CBO_Value.TabIndex = 1;
            // 
            // NUD_Value
            // 
            this.NUD_Value.Location = new System.Drawing.Point(11, 24);
            this.NUD_Value.Name = "NUD_Value";
            this.NUD_Value.Size = new System.Drawing.Size(130, 20);
            this.NUD_Value.TabIndex = 2;
            this.NUD_Value.ValueChanged += new System.EventHandler(this.NUD_Value_ValueChanged);
            // 
            // TB_Value
            // 
            this.TB_Value.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TB_Value.Location = new System.Drawing.Point(13, 27);
            this.TB_Value.Name = "TB_Value";
            this.TB_Value.Size = new System.Drawing.Size(100, 13);
            this.TB_Value.TabIndex = 3;
            this.TB_Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Value_KeyPress);
            this.TB_Value.Leave += new System.EventHandler(this.TB_Value_Leave);
            // 
            // CapabilityItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TB_Value);
            this.Controls.Add(this.NUD_Value);
            this.Controls.Add(this.CBO_Value);
            this.Controls.Add(this.LBL_KeyName);
            this.Name = "CapabilityItem";
            this.Size = new System.Drawing.Size(250, 50);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Value)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_KeyName;
        private System.Windows.Forms.ComboBox CBO_Value;
        private System.Windows.Forms.NumericUpDown NUD_Value;
        private System.Windows.Forms.TextBox TB_Value;
    }
}
