﻿
using System.Windows.Forms;

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
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.TB_LogDir = new System.Windows.Forms.TextBox();
            this.BTN_LogDir = new System.Windows.Forms.Button();
            this.BTN_Payload = new System.Windows.Forms.Button();
            this.BTN_Remove = new System.Windows.Forms.Button();
            this.LB_IPs = new System.Windows.Forms.ListBox();
            this.BTN_Clr = new System.Windows.Forms.Button();
            this.BTN_MakeList = new System.Windows.Forms.Button();
            this.BTN_Import = new System.Windows.Forms.Button();
            this.BTN_Auto = new System.Windows.Forms.Button();
            this.LBL_Auto = new System.Windows.Forms.Label();
            this.LBL_VAR = new System.Windows.Forms.Label();
            this.LBL_OTA = new System.Windows.Forms.Label();
            this.LBL_UD = new System.Windows.Forms.Label();
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
            this.DGV_Data.Location = new System.Drawing.Point(127, 88);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(546, 247);
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
            // BTN_Payload
            // 
            this.BTN_Payload.Location = new System.Drawing.Point(21, 260);
            this.BTN_Payload.Name = "BTN_Payload";
            this.BTN_Payload.Size = new System.Drawing.Size(100, 35);
            this.BTN_Payload.TabIndex = 14;
            this.BTN_Payload.Text = "Run Test List";
            this.BTN_Payload.UseVisualStyleBackColor = true;
            this.BTN_Payload.Click += new System.EventHandler(this.BTN_Payload_Click);
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(21, 231);
            this.BTN_Remove.Name = "BTN_Remove";
            this.BTN_Remove.Size = new System.Drawing.Size(100, 23);
            this.BTN_Remove.TabIndex = 85;
            this.BTN_Remove.Text = "Remove IP";
            this.BTN_Remove.UseVisualStyleBackColor = true;
            this.BTN_Remove.Click += new System.EventHandler(this.BTN_Remove_Click);
            // 
            // LB_IPs
            // 
            this.LB_IPs.FormattingEnabled = true;
            this.LB_IPs.Location = new System.Drawing.Point(21, 88);
            this.LB_IPs.Name = "LB_IPs";
            this.LB_IPs.Size = new System.Drawing.Size(100, 134);
            this.LB_IPs.TabIndex = 87;
            // 
            // BTN_Clr
            // 
            this.BTN_Clr.Location = new System.Drawing.Point(21, 301);
            this.BTN_Clr.Name = "BTN_Clr";
            this.BTN_Clr.Size = new System.Drawing.Size(100, 36);
            this.BTN_Clr.TabIndex = 89;
            this.BTN_Clr.Text = "Clear";
            this.BTN_Clr.UseVisualStyleBackColor = true;
            this.BTN_Clr.Click += new System.EventHandler(this.BTN_Clr_Click);
            // 
            // BTN_MakeList
            // 
            this.BTN_MakeList.Location = new System.Drawing.Point(21, 53);
            this.BTN_MakeList.Name = "BTN_MakeList";
            this.BTN_MakeList.Size = new System.Drawing.Size(97, 23);
            this.BTN_MakeList.TabIndex = 96;
            this.BTN_MakeList.Text = "Test List Control";
            this.BTN_MakeList.UseVisualStyleBackColor = true;
            this.BTN_MakeList.Click += new System.EventHandler(this.BTN_MakeList_Click);
            // 
            // BTN_Import
            // 
            this.BTN_Import.Location = new System.Drawing.Point(127, 53);
            this.BTN_Import.Name = "BTN_Import";
            this.BTN_Import.Size = new System.Drawing.Size(92, 23);
            this.BTN_Import.TabIndex = 97;
            this.BTN_Import.Text = "Import Test List";
            this.BTN_Import.UseVisualStyleBackColor = true;
            this.BTN_Import.Click += new System.EventHandler(this.BTN_Import_Click);
            // 
            // BTN_Auto
            // 
            this.BTN_Auto.Location = new System.Drawing.Point(225, 53);
            this.BTN_Auto.Name = "BTN_Auto";
            this.BTN_Auto.Size = new System.Drawing.Size(97, 23);
            this.BTN_Auto.TabIndex = 98;
            this.BTN_Auto.Text = "Auto Test List";
            this.BTN_Auto.UseVisualStyleBackColor = true;
            this.BTN_Auto.Click += new System.EventHandler(this.BTN_Auto_Click);
            // 
            // LBL_Auto
            // 
            this.LBL_Auto.AutoSize = true;
            this.LBL_Auto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_Auto.Location = new System.Drawing.Point(437, 58);
            this.LBL_Auto.Name = "LBL_Auto";
            this.LBL_Auto.Size = new System.Drawing.Size(58, 15);
            this.LBL_Auto.TabIndex = 99;
            this.LBL_Auto.Text = "PENDING";
            this.LBL_Auto.Visible = false;
            // 
            // LBL_VAR
            // 
            this.LBL_VAR.AutoSize = true;
            this.LBL_VAR.Location = new System.Drawing.Point(526, 58);
            this.LBL_VAR.Name = "LBL_VAR";
            this.LBL_VAR.Size = new System.Drawing.Size(34, 13);
            this.LBL_VAR.TabIndex = 100;
            this.LBL_VAR.Text = "Type:";
            this.LBL_VAR.Visible = false;
            // 
            // LBL_OTA
            // 
            this.LBL_OTA.AutoSize = true;
            this.LBL_OTA.Location = new System.Drawing.Point(366, 58);
            this.LBL_OTA.Name = "LBL_OTA";
            this.LBL_OTA.Size = new System.Drawing.Size(65, 13);
            this.LBL_OTA.TabIndex = 101;
            this.LBL_OTA.Text = "OTA Status:";
            this.LBL_OTA.Visible = false;
            // 
            // LBL_UD
            // 
            this.LBL_UD.AutoSize = true;
            this.LBL_UD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_UD.Location = new System.Drawing.Point(566, 58);
            this.LBL_UD.Name = "LBL_UD";
            this.LBL_UD.Size = new System.Drawing.Size(62, 15);
            this.LBL_UD.TabIndex = 102;
            this.LBL_UD.Text = "UPGRADE";
            this.LBL_UD.Visible = false;
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 361);
            this.Controls.Add(this.LBL_UD);
            this.Controls.Add(this.LBL_OTA);
            this.Controls.Add(this.LBL_VAR);
            this.Controls.Add(this.LBL_Auto);
            this.Controls.Add(this.BTN_Auto);
            this.Controls.Add(this.BTN_Import);
            this.Controls.Add(this.BTN_MakeList);
            this.Controls.Add(this.BTN_Clr);
            this.Controls.Add(this.LB_IPs);
            this.Controls.Add(this.BTN_Remove);
            this.Controls.Add(this.BTN_Payload);
            this.Controls.Add(this.TB_LogDir);
            this.Controls.Add(this.BTN_LogDir);
            this.Controls.Add(this.DGV_Data);
            this.Name = "Venom";
            this.Text = "Venom";
            this.Load += new System.EventHandler(this.Venom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BTN_LogDir;
        private System.Windows.Forms.Button BTN_Payload;
        private System.Windows.Forms.Button BTN_Remove;
        private System.Windows.Forms.Button BTN_Clr;
        public System.Windows.Forms.ListBox LB_IPs;
        public System.Windows.Forms.DataGridView DGV_Data;
        public System.Windows.Forms.TextBox TB_LogDir;
        private Button BTN_Auto;
        public Button BTN_MakeList;
        public Button BTN_Import;
        private Label LBL_Auto;
        private Label LBL_VAR;
        private Label LBL_OTA;
        private Label LBL_UD;
    }
}

