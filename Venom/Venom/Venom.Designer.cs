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
            this.TB_IP = new System.Windows.Forms.TextBox();
            this.BTN_Ping = new System.Windows.Forms.Button();
            this.BTN_Add = new System.Windows.Forms.Button();
            this.LB_IPs = new System.Windows.Forms.ListBox();
            this.BTN_Remove = new System.Windows.Forms.Button();
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.BTN_LogDir = new System.Windows.Forms.Button();
            this.TB_LogDir = new System.Windows.Forms.TextBox();
            this.TB_Payload = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(1, 53);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(100, 20);
            this.TB_IP.TabIndex = 0;
            // 
            // BTN_Ping
            // 
            this.BTN_Ping.Location = new System.Drawing.Point(1, 213);
            this.BTN_Ping.Name = "BTN_Ping";
            this.BTN_Ping.Size = new System.Drawing.Size(100, 34);
            this.BTN_Ping.TabIndex = 1;
            this.BTN_Ping.Text = "Start Auto Ping";
            this.BTN_Ping.UseVisualStyleBackColor = true;
            this.BTN_Ping.Click += new System.EventHandler(this.BTN_Ping_Click);
            // 
            // BTN_Add
            // 
            this.BTN_Add.Location = new System.Drawing.Point(1, 80);
            this.BTN_Add.Name = "BTN_Add";
            this.BTN_Add.Size = new System.Drawing.Size(100, 23);
            this.BTN_Add.TabIndex = 3;
            this.BTN_Add.Text = "Add IP";
            this.BTN_Add.UseVisualStyleBackColor = true;
            this.BTN_Add.Click += new System.EventHandler(this.BTN_Add_Click);
            // 
            // LB_IPs
            // 
            this.LB_IPs.FormattingEnabled = true;
            this.LB_IPs.Location = new System.Drawing.Point(1, 109);
            this.LB_IPs.Name = "LB_IPs";
            this.LB_IPs.Size = new System.Drawing.Size(100, 69);
            this.LB_IPs.TabIndex = 4;
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(1, 184);
            this.BTN_Remove.Name = "BTN_Remove";
            this.BTN_Remove.Size = new System.Drawing.Size(100, 23);
            this.BTN_Remove.TabIndex = 5;
            this.BTN_Remove.Text = "Remove IP";
            this.BTN_Remove.UseVisualStyleBackColor = true;
            this.BTN_Remove.Click += new System.EventHandler(this.BTN_Remove_Click);
            // 
            // DGV_Data
            // 
            this.DGV_Data.AllowUserToAddRows = false;
            this.DGV_Data.AllowUserToDeleteRows = false;
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(115, 82);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(508, 165);
            this.DGV_Data.TabIndex = 7;
            // 
            // BTN_LogDir
            // 
            this.BTN_LogDir.Location = new System.Drawing.Point(115, 23);
            this.BTN_LogDir.Name = "BTN_LogDir";
            this.BTN_LogDir.Size = new System.Drawing.Size(119, 23);
            this.BTN_LogDir.TabIndex = 8;
            this.BTN_LogDir.Text = "Set Log Directory";
            this.BTN_LogDir.UseVisualStyleBackColor = true;
            this.BTN_LogDir.Click += new System.EventHandler(this.BTN_LogDir_Click);
            // 
            // TB_LogDir
            // 
            this.TB_LogDir.Location = new System.Drawing.Point(240, 23);
            this.TB_LogDir.Name = "TB_LogDir";
            this.TB_LogDir.Size = new System.Drawing.Size(383, 20);
            this.TB_LogDir.TabIndex = 9;
            // 
            // TB_Payload
            // 
            this.TB_Payload.Location = new System.Drawing.Point(115, 53);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(492, 20);
            this.TB_Payload.TabIndex = 10;
            this.TB_Payload.Text = "OTA Payload";
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 273);
            this.Controls.Add(this.TB_Payload);
            this.Controls.Add(this.TB_LogDir);
            this.Controls.Add(this.BTN_LogDir);
            this.Controls.Add(this.DGV_Data);
            this.Controls.Add(this.BTN_Remove);
            this.Controls.Add(this.LB_IPs);
            this.Controls.Add(this.BTN_Add);
            this.Controls.Add(this.BTN_Ping);
            this.Controls.Add(this.TB_IP);
            this.Name = "Venom";
            this.Text = "Venom";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.Button BTN_Ping;
        private System.Windows.Forms.Button BTN_Add;
        private System.Windows.Forms.ListBox LB_IPs;
        private System.Windows.Forms.Button BTN_Remove;
        private System.Windows.Forms.DataGridView DGV_Data;
        private System.Windows.Forms.Button BTN_LogDir;
        private System.Windows.Forms.TextBox TB_LogDir;
        private System.Windows.Forms.TextBox TB_Payload;
    }
}
