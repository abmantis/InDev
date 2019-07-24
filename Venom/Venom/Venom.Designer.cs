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
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.TB_LogDir = new System.Windows.Forms.TextBox();
            this.BTN_LogDir = new System.Windows.Forms.Button();
            this.TB_Payload = new System.Windows.Forms.TextBox();
            this.BTN_Payload = new System.Windows.Forms.Button();
            this.BTN_Add = new System.Windows.Forms.Button();
            this.BTN_Remove = new System.Windows.Forms.Button();
            this.LB_IPs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(18, 98);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(100, 20);
            this.TB_IP.TabIndex = 0;
            // 
            // DGV_Data
            // 
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(143, 98);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.Size = new System.Drawing.Size(383, 195);
            this.DGV_Data.TabIndex = 2;
            // 
            // TB_LogDir
            // 
            this.TB_LogDir.Location = new System.Drawing.Point(143, 12);
            this.TB_LogDir.Name = "TB_LogDir";
            this.TB_LogDir.Size = new System.Drawing.Size(383, 20);
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
            // TB_Payload
            // 
            this.TB_Payload.Location = new System.Drawing.Point(143, 53);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(383, 20);
            this.TB_Payload.TabIndex = 12;
            // 
            // BTN_Payload
            // 
            this.BTN_Payload.Location = new System.Drawing.Point(18, 258);
            this.BTN_Payload.Name = "BTN_Payload";
            this.BTN_Payload.Size = new System.Drawing.Size(100, 35);
            this.BTN_Payload.TabIndex = 14;
            this.BTN_Payload.Text = "Send Payload";
            this.BTN_Payload.UseVisualStyleBackColor = true;
            this.BTN_Payload.Click += new System.EventHandler(this.BTN_Payload_Click);
            // 
            // BTN_Add
            // 
            this.BTN_Add.Location = new System.Drawing.Point(18, 124);
            this.BTN_Add.Name = "BTN_Add";
            this.BTN_Add.Size = new System.Drawing.Size(100, 23);
            this.BTN_Add.TabIndex = 84;
            this.BTN_Add.Text = "Add IP + Payload";
            this.BTN_Add.UseVisualStyleBackColor = true;
            this.BTN_Add.Click += new System.EventHandler(this.BTN_Add_Click);
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(18, 229);
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
            this.LB_IPs.Location = new System.Drawing.Point(18, 153);
            this.LB_IPs.Name = "LB_IPs";
            this.LB_IPs.Size = new System.Drawing.Size(100, 69);
            this.LB_IPs.TabIndex = 87;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(36, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 88;
            this.label1.Text = "OTA Payload for IP:";
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 320);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LB_IPs);
            this.Controls.Add(this.BTN_Remove);
            this.Controls.Add(this.BTN_Add);
            this.Controls.Add(this.BTN_Payload);
            this.Controls.Add(this.TB_Payload);
            this.Controls.Add(this.TB_LogDir);
            this.Controls.Add(this.BTN_LogDir);
            this.Controls.Add(this.DGV_Data);
            this.Controls.Add(this.TB_IP);
            this.Name = "Venom";
            this.Text = "Venom";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.DataGridView DGV_Data;
        private System.Windows.Forms.TextBox TB_LogDir;
        private System.Windows.Forms.Button BTN_LogDir;
        private System.Windows.Forms.TextBox TB_Payload;
        private System.Windows.Forms.Button BTN_Payload;
        private System.Windows.Forms.Button BTN_Add;
        private System.Windows.Forms.Button BTN_Remove;
        private System.Windows.Forms.ListBox LB_IPs;
        private System.Windows.Forms.Label label1;
    }
}

