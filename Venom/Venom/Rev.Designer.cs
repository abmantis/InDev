namespace VenomNamespace
{
    partial class Rev
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
            this.TB_Version = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.BTN_Start = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Max = new System.Windows.Forms.TextBox();
            this.TB_Payload = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_Version
            // 
            this.TB_Version.Location = new System.Drawing.Point(263, 35);
            this.TB_Version.Name = "TB_Version";
            this.TB_Version.Size = new System.Drawing.Size(59, 20);
            this.TB_Version.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target Version:";
            // 
            // DGV_Data
            // 
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(20, 82);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.Size = new System.Drawing.Size(578, 238);
            this.DGV_Data.TabIndex = 2;
            // 
            // BTN_Start
            // 
            this.BTN_Start.Location = new System.Drawing.Point(20, 347);
            this.BTN_Start.Name = "BTN_Start";
            this.BTN_Start.Size = new System.Drawing.Size(75, 23);
            this.BTN_Start.TabIndex = 3;
            this.BTN_Start.Text = "Start";
            this.BTN_Start.UseVisualStyleBackColor = true;
            this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Target Amount:";
            // 
            // TB_Max
            // 
            this.TB_Max.Location = new System.Drawing.Point(103, 35);
            this.TB_Max.Name = "TB_Max";
            this.TB_Max.Size = new System.Drawing.Size(58, 20);
            this.TB_Max.TabIndex = 5;
            // 
            // TB_Payload
            // 
            this.TB_Payload.Location = new System.Drawing.Point(498, 34);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(100, 20);
            this.TB_Payload.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(412, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Payload URL:";
            // 
            // Rev
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 416);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_Payload);
            this.Controls.Add(this.TB_Max);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BTN_Start);
            this.Controls.Add(this.DGV_Data);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_Version);
            this.Name = "Rev";
            this.Text = "Auto Revelation";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Version;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView DGV_Data;
        private System.Windows.Forms.Button BTN_Start;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_Max;
        private System.Windows.Forms.TextBox TB_Payload;
        private System.Windows.Forms.Label label3;
    }
}