
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
            this.components = new System.ComponentModel.Container();
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.TB_LogDir = new System.Windows.Forms.TextBox();
            this.BTN_LogDir = new System.Windows.Forms.Button();
            this.BTN_Start = new System.Windows.Forms.Button();
            this.BTN_Clr = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TB_Payload = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_Version = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.LBL_Time = new System.Windows.Forms.Label();
            this.TMR_Tick = new System.Windows.Forms.Timer(this.components);
            this.BTN_Import = new System.Windows.Forms.Button();
            this.BTN_Rmv = new System.Windows.Forms.Button();
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
            this.DGV_Data.Location = new System.Drawing.Point(155, 88);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(544, 213);
            this.DGV_Data.TabIndex = 2;
            this.DGV_Data.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Data_CellContentClick);
            // 
            // TB_LogDir
            // 
            this.TB_LogDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_LogDir.Location = new System.Drawing.Point(155, 12);
            this.TB_LogDir.Name = "TB_LogDir";
            this.TB_LogDir.Size = new System.Drawing.Size(544, 20);
            this.TB_LogDir.TabIndex = 11;
            // 
            // BTN_LogDir
            // 
            this.BTN_LogDir.Location = new System.Drawing.Point(26, 12);
            this.BTN_LogDir.Name = "BTN_LogDir";
            this.BTN_LogDir.Size = new System.Drawing.Size(119, 23);
            this.BTN_LogDir.TabIndex = 10;
            this.BTN_LogDir.Text = "Set Log Directory";
            this.BTN_LogDir.UseVisualStyleBackColor = true;
            this.BTN_LogDir.Click += new System.EventHandler(this.BTN_LogDir_Click);
            // 
            // BTN_Start
            // 
            this.BTN_Start.Location = new System.Drawing.Point(12, 183);
            this.BTN_Start.Name = "BTN_Start";
            this.BTN_Start.Size = new System.Drawing.Size(125, 35);
            this.BTN_Start.TabIndex = 14;
            this.BTN_Start.Text = "Start";
            this.BTN_Start.UseVisualStyleBackColor = true;
            this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // BTN_Clr
            // 
            this.BTN_Clr.Location = new System.Drawing.Point(12, 224);
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
            this.TB_Payload.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Payload.Enabled = false;
            this.TB_Payload.Location = new System.Drawing.Point(155, 55);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(544, 20);
            this.TB_Payload.TabIndex = 103;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 100;
            this.label1.Text = "Target Version:";
            // 
            // TB_Version
            // 
            this.TB_Version.Enabled = false;
            this.TB_Version.Location = new System.Drawing.Point(94, 110);
            this.TB_Version.Name = "TB_Version";
            this.TB_Version.Size = new System.Drawing.Size(51, 20);
            this.TB_Version.TabIndex = 99;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 106;
            this.label4.Text = "Time Until Check:";
            // 
            // LBL_Time
            // 
            this.LBL_Time.AutoSize = true;
            this.LBL_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_Time.Location = new System.Drawing.Point(94, 87);
            this.LBL_Time.Name = "LBL_Time";
            this.LBL_Time.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LBL_Time.Size = new System.Drawing.Size(51, 15);
            this.LBL_Time.TabIndex = 105;
            this.LBL_Time.Text = "00:00:00";
            // 
            // TMR_Tick
            // 
            this.TMR_Tick.Interval = 1000;
            this.TMR_Tick.Tick += new System.EventHandler(this.TMR_Tick_Tick);
            // 
            // BTN_Import
            // 
            this.BTN_Import.Location = new System.Drawing.Point(13, 142);
            this.BTN_Import.Name = "BTN_Import";
            this.BTN_Import.Size = new System.Drawing.Size(125, 35);
            this.BTN_Import.TabIndex = 107;
            this.BTN_Import.Text = "Import";
            this.BTN_Import.UseVisualStyleBackColor = true;
            this.BTN_Import.Click += new System.EventHandler(this.BTN_Import_Click);
            // 
            // BTN_Rmv
            // 
            this.BTN_Rmv.Enabled = false;
            this.BTN_Rmv.Location = new System.Drawing.Point(13, 266);
            this.BTN_Rmv.Name = "BTN_Rmv";
            this.BTN_Rmv.Size = new System.Drawing.Size(125, 36);
            this.BTN_Rmv.TabIndex = 108;
            this.BTN_Rmv.Text = "Remove";
            this.BTN_Rmv.UseVisualStyleBackColor = true;
            this.BTN_Rmv.Click += new System.EventHandler(this.BTN_Rmv_Click);
            // 
            // Revelation_OTA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 313);
            this.Controls.Add(this.BTN_Rmv);
            this.Controls.Add(this.BTN_Import);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LBL_Time);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_Payload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_Version);
            this.Controls.Add(this.BTN_Clr);
            this.Controls.Add(this.BTN_Start);
            this.Controls.Add(this.TB_LogDir);
            this.Controls.Add(this.BTN_LogDir);
            this.Controls.Add(this.DGV_Data);
            this.Name = "Revelation_OTA";
            this.Text = "Venom.Revelation_OTA";
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
        private Label label1;
        private TextBox TB_Version;
        private Label label4;
        private Label LBL_Time;
        private Timer TMR_Tick;
        private Button BTN_Import;
        private Button BTN_Rmv;
    }
}

