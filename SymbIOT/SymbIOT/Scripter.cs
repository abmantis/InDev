using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace SymbIOT
{
    public partial class Scripter : Form
    {
        public List<KeyValue> keyValues;        
        ListBox activeLB;
        public RelationalItem2 selectedChild;
        public SymbIOT parent;
        private XmlDocument batch;
        private bool batchrunning;

        public delegate void UpdateScripterLog();
        public UpdateScripterLog scriptDelegate;

        public Scripter(SymbIOT p, List<KeyValue> kv)
        {
            InitializeComponent();
            parent = p;
            keyValues = kv;
            activeLB = null;
            batchrunning = false;
            LoadLBs(false);

            scriptDelegate = new UpdateScripterLog(UpdateLog);

            batch = new XmlDocument();
            XmlNode node = batch.CreateNode(XmlNodeType.Element, "Batch", null);
            batch.AppendChild(node);
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Hide();
            parent.listening = false;
            parent.listenTimer.Enabled = false;
        }

        private void LoadLBs(bool allkvs)
        {
            int curtab = 0;
            foreach (KeyValue k in keyValues)
            {
                if ((allkvs || k.IsUsed))
                {
                    bool found = false;
                    string instance = k.Instance.Split('_')[0];
                    instance = instance.StartsWith("Config") ? "Config" : instance;
                    for (int i = 0; i < PAN_Keys.Controls.Count; i++)
                    {
                        if (PAN_Keys.Controls[i].Text == instance)
                        {
                            found = true;
                            curtab = i;
                        }
                    }
                    if (!found)
                    {
                        AddGroupBox(PAN_Keys.Controls.Count, instance);
                        curtab = PAN_Keys.Controls.Count - 1;
                    }

                    if (k.KeyID.StartsWith("FD"))
                    {
                        //relationalTypes.Add(k);
                    }
                    else
                    {
                        ((ListBox)PAN_Keys.Controls[curtab].Controls[0]).Items.Add(k.AttributeName + " (0x" + k.KeyID + ")");
                    }

                    /*XCatItem xc = new XCatItem(k, this);
                    xc.Top = TAB_Keys.TabPages[curtab].Controls.Count * xc.Height;
                    xc.Left = 0;
                    TAB_Keys.TabPages[curtab].Controls.Add(xc);*/

                }
            }
        }

        private void AddGroupBox(int index, string name)
        {
            GroupBox gb = new GroupBox();
            gb.Text = name;
            gb.Width = PAN_Keys.Width - 20;
            gb.Top = (gb.Height + 5) * index;
            gb.Left = 0;
            ListBox lb = new ListBox();
            lb.Click += new EventHandler(lb_Click);
            lb.Width = gb.Width - 10;
            lb.Height = gb.Height - 10;
            lb.Top = 15;
            lb.Left = 5;
            gb.Controls.Add(lb);
            PAN_Keys.Controls.Add(gb);
            /*if (name == "Presets")
            {
                presetLB = lb;
            }*/
        }

        void lb_Click(object sender, EventArgs e)
        {
            activeLB = (ListBox)sender;
            foreach (GroupBox g in PAN_Keys.Controls)
            {
                if (g.Controls[0] != activeLB)
                {
                    ((ListBox)g.Controls[0]).SelectedIndex = -1;
                    ((ListBox)g.Controls[0]).SelectedItem = null;
                }
            }
            //BTN_DelPreset.Enabled = (activeLB == presetLB) && presetLB.SelectedIndex >= 0;
        }

        private void BTN_AddSend_Click(object sender, EventArgs e)
        {
            //if (this.selectedChild != null)
            {
                AddToPanel(PAN_Send, activeLB.SelectedItem.ToString(),"");
            }
        }

        private void AddToPanel(Panel pan, string kvp, string val)
        {
            KeyValue inkey = null;
            try
            {
                foreach (KeyValue k in keyValues)
                {
                    if (kvp.Contains("0x" + k.KeyID))
                    {
                        inkey = k;
                        break;
                    }
                }
                if (inkey.isSet || pan == PAN_Recv)
                {
                    RelationalItem2 rel = new RelationalItem2(inkey, this, pan, val, true);
                    pan.Controls.Add(rel);
                    foreach (RelationalItem2 r in pan.Controls)
                    {
                        r.Top = r.Height * pan.Controls.GetChildIndex(r);
                    }
       
                }
                else
                {
                    MessageBox.Show("Selected key is read-only and cannot be sent in a command.");
                }
            }
            catch
            {
                MessageBox.Show("No key selected.");
            }
        }

        private void BTN_AddRcv_Click(object sender, EventArgs e)
        {
            AddToPanel(PAN_Recv, activeLB.SelectedItem.ToString(), "");
        }

        private void BTN_Run_Click(object sender, EventArgs e)
        {
            if (BTN_Run.Text == "Run Step")
            {
                string setmsg = "";
                bool validcmd = true;
                string sentlist = "";
                foreach (RelationalItem2 r in PAN_Send.Controls)
                {
                    if (r.KVPValue(false) != "")
                    {
                        setmsg += r.KVPMessage();
                        sentlist += "(0x" + r.KeyValue + ")" + r.KeyName + ": " + r.KVPValue(false) + "\r\n";
                    }
                    else
                    {
                        validcmd = false;
                    }
                }
                /*if (validcmd && setmsg.Length > 0)
                {
                    parent.SendExternal(setmsg, false, true);
                }
                else
                {
                    MessageBox.Show("All command KVPs must have an assigned value.");
                }*/
                parent.listenList.Clear();

                foreach (RelationalItem2 r in PAN_Recv.Controls)
                {
                    DataRow dr = parent.listenList.NewRow();
                    dr["Key"] = r.KeyValue;
                    dr["Name"] = r.KeyName;
                    dr["Value"] = r.KVPValue(false);
                    dr["HexValue"] = r.KVPMessage();
                    dr["Result"] = "";
                    dr["Pass"] = "";
                    parent.listenList.Rows.Add(dr);
                }

                try
                {
                    //parent.listenUntil = DateTime.Now.AddMilliseconds(int.Parse(TB_Wait.Text));
                    parent.listenTimer.Interval = int.Parse(TB_Wait.Text);
                    if ((validcmd && setmsg.Length > 0) || PAN_Send.Controls.Count == 0)
                    {                        
                        parent.listening = true;
                        parent.listenTimer.Enabled = true;
                        bool sent = parent.SendExternal(setmsg, false, true);
                        if (sent)
                        {
                            RTB_Log.AppendText("Running " + TB_Name.Text + "...\r\n\r\n");
                            RTB_Log.AppendText("Sent:\r\n" + sentlist + "\r\n");
                            RTB_Log.AppendText("Waiting " + TB_Wait.Text + "ms for responses...\r\n\r\n");
                            RTB_Log.ScrollToCaret();
                            if (!batchrunning)
                            {
                                BTN_Run.Text = "Stop";
                                LB_Batch.Enabled = false;
                                foreach (Button b in PAN_Buttons.Controls)
                                {
                                    if (!(b == BTN_Run || b == BTN_Close))
                                    {
                                        b.Enabled = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            parent.listening = false;
                            parent.listenTimer.Enabled = false;
                            batchrunning = false;
                            LB_Batch.Enabled = true;
                            BTN_Run.Text = "Run Step";
                            BTN_BatchRun.Text = "Run Batch";
                            foreach (Button b in PAN_Buttons.Controls)
                            {
                                b.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("All command KVPs must have an assigned value.");
                    }
                }
                catch
                {
                    MessageBox.Show("Wait time is an invalid value.");
                }
            }
            else
            {
                BTN_Run.Text = "Run Step";
                parent.listening = false;
                parent.listenTimer.Enabled = false;
                RTB_Log.AppendText("Aborted.\r\n\r\n");
                LB_Batch.Enabled = true;
                foreach (Button b in PAN_Buttons.Controls)
                {
                    b.Enabled = true;
                }
            }
        }

        public void UpdateLog()
        {
            bool pass = true;
            bool check = false;
            foreach (DataRow dr in parent.listenList.Rows)
            {
                if (dr["Result"].ToString() != "")
                {
                    string result = "PASS";
                    Color resultcolor = Color.LightGreen;
                    if (!dr["Pass"].ToString().Contains("0"))
                    {
                        result = "\r\nPASS:";
                        resultcolor = Color.LightGreen;
                    }
                    else if(!dr["Pass"].ToString().Contains("1"))
                    {
                        result = "\r\nFAIL:";
                        resultcolor = Color.LightCoral;
                        pass = false;
                    }
                    else
                    {
                        result = "\r\nCHECK:";
                        resultcolor = Color.Yellow;
                        check = true;
                    }
                    result += "\r\n\tExpected result: (0x" + dr["Key"] + ")" + dr["Name"] + ": " + dr["Value"] + "\r\n";
                    int selstart = RTB_Log.TextLength;
                    RTB_Log.AppendText(result);
                    int selend = RTB_Log.TextLength;

                    RTB_Log.Select(selstart, selend - selstart);
                    RTB_Log.SelectionBackColor = resultcolor;

                    string[] results = dr["Result"].ToString().Split('\t');
                    string[] passes = dr["Pass"].ToString().Split(';');
                    string[] timestamps = dr["TimeStamp"].ToString().Split(';');
                    for (int i = 1; i < results.Length; i++)
                    {
                        selstart = RTB_Log.TextLength;
                        resultcolor = passes[i] == "1" ? Color.LightGreen : Color.LightCoral;
                        RTB_Log.AppendText(timestamps[i] + "\t" + results[i]);
                        selend = RTB_Log.TextLength;
                        RTB_Log.Select(selstart, selend - selstart);
                        RTB_Log.SelectionBackColor = resultcolor;
                    }
                }
                else
                {
                    int selstart = RTB_Log.TextLength;
                    RTB_Log.AppendText("FAIL:\r\n" + DateTime.Now.ToString("MMddyy hh:mm:ss") + "\t(0x" + dr["Key"] + ")" + dr["Name"] + " not received in " + TB_Wait.Text + "ms\r\n");
                    int selend = RTB_Log.TextLength;
                    RTB_Log.Select(selstart, selend - selstart);
                    RTB_Log.SelectionBackColor = dr["Result"].ToString().StartsWith("PASS") ? Color.LightGreen : Color.LightCoral;
                    pass = false;
                }
                RTB_Log.SelectionLength = 0;
            }
            RTB_Log.AppendText("Done.  Overall Result = ");
            int start = RTB_Log.TextLength;
            string overalltext = check && pass ? "CHECK" : "PASS";
            overalltext = pass ? overalltext : "FAIL";
            RTB_Log.AppendText(overalltext + "\r\n\r\n");
            int end = RTB_Log.TextLength;
            RTB_Log.Select(start, end - start);
            Color overallcolor = check && pass ? Color.Yellow : Color.LightGreen;
            overallcolor = pass ? overallcolor : Color.LightCoral;
            RTB_Log.SelectionBackColor = overallcolor;
            RTB_Log.SelectionLength = 0;
            RTB_Log.ScrollToCaret();
            if (batchrunning || BTN_Run.Text == "Stop")
            {
                if (LB_Batch.SelectedIndex < LB_Batch.Items.Count-1 && BTN_Run.Text != "Stop")
                {
                    LB_Batch.SelectedIndex++;
                }
                else
                {
                    batchrunning = false;
                    parent.listening = false;
                    parent.listenTimer.Enabled = false;
                    BTN_BatchRun.Text = "Run Batch";
                    BTN_Run.Text = "Run Step";
                    LB_Batch.Enabled = true;
                    foreach (Button b in PAN_Buttons.Controls)
                    {
                        b.Enabled = true;
                    }
                }
            }
        }

        private void BTN_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "SymbIOT_Script-" + parent.dm_name + "-" + TB_Name.Text.Replace(" ","") + ".xml";
            sfd.Filter = "Script Files (*.xml)|*.xml";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK && sfd.FileName != "")
            {
                XmlDocument doc = CreateXMLDoc(TB_Name.Text);
                doc.Save(sfd.FileName);
            }
            

        }

        private XmlDocument CreateXMLDoc(string docname)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Script", null);
            XmlNode name = doc.CreateNode(XmlNodeType.Attribute, "Name", null);
            name.Value = docname;
            node.Attributes.SetNamedItem(name);
            foreach (RelationalItem2 r in PAN_Send.Controls)
            {
                XmlNode outkey = doc.CreateElement("OutKey");
                XmlNode kvp = doc.CreateElement("KVP");
                kvp.InnerText = r.KeyValue;
                XmlNode val = doc.CreateElement("Value");
                val.InnerText = r.KVPValue(true);
                outkey.AppendChild(kvp);
                outkey.AppendChild(val);
                node.AppendChild(outkey);
            }
            foreach (RelationalItem2 r in PAN_Recv.Controls)
            {
                XmlNode inkey = doc.CreateElement("InKey");
                XmlNode kvp = doc.CreateElement("KVP");
                kvp.InnerText = r.KeyValue;
                XmlNode val = doc.CreateElement("Value");
                val.InnerText = r.KVPValue(true);
                inkey.AppendChild(kvp);
                inkey.AppendChild(val);
                node.AppendChild(inkey);
            }
            XmlNode wait = doc.CreateElement("Wait");
            wait.InnerText = TB_Wait.Text;
            node.AppendChild(wait);
            doc.AppendChild(node);

            return doc;
        }

        private void BTN_LoadSaved_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Script Files (*.xml)|*.xml";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK && ofd.FileName != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(ofd.FileName);
                XmlNodeList n = xml.SelectNodes("Batch");
                if (n.Count == 0)
                {
                    LoadDoc(xml);                    
                }
                else
                {
                    batch = xml;
                    LB_Batch.Items.Clear();
                    XmlNodeList steplist = batch.SelectNodes("//Batch//Script");
                    foreach (XmlNode xn in steplist)
                    {
                        LB_Batch.Items.Add(xn.Attributes.GetNamedItem("Name").InnerText);
                    }
                    LB_Batch.SelectedIndex = 0;
                    BTN_SaveBatch.Enabled = true;
                    BTN_BatchRun.Enabled = true;
                }
            }
        }

        private void LoadDoc(XmlDocument xml)
        {            
            PAN_Recv.Controls.Clear();
            PAN_Send.Controls.Clear();
            XmlNodeList nodes = xml.GetElementsByTagName("OutKey");
            foreach (XmlNode n in nodes)
            {
                AddToPanel(PAN_Send, "0x" + n.SelectSingleNode("KVP").InnerText, n.SelectSingleNode("Value").InnerText);
            }
            nodes = xml.GetElementsByTagName("InKey");
            foreach (XmlNode n in nodes)
            {
                AddToPanel(PAN_Recv, "0x" + n.SelectSingleNode("KVP").InnerText, n.SelectSingleNode("Value").InnerText);
            }
            nodes = xml.GetElementsByTagName("Wait");
            foreach (XmlNode n in nodes)
            {
                TB_Wait.Text = n.InnerText;
            }
            TB_Name.Text = xml.SelectSingleNode("Script").Attributes.GetNamedItem("Name").InnerText;
        }

        private void BTN_ClearLog_Click(object sender, EventArgs e)
        {
            RTB_Log.Text = "";
        }

        private void BTN_SaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "SymbIOTScriptLog" + DateTime.Now.ToString("yyMMddhhmmss") + ".txt";
            sfd.Filter = "Text Files (*.txt)|*.txt";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK && sfd.FileName != "")
            {
                Stream str = sfd.OpenFile();
                StreamWriter sw = new StreamWriter(str);
                sw.WriteLine("Log generated by SymbIOT on " + DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
                sw.WriteLine("Using Data Model '" + parent.dm_name + "'");
                sw.WriteLine("---------------------------------------");
                sw.Write(RTB_Log.Text.Replace("\n","\r\n"));
                str.Flush();
                sw.Close();
                MessageBox.Show("Log file saved.");
            }
        }

        private void Scripter_Resize(object sender, EventArgs e)
        {
            RTB_Log.Height = Math.Max(this.Height - 404, 1);
            PAN_Keys.Height = Math.Max(this.Height - 57, 1);
        }

        private void BTN_BatchAdd_Click(object sender, EventArgs e)
        {
            if (TB_Name.Text != "")
            {
                bool canadd = true;
                XmlDocument doc = CreateXMLDoc(TB_Name.Text);
                XmlNodeList inbatch = batch.SelectNodes("//Batch//Script[@Name='" + TB_Name.Text + "']");
                if (inbatch.Count > 0)
                {
                    if (inbatch[0].InnerText != doc.SelectSingleNode("Script").InnerText)
                    {
                        canadd = false;
                        if (MessageBox.Show("Step with the same name but different content already in list.  Press Yes to replace current step or No to cancel.\r\nTo add as a separate step, provide a unique name.","Duplicate Step", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            XmlNode xn = batch.ImportNode(doc.SelectSingleNode("Script"),true);
                            XmlNode oldnode = batch.SelectSingleNode("//Batch//Script[@Name='" + TB_Name.Text + "']");
                            oldnode.ParentNode.ReplaceChild(xn, oldnode);
                        }
                    }
                }
                if (canadd)
                {
                    LB_Batch.Items.Add(TB_Name.Text);
                    XmlNode newscript = batch.ImportNode(doc.SelectSingleNode("Script"), true);
                    batch.SelectSingleNode("Batch").AppendChild(newscript);
                    BTN_BatchRun.Enabled = true;
                    BTN_SaveBatch.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Batch step requires a name.");
                TB_Name.Focus();
            }
        }

        private void LB_Batch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                XmlNodeList sel = batch.SelectNodes("//Batch//Script[@Name='" + LB_Batch.SelectedItem.ToString() + "']");
                XmlDocument xml = new XmlDocument();
                XmlNode script = xml.ImportNode(sel[0], true);
                xml.AppendChild(script);
                LoadDoc(xml);
                if (batchrunning)
                {
                    BTN_Run_Click(this, null);
                }
            }
            catch { }
        }

        private void BTN_BatchRun_Click(object sender, EventArgs e)
        {
            if (BTN_BatchRun.Text == "Run Batch")
            {
                batchrunning = true;
                if (LB_Batch.SelectedIndex == 0)
                {
                    LB_Batch_SelectedIndexChanged(this, null);
                }
                else
                {
                    LB_Batch.SelectedIndex = 0;
                }
                BTN_BatchRun.Text = "Stop Batch Run";
                LB_Batch.Enabled = false;
                foreach (Button b in PAN_Buttons.Controls)
                {
                    if (!(b == BTN_BatchRun || b == BTN_Close))
                    {
                        b.Enabled = false;
                    }
                }
            }
            else
            {
                batchrunning = false;
                parent.listenTimer.Enabled = false;
                parent.listening = false;
                RTB_Log.AppendText("Aborted.\r\n\r\n");

                BTN_BatchRun.Text = "Run Batch";
                LB_Batch.Enabled = true;
                foreach (Button b in PAN_Buttons.Controls)
                {
                    b.Enabled = true;
                }
            }
            
        }

        private void BTN_SaveBatch_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "SymbIOT_Batch-" + parent.dm_name + ".xml";
            sfd.Filter = "Script Files (*.xml)|*.xml";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK && sfd.FileName != "")
            {                
                batch.Save(sfd.FileName);
            }

        }

        private void TS_MoveUp_Click(object sender, EventArgs e)
        {
            MoveItem(-1);
        }

        private void TS_MoveDown_Click(object sender, EventArgs e)
        {
            MoveItem(1);
        }

        private void TS_Remove_Click(object sender, EventArgs e)
        {
            MoveItem(0);
            if (LB_Batch.Items.Count == 0)
            {
                BTN_BatchRun.Enabled = false;
                BTN_SaveBatch.Enabled = false;
            }
        }

        private void MoveItem(int direction)
        {
            // Checking selected item
            if (LB_Batch.SelectedItem == null || LB_Batch.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = LB_Batch.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= LB_Batch.Items.Count)
                return; // Index out of range - nothing to do

            object selected = LB_Batch.SelectedItem;

            // Removing removable element
            LB_Batch.Items.Remove(selected);
            if (direction != 0)
            {
                // Insert it in new position
                LB_Batch.Items.Insert(newIndex, selected);
                // Restore selection
                LB_Batch.SetSelected(newIndex, true);
            }

            RefreshBatch();
        }

        private void RefreshBatch()
        {
            XmlDocument copybatch = new XmlDocument();
            XmlNode node = copybatch.CreateNode(XmlNodeType.Element, "Batch", null);
            copybatch.AppendChild(node);
          
            foreach (string s in LB_Batch.Items)
            {
                XmlNodeList sel = batch.SelectNodes("//Batch//Script[@Name='" + s + "']");
                XmlDocument xml = new XmlDocument();
                XmlNode script = xml.ImportNode(sel[0], true);
                xml.AppendChild(script);
                
                    //LB_Batch.Items.Add(TB_Name.Text);
                    XmlNode newscript = copybatch.ImportNode(xml.SelectSingleNode("Script"), true);
                    copybatch.SelectSingleNode("Batch").AppendChild(newscript);
               
            }
            batch = copybatch;
        }
    }
}

