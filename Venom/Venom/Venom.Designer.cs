﻿namespace VenomNamespace
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
            this.BTN_Clr = new System.Windows.Forms.Button();
            this.RB_Reveal = new System.Windows.Forms.RadioButton();
            this.RB_MQTT = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BTN_MQTT = new System.Windows.Forms.Button();
            this.LED_Internet = new VenomNamespace.LED();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(18, 140);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(100, 20);
            this.TB_IP.TabIndex = 0;
            // 
            // DGV_Data
            // 
            this.DGV_Data.AllowUserToAddRows = false;
            this.DGV_Data.AllowUserToDeleteRows = false;
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(124, 140);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.ReadOnly = true;
            this.DGV_Data.Size = new System.Drawing.Size(487, 237);
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
            this.BTN_Payload.Location = new System.Drawing.Point(18, 300);
            this.BTN_Payload.Name = "BTN_Payload";
            this.BTN_Payload.Size = new System.Drawing.Size(100, 35);
            this.BTN_Payload.TabIndex = 14;
            this.BTN_Payload.Text = "Send Payload";
            this.BTN_Payload.UseVisualStyleBackColor = true;
            this.BTN_Payload.Click += new System.EventHandler(this.BTN_Payload_Click);
            // 
            // BTN_Add
            // 
            this.BTN_Add.Location = new System.Drawing.Point(18, 166);
            this.BTN_Add.Name = "BTN_Add";
            this.BTN_Add.Size = new System.Drawing.Size(100, 23);
            this.BTN_Add.TabIndex = 84;
            this.BTN_Add.Text = "Add IP + Payload";
            this.BTN_Add.UseVisualStyleBackColor = true;
            this.BTN_Add.Click += new System.EventHandler(this.BTN_Add_Click);
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(18, 271);
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
            this.LB_IPs.Location = new System.Drawing.Point(18, 195);
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
            // BTN_Clr
            // 
            this.BTN_Clr.Location = new System.Drawing.Point(18, 341);
            this.BTN_Clr.Name = "BTN_Clr";
            this.BTN_Clr.Size = new System.Drawing.Size(100, 36);
            this.BTN_Clr.TabIndex = 89;
            this.BTN_Clr.Text = "Clear";
            this.BTN_Clr.UseVisualStyleBackColor = true;
            this.BTN_Clr.Click += new System.EventHandler(this.BTN_Clr_Click);
            // 
            // RB_Reveal
            // 
            this.RB_Reveal.AutoSize = true;
            this.RB_Reveal.Enabled = false;
            this.RB_Reveal.Location = new System.Drawing.Point(538, 56);
            this.RB_Reveal.Name = "RB_Reveal";
            this.RB_Reveal.Size = new System.Drawing.Size(76, 17);
            this.RB_Reveal.TabIndex = 90;
            this.RB_Reveal.TabStop = true;
            this.RB_Reveal.Text = "Revelation";
            this.RB_Reveal.UseVisualStyleBackColor = true;
            this.RB_Reveal.Visible = false;
            // 
            // RB_MQTT
            // 
            this.RB_MQTT.AutoSize = true;
            this.RB_MQTT.Location = new System.Drawing.Point(538, 33);
            this.RB_MQTT.Name = "RB_MQTT";
            this.RB_MQTT.Size = new System.Drawing.Size(56, 17);
            this.RB_MQTT.TabIndex = 91;
            this.RB_MQTT.TabStop = true;
            this.RB_MQTT.Text = "MQTT";
            this.RB_MQTT.UseVisualStyleBackColor = true;
            this.RB_MQTT.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(374, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 13);
            this.label2.TabIndex = 93;
            this.label2.Text = "Grey - None | Yellow - Not Subscribed | Green - Good";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(416, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 13);
            this.label3.TabIndex = 94;
            this.label3.Text = "MQTT Status for IP in Textbox";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Visible = false;
            // 
            // BTN_MQTT
            // 
            this.BTN_MQTT.Enabled = false;
            this.BTN_MQTT.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
            this.BTN_MQTT.Location = new System.Drawing.Point(521, 79);
            this.BTN_MQTT.Name = "BTN_MQTT";
            this.BTN_MQTT.Size = new System.Drawing.Size(75, 29);
            this.BTN_MQTT.TabIndex = 95;
            this.BTN_MQTT.Text = "Check MQTT";
            this.BTN_MQTT.UseVisualStyleBackColor = true;
            this.BTN_MQTT.Visible = false;
            this.BTN_MQTT.Click += new System.EventHandler(this.BTN_MQTT_Click);
            // 
            // LED_Internet
            // 
            this.LED_Internet.Enabled = false;
            this.LED_Internet.Location = new System.Drawing.Point(499, 92);
            this.LED_Internet.Name = "LED_Internet";
            this.LED_Internet.Size = new System.Drawing.Size(16, 16);
            this.LED_Internet.TabIndex = 92;
            this.LED_Internet.Visible = false;
            // 
            // Venom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 400);
            this.Controls.Add(this.BTN_MQTT);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LED_Internet);
            this.Controls.Add(this.RB_MQTT);
            this.Controls.Add(this.RB_Reveal);
            this.Controls.Add(this.BTN_Clr);
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
        private System.Windows.Forms.Button BTN_Clr;
        private System.Windows.Forms.RadioButton RB_Reveal;
        private System.Windows.Forms.RadioButton RB_MQTT;
        private LED LED_Internet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BTN_MQTT;
    }
}

