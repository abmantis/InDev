namespace SymbIOT
{
    partial class RelationalBuilder2
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
            this.TB_Import = new System.Windows.Forms.TextBox();
            this.BTN_Import = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PAN_Keys = new System.Windows.Forms.Panel();
            this.BTN_Insert = new System.Windows.Forms.Button();
            this.BTN_Envelope = new System.Windows.Forms.Button();
            this.BTN_Array = new System.Windows.Forms.Button();
            this.BTN_Entity = new System.Windows.Forms.Button();
            this.PAN_Relational = new System.Windows.Forms.Panel();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.BTN_Send = new System.Windows.Forms.Button();
            this.BTN_MQTT = new System.Windows.Forms.Button();
            this.BTN_LUA = new System.Windows.Forms.Button();
            this.BTN_Preset = new System.Windows.Forms.Button();
            this.BTN_Clear = new System.Windows.Forms.Button();
            this.CB_GEBL = new System.Windows.Forms.CheckBox();
            this.BTN_DelPreset = new System.Windows.Forms.Button();
            this.CB_FullDM = new System.Windows.Forms.CheckBox();
            this.BTN_JSON = new System.Windows.Forms.Button();
            this.BTN_Swap = new System.Windows.Forms.Button();
            this.PAN_Buttons = new System.Windows.Forms.Panel();
            this.RB_Name = new System.Windows.Forms.RadioButton();
            this.RB_Key = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.PAN_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // TB_Import
            // 
            this.TB_Import.AcceptsReturn = true;
            this.TB_Import.AcceptsTab = true;
            this.TB_Import.Location = new System.Drawing.Point(249, 11);
            this.TB_Import.Multiline = true;
            this.TB_Import.Name = "TB_Import";
            this.TB_Import.Size = new System.Drawing.Size(541, 20);
            this.TB_Import.TabIndex = 0;
            // 
            // BTN_Import
            // 
            this.BTN_Import.Location = new System.Drawing.Point(796, 8);
            this.BTN_Import.Name = "BTN_Import";
            this.BTN_Import.Size = new System.Drawing.Size(57, 23);
            this.BTN_Import.TabIndex = 1;
            this.BTN_Import.Text = "Import";
            this.BTN_Import.UseVisualStyleBackColor = true;
            this.BTN_Import.Click += new System.EventHandler(this.BTN_Import_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(249, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(662, 2);
            this.label1.TabIndex = 2;
            // 
            // PAN_Keys
            // 
            this.PAN_Keys.AutoScroll = true;
            this.PAN_Keys.Location = new System.Drawing.Point(9, 31);
            this.PAN_Keys.Name = "PAN_Keys";
            this.PAN_Keys.Size = new System.Drawing.Size(234, 293);
            this.PAN_Keys.TabIndex = 3;
            // 
            // BTN_Insert
            // 
            this.BTN_Insert.Location = new System.Drawing.Point(11, 2);
            this.BTN_Insert.Name = "BTN_Insert";
            this.BTN_Insert.Size = new System.Drawing.Size(231, 23);
            this.BTN_Insert.TabIndex = 4;
            this.BTN_Insert.Text = "Insert Inside/After Selected Item >>";
            this.BTN_Insert.UseVisualStyleBackColor = true;
            this.BTN_Insert.Click += new System.EventHandler(this.BTN_Insert_Click);
            // 
            // BTN_Envelope
            // 
            this.BTN_Envelope.Location = new System.Drawing.Point(662, 41);
            this.BTN_Envelope.Name = "BTN_Envelope";
            this.BTN_Envelope.Size = new System.Drawing.Size(92, 23);
            this.BTN_Envelope.TabIndex = 7;
            this.BTN_Envelope.Text = "Add Envelope";
            this.BTN_Envelope.UseVisualStyleBackColor = true;
            this.BTN_Envelope.Click += new System.EventHandler(this.BTN_AddContainer_Click);
            // 
            // BTN_Array
            // 
            this.BTN_Array.Enabled = false;
            this.BTN_Array.Location = new System.Drawing.Point(760, 41);
            this.BTN_Array.Name = "BTN_Array";
            this.BTN_Array.Size = new System.Drawing.Size(73, 23);
            this.BTN_Array.TabIndex = 8;
            this.BTN_Array.Text = "Add Array";
            this.BTN_Array.UseVisualStyleBackColor = true;
            this.BTN_Array.Click += new System.EventHandler(this.BTN_AddContainer_Click);
            // 
            // BTN_Entity
            // 
            this.BTN_Entity.Enabled = false;
            this.BTN_Entity.Location = new System.Drawing.Point(839, 41);
            this.BTN_Entity.Name = "BTN_Entity";
            this.BTN_Entity.Size = new System.Drawing.Size(75, 23);
            this.BTN_Entity.TabIndex = 9;
            this.BTN_Entity.Text = "Add Entity";
            this.BTN_Entity.UseVisualStyleBackColor = true;
            this.BTN_Entity.Click += new System.EventHandler(this.BTN_AddContainer_Click);
            // 
            // PAN_Relational
            // 
            this.PAN_Relational.AutoScroll = true;
            this.PAN_Relational.Location = new System.Drawing.Point(252, 72);
            this.PAN_Relational.Name = "PAN_Relational";
            this.PAN_Relational.Size = new System.Drawing.Size(662, 252);
            this.PAN_Relational.TabIndex = 10;
            this.PAN_Relational.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PAN_Relational_Scroll);
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(838, 2);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(75, 23);
            this.BTN_Close.TabIndex = 11;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // BTN_Send
            // 
            this.BTN_Send.Location = new System.Drawing.Point(759, 2);
            this.BTN_Send.Name = "BTN_Send";
            this.BTN_Send.Size = new System.Drawing.Size(73, 23);
            this.BTN_Send.TabIndex = 12;
            this.BTN_Send.Text = "Send";
            this.BTN_Send.UseVisualStyleBackColor = true;
            this.BTN_Send.Click += new System.EventHandler(this.BTN_Send_Click);
            // 
            // BTN_MQTT
            // 
            this.BTN_MQTT.Location = new System.Drawing.Point(641, 2);
            this.BTN_MQTT.Name = "BTN_MQTT";
            this.BTN_MQTT.Size = new System.Drawing.Size(112, 23);
            this.BTN_MQTT.TabIndex = 13;
            this.BTN_MQTT.Text = "Copy as MQTT";
            this.BTN_MQTT.UseVisualStyleBackColor = true;
            this.BTN_MQTT.Click += new System.EventHandler(this.BTN_MQTT_Click);
            // 
            // BTN_LUA
            // 
            this.BTN_LUA.Location = new System.Drawing.Point(535, 2);
            this.BTN_LUA.Name = "BTN_LUA";
            this.BTN_LUA.Size = new System.Drawing.Size(100, 23);
            this.BTN_LUA.TabIndex = 14;
            this.BTN_LUA.Text = "Copy as LUA";
            this.BTN_LUA.UseVisualStyleBackColor = true;
            this.BTN_LUA.Click += new System.EventHandler(this.BTN_LUA_Click);
            // 
            // BTN_Preset
            // 
            this.BTN_Preset.Enabled = false;
            this.BTN_Preset.Location = new System.Drawing.Point(252, 41);
            this.BTN_Preset.Name = "BTN_Preset";
            this.BTN_Preset.Size = new System.Drawing.Size(103, 23);
            this.BTN_Preset.TabIndex = 16;
            this.BTN_Preset.Text = "Save as Preset";
            this.BTN_Preset.UseVisualStyleBackColor = true;
            this.BTN_Preset.Click += new System.EventHandler(this.BTN_Preset_Click);
            // 
            // BTN_Clear
            // 
            this.BTN_Clear.Location = new System.Drawing.Point(859, 8);
            this.BTN_Clear.Name = "BTN_Clear";
            this.BTN_Clear.Size = new System.Drawing.Size(55, 23);
            this.BTN_Clear.TabIndex = 17;
            this.BTN_Clear.Text = "Clear";
            this.BTN_Clear.UseVisualStyleBackColor = true;
            this.BTN_Clear.Click += new System.EventHandler(this.BTN_Clear_Click);
            // 
            // CB_GEBL
            // 
            this.CB_GEBL.AutoSize = true;
            this.CB_GEBL.Location = new System.Drawing.Point(332, 6);
            this.CB_GEBL.Name = "CB_GEBL";
            this.CB_GEBL.Size = new System.Drawing.Size(97, 17);
            this.CB_GEBL.TabIndex = 18;
            this.CB_GEBL.Text = "GEBL or earlier";
            this.CB_GEBL.UseVisualStyleBackColor = true;
            this.CB_GEBL.CheckedChanged += new System.EventHandler(this.CB_GEBL_CheckedChanged);
            // 
            // BTN_DelPreset
            // 
            this.BTN_DelPreset.Enabled = false;
            this.BTN_DelPreset.Location = new System.Drawing.Point(361, 41);
            this.BTN_DelPreset.Name = "BTN_DelPreset";
            this.BTN_DelPreset.Size = new System.Drawing.Size(103, 23);
            this.BTN_DelPreset.TabIndex = 19;
            this.BTN_DelPreset.Text = "Delete Preset";
            this.BTN_DelPreset.UseVisualStyleBackColor = true;
            this.BTN_DelPreset.Click += new System.EventHandler(this.BTN_DelPreset_Click);
            // 
            // CB_FullDM
            // 
            this.CB_FullDM.AutoSize = true;
            this.CB_FullDM.Location = new System.Drawing.Point(251, 6);
            this.CB_FullDM.Name = "CB_FullDM";
            this.CB_FullDM.Size = new System.Drawing.Size(81, 17);
            this.CB_FullDM.TabIndex = 20;
            this.CB_FullDM.Text = "Use full DM";
            this.CB_FullDM.UseVisualStyleBackColor = true;
            this.CB_FullDM.CheckedChanged += new System.EventHandler(this.CB_FullDM_CheckedChanged);
            // 
            // BTN_JSON
            // 
            this.BTN_JSON.Location = new System.Drawing.Point(429, 2);
            this.BTN_JSON.Name = "BTN_JSON";
            this.BTN_JSON.Size = new System.Drawing.Size(100, 23);
            this.BTN_JSON.TabIndex = 21;
            this.BTN_JSON.Text = "Copy as JSON";
            this.BTN_JSON.UseVisualStyleBackColor = true;
            this.BTN_JSON.Click += new System.EventHandler(this.BTN_JSON_Click);
            // 
            // BTN_Swap
            // 
            this.BTN_Swap.Location = new System.Drawing.Point(513, 41);
            this.BTN_Swap.Name = "BTN_Swap";
            this.BTN_Swap.Size = new System.Drawing.Size(103, 23);
            this.BTN_Swap.TabIndex = 22;
            this.BTN_Swap.Text = "Swap Cavities";
            this.BTN_Swap.UseVisualStyleBackColor = true;
            this.BTN_Swap.Click += new System.EventHandler(this.BTN_Swap_Click);
            // 
            // PAN_Buttons
            // 
            this.PAN_Buttons.Controls.Add(this.BTN_JSON);
            this.PAN_Buttons.Controls.Add(this.BTN_Insert);
            this.PAN_Buttons.Controls.Add(this.BTN_Close);
            this.PAN_Buttons.Controls.Add(this.CB_FullDM);
            this.PAN_Buttons.Controls.Add(this.BTN_Send);
            this.PAN_Buttons.Controls.Add(this.BTN_MQTT);
            this.PAN_Buttons.Controls.Add(this.CB_GEBL);
            this.PAN_Buttons.Controls.Add(this.BTN_LUA);
            this.PAN_Buttons.Location = new System.Drawing.Point(0, 325);
            this.PAN_Buttons.Name = "PAN_Buttons";
            this.PAN_Buttons.Size = new System.Drawing.Size(921, 29);
            this.PAN_Buttons.TabIndex = 23;
            // 
            // RB_Name
            // 
            this.RB_Name.AutoSize = true;
            this.RB_Name.Location = new System.Drawing.Point(152, 8);
            this.RB_Name.Name = "RB_Name";
            this.RB_Name.Size = new System.Drawing.Size(53, 17);
            this.RB_Name.TabIndex = 24;
            this.RB_Name.Text = "Name";
            this.RB_Name.UseVisualStyleBackColor = true;
            this.RB_Name.CheckedChanged += new System.EventHandler(this.RB_Sort_CheckedChanged);
            // 
            // RB_Key
            // 
            this.RB_Key.AutoSize = true;
            this.RB_Key.Checked = true;
            this.RB_Key.Location = new System.Drawing.Point(94, 8);
            this.RB_Key.Name = "RB_Key";
            this.RB_Key.Size = new System.Drawing.Size(43, 17);
            this.RB_Key.TabIndex = 25;
            this.RB_Key.TabStop = true;
            this.RB_Key.Text = "Key";
            this.RB_Key.UseVisualStyleBackColor = true;
            this.RB_Key.CheckedChanged += new System.EventHandler(this.RB_Sort_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Sort by:";
            // 
            // RelationalBuilder2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 355);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RB_Key);
            this.Controls.Add(this.RB_Name);
            this.Controls.Add(this.PAN_Buttons);
            this.Controls.Add(this.BTN_Swap);
            this.Controls.Add(this.BTN_DelPreset);
            this.Controls.Add(this.BTN_Clear);
            this.Controls.Add(this.BTN_Preset);
            this.Controls.Add(this.PAN_Relational);
            this.Controls.Add(this.BTN_Entity);
            this.Controls.Add(this.BTN_Array);
            this.Controls.Add(this.BTN_Envelope);
            this.Controls.Add(this.PAN_Keys);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTN_Import);
            this.Controls.Add(this.TB_Import);
            this.Name = "RelationalBuilder2";
            this.Text = "RelationalBuilder2";
            this.Resize += new System.EventHandler(this.RelationalBuilder2_Resize);
            this.PAN_Buttons.ResumeLayout(false);
            this.PAN_Buttons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Import;
        private System.Windows.Forms.Button BTN_Import;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PAN_Keys;
        private System.Windows.Forms.Button BTN_Insert;
        private System.Windows.Forms.Button BTN_Envelope;
        private System.Windows.Forms.Button BTN_Array;
        private System.Windows.Forms.Button BTN_Entity;
        private System.Windows.Forms.Panel PAN_Relational;
        private System.Windows.Forms.Button BTN_Close;
        private System.Windows.Forms.Button BTN_Send;
        private System.Windows.Forms.Button BTN_MQTT;
        private System.Windows.Forms.Button BTN_LUA;
        private System.Windows.Forms.Button BTN_Preset;
        private System.Windows.Forms.Button BTN_Clear;
        private System.Windows.Forms.CheckBox CB_GEBL;
        private System.Windows.Forms.Button BTN_DelPreset;
        private System.Windows.Forms.CheckBox CB_FullDM;
        private System.Windows.Forms.Button BTN_JSON;
        private System.Windows.Forms.Button BTN_Swap;
        private System.Windows.Forms.Panel PAN_Buttons;
        private System.Windows.Forms.RadioButton RB_Name;
        private System.Windows.Forms.RadioButton RB_Key;
        private System.Windows.Forms.Label label2;
    }
}