
using System.Windows.Forms;

namespace VenomNamespace
{
    partial class Revelation_OTA
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
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.TB_LogDir = new System.Windows.Forms.TextBox();
            this.BTN_LogDir = new System.Windows.Forms.Button();
            this.BTN_Start = new System.Windows.Forms.Button();
            this.BTN_Clr = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TB_Payload = new System.Windows.Forms.TextBox();
            this.TB_Max = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_Version = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_Data
            // 
            this.DGV_Data.AllowUserToAddRows = false;
            this.DGV_Data.AllowUserToDeleteRows = false;
            this.DGV_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_Data.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(143, 88);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(530, 214);
            this.DGV_Data.TabIndex = 2;
            // 
            // TB_LogDir
            // 
            this.TB_LogDir.Location = new System.Drawing.Point(143, 12);
            this.TB_LogDir.Name = "TB_LogDir";
            this.TB_LogDir.Size = new System.Drawing.Size(530, 20);
            this.TB_LogDir.TabIndex = 11;
            // 
            // BTN_LogDir
            // 
            this.BTN_LogDir.Location = new System.Drawing.Point(18, 12);
            this.BTN_LogDir.Name = "BTN_LogDir";
            this.BTN_LogDir.Size = new System.Drawing.Size(119, 23);
            this.BTN_LogDir.TabIndex = 10;
            this.BTN_LogDir.Text = "Set Log Directory";
            this.BTN_LogDir.UseVisualStyleBackColor = true;
            this.BTN_LogDir.Click += new System.EventHandler(this.BTN_LogDir_Click);
            // 
            // BTN_Start
            // 
            this.BTN_Start.Location = new System.Drawing.Point(12, 225);
            this.BTN_Start.Name = "BTN_Start";
            this.BTN_Start.Size = new System.Drawing.Size(125, 35);
            this.BTN_Start.TabIndex = 14;
            this.BTN_Start.Text = "Start";
            this.BTN_Start.UseVisualStyleBackColor = true;
            this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // BTN_Clr
            // 
            this.BTN_Clr.Location = new System.Drawing.Point(12, 266);
            this.BTN_Clr.Name = "BTN_Clr";
            this.BTN_Clr.Size = new System.Drawing.Size(125, 36);
            this.BTN_Clr.TabIndex = 89;
            this.BTN_Clr.Text = "Clear";
            this.BTN_Clr.UseVisualStyleBackColor = true;
            this.BTN_Clr.Click += new System.EventHandler(this.BTN_Clr_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Payload URL:";
            // 
            // TB_Payload
            // 
            this.TB_Payload.Location = new System.Drawing.Point(143, 55);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(530, 20);
            this.TB_Payload.TabIndex = 103;
            // 
            // TB_Max
            // 
            this.TB_Max.Location = new System.Drawing.Point(103, 127);
            this.TB_Max.Name = "TB_Max";
            this.TB_Max.Size = new System.Drawing.Size(34, 20);
            this.TB_Max.TabIndex = 102;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 101;
            this.label2.Text = "Maximum Amount:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 100;
            this.label1.Text = "Final Version:";
            // 
            // TB_Version
            // 
            this.TB_Version.Location = new System.Drawing.Point(103, 171);
            this.TB_Version.Name = "TB_Version";
            this.TB_Version.Size = new System.Drawing.Size(34, 20);
            this.TB_Version.TabIndex = 99;
            // 
            // Revelation_OTA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 318);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_Payload);
            this.Controls.Add(this.TB_Max);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_Version);
            this.Controls.Add(this.BTN_Clr);
            this.Controls.Add(this.BTN_Start);
            this.Controls.Add(this.TB_LogDir);
            this.Controls.Add(this.BTN_LogDir);
            this.Controls.Add(this.DGV_Data);
            this.Name = "Revelation_OTA";
            this.Text = "Revelation_OTA";
            this.Load += new System.EventHandler(this.Venom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BTN_LogDir;
        private System.Windows.Forms.Button BTN_Start;
        private System.Windows.Forms.Button BTN_Clr;
        public System.Windows.Forms.DataGridView DGV_Data;
        public System.Windows.Forms.TextBox TB_LogDir;
        private Label label3;
        private TextBox TB_Payload;
        private TextBox TB_Max;
        private Label label2;
        private Label label1;
        private TextBox TB_Version;
    }
}

