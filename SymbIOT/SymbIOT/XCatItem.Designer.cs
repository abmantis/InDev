namespace SymbIOT
{
    partial class XCatItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LBL_Name = new System.Windows.Forms.Label();
            this.TB_Value = new System.Windows.Forms.TextBox();
            this.BTN_Set = new System.Windows.Forms.Button();
            this.BTN_Get = new System.Windows.Forms.Button();
            this.CBO_Value = new System.Windows.Forms.ComboBox();
            this.CB_XCat = new System.Windows.Forms.CheckBox();
            this.TT_Label = new System.Windows.Forms.ToolTip(this.components);
            this.CBO_SetValue = new System.Windows.Forms.ComboBox();
            this.TB_SetValue = new System.Windows.Forms.TextBox();
            this.LLBL_Info = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // LBL_Name
            // 
            this.LBL_Name.AutoSize = true;
            this.LBL_Name.Location = new System.Drawing.Point(29, 11);
            this.LBL_Name.Name = "LBL_Name";
            this.LBL_Name.Size = new System.Drawing.Size(35, 13);
            this.LBL_Name.TabIndex = 0;
            this.LBL_Name.Text = "label1";
            // 
            // TB_Value
            // 
            this.TB_Value.Location = new System.Drawing.Point(318, 8);
            this.TB_Value.Name = "TB_Value";
            this.TB_Value.ReadOnly = true;
            this.TB_Value.Size = new System.Drawing.Size(160, 20);
            this.TB_Value.TabIndex = 1;
            // 
            // BTN_Set
            // 
            this.BTN_Set.Location = new System.Drawing.Point(271, 6);
            this.BTN_Set.Name = "BTN_Set";
            this.BTN_Set.Size = new System.Drawing.Size(41, 23);
            this.BTN_Set.TabIndex = 2;
            this.BTN_Set.Text = "Set";
            this.BTN_Set.UseVisualStyleBackColor = true;
            this.BTN_Set.Click += new System.EventHandler(this.BTN_Set_Click);
            // 
            // BTN_Get
            // 
            this.BTN_Get.Location = new System.Drawing.Point(224, 6);
            this.BTN_Get.Name = "BTN_Get";
            this.BTN_Get.Size = new System.Drawing.Size(41, 23);
            this.BTN_Get.TabIndex = 3;
            this.BTN_Get.Text = "Get";
            this.BTN_Get.UseVisualStyleBackColor = true;
            this.BTN_Get.Click += new System.EventHandler(this.BTN_Get_Click);
            // 
            // CBO_Value
            // 
            this.CBO_Value.Enabled = false;
            this.CBO_Value.FormattingEnabled = true;
            this.CBO_Value.Location = new System.Drawing.Point(318, 8);
            this.CBO_Value.Name = "CBO_Value";
            this.CBO_Value.Size = new System.Drawing.Size(160, 21);
            this.CBO_Value.TabIndex = 4;
            // 
            // CB_XCat
            // 
            this.CB_XCat.AutoSize = true;
            this.CB_XCat.Location = new System.Drawing.Point(9, 11);
            this.CB_XCat.Name = "CB_XCat";
            this.CB_XCat.Size = new System.Drawing.Size(15, 14);
            this.CB_XCat.TabIndex = 5;
            this.CB_XCat.UseVisualStyleBackColor = true;
            // 
            // CBO_SetValue
            // 
            this.CBO_SetValue.FormattingEnabled = true;
            this.CBO_SetValue.Location = new System.Drawing.Point(484, 8);
            this.CBO_SetValue.Name = "CBO_SetValue";
            this.CBO_SetValue.Size = new System.Drawing.Size(160, 21);
            this.CBO_SetValue.TabIndex = 7;
            // 
            // TB_SetValue
            // 
            this.TB_SetValue.Location = new System.Drawing.Point(484, 8);
            this.TB_SetValue.Name = "TB_SetValue";
            this.TB_SetValue.Size = new System.Drawing.Size(160, 20);
            this.TB_SetValue.TabIndex = 6;
            // 
            // LLBL_Info
            // 
            this.LLBL_Info.AutoSize = true;
            this.LLBL_Info.Location = new System.Drawing.Point(23, 4);
            this.LLBL_Info.Name = "LLBL_Info";
            this.LLBL_Info.Size = new System.Drawing.Size(9, 13);
            this.LLBL_Info.TabIndex = 8;
            this.LLBL_Info.TabStop = true;
            this.LLBL_Info.Text = "i";
            this.LLBL_Info.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLBL_Info_LinkClicked);
            // 
            // XCatItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LLBL_Info);
            this.Controls.Add(this.CBO_SetValue);
            this.Controls.Add(this.TB_SetValue);
            this.Controls.Add(this.CB_XCat);
            this.Controls.Add(this.CBO_Value);
            this.Controls.Add(this.BTN_Get);
            this.Controls.Add(this.BTN_Set);
            this.Controls.Add(this.TB_Value);
            this.Controls.Add(this.LBL_Name);
            this.Name = "XCatItem";
            this.Size = new System.Drawing.Size(651, 36);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_Name;
        private System.Windows.Forms.TextBox TB_Value;
        private System.Windows.Forms.Button BTN_Set;
        private System.Windows.Forms.Button BTN_Get;
        private System.Windows.Forms.ComboBox CBO_Value;
        private System.Windows.Forms.CheckBox CB_XCat;
        private System.Windows.Forms.ToolTip TT_Label;
        private System.Windows.Forms.ComboBox CBO_SetValue;
        private System.Windows.Forms.TextBox TB_SetValue;
        private System.Windows.Forms.LinkLabel LLBL_Info;
    }
}
