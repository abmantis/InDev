namespace SymbIOT
{
    partial class Capability
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
            this.BTN_Load = new System.Windows.Forms.Button();
            this.PAN_Cycle = new System.Windows.Forms.Panel();
            this.BTN_Display = new System.Windows.Forms.Button();
            this.BTN_Start = new System.Windows.Forms.Button();
            this.LBL_CycleName = new System.Windows.Forms.Label();
            this.PAN_CycleCommands = new System.Windows.Forms.Panel();
            this.TAB_Cavities = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.BTN_Generate = new System.Windows.Forms.Button();
            this.CB_Cancel = new System.Windows.Forms.CheckBox();
            this.CB_CookTime = new System.Windows.Forms.CheckBox();
            this.CB_DelayTime = new System.Windows.Forms.CheckBox();
            this.TB_CookTime = new System.Windows.Forms.TextBox();
            this.TB_DelayTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_CancelTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RB_TempRandom = new System.Windows.Forms.RadioButton();
            this.RB_TempMin = new System.Windows.Forms.RadioButton();
            this.RB_TempMax = new System.Windows.Forms.RadioButton();
            this.RB_TempDefault = new System.Windows.Forms.RadioButton();
            this.CB_Optional = new System.Windows.Forms.CheckBox();
            this.CB_Randomize = new System.Windows.Forms.CheckBox();
            this.BTN_LUA = new System.Windows.Forms.Button();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.PAN_Cycle.SuspendLayout();
            this.TAB_Cavities.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BTN_Load
            // 
            this.BTN_Load.Location = new System.Drawing.Point(137, 12);
            this.BTN_Load.Name = "BTN_Load";
            this.BTN_Load.Size = new System.Drawing.Size(114, 30);
            this.BTN_Load.TabIndex = 0;
            this.BTN_Load.Text = "Load Capability";
            this.BTN_Load.UseVisualStyleBackColor = true;
            this.BTN_Load.Click += new System.EventHandler(this.BTN_Load_Click);
            // 
            // PAN_Cycle
            // 
            this.PAN_Cycle.Controls.Add(this.BTN_Display);
            this.PAN_Cycle.Controls.Add(this.BTN_Start);
            this.PAN_Cycle.Controls.Add(this.LBL_CycleName);
            this.PAN_Cycle.Controls.Add(this.PAN_CycleCommands);
            this.PAN_Cycle.Location = new System.Drawing.Point(12, 48);
            this.PAN_Cycle.Name = "PAN_Cycle";
            this.PAN_Cycle.Size = new System.Drawing.Size(379, 268);
            this.PAN_Cycle.TabIndex = 2;
            // 
            // BTN_Display
            // 
            this.BTN_Display.Location = new System.Drawing.Point(295, 68);
            this.BTN_Display.Name = "BTN_Display";
            this.BTN_Display.Size = new System.Drawing.Size(75, 38);
            this.BTN_Display.TabIndex = 3;
            this.BTN_Display.Tag = "03";
            this.BTN_Display.Text = "Set on Display";
            this.BTN_Display.UseVisualStyleBackColor = true;
            this.BTN_Display.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // BTN_Start
            // 
            this.BTN_Start.Location = new System.Drawing.Point(295, 39);
            this.BTN_Start.Name = "BTN_Start";
            this.BTN_Start.Size = new System.Drawing.Size(75, 23);
            this.BTN_Start.TabIndex = 2;
            this.BTN_Start.Tag = "02";
            this.BTN_Start.Text = "Start";
            this.BTN_Start.UseVisualStyleBackColor = true;
            this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // LBL_CycleName
            // 
            this.LBL_CycleName.AutoSize = true;
            this.LBL_CycleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_CycleName.Location = new System.Drawing.Point(11, 10);
            this.LBL_CycleName.Name = "LBL_CycleName";
            this.LBL_CycleName.Size = new System.Drawing.Size(0, 20);
            this.LBL_CycleName.TabIndex = 1;
            // 
            // PAN_CycleCommands
            // 
            this.PAN_CycleCommands.AutoScroll = true;
            this.PAN_CycleCommands.Location = new System.Drawing.Point(14, 39);
            this.PAN_CycleCommands.Name = "PAN_CycleCommands";
            this.PAN_CycleCommands.Size = new System.Drawing.Size(275, 215);
            this.PAN_CycleCommands.TabIndex = 0;
            // 
            // TAB_Cavities
            // 
            this.TAB_Cavities.Controls.Add(this.tabPage1);
            this.TAB_Cavities.Controls.Add(this.tabPage2);
            this.TAB_Cavities.Controls.Add(this.tabPage3);
            this.TAB_Cavities.Location = new System.Drawing.Point(400, 12);
            this.TAB_Cavities.Name = "TAB_Cavities";
            this.TAB_Cavities.SelectedIndex = 0;
            this.TAB_Cavities.Size = new System.Drawing.Size(260, 304);
            this.TAB_Cavities.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(252, 278);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Upper";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(252, 278);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lower";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(252, 278);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "MWO";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // BTN_Generate
            // 
            this.BTN_Generate.Location = new System.Drawing.Point(563, 322);
            this.BTN_Generate.Name = "BTN_Generate";
            this.BTN_Generate.Size = new System.Drawing.Size(97, 40);
            this.BTN_Generate.TabIndex = 4;
            this.BTN_Generate.Text = "Generate SymbIOT Script";
            this.BTN_Generate.UseVisualStyleBackColor = true;
            this.BTN_Generate.Click += new System.EventHandler(this.BTN_Generate_Click);
            // 
            // CB_Cancel
            // 
            this.CB_Cancel.AutoSize = true;
            this.CB_Cancel.Checked = true;
            this.CB_Cancel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_Cancel.Location = new System.Drawing.Point(12, 322);
            this.CB_Cancel.Name = "CB_Cancel";
            this.CB_Cancel.Size = new System.Drawing.Size(138, 17);
            this.CB_Cancel.TabIndex = 5;
            this.CB_Cancel.Text = "Cancel each cycle after";
            this.CB_Cancel.UseVisualStyleBackColor = true;
            // 
            // CB_CookTime
            // 
            this.CB_CookTime.AutoSize = true;
            this.CB_CookTime.Location = new System.Drawing.Point(12, 345);
            this.CB_CookTime.Name = "CB_CookTime";
            this.CB_CookTime.Size = new System.Drawing.Size(111, 17);
            this.CB_CookTime.TabIndex = 6;
            this.CB_CookTime.Text = "Set cook timers to";
            this.CB_CookTime.UseVisualStyleBackColor = true;
            // 
            // CB_DelayTime
            // 
            this.CB_DelayTime.AutoSize = true;
            this.CB_DelayTime.Location = new System.Drawing.Point(12, 368);
            this.CB_DelayTime.Name = "CB_DelayTime";
            this.CB_DelayTime.Size = new System.Drawing.Size(112, 17);
            this.CB_DelayTime.TabIndex = 7;
            this.CB_DelayTime.Text = "Set delay timers to";
            this.CB_DelayTime.UseVisualStyleBackColor = true;
            // 
            // TB_CookTime
            // 
            this.TB_CookTime.Location = new System.Drawing.Point(119, 342);
            this.TB_CookTime.Name = "TB_CookTime";
            this.TB_CookTime.Size = new System.Drawing.Size(44, 20);
            this.TB_CookTime.TabIndex = 8;
            // 
            // TB_DelayTime
            // 
            this.TB_DelayTime.Location = new System.Drawing.Point(124, 365);
            this.TB_DelayTime.Name = "TB_DelayTime";
            this.TB_DelayTime.Size = new System.Drawing.Size(39, 20);
            this.TB_DelayTime.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 346);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "min";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 368);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "min";
            // 
            // TB_CancelTime
            // 
            this.TB_CancelTime.Location = new System.Drawing.Point(149, 319);
            this.TB_CancelTime.Name = "TB_CancelTime";
            this.TB_CancelTime.Size = new System.Drawing.Size(38, 20);
            this.TB_CancelTime.TabIndex = 12;
            this.TB_CancelTime.Text = "10";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(193, 322);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "sec";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RB_TempRandom);
            this.groupBox1.Controls.Add(this.RB_TempMin);
            this.groupBox1.Controls.Add(this.RB_TempMax);
            this.groupBox1.Controls.Add(this.RB_TempDefault);
            this.groupBox1.Location = new System.Drawing.Point(223, 322);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(124, 108);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set TargetTemp to";
            // 
            // RB_TempRandom
            // 
            this.RB_TempRandom.AutoSize = true;
            this.RB_TempRandom.Location = new System.Drawing.Point(6, 85);
            this.RB_TempRandom.Name = "RB_TempRandom";
            this.RB_TempRandom.Size = new System.Drawing.Size(65, 17);
            this.RB_TempRandom.TabIndex = 3;
            this.RB_TempRandom.Text = "Random";
            this.RB_TempRandom.UseVisualStyleBackColor = true;
            // 
            // RB_TempMin
            // 
            this.RB_TempMin.AutoSize = true;
            this.RB_TempMin.Location = new System.Drawing.Point(6, 62);
            this.RB_TempMin.Name = "RB_TempMin";
            this.RB_TempMin.Size = new System.Drawing.Size(42, 17);
            this.RB_TempMin.TabIndex = 2;
            this.RB_TempMin.Text = "Min";
            this.RB_TempMin.UseVisualStyleBackColor = true;
            // 
            // RB_TempMax
            // 
            this.RB_TempMax.AutoSize = true;
            this.RB_TempMax.Location = new System.Drawing.Point(6, 39);
            this.RB_TempMax.Name = "RB_TempMax";
            this.RB_TempMax.Size = new System.Drawing.Size(45, 17);
            this.RB_TempMax.TabIndex = 1;
            this.RB_TempMax.Text = "Max";
            this.RB_TempMax.UseVisualStyleBackColor = true;
            // 
            // RB_TempDefault
            // 
            this.RB_TempDefault.AutoSize = true;
            this.RB_TempDefault.Checked = true;
            this.RB_TempDefault.Location = new System.Drawing.Point(6, 16);
            this.RB_TempDefault.Name = "RB_TempDefault";
            this.RB_TempDefault.Size = new System.Drawing.Size(59, 17);
            this.RB_TempDefault.TabIndex = 0;
            this.RB_TempDefault.TabStop = true;
            this.RB_TempDefault.Text = "Default";
            this.RB_TempDefault.UseVisualStyleBackColor = true;
            // 
            // CB_Optional
            // 
            this.CB_Optional.AutoSize = true;
            this.CB_Optional.Location = new System.Drawing.Point(12, 391);
            this.CB_Optional.Name = "CB_Optional";
            this.CB_Optional.Size = new System.Drawing.Size(148, 17);
            this.CB_Optional.TabIndex = 15;
            this.CB_Optional.Text = "Exclude optional modifiers";
            this.CB_Optional.UseVisualStyleBackColor = true;
            // 
            // CB_Randomize
            // 
            this.CB_Randomize.AutoSize = true;
            this.CB_Randomize.Location = new System.Drawing.Point(12, 413);
            this.CB_Randomize.Name = "CB_Randomize";
            this.CB_Randomize.Size = new System.Drawing.Size(150, 17);
            this.CB_Randomize.TabIndex = 16;
            this.CB_Randomize.Text = "Randomize other modifiers";
            this.CB_Randomize.UseVisualStyleBackColor = true;
            // 
            // BTN_LUA
            // 
            this.BTN_LUA.Location = new System.Drawing.Point(563, 368);
            this.BTN_LUA.Name = "BTN_LUA";
            this.BTN_LUA.Size = new System.Drawing.Size(97, 40);
            this.BTN_LUA.TabIndex = 17;
            this.BTN_LUA.Text = "Generate LUA Script";
            this.BTN_LUA.UseVisualStyleBackColor = true;
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(563, 414);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(97, 23);
            this.BTN_Close.TabIndex = 18;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // Capability
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 442);
            this.Controls.Add(this.BTN_Close);
            this.Controls.Add(this.BTN_LUA);
            this.Controls.Add(this.CB_Randomize);
            this.Controls.Add(this.CB_Optional);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_CancelTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_DelayTime);
            this.Controls.Add(this.TB_CookTime);
            this.Controls.Add(this.CB_DelayTime);
            this.Controls.Add(this.CB_CookTime);
            this.Controls.Add(this.CB_Cancel);
            this.Controls.Add(this.BTN_Generate);
            this.Controls.Add(this.TAB_Cavities);
            this.Controls.Add(this.PAN_Cycle);
            this.Controls.Add(this.BTN_Load);
            this.Name = "Capability";
            this.Text = "Capability";
            this.PAN_Cycle.ResumeLayout(false);
            this.PAN_Cycle.PerformLayout();
            this.TAB_Cavities.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_Load;
        private System.Windows.Forms.Panel PAN_Cycle;
        private System.Windows.Forms.Button BTN_Display;
        private System.Windows.Forms.Button BTN_Start;
        private System.Windows.Forms.Label LBL_CycleName;
        private System.Windows.Forms.Panel PAN_CycleCommands;
        private System.Windows.Forms.TabControl TAB_Cavities;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button BTN_Generate;
        private System.Windows.Forms.CheckBox CB_Cancel;
        private System.Windows.Forms.CheckBox CB_CookTime;
        private System.Windows.Forms.CheckBox CB_DelayTime;
        private System.Windows.Forms.TextBox TB_CookTime;
        private System.Windows.Forms.TextBox TB_DelayTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_CancelTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton RB_TempRandom;
        private System.Windows.Forms.RadioButton RB_TempMin;
        private System.Windows.Forms.RadioButton RB_TempMax;
        private System.Windows.Forms.RadioButton RB_TempDefault;
        private System.Windows.Forms.CheckBox CB_Optional;
        private System.Windows.Forms.CheckBox CB_Randomize;
        private System.Windows.Forms.Button BTN_LUA;
        private System.Windows.Forms.Button BTN_Close;
    }
}