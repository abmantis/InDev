
using System.Windows.Forms;

namespace VenomNamespace
{
    partial class MQTT_URL
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
            this.BTN_Payload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_NARS = new System.Windows.Forms.CheckBox();
            this.CB_NARP = new System.Windows.Forms.CheckBox();
            this.CB_EMEAP = new System.Windows.Forms.CheckBox();
            this.CB_Custom = new System.Windows.Forms.CheckBox();
            this.TB_Custom = new System.Windows.Forms.TextBox();
            this.BTN_Reset = new System.Windows.Forms.Button();
            this.BTN_GET = new System.Windows.Forms.Button();
            this.CB_Suppress = new System.Windows.Forms.CheckBox();
            this.CB_Org = new System.Windows.Forms.CheckBox();
            this.CB_Legacy = new System.Windows.Forms.CheckBox();
            this.CB_MQTT = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(129, 33);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(142, 20);
            this.TB_IP.TabIndex = 11;
            // 
            // BTN_Payload
            // 
            this.BTN_Payload.Location = new System.Drawing.Point(31, 205);
            this.BTN_Payload.Name = "BTN_Payload";
            this.BTN_Payload.Size = new System.Drawing.Size(100, 35);
            this.BTN_Payload.TabIndex = 14;
            this.BTN_Payload.Text = "Set";
            this.BTN_Payload.UseVisualStyleBackColor = true;
            this.BTN_Payload.Click += new System.EventHandler(this.BTN_Payload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Target IP Address:";
            // 
            // CB_NARS
            // 
            this.CB_NARS.AutoSize = true;
            this.CB_NARS.Location = new System.Drawing.Point(31, 76);
            this.CB_NARS.Name = "CB_NARS";
            this.CB_NARS.Size = new System.Drawing.Size(88, 17);
            this.CB_NARS.TabIndex = 16;
            this.CB_NARS.Text = "NAR Staging";
            this.CB_NARS.UseVisualStyleBackColor = true;
            this.CB_NARS.CheckedChanged += new System.EventHandler(this.CB_NARS_CheckedChanged);
            // 
            // CB_NARP
            // 
            this.CB_NARP.AutoSize = true;
            this.CB_NARP.Location = new System.Drawing.Point(168, 76);
            this.CB_NARP.Name = "CB_NARP";
            this.CB_NARP.Size = new System.Drawing.Size(103, 17);
            this.CB_NARP.TabIndex = 17;
            this.CB_NARP.Text = "NAR Production";
            this.CB_NARP.UseVisualStyleBackColor = true;
            this.CB_NARP.CheckedChanged += new System.EventHandler(this.CB_NARP_CheckedChanged);
            // 
            // CB_EMEAP
            // 
            this.CB_EMEAP.AutoSize = true;
            this.CB_EMEAP.Location = new System.Drawing.Point(305, 76);
            this.CB_EMEAP.Name = "CB_EMEAP";
            this.CB_EMEAP.Size = new System.Drawing.Size(110, 17);
            this.CB_EMEAP.TabIndex = 18;
            this.CB_EMEAP.Text = "EMEA Production";
            this.CB_EMEAP.UseVisualStyleBackColor = true;
            this.CB_EMEAP.CheckedChanged += new System.EventHandler(this.CB_EMEAP_CheckedChanged);
            // 
            // CB_Custom
            // 
            this.CB_Custom.AutoSize = true;
            this.CB_Custom.Location = new System.Drawing.Point(31, 116);
            this.CB_Custom.Name = "CB_Custom";
            this.CB_Custom.Size = new System.Drawing.Size(61, 17);
            this.CB_Custom.TabIndex = 19;
            this.CB_Custom.Text = "Custom";
            this.CB_Custom.UseVisualStyleBackColor = true;
            this.CB_Custom.CheckedChanged += new System.EventHandler(this.CB_Custom_CheckedChanged);
            // 
            // TB_Custom
            // 
            this.TB_Custom.Location = new System.Drawing.Point(98, 113);
            this.TB_Custom.Name = "TB_Custom";
            this.TB_Custom.Size = new System.Drawing.Size(170, 20);
            this.TB_Custom.TabIndex = 20;
            this.TB_Custom.Visible = false;
            // 
            // BTN_Reset
            // 
            this.BTN_Reset.Location = new System.Drawing.Point(305, 205);
            this.BTN_Reset.Name = "BTN_Reset";
            this.BTN_Reset.Size = new System.Drawing.Size(100, 35);
            this.BTN_Reset.TabIndex = 21;
            this.BTN_Reset.Text = "Reset";
            this.BTN_Reset.UseVisualStyleBackColor = true;
            this.BTN_Reset.Click += new System.EventHandler(this.BTN_Reset_Click);
            // 
            // BTN_GET
            // 
            this.BTN_GET.Location = new System.Drawing.Point(168, 205);
            this.BTN_GET.Name = "BTN_GET";
            this.BTN_GET.Size = new System.Drawing.Size(100, 35);
            this.BTN_GET.TabIndex = 22;
            this.BTN_GET.Text = "Get";
            this.BTN_GET.UseVisualStyleBackColor = true;
            this.BTN_GET.Click += new System.EventHandler(this.BTN_GET_Click);
            // 
            // CB_Suppress
            // 
            this.CB_Suppress.AutoSize = true;
            this.CB_Suppress.Location = new System.Drawing.Point(305, 36);
            this.CB_Suppress.Name = "CB_Suppress";
            this.CB_Suppress.Size = new System.Drawing.Size(65, 17);
            this.CB_Suppress.TabIndex = 23;
            this.CB_Suppress.Text = "No Help";
            this.CB_Suppress.UseVisualStyleBackColor = true;
            // 
            // CB_Org
            // 
            this.CB_Org.AutoSize = true;
            this.CB_Org.Location = new System.Drawing.Point(214, 160);
            this.CB_Org.Name = "CB_Org";
            this.CB_Org.Size = new System.Drawing.Size(57, 17);
            this.CB_Org.TabIndex = 24;
            this.CB_Org.Text = "Org ID";
            this.CB_Org.UseMnemonic = false;
            this.CB_Org.UseVisualStyleBackColor = true;
            this.CB_Org.CheckedChanged += new System.EventHandler(this.CB_Org_CheckedChanged);
            // 
            // CB_Legacy
            // 
            this.CB_Legacy.AutoSize = true;
            this.CB_Legacy.Enabled = false;
            this.CB_Legacy.Location = new System.Drawing.Point(262, 174);
            this.CB_Legacy.Name = "CB_Legacy";
            this.CB_Legacy.Size = new System.Drawing.Size(87, 17);
            this.CB_Legacy.TabIndex = 25;
            this.CB_Legacy.Text = "NAR Legacy";
            this.CB_Legacy.UseVisualStyleBackColor = true;
            this.CB_Legacy.Visible = false;
            // 
            // CB_MQTT
            // 
            this.CB_MQTT.AutoSize = true;
            this.CB_MQTT.Location = new System.Drawing.Point(98, 160);
            this.CB_MQTT.Name = "CB_MQTT";
            this.CB_MQTT.Size = new System.Drawing.Size(82, 17);
            this.CB_MQTT.TabIndex = 26;
            this.CB_MQTT.Text = "MQTT URL";
            this.CB_MQTT.UseVisualStyleBackColor = true;
            this.CB_MQTT.CheckedChanged += new System.EventHandler(this.CB_MQTT_CheckedChanged);
            // 
            // MQTT_URL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 252);
            this.Controls.Add(this.CB_MQTT);
            this.Controls.Add(this.CB_Legacy);
            this.Controls.Add(this.CB_Org);
            this.Controls.Add(this.CB_Suppress);
            this.Controls.Add(this.BTN_GET);
            this.Controls.Add(this.BTN_Reset);
            this.Controls.Add(this.TB_Custom);
            this.Controls.Add(this.CB_Custom);
            this.Controls.Add(this.CB_EMEAP);
            this.Controls.Add(this.CB_NARP);
            this.Controls.Add(this.CB_NARS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTN_Payload);
            this.Controls.Add(this.TB_IP);
            this.Name = "MQTT_URL";
            this.Text = "MQTT_URL";
            this.Load += new System.EventHandler(this.MQTT_URL_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BTN_Payload;
        public System.Windows.Forms.TextBox TB_IP;
        private Label label1;
        private CheckBox CB_NARS;
        private CheckBox CB_NARP;
        private CheckBox CB_EMEAP;
        private CheckBox CB_Custom;
        public TextBox TB_Custom;
        private Button BTN_Reset;
        private Button BTN_GET;
        private CheckBox CB_Suppress;
        private CheckBox CB_Org;
        private CheckBox CB_Legacy;
        private CheckBox CB_MQTT;
    }
}

