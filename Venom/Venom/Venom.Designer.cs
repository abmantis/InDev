
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
            this.components = new System.ComponentModel.Container();
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
            this.TMR_Tick = new System.Windows.Forms.Timer(this.components);
            this.LBL_Rmn = new System.Windows.Forms.Label();
            this.LBL_Time = new System.Windows.Forms.Label();
            this.LBL_Phase = new System.Windows.Forms.Label();
            this.LBL_i = new System.Windows.Forms.Label();
            this.RTB_Diag = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_Data
            // 
            this.DGV_Data.AllowUserToAddRows = false;
            this.DGV_Data.AllowUserToDeleteRows = false;
            this.DGV_Data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_Data.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(127, 88);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(624, 326);
            this.DGV_Data.TabIndex = 2;
            // 
            // TB_LogDir
            // 
            this.TB_LogDir.Location = new System.Drawing.Point(143, 12);
            this.TB_LogDir.Name = "TB_LogDir";
            this.TB_LogDir.Size = new System.Drawing.Size(608, 20);
            this.TB_LogDir.TabIndex = 11;
            // 
            // BTN_LogDir
            // 
            this.BTN_LogDir.Location = new System.Drawing.Point(21, 12);
            this.BTN_LogDir.Name = "BTN_LogDir";
            this.BTN_LogDir.Size = new System.Drawing.Size(116, 23);
            this.BTN_LogDir.TabIndex = 10;
            this.BTN_LogDir.Text = "Set Log Directory";
            this.BTN_LogDir.UseVisualStyleBackColor = true;
            this.BTN_LogDir.Click += new System.EventHandler(this.BTN_LogDir_Click);
            // 
            // BTN_Payload
            // 
            this.BTN_Payload.Location = new System.Drawing.Point(21, 214);
            this.BTN_Payload.Name = "BTN_Payload";
            this.BTN_Payload.Size = new System.Drawing.Size(100, 35);
            this.BTN_Payload.TabIndex = 14;
            this.BTN_Payload.Text = "Run Test List";
            this.BTN_Payload.UseVisualStyleBackColor = true;
            this.BTN_Payload.Click += new System.EventHandler(this.BTN_Payload_Click);
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Enabled = false;
            this.BTN_Remove.Location = new System.Drawing.Point(21, 288);
            this.BTN_Remove.Name = "BTN_Remove";
            this.BTN_Remove.Size = new System.Drawing.Size(100, 23);
            this.BTN_Remove.TabIndex = 85;
            this.BTN_Remove.Text = "Remove IP";
            this.BTN_Remove.UseVisualStyleBackColor = true;
            this.BTN_Remove.Visible = false;
            this.BTN_Remove.Click += new System.EventHandler(this.BTN_Remove_Click);
            // 
            // LB_IPs
            // 
            this.LB_IPs.FormattingEnabled = true;
            this.LB_IPs.Location = new System.Drawing.Point(21, 88);
            this.LB_IPs.Name = "LB_IPs";
            this.LB_IPs.Size = new System.Drawing.Size(100, 108);
            this.LB_IPs.TabIndex = 87;
            // 
            // BTN_Clr
            // 
            this.BTN_Clr.Location = new System.Drawing.Point(21, 255);
            this.BTN_Clr.Name = "BTN_Clr";
            this.BTN_Clr.Size = new System.Drawing.Size(100, 36);
            this.BTN_Clr.TabIndex = 89;
            this.BTN_Clr.Text = "Clear";
            this.BTN_Clr.UseVisualStyleBackColor = true;
            this.BTN_Clr.Click += new System.EventHandler(this.BTN_Clr_Click);
            // 
            // BTN_MakeList
            // 
            this.BTN_MakeList.Location = new System.Drawing.Point(24, 307);
            this.BTN_MakeList.Name = "BTN_MakeList";
            this.BTN_MakeList.Size = new System.Drawing.Size(97, 23);
            this.BTN_MakeList.TabIndex = 96;
            this.BTN_MakeList.Text = "Test List Control";
            this.BTN_MakeList.UseVisualStyleBackColor = true;
            this.BTN_MakeList.Visible = false;
            this.BTN_MakeList.Click += new System.EventHandler(this.BTN_MakeList_Click);
            // 
            // BTN_Import
            // 
            this.BTN_Import.Location = new System.Drawing.Point(24, 336);
            this.BTN_Import.Name = "BTN_Import";
            this.BTN_Import.Size = new System.Drawing.Size(92, 23);
            this.BTN_Import.TabIndex = 97;
            this.BTN_Import.Text = "Import Test List";
            this.BTN_Import.UseVisualStyleBackColor = true;
            this.BTN_Import.Visible = false;
            this.BTN_Import.Click += new System.EventHandler(this.BTN_Import_Click);
            // 
            // BTN_Auto
            // 
            this.BTN_Auto.Location = new System.Drawing.Point(21, 50);
            this.BTN_Auto.Name = "BTN_Auto";
            this.BTN_Auto.Size = new System.Drawing.Size(100, 23);
            this.BTN_Auto.TabIndex = 98;
            this.BTN_Auto.Text = "Auto Test List";
            this.BTN_Auto.UseVisualStyleBackColor = true;
            this.BTN_Auto.Click += new System.EventHandler(this.BTN_Auto_Click);
            // 
            // LBL_Auto
            // 
            this.LBL_Auto.AutoSize = true;
            this.LBL_Auto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_Auto.Location = new System.Drawing.Point(212, 55);
            this.LBL_Auto.Name = "LBL_Auto";
            this.LBL_Auto.Size = new System.Drawing.Size(58, 15);
            this.LBL_Auto.TabIndex = 99;
            this.LBL_Auto.Text = "PENDING";
            this.LBL_Auto.Visible = false;
            // 
            // LBL_VAR
            // 
            this.LBL_VAR.AutoSize = true;
            this.LBL_VAR.Location = new System.Drawing.Point(319, 55);
            this.LBL_VAR.Name = "LBL_VAR";
            this.LBL_VAR.Size = new System.Drawing.Size(34, 13);
            this.LBL_VAR.TabIndex = 100;
            this.LBL_VAR.Text = "Type:";
            this.LBL_VAR.Visible = false;
            // 
            // LBL_OTA
            // 
            this.LBL_OTA.AutoSize = true;
            this.LBL_OTA.Location = new System.Drawing.Point(141, 55);
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
            this.LBL_UD.Location = new System.Drawing.Point(359, 55);
            this.LBL_UD.Name = "LBL_UD";
            this.LBL_UD.Size = new System.Drawing.Size(58, 15);
            this.LBL_UD.TabIndex = 102;
            this.LBL_UD.Text = "PENDING";
            this.LBL_UD.Visible = false;
            // 
            // TMR_Tick
            // 
            this.TMR_Tick.Interval = 1000;
            this.TMR_Tick.Tick += new System.EventHandler(this.TMR_Tick_Tick);
            // 
            // LBL_Rmn
            // 
            this.LBL_Rmn.AutoSize = true;
            this.LBL_Rmn.Location = new System.Drawing.Point(463, 55);
            this.LBL_Rmn.Name = "LBL_Rmn";
            this.LBL_Rmn.Size = new System.Drawing.Size(60, 13);
            this.LBL_Rmn.TabIndex = 103;
            this.LBL_Rmn.Text = "Remaining:";
            this.LBL_Rmn.Visible = false;
            // 
            // LBL_Time
            // 
            this.LBL_Time.AutoSize = true;
            this.LBL_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_Time.Location = new System.Drawing.Point(527, 55);
            this.LBL_Time.Name = "LBL_Time";
            this.LBL_Time.Size = new System.Drawing.Size(51, 15);
            this.LBL_Time.TabIndex = 104;
            this.LBL_Time.Text = "00:00:00";
            this.LBL_Time.Visible = false;
            // 
            // LBL_Phase
            // 
            this.LBL_Phase.AutoSize = true;
            this.LBL_Phase.Location = new System.Drawing.Point(602, 55);
            this.LBL_Phase.Name = "LBL_Phase";
            this.LBL_Phase.Size = new System.Drawing.Size(77, 13);
            this.LBL_Phase.TabIndex = 105;
            this.LBL_Phase.Text = "Current Phase:";
            this.LBL_Phase.Visible = false;
            // 
            // LBL_i
            // 
            this.LBL_i.AutoSize = true;
            this.LBL_i.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBL_i.Location = new System.Drawing.Point(693, 55);
            this.LBL_i.Name = "LBL_i";
            this.LBL_i.Size = new System.Drawing.Size(58, 15);
            this.LBL_i.TabIndex = 106;
            this.LBL_i.Text = "PENDING";
            this.LBL_i.Visible = false;
            // 
            // RTB_Diag
            // 
            this.RTB_Diag.Enabled = false;
            this.RTB_Diag.Location = new System.Drawing.Point(127, 420);
            this.RTB_Diag.Name = "RTB_Diag";
            this.RTB_Diag.Size = new System.Drawing.Size(624, 106);
            this.RTB_Diag.TabIndex = 107;
            this.RTB_Diag.Text = "";
            this.RTB_Diag.Visible = false;
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 538);
            this.Controls.Add(this.RTB_Diag);
            this.Controls.Add(this.LBL_i);
            this.Controls.Add(this.LBL_Phase);
            this.Controls.Add(this.LBL_Time);
            this.Controls.Add(this.LBL_Rmn);
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
        private Timer TMR_Tick;
        private Label LBL_Rmn;
        private Label LBL_Time;
        private Label LBL_Phase;
        private Label LBL_i;
        private RichTextBox RTB_Diag;
    }
}

