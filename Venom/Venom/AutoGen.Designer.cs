namespace VenomNamespace
{
    partial class AutoGen
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
            this.label1 = new System.Windows.Forms.Label();
            this.TB_UP = new System.Windows.Forms.TextBox();
            this.TB_DWN = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BTN_Gen = new System.Windows.Forms.Button();
            this.CB_Product = new System.Windows.Forms.ComboBox();
            this.TB_Other = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CB_Variant = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(86, 12);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(127, 20);
            this.TB_IP.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address:";
            // 
            // TB_UP
            // 
            this.TB_UP.Location = new System.Drawing.Point(132, 58);
            this.TB_UP.Name = "TB_UP";
            this.TB_UP.Size = new System.Drawing.Size(572, 20);
            this.TB_UP.TabIndex = 7;
            // 
            // TB_DWN
            // 
            this.TB_DWN.Location = new System.Drawing.Point(132, 96);
            this.TB_DWN.Name = "TB_DWN";
            this.TB_DWN.Size = new System.Drawing.Size(572, 20);
            this.TB_DWN.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Upgrade Payload:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Downgrade Payload:";
            // 
            // BTN_Gen
            // 
            this.BTN_Gen.Location = new System.Drawing.Point(20, 136);
            this.BTN_Gen.Name = "BTN_Gen";
            this.BTN_Gen.Size = new System.Drawing.Size(94, 23);
            this.BTN_Gen.TabIndex = 24;
            this.BTN_Gen.Text = "Generate Tests";
            this.BTN_Gen.UseVisualStyleBackColor = true;
            this.BTN_Gen.Click += new System.EventHandler(this.BTN_Gen_Click);
            // 
            // CB_Product
            // 
            this.CB_Product.FormattingEnabled = true;
            this.CB_Product.Location = new System.Drawing.Point(422, 11);
            this.CB_Product.Name = "CB_Product";
            this.CB_Product.Size = new System.Drawing.Size(158, 21);
            this.CB_Product.TabIndex = 25;
            this.CB_Product.SelectedValueChanged += new System.EventHandler(this.CB_Variant_SelectedValueChanged);
            // 
            // TB_Other
            // 
            this.TB_Other.Enabled = false;
            this.TB_Other.Location = new System.Drawing.Point(597, 12);
            this.TB_Other.Name = "TB_Other";
            this.TB_Other.Size = new System.Drawing.Size(107, 20);
            this.TB_Other.TabIndex = 26;
            this.TB_Other.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(372, 14);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 27;
            this.label15.Text = "Product";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(229, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Node Type";
            // 
            // CB_Variant
            // 
            this.CB_Variant.FormattingEnabled = true;
            this.CB_Variant.Location = new System.Drawing.Point(291, 11);
            this.CB_Variant.Name = "CB_Variant";
            this.CB_Variant.Size = new System.Drawing.Size(75, 21);
            this.CB_Variant.TabIndex = 28;
            // 
            // AutoGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 176);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CB_Variant);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.TB_Other);
            this.Controls.Add(this.CB_Product);
            this.Controls.Add(this.BTN_Gen);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TB_DWN);
            this.Controls.Add(this.TB_UP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_IP);
            this.Name = "AutoGen";
            this.Text = "AutoGen";
            this.Load += new System.EventHandler(this.AutoGen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_UP;
        private System.Windows.Forms.TextBox TB_DWN;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BTN_Gen;
        private System.Windows.Forms.ComboBox CB_Product;
        private System.Windows.Forms.TextBox TB_Other;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CB_Variant;
    }
}