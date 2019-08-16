namespace VenomNamespace
{
    partial class TabSelect
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
            this.LB_Tabs = new System.Windows.Forms.ListBox();
            this.BTN_Select = new System.Windows.Forms.Button();
            this.LBL_Header = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LB_Tabs
            // 
            this.LB_Tabs.FormattingEnabled = true;
            this.LB_Tabs.Location = new System.Drawing.Point(12, 45);
            this.LB_Tabs.Name = "LB_Tabs";
            this.LB_Tabs.Size = new System.Drawing.Size(260, 173);
            this.LB_Tabs.TabIndex = 0;
            // 
            // BTN_Select
            // 
            this.BTN_Select.Location = new System.Drawing.Point(107, 227);
            this.BTN_Select.Name = "BTN_Select";
            this.BTN_Select.Size = new System.Drawing.Size(75, 23);
            this.BTN_Select.TabIndex = 1;
            this.BTN_Select.Text = "Select";
            this.BTN_Select.UseVisualStyleBackColor = true;
            this.BTN_Select.Click += new System.EventHandler(this.BTN_Select_Click);
            // 
            // LBL_Header
            // 
            this.LBL_Header.AutoSize = true;
            this.LBL_Header.Location = new System.Drawing.Point(9, 18);
            this.LBL_Header.Name = "LBL_Header";
            this.LBL_Header.Size = new System.Drawing.Size(217, 13);
            this.LBL_Header.TabIndex = 2;
            this.LBL_Header.Text = "Select the tab containing the KVP definitions";
            // 
            // TabSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.LBL_Header);
            this.Controls.Add(this.BTN_Select);
            this.Controls.Add(this.LB_Tabs);
            this.Name = "TabSelect";
            this.Text = "TabSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox LB_Tabs;
        private System.Windows.Forms.Button BTN_Select;
        private System.Windows.Forms.Label LBL_Header;
    }
}