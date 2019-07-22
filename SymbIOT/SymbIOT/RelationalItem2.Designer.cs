namespace SymbIOT
{
    partial class RelationalItem2
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
            this.BTN_Remove = new System.Windows.Forms.Button();
            this.BTN_Down = new System.Windows.Forms.Button();
            this.BTN_Up = new System.Windows.Forms.Button();
            this.LBL_Name = new System.Windows.Forms.Label();
            this.TB_Value = new System.Windows.Forms.TextBox();
            this.CBO_Value = new System.Windows.Forms.ComboBox();
            this.CBO_Type = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // BTN_Remove
            // 
            this.BTN_Remove.Location = new System.Drawing.Point(2, 1);
            this.BTN_Remove.Name = "BTN_Remove";
            this.BTN_Remove.Size = new System.Drawing.Size(20, 23);
            this.BTN_Remove.TabIndex = 0;
            this.BTN_Remove.Text = "X";
            this.BTN_Remove.UseVisualStyleBackColor = true;
            this.BTN_Remove.Click += new System.EventHandler(this.BTN_Remove_Click);
            // 
            // BTN_Down
            // 
            this.BTN_Down.Location = new System.Drawing.Point(43, 1);
            this.BTN_Down.Name = "BTN_Down";
            this.BTN_Down.Size = new System.Drawing.Size(20, 23);
            this.BTN_Down.TabIndex = 1;
            this.BTN_Down.Text = "v";
            this.BTN_Down.UseVisualStyleBackColor = true;
            this.BTN_Down.Click += new System.EventHandler(this.BTN_Down_Click);
            // 
            // BTN_Up
            // 
            this.BTN_Up.Location = new System.Drawing.Point(22, 1);
            this.BTN_Up.Name = "BTN_Up";
            this.BTN_Up.Size = new System.Drawing.Size(20, 23);
            this.BTN_Up.TabIndex = 2;
            this.BTN_Up.Text = "^";
            this.BTN_Up.UseVisualStyleBackColor = true;
            this.BTN_Up.Click += new System.EventHandler(this.BTN_Up_Click);
            // 
            // LBL_Name
            // 
            this.LBL_Name.AutoSize = true;
            this.LBL_Name.Location = new System.Drawing.Point(65, 6);
            this.LBL_Name.Name = "LBL_Name";
            this.LBL_Name.Size = new System.Drawing.Size(35, 13);
            this.LBL_Name.TabIndex = 3;
            this.LBL_Name.Text = "label1";
            this.LBL_Name.Click += new System.EventHandler(this.RelationalItem2_Click);
            // 
            // TB_Value
            // 
            this.TB_Value.Location = new System.Drawing.Point(453, 4);
            this.TB_Value.Name = "TB_Value";
            this.TB_Value.Size = new System.Drawing.Size(184, 20);
            this.TB_Value.TabIndex = 4;
            this.TB_Value.Click += new System.EventHandler(this.RelationalItem2_Click);
            // 
            // CBO_Value
            // 
            this.CBO_Value.FormattingEnabled = true;
            this.CBO_Value.Location = new System.Drawing.Point(453, 3);
            this.CBO_Value.Name = "CBO_Value";
            this.CBO_Value.Size = new System.Drawing.Size(184, 21);
            this.CBO_Value.TabIndex = 5;
            this.CBO_Value.Click += new System.EventHandler(this.RelationalItem2_Click);
            // 
            // CBO_Type
            // 
            this.CBO_Type.FormattingEnabled = true;
            this.CBO_Type.Items.AddRange(new object[] {
            "Cycle Data",
            "Ingredients",
            "User Instruction",
            "Machine Instruction",
            "User/Machine Instruction",
            "Facade Data"});
            this.CBO_Type.Location = new System.Drawing.Point(326, 3);
            this.CBO_Type.Name = "CBO_Type";
            this.CBO_Type.Size = new System.Drawing.Size(121, 21);
            this.CBO_Type.TabIndex = 6;
            this.CBO_Type.SelectedIndexChanged += new System.EventHandler(this.CBO_Type_SelectedIndexChanged);
            // 
            // RelationalItem2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CBO_Type);
            this.Controls.Add(this.CBO_Value);
            this.Controls.Add(this.TB_Value);
            this.Controls.Add(this.LBL_Name);
            this.Controls.Add(this.BTN_Up);
            this.Controls.Add(this.BTN_Down);
            this.Controls.Add(this.BTN_Remove);
            this.Name = "RelationalItem2";
            this.Size = new System.Drawing.Size(640, 26);
            this.Click += new System.EventHandler(this.RelationalItem2_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_Remove;
        private System.Windows.Forms.Button BTN_Down;
        private System.Windows.Forms.Button BTN_Up;
        private System.Windows.Forms.Label LBL_Name;
        private System.Windows.Forms.TextBox TB_Value;
        private System.Windows.Forms.ComboBox CBO_Value;
        private System.Windows.Forms.ComboBox CBO_Type;
    }
}
