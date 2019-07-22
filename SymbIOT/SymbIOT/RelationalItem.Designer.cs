namespace SymbIOT
{
    partial class RelationalItem
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
            this.RB_Cycle = new System.Windows.Forms.RadioButton();
            this.RB_Text = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.LBL_Cycle = new System.Windows.Forms.Label();
            this.CBO_Cycle = new System.Windows.Forms.ComboBox();
            this.LBL_SetTemp = new System.Windows.Forms.Label();
            this.TB_Temp = new System.Windows.Forms.TextBox();
            this.LBL_Step = new System.Windows.Forms.Label();
            this.LBL_SetTempF = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TB_Time = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.LBL_SetProbeF = new System.Windows.Forms.Label();
            this.TB_Probe = new System.Windows.Forms.TextBox();
            this.LBL_SetProbe = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.CBO_End = new System.Windows.Forms.ComboBox();
            this.BTN_Delete = new System.Windows.Forms.Button();
            this.BTN_Up = new System.Windows.Forms.Button();
            this.BTN_Down = new System.Windows.Forms.Button();
            this.TB_Message = new System.Windows.Forms.TextBox();
            this.LBL_Zone = new System.Windows.Forms.Label();
            this.CBO_Amt = new System.Windows.Forms.ComboBox();
            this.CBO_ZoneDoneness = new System.Windows.Forms.ComboBox();
            this.TB_Zone = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // RB_Cycle
            // 
            this.RB_Cycle.AutoSize = true;
            this.RB_Cycle.Checked = true;
            this.RB_Cycle.Location = new System.Drawing.Point(9, 48);
            this.RB_Cycle.Name = "RB_Cycle";
            this.RB_Cycle.Size = new System.Drawing.Size(51, 17);
            this.RB_Cycle.TabIndex = 0;
            this.RB_Cycle.TabStop = true;
            this.RB_Cycle.Text = "Cycle";
            this.RB_Cycle.UseVisualStyleBackColor = true;
            this.RB_Cycle.CheckedChanged += new System.EventHandler(this.RB_Cycle_CheckedChanged);
            // 
            // RB_Text
            // 
            this.RB_Text.AutoSize = true;
            this.RB_Text.Location = new System.Drawing.Point(9, 65);
            this.RB_Text.Name = "RB_Text";
            this.RB_Text.Size = new System.Drawing.Size(46, 17);
            this.RB_Text.TabIndex = 1;
            this.RB_Text.TabStop = true;
            this.RB_Text.Text = "Text";
            this.RB_Text.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(63, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(2, 75);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // LBL_Cycle
            // 
            this.LBL_Cycle.AutoSize = true;
            this.LBL_Cycle.Location = new System.Drawing.Point(71, 12);
            this.LBL_Cycle.Name = "LBL_Cycle";
            this.LBL_Cycle.Size = new System.Drawing.Size(36, 13);
            this.LBL_Cycle.TabIndex = 3;
            this.LBL_Cycle.Text = "Cycle:";
            // 
            // CBO_Cycle
            // 
            this.CBO_Cycle.FormattingEnabled = true;
            this.CBO_Cycle.Location = new System.Drawing.Point(110, 8);
            this.CBO_Cycle.Name = "CBO_Cycle";
            this.CBO_Cycle.Size = new System.Drawing.Size(374, 21);
            this.CBO_Cycle.TabIndex = 4;
            this.CBO_Cycle.SelectedIndexChanged += new System.EventHandler(this.CBO_Cycle_SelectedIndexChanged);
            // 
            // LBL_SetTemp
            // 
            this.LBL_SetTemp.AutoSize = true;
            this.LBL_SetTemp.Location = new System.Drawing.Point(71, 35);
            this.LBL_SetTemp.Name = "LBL_SetTemp";
            this.LBL_SetTemp.Size = new System.Drawing.Size(37, 13);
            this.LBL_SetTemp.TabIndex = 5;
            this.LBL_SetTemp.Text = "Temp:";
            // 
            // TB_Temp
            // 
            this.TB_Temp.Location = new System.Drawing.Point(110, 33);
            this.TB_Temp.Name = "TB_Temp";
            this.TB_Temp.Size = new System.Drawing.Size(65, 20);
            this.TB_Temp.TabIndex = 6;
            this.TB_Temp.TextChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // LBL_Step
            // 
            this.LBL_Step.AutoSize = true;
            this.LBL_Step.Location = new System.Drawing.Point(11, 4);
            this.LBL_Step.Name = "LBL_Step";
            this.LBL_Step.Size = new System.Drawing.Size(38, 13);
            this.LBL_Step.TabIndex = 7;
            this.LBL_Step.Text = "Step 1";
            // 
            // LBL_SetTempF
            // 
            this.LBL_SetTempF.AutoSize = true;
            this.LBL_SetTempF.Location = new System.Drawing.Point(177, 36);
            this.LBL_SetTempF.Name = "LBL_SetTempF";
            this.LBL_SetTempF.Size = new System.Drawing.Size(13, 13);
            this.LBL_SetTempF.TabIndex = 8;
            this.LBL_SetTempF.Text = "F";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(462, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "min";
            // 
            // TB_Time
            // 
            this.TB_Time.Location = new System.Drawing.Point(395, 33);
            this.TB_Time.Name = "TB_Time";
            this.TB_Time.Size = new System.Drawing.Size(65, 20);
            this.TB_Time.TabIndex = 10;
            this.TB_Time.TextChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(359, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Time:";
            // 
            // LBL_SetProbeF
            // 
            this.LBL_SetProbeF.AutoSize = true;
            this.LBL_SetProbeF.Location = new System.Drawing.Point(335, 36);
            this.LBL_SetProbeF.Name = "LBL_SetProbeF";
            this.LBL_SetProbeF.Size = new System.Drawing.Size(13, 13);
            this.LBL_SetProbeF.TabIndex = 14;
            this.LBL_SetProbeF.Text = "F";
            // 
            // TB_Probe
            // 
            this.TB_Probe.Location = new System.Drawing.Point(268, 33);
            this.TB_Probe.Name = "TB_Probe";
            this.TB_Probe.Size = new System.Drawing.Size(65, 20);
            this.TB_Probe.TabIndex = 13;
            this.TB_Probe.TextChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // LBL_SetProbe
            // 
            this.LBL_SetProbe.AutoSize = true;
            this.LBL_SetProbe.Location = new System.Drawing.Point(196, 36);
            this.LBL_SetProbe.Name = "LBL_SetProbe";
            this.LBL_SetProbe.Size = new System.Drawing.Size(68, 13);
            this.LBL_SetProbe.TabIndex = 12;
            this.LBL_SetProbe.Text = "Probe Temp:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(78, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "End:";
            // 
            // CBO_End
            // 
            this.CBO_End.FormattingEnabled = true;
            this.CBO_End.Location = new System.Drawing.Point(110, 58);
            this.CBO_End.Name = "CBO_End";
            this.CBO_End.Size = new System.Drawing.Size(223, 21);
            this.CBO_End.TabIndex = 16;
            this.CBO_End.SelectedIndexChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // BTN_Delete
            // 
            this.BTN_Delete.Location = new System.Drawing.Point(2, 22);
            this.BTN_Delete.Name = "BTN_Delete";
            this.BTN_Delete.Size = new System.Drawing.Size(19, 23);
            this.BTN_Delete.TabIndex = 17;
            this.BTN_Delete.Text = "X";
            this.BTN_Delete.UseVisualStyleBackColor = true;
            this.BTN_Delete.Click += new System.EventHandler(this.BTN_Delete_Click);
            // 
            // BTN_Up
            // 
            this.BTN_Up.Location = new System.Drawing.Point(21, 22);
            this.BTN_Up.Name = "BTN_Up";
            this.BTN_Up.Size = new System.Drawing.Size(19, 23);
            this.BTN_Up.TabIndex = 18;
            this.BTN_Up.Text = "^";
            this.BTN_Up.UseVisualStyleBackColor = true;
            this.BTN_Up.Click += new System.EventHandler(this.BTN_Up_Click);
            // 
            // BTN_Down
            // 
            this.BTN_Down.Location = new System.Drawing.Point(41, 22);
            this.BTN_Down.Name = "BTN_Down";
            this.BTN_Down.Size = new System.Drawing.Size(19, 23);
            this.BTN_Down.TabIndex = 19;
            this.BTN_Down.Text = "v";
            this.BTN_Down.UseVisualStyleBackColor = true;
            this.BTN_Down.Click += new System.EventHandler(this.BTN_Down_Click);
            // 
            // TB_Message
            // 
            this.TB_Message.Location = new System.Drawing.Point(110, 8);
            this.TB_Message.Name = "TB_Message";
            this.TB_Message.Size = new System.Drawing.Size(373, 20);
            this.TB_Message.TabIndex = 20;
            this.TB_Message.Visible = false;
            this.TB_Message.TextChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // LBL_Zone
            // 
            this.LBL_Zone.AutoSize = true;
            this.LBL_Zone.Location = new System.Drawing.Point(357, 61);
            this.LBL_Zone.Name = "LBL_Zone";
            this.LBL_Zone.Size = new System.Drawing.Size(35, 13);
            this.LBL_Zone.TabIndex = 21;
            this.LBL_Zone.Text = "Zone:";
            this.LBL_Zone.Visible = false;
            // 
            // CBO_Amt
            // 
            this.CBO_Amt.FormattingEnabled = true;
            this.CBO_Amt.Location = new System.Drawing.Point(268, 33);
            this.CBO_Amt.Name = "CBO_Amt";
            this.CBO_Amt.Size = new System.Drawing.Size(65, 21);
            this.CBO_Amt.TabIndex = 23;
            // 
            // CBO_Doneness
            // 
            this.CBO_ZoneDoneness.FormattingEnabled = true;
            this.CBO_ZoneDoneness.Location = new System.Drawing.Point(395, 55);
            this.CBO_ZoneDoneness.Name = "CBO_Doneness";
            this.CBO_ZoneDoneness.Size = new System.Drawing.Size(88, 21);
            this.CBO_ZoneDoneness.TabIndex = 24;
            this.CBO_ZoneDoneness.Visible = false;
            this.CBO_ZoneDoneness.SelectedIndexChanged += new System.EventHandler(this.CBO_Doneness_SelectedIndexChanged);
            // 
            // TB_Zone
            // 
            this.TB_Zone.Location = new System.Drawing.Point(395, 56);
            this.TB_Zone.Name = "TB_Zone";
            this.TB_Zone.Size = new System.Drawing.Size(65, 20);
            this.TB_Zone.TabIndex = 22;
            this.TB_Zone.Visible = false;
            this.TB_Zone.TextChanged += new System.EventHandler(this.UpdateInnerStep);
            // 
            // RelationalItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.CBO_ZoneDoneness);
            this.Controls.Add(this.CBO_Amt);
            this.Controls.Add(this.TB_Zone);
            this.Controls.Add(this.LBL_Zone);
            this.Controls.Add(this.TB_Message);
            this.Controls.Add(this.BTN_Down);
            this.Controls.Add(this.BTN_Up);
            this.Controls.Add(this.BTN_Delete);
            this.Controls.Add(this.CBO_End);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.LBL_SetProbeF);
            this.Controls.Add(this.TB_Probe);
            this.Controls.Add(this.LBL_SetProbe);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TB_Time);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LBL_SetTempF);
            this.Controls.Add(this.LBL_Step);
            this.Controls.Add(this.TB_Temp);
            this.Controls.Add(this.LBL_SetTemp);
            this.Controls.Add(this.CBO_Cycle);
            this.Controls.Add(this.LBL_Cycle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RB_Text);
            this.Controls.Add(this.RB_Cycle);
            this.Name = "RelationalItem";
            this.Size = new System.Drawing.Size(498, 88);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton RB_Cycle;
        private System.Windows.Forms.RadioButton RB_Text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LBL_Cycle;
        private System.Windows.Forms.ComboBox CBO_Cycle;
        private System.Windows.Forms.Label LBL_SetTemp;
        private System.Windows.Forms.TextBox TB_Temp;
        private System.Windows.Forms.Label LBL_Step;
        private System.Windows.Forms.Label LBL_SetTempF;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TB_Time;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label LBL_SetProbeF;
        private System.Windows.Forms.TextBox TB_Probe;
        private System.Windows.Forms.Label LBL_SetProbe;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox CBO_End;
        private System.Windows.Forms.Button BTN_Delete;
        private System.Windows.Forms.Button BTN_Up;
        private System.Windows.Forms.Button BTN_Down;
        private System.Windows.Forms.TextBox TB_Message;
        private System.Windows.Forms.Label LBL_Zone;
        private System.Windows.Forms.ComboBox CBO_Amt;
        private System.Windows.Forms.ComboBox CBO_ZoneDoneness;
        private System.Windows.Forms.TextBox TB_Zone;
    }
}
