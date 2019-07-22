namespace SymbIOT
{
    partial class Scripter
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
            this.components = new System.ComponentModel.Container();
            this.PAN_Keys = new System.Windows.Forms.Panel();
            this.PAN_Send = new System.Windows.Forms.Panel();
            this.PAN_Recv = new System.Windows.Forms.Panel();
            this.BTN_AddSend = new System.Windows.Forms.Button();
            this.BTN_AddRcv = new System.Windows.Forms.Button();
            this.LB_Batch = new System.Windows.Forms.ListBox();
            this.CM_Batch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TS_MoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.TS_MoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.TS_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.BTN_LoadSaved = new System.Windows.Forms.Button();
            this.BTN_BatchAdd = new System.Windows.Forms.Button();
            this.BTN_Save = new System.Windows.Forms.Button();
            this.BTN_Run = new System.Windows.Forms.Button();
            this.BTN_BatchRun = new System.Windows.Forms.Button();
            this.BTN_SaveLog = new System.Windows.Forms.Button();
            this.BTN_ClearLog = new System.Windows.Forms.Button();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Wait = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RTB_Log = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TB_Name = new System.Windows.Forms.TextBox();
            this.BTN_SaveBatch = new System.Windows.Forms.Button();
            this.PAN_Buttons = new System.Windows.Forms.Panel();
            this.CM_Batch.SuspendLayout();
            this.PAN_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // PAN_Keys
            // 
            this.PAN_Keys.AutoScroll = true;
            this.PAN_Keys.Location = new System.Drawing.Point(5, 7);
            this.PAN_Keys.Name = "PAN_Keys";
            this.PAN_Keys.Size = new System.Drawing.Size(252, 456);
            this.PAN_Keys.TabIndex = 0;
            // 
            // PAN_Send
            // 
            this.PAN_Send.AutoScroll = true;
            this.PAN_Send.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PAN_Send.Location = new System.Drawing.Point(302, 23);
            this.PAN_Send.Name = "PAN_Send";
            this.PAN_Send.Size = new System.Drawing.Size(543, 155);
            this.PAN_Send.TabIndex = 1;
            // 
            // PAN_Recv
            // 
            this.PAN_Recv.AutoScroll = true;
            this.PAN_Recv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PAN_Recv.Location = new System.Drawing.Point(302, 205);
            this.PAN_Recv.Name = "PAN_Recv";
            this.PAN_Recv.Size = new System.Drawing.Size(543, 144);
            this.PAN_Recv.TabIndex = 2;
            // 
            // BTN_AddSend
            // 
            this.BTN_AddSend.Location = new System.Drawing.Point(261, 81);
            this.BTN_AddSend.Name = "BTN_AddSend";
            this.BTN_AddSend.Size = new System.Drawing.Size(37, 23);
            this.BTN_AddSend.TabIndex = 4;
            this.BTN_AddSend.Text = "-->";
            this.BTN_AddSend.UseVisualStyleBackColor = true;
            this.BTN_AddSend.Click += new System.EventHandler(this.BTN_AddSend_Click);
            // 
            // BTN_AddRcv
            // 
            this.BTN_AddRcv.Location = new System.Drawing.Point(259, 270);
            this.BTN_AddRcv.Name = "BTN_AddRcv";
            this.BTN_AddRcv.Size = new System.Drawing.Size(37, 23);
            this.BTN_AddRcv.TabIndex = 5;
            this.BTN_AddRcv.Text = "-->";
            this.BTN_AddRcv.UseVisualStyleBackColor = true;
            this.BTN_AddRcv.Click += new System.EventHandler(this.BTN_AddRcv_Click);
            // 
            // LB_Batch
            // 
            this.LB_Batch.ContextMenuStrip = this.CM_Batch;
            this.LB_Batch.FormattingEnabled = true;
            this.LB_Batch.Location = new System.Drawing.Point(854, 10);
            this.LB_Batch.Name = "LB_Batch";
            this.LB_Batch.Size = new System.Drawing.Size(140, 186);
            this.LB_Batch.TabIndex = 7;
            this.LB_Batch.SelectedIndexChanged += new System.EventHandler(this.LB_Batch_SelectedIndexChanged);
            // 
            // CM_Batch
            // 
            this.CM_Batch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TS_MoveUp,
            this.TS_MoveDown,
            this.TS_Remove});
            this.CM_Batch.Name = "CM_Batch";
            this.CM_Batch.Size = new System.Drawing.Size(139, 70);
            // 
            // TS_MoveUp
            // 
            this.TS_MoveUp.Name = "TS_MoveUp";
            this.TS_MoveUp.Size = new System.Drawing.Size(138, 22);
            this.TS_MoveUp.Text = "Move Up";
            this.TS_MoveUp.Click += new System.EventHandler(this.TS_MoveUp_Click);
            // 
            // TS_MoveDown
            // 
            this.TS_MoveDown.Name = "TS_MoveDown";
            this.TS_MoveDown.Size = new System.Drawing.Size(138, 22);
            this.TS_MoveDown.Text = "Move Down";
            this.TS_MoveDown.Click += new System.EventHandler(this.TS_MoveDown_Click);
            // 
            // TS_Remove
            // 
            this.TS_Remove.Name = "TS_Remove";
            this.TS_Remove.Size = new System.Drawing.Size(138, 22);
            this.TS_Remove.Text = "Remove";
            this.TS_Remove.Click += new System.EventHandler(this.TS_Remove_Click);
            // 
            // BTN_LoadSaved
            // 
            this.BTN_LoadSaved.Location = new System.Drawing.Point(-1, 61);
            this.BTN_LoadSaved.Name = "BTN_LoadSaved";
            this.BTN_LoadSaved.Size = new System.Drawing.Size(140, 23);
            this.BTN_LoadSaved.TabIndex = 8;
            this.BTN_LoadSaved.Text = "Load From File";
            this.BTN_LoadSaved.UseVisualStyleBackColor = true;
            this.BTN_LoadSaved.Click += new System.EventHandler(this.BTN_LoadSaved_Click);
            // 
            // BTN_BatchAdd
            // 
            this.BTN_BatchAdd.Location = new System.Drawing.Point(-1, 32);
            this.BTN_BatchAdd.Name = "BTN_BatchAdd";
            this.BTN_BatchAdd.Size = new System.Drawing.Size(140, 23);
            this.BTN_BatchAdd.TabIndex = 9;
            this.BTN_BatchAdd.Text = "Add to Batch";
            this.BTN_BatchAdd.UseVisualStyleBackColor = true;
            this.BTN_BatchAdd.Click += new System.EventHandler(this.BTN_BatchAdd_Click);
            // 
            // BTN_Save
            // 
            this.BTN_Save.Location = new System.Drawing.Point(-1, 119);
            this.BTN_Save.Name = "BTN_Save";
            this.BTN_Save.Size = new System.Drawing.Size(140, 23);
            this.BTN_Save.TabIndex = 10;
            this.BTN_Save.Text = "Save Step to File";
            this.BTN_Save.UseVisualStyleBackColor = true;
            this.BTN_Save.Click += new System.EventHandler(this.BTN_Save_Click);
            // 
            // BTN_Run
            // 
            this.BTN_Run.Location = new System.Drawing.Point(-1, 148);
            this.BTN_Run.Name = "BTN_Run";
            this.BTN_Run.Size = new System.Drawing.Size(140, 23);
            this.BTN_Run.TabIndex = 11;
            this.BTN_Run.Text = "Run Step";
            this.BTN_Run.UseVisualStyleBackColor = true;
            this.BTN_Run.Click += new System.EventHandler(this.BTN_Run_Click);
            // 
            // BTN_BatchRun
            // 
            this.BTN_BatchRun.Enabled = false;
            this.BTN_BatchRun.Location = new System.Drawing.Point(-1, 3);
            this.BTN_BatchRun.Name = "BTN_BatchRun";
            this.BTN_BatchRun.Size = new System.Drawing.Size(140, 23);
            this.BTN_BatchRun.TabIndex = 12;
            this.BTN_BatchRun.Text = "Run Batch";
            this.BTN_BatchRun.UseVisualStyleBackColor = true;
            this.BTN_BatchRun.Click += new System.EventHandler(this.BTN_BatchRun_Click);
            // 
            // BTN_SaveLog
            // 
            this.BTN_SaveLog.Location = new System.Drawing.Point(-1, 177);
            this.BTN_SaveLog.Name = "BTN_SaveLog";
            this.BTN_SaveLog.Size = new System.Drawing.Size(140, 23);
            this.BTN_SaveLog.TabIndex = 13;
            this.BTN_SaveLog.Text = "Save Log";
            this.BTN_SaveLog.UseVisualStyleBackColor = true;
            this.BTN_SaveLog.Click += new System.EventHandler(this.BTN_SaveLog_Click);
            // 
            // BTN_ClearLog
            // 
            this.BTN_ClearLog.Location = new System.Drawing.Point(-1, 206);
            this.BTN_ClearLog.Name = "BTN_ClearLog";
            this.BTN_ClearLog.Size = new System.Drawing.Size(140, 23);
            this.BTN_ClearLog.TabIndex = 14;
            this.BTN_ClearLog.Text = "Clear Log";
            this.BTN_ClearLog.UseVisualStyleBackColor = true;
            this.BTN_ClearLog.Click += new System.EventHandler(this.BTN_ClearLog_Click);
            // 
            // BTN_Close
            // 
            this.BTN_Close.Location = new System.Drawing.Point(-1, 236);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(140, 23);
            this.BTN_Close.TabIndex = 15;
            this.BTN_Close.Text = "Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(299, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Send Messages:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(304, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Receive Messages within";
            // 
            // TB_Wait
            // 
            this.TB_Wait.Location = new System.Drawing.Point(438, 183);
            this.TB_Wait.Name = "TB_Wait";
            this.TB_Wait.Size = new System.Drawing.Size(57, 20);
            this.TB_Wait.TabIndex = 18;
            this.TB_Wait.Text = "10000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(498, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "ms";
            // 
            // RTB_Log
            // 
            this.RTB_Log.Location = new System.Drawing.Point(263, 354);
            this.RTB_Log.Name = "RTB_Log";
            this.RTB_Log.Size = new System.Drawing.Size(582, 109);
            this.RTB_Log.TabIndex = 20;
            this.RTB_Log.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(677, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Name:";
            // 
            // TB_Name
            // 
            this.TB_Name.Location = new System.Drawing.Point(716, 1);
            this.TB_Name.Name = "TB_Name";
            this.TB_Name.Size = new System.Drawing.Size(124, 20);
            this.TB_Name.TabIndex = 22;
            // 
            // BTN_SaveBatch
            // 
            this.BTN_SaveBatch.Enabled = false;
            this.BTN_SaveBatch.Location = new System.Drawing.Point(-1, 90);
            this.BTN_SaveBatch.Name = "BTN_SaveBatch";
            this.BTN_SaveBatch.Size = new System.Drawing.Size(140, 23);
            this.BTN_SaveBatch.TabIndex = 23;
            this.BTN_SaveBatch.Text = "Save Batch to File";
            this.BTN_SaveBatch.UseVisualStyleBackColor = true;
            this.BTN_SaveBatch.Click += new System.EventHandler(this.BTN_SaveBatch_Click);
            // 
            // PAN_Buttons
            // 
            this.PAN_Buttons.Controls.Add(this.BTN_BatchRun);
            this.PAN_Buttons.Controls.Add(this.BTN_SaveBatch);
            this.PAN_Buttons.Controls.Add(this.BTN_LoadSaved);
            this.PAN_Buttons.Controls.Add(this.BTN_Save);
            this.PAN_Buttons.Controls.Add(this.BTN_Run);
            this.PAN_Buttons.Controls.Add(this.BTN_BatchAdd);
            this.PAN_Buttons.Controls.Add(this.BTN_SaveLog);
            this.PAN_Buttons.Controls.Add(this.BTN_ClearLog);
            this.PAN_Buttons.Controls.Add(this.BTN_Close);
            this.PAN_Buttons.Location = new System.Drawing.Point(854, 202);
            this.PAN_Buttons.Name = "PAN_Buttons";
            this.PAN_Buttons.Size = new System.Drawing.Size(142, 260);
            this.PAN_Buttons.TabIndex = 24;
            // 
            // Scripter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 475);
            this.Controls.Add(this.PAN_Buttons);
            this.Controls.Add(this.TB_Name);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.RTB_Log);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_Wait);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LB_Batch);
            this.Controls.Add(this.BTN_AddRcv);
            this.Controls.Add(this.BTN_AddSend);
            this.Controls.Add(this.PAN_Recv);
            this.Controls.Add(this.PAN_Send);
            this.Controls.Add(this.PAN_Keys);
            this.Name = "Scripter";
            this.Text = "Scripter";
            this.Resize += new System.EventHandler(this.Scripter_Resize);
            this.CM_Batch.ResumeLayout(false);
            this.PAN_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PAN_Keys;
        private System.Windows.Forms.Panel PAN_Send;
        private System.Windows.Forms.Panel PAN_Recv;
        private System.Windows.Forms.Button BTN_AddSend;
        private System.Windows.Forms.Button BTN_AddRcv;
        private System.Windows.Forms.ListBox LB_Batch;
        private System.Windows.Forms.Button BTN_LoadSaved;
        private System.Windows.Forms.Button BTN_BatchAdd;
        private System.Windows.Forms.Button BTN_Save;
        private System.Windows.Forms.Button BTN_Run;
        private System.Windows.Forms.Button BTN_BatchRun;
        private System.Windows.Forms.Button BTN_SaveLog;
        private System.Windows.Forms.Button BTN_ClearLog;
        private System.Windows.Forms.Button BTN_Close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_Wait;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox RTB_Log;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TB_Name;
        private System.Windows.Forms.Button BTN_SaveBatch;
        private System.Windows.Forms.Panel PAN_Buttons;
        private System.Windows.Forms.ContextMenuStrip CM_Batch;
        private System.Windows.Forms.ToolStripMenuItem TS_MoveUp;
        private System.Windows.Forms.ToolStripMenuItem TS_MoveDown;
        private System.Windows.Forms.ToolStripMenuItem TS_Remove;
    }
}