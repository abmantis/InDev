namespace SymbIOT
{
    partial class XCategory
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
            this.BTN_Close = new System.Windows.Forms.Button();
            this.BTN_GetAll = new System.Windows.Forms.Button();
            this.TAB_Keys = new System.Windows.Forms.TabControl();
            this.BTN_SetAll = new System.Windows.Forms.Button();
            this.BTN_ClearAll = new System.Windows.Forms.Button();
            this.BTN_ClearRcv = new System.Windows.Forms.Button();
            this.PAN_Buttons = new System.Windows.Forms.Panel();
            this.PAN_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(610, 0);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(75, 23);
            this.BTN_Close.TabIndex = 1;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // BTN_GetAll
            // 
            this.BTN_GetAll.Location = new System.Drawing.Point(533, 0);
            this.BTN_GetAll.Name = "BTN_GetAll";
            this.BTN_GetAll.Size = new System.Drawing.Size(71, 23);
            this.BTN_GetAll.TabIndex = 2;
            this.BTN_GetAll.Text = "Get All";
            this.BTN_GetAll.UseVisualStyleBackColor = true;
            this.BTN_GetAll.Click += new System.EventHandler(this.BTN_Refresh_Click);
            // 
            // TAB_Keys
            // 
            this.TAB_Keys.Location = new System.Drawing.Point(1, -1);
            this.TAB_Keys.Name = "TAB_Keys";
            this.TAB_Keys.SelectedIndex = 0;
            this.TAB_Keys.Size = new System.Drawing.Size(685, 229);
            this.TAB_Keys.TabIndex = 3;
            // 
            // BTN_SetAll
            // 
            this.BTN_SetAll.Location = new System.Drawing.Point(428, 0);
            this.BTN_SetAll.Name = "BTN_SetAll";
            this.BTN_SetAll.Size = new System.Drawing.Size(99, 23);
            this.BTN_SetAll.TabIndex = 4;
            this.BTN_SetAll.Text = "Set All Checked";
            this.BTN_SetAll.UseVisualStyleBackColor = true;
            this.BTN_SetAll.Click += new System.EventHandler(this.BTN_SetAll_Click);
            // 
            // BTN_ClearAll
            // 
            this.BTN_ClearAll.Location = new System.Drawing.Point(336, 0);
            this.BTN_ClearAll.Name = "BTN_ClearAll";
            this.BTN_ClearAll.Size = new System.Drawing.Size(86, 23);
            this.BTN_ClearAll.TabIndex = 5;
            this.BTN_ClearAll.Text = "Uncheck All";
            this.BTN_ClearAll.UseVisualStyleBackColor = true;
            this.BTN_ClearAll.Click += new System.EventHandler(this.BTN_ClearAll_Click);
            // 
            // BTN_ClearRcv
            // 
            this.BTN_ClearRcv.Location = new System.Drawing.Point(223, 0);
            this.BTN_ClearRcv.Name = "BTN_ClearRcv";
            this.BTN_ClearRcv.Size = new System.Drawing.Size(107, 23);
            this.BTN_ClearRcv.TabIndex = 6;
            this.BTN_ClearRcv.Text = "Clear Received";
            this.BTN_ClearRcv.UseVisualStyleBackColor = true;
            this.BTN_ClearRcv.Click += new System.EventHandler(this.BTN_ClearRcv_Click);
            // 
            // PAN_Buttons
            // 
            this.PAN_Buttons.Controls.Add(this.BTN_Close);
            this.PAN_Buttons.Controls.Add(this.BTN_ClearRcv);
            this.PAN_Buttons.Controls.Add(this.BTN_GetAll);
            this.PAN_Buttons.Controls.Add(this.BTN_ClearAll);
            this.PAN_Buttons.Controls.Add(this.BTN_SetAll);
            this.PAN_Buttons.Location = new System.Drawing.Point(1, 230);
            this.PAN_Buttons.Name = "PAN_Buttons";
            this.PAN_Buttons.Size = new System.Drawing.Size(685, 30);
            this.PAN_Buttons.TabIndex = 7;
            // 
            // XCategory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 257);
            this.Controls.Add(this.PAN_Buttons);
            this.Controls.Add(this.TAB_Keys);
            this.Name = "XCategory";
            this.Text = "All Keys";
            this.Resize += new System.EventHandler(this.XCategory_Resize);
            this.PAN_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BTN_Close;
        private System.Windows.Forms.Button BTN_GetAll;
        private System.Windows.Forms.TabControl TAB_Keys;
        private System.Windows.Forms.Button BTN_SetAll;
        private System.Windows.Forms.Button BTN_ClearAll;
        private System.Windows.Forms.Button BTN_ClearRcv;
        private System.Windows.Forms.Panel PAN_Buttons;
    }
}