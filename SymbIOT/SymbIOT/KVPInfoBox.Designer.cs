namespace SymbIOT
{
    partial class KVPInfoBox
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
            this.LBL_KVPName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.RTB_Description = new System.Windows.Forms.RichTextBox();
            this.LBL_Enums = new System.Windows.Forms.Label();
            this.TB_Key = new System.Windows.Forms.TextBox();
            this.TB_Size = new System.Windows.Forms.TextBox();
            this.TB_Format = new System.Windows.Forms.TextBox();
            this.CBO_Enums = new System.Windows.Forms.ComboBox();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LBL_KVPName
            // 
            this.LBL_KVPName.AutoSize = true;
            this.LBL_KVPName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_KVPName.Location = new System.Drawing.Point(12, 9);
            this.LBL_KVPName.Name = "LBL_KVPName";
            this.LBL_KVPName.Size = new System.Drawing.Size(243, 20);
            this.LBL_KVPName.TabIndex = 0;
            this.LBL_KVPName.Text = "XCat_PersistentInfo_Version";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Key:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Data Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Display Format:";
            // 
            // RTB_Description
            // 
            this.RTB_Description.Location = new System.Drawing.Point(12, 32);
            this.RTB_Description.Name = "RTB_Description";
            this.RTB_Description.Size = new System.Drawing.Size(443, 58);
            this.RTB_Description.TabIndex = 4;
            this.RTB_Description.Text = "";
            // 
            // LBL_Enums
            // 
            this.LBL_Enums.AutoSize = true;
            this.LBL_Enums.Location = new System.Drawing.Point(21, 168);
            this.LBL_Enums.Name = "LBL_Enums";
            this.LBL_Enums.Size = new System.Drawing.Size(74, 13);
            this.LBL_Enums.TabIndex = 5;
            this.LBL_Enums.Text = "Enumerations:";
            this.LBL_Enums.Visible = false;
            // 
            // TB_Key
            // 
            this.TB_Key.Location = new System.Drawing.Point(97, 92);
            this.TB_Key.Name = "TB_Key";
            this.TB_Key.Size = new System.Drawing.Size(153, 20);
            this.TB_Key.TabIndex = 6;
            // 
            // TB_Size
            // 
            this.TB_Size.Location = new System.Drawing.Point(97, 118);
            this.TB_Size.Name = "TB_Size";
            this.TB_Size.Size = new System.Drawing.Size(153, 20);
            this.TB_Size.TabIndex = 7;
            // 
            // TB_Format
            // 
            this.TB_Format.Location = new System.Drawing.Point(97, 143);
            this.TB_Format.Name = "TB_Format";
            this.TB_Format.Size = new System.Drawing.Size(153, 20);
            this.TB_Format.TabIndex = 8;
            // 
            // CBO_Enums
            // 
            this.CBO_Enums.FormattingEnabled = true;
            this.CBO_Enums.Location = new System.Drawing.Point(97, 165);
            this.CBO_Enums.Name = "CBO_Enums";
            this.CBO_Enums.Size = new System.Drawing.Size(212, 21);
            this.CBO_Enums.TabIndex = 9;
            this.CBO_Enums.Visible = false;
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(392, 165);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(75, 23);
            this.BTN_Close.TabIndex = 10;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // KVPInfoBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 194);
            this.Controls.Add(this.BTN_Close);
            this.Controls.Add(this.CBO_Enums);
            this.Controls.Add(this.TB_Format);
            this.Controls.Add(this.TB_Size);
            this.Controls.Add(this.TB_Key);
            this.Controls.Add(this.LBL_Enums);
            this.Controls.Add(this.RTB_Description);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LBL_KVPName);
            this.Name = "KVPInfoBox";
            this.Text = "KVPInfoBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_KVPName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox RTB_Description;
        private System.Windows.Forms.Label LBL_Enums;
        private System.Windows.Forms.TextBox TB_Key;
        private System.Windows.Forms.TextBox TB_Size;
        private System.Windows.Forms.TextBox TB_Format;
        private System.Windows.Forms.ComboBox CBO_Enums;
        private System.Windows.Forms.Button BTN_Close;
    }
}