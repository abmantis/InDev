namespace VenomNamespace
{
    partial class PayList
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
            this.TB_Payload = new System.Windows.Forms.TextBox();
            this.CB_Type = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DGV_Data = new System.Windows.Forms.DataGridView();
            this.BTN_Add = new System.Windows.Forms.Button();
            this.BTN_Remove = new System.Windows.Forms.Button();
            this.BTN_Clear = new System.Windows.Forms.Button();
            this.BTN_Save = new System.Windows.Forms.Button();
            this.CB_Variant = new System.Windows.Forms.ComboBox();
            this.TB_IPDisplay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.BTN_Auto = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_Payload
            // 
            this.TB_Payload.Location = new System.Drawing.Point(86, 50);
            this.TB_Payload.Name = "TB_Payload";
            this.TB_Payload.Size = new System.Drawing.Size(368, 20);
            this.TB_Payload.TabIndex = 13;
            // 
            // CB_Type
            // 
            this.CB_Type.FormattingEnabled = true;
            this.CB_Type.Location = new System.Drawing.Point(665, 50);
            this.CB_Type.Name = "CB_Type";
            this.CB_Type.Size = new System.Drawing.Size(92, 21);
            this.CB_Type.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "OTA Payload:";
            // 
            // DGV_Data
            // 
            this.DGV_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Data.Location = new System.Drawing.Point(110, 89);
            this.DGV_Data.Name = "DGV_Data";
            this.DGV_Data.Size = new System.Drawing.Size(647, 254);
            this.DGV_Data.TabIndex = 16;
            // 
            // BTN_Add
            // 
            this.BTN_Add.Location = new System.Drawing.Point(13, 89);
            this.BTN_Add.Name = "BTN_Add";
            this.BTN_Add.Size = new System.Drawing.Size(75, 23);
            this.BTN_Add.TabIndex = 17;
            this.BTN_Add.Text = "Add";
            this.BTN_Add.UseVisualStyleBackColor = true;
            this.BTN_Add.Click += new System.EventHandler(this.BTN_Add_Click);
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(13, 135);
            this.BTN_Remove.Name = "BTN_Remove";
            this.BTN_Remove.Size = new System.Drawing.Size(75, 23);
            this.BTN_Remove.TabIndex = 18;
            this.BTN_Remove.Text = "Remove";
            this.BTN_Remove.UseVisualStyleBackColor = true;
            this.BTN_Remove.Click += new System.EventHandler(this.BTN_Remove_Click);
            // 
            // BTN_Clear
            // 
            this.BTN_Clear.Location = new System.Drawing.Point(13, 183);
            this.BTN_Clear.Name = "BTN_Clear";
            this.BTN_Clear.Size = new System.Drawing.Size(75, 23);
            this.BTN_Clear.TabIndex = 19;
            this.BTN_Clear.Text = "Clear";
            this.BTN_Clear.UseVisualStyleBackColor = true;
            this.BTN_Clear.Click += new System.EventHandler(this.BTN_Clear_Click);
            // 
            // BTN_Save
            // 
            this.BTN_Save.Location = new System.Drawing.Point(13, 276);
            this.BTN_Save.Name = "BTN_Save";
            this.BTN_Save.Size = new System.Drawing.Size(75, 23);
            this.BTN_Save.TabIndex = 20;
            this.BTN_Save.Text = "Save";
            this.BTN_Save.UseVisualStyleBackColor = true;
            this.BTN_Save.Click += new System.EventHandler(this.BTN_Save_Click);
            // 
            // CB_Variant
            // 
            this.CB_Variant.FormattingEnabled = true;
            this.CB_Variant.Location = new System.Drawing.Point(527, 50);
            this.CB_Variant.Name = "CB_Variant";
            this.CB_Variant.Size = new System.Drawing.Size(75, 21);
            this.CB_Variant.TabIndex = 21;
            // 
            // TB_IPDisplay
            // 
            this.TB_IPDisplay.Location = new System.Drawing.Point(361, 12);
            this.TB_IPDisplay.Name = "TB_IPDisplay";
            this.TB_IPDisplay.Size = new System.Drawing.Size(164, 20);
            this.TB_IPDisplay.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Current IP Address:";
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(15, 320);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(75, 23);
            this.BTN_Close.TabIndex = 24;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // BTN_Auto
            // 
            this.BTN_Auto.Location = new System.Drawing.Point(665, 12);
            this.BTN_Auto.Name = "BTN_Auto";
            this.BTN_Auto.Size = new System.Drawing.Size(90, 23);
            this.BTN_Auto.TabIndex = 25;
            this.BTN_Auto.Text = "Auto Test Plan";
            this.BTN_Auto.UseVisualStyleBackColor = true;
            this.BTN_Auto.Click += new System.EventHandler(this.BTN_Auto_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(608, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "OTA Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(465, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Node Type";
            // 
            // PayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 371);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BTN_Auto);
            this.Controls.Add(this.BTN_Close);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TB_IPDisplay);
            this.Controls.Add(this.CB_Variant);
            this.Controls.Add(this.BTN_Save);
            this.Controls.Add(this.BTN_Clear);
            this.Controls.Add(this.BTN_Remove);
            this.Controls.Add(this.BTN_Add);
            this.Controls.Add(this.DGV_Data);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CB_Type);
            this.Controls.Add(this.TB_Payload);
            this.Name = "PayList";
            this.Text = "Payload Run List";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Payload;
        private System.Windows.Forms.ComboBox CB_Type;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTN_Add;
        private System.Windows.Forms.Button BTN_Remove;
        private System.Windows.Forms.Button BTN_Clear;
        private System.Windows.Forms.Button BTN_Save;
        private System.Windows.Forms.ComboBox CB_Variant;
        private System.Windows.Forms.TextBox TB_IPDisplay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BTN_Close;
        private System.Windows.Forms.Button BTN_Auto;
        public System.Windows.Forms.DataGridView DGV_Data;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}