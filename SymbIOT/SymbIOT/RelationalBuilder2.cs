using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace SymbIOT
{
    public partial class RelationalBuilder2 : Form
    {
        public List<KeyValue> keyValues;
        public List<KeyValue> relationalTypes;
        ListBox activeLB;
        public RelationalItem2 selectedChild;
        public RelationalContainer selectedContainer;
        public SymbIOT parent;
        public string RelationalID;
        public int arrayCount = 0;
        public string envLSB = "01";
        public bool updateValues = true;
        string cyclestring = "";
        public XmlNodeList presets;
        public XmlDocument xml;
        ListBox presetLB;
        int scrollpos;
        public int numContainers;

        public RelationalBuilder2(SymbIOT p, List<KeyValue> kv)
        {
            InitializeComponent();
            parent = p;
            keyValues = kv;
            activeLB = null;
            selectedChild = null;
            selectedContainer = null;
            relationalTypes = new List<KeyValue>();
            RelationalID = "FD020001";
            scrollpos = PAN_Relational.VerticalScroll.Value;
            numContainers = 0;

            BTN_Swap.Visible = parent.dm_name == "Cooking";

            AddGroupBox(0, "Presets");

            AddGroupBox(PAN_Keys.Controls.Count, "Recent");

            LoadLBs(CB_FullDM.Checked);

            if (!File.Exists("SymbIOT_Presets.xml"))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText("SymbIOT_Presets.xml"))
                {
                    sw.WriteLine(Properties.Resources.SymbIOT_Presets);
                }
            }

            LoadXML();

        }

        private void LoadXML()
        {
            xml = new XmlDocument();
            xml.Load("SymbIOT_Presets.xml");
            presets = xml.SelectNodes("//Presets//Preset");
            RefreshPresets();
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
            if (name == "Presets")
            {
                presetLB = lb;
            }
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
            BTN_DelPreset.Enabled = (activeLB == presetLB) && presetLB.SelectedIndex >= 0;
        }

        private void LoadLBs(bool allkvs)
        {
            int curtab = 0;
            foreach (KeyValue k in keyValues)
            {
                if ((allkvs || k.IsUsed) && k.isSet)
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
                        relationalTypes.Add(k);
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

        public void ClearAllSelected()
        {
            foreach (RelationalContainer rc in PAN_Relational.Controls)
            {
                rc.ClearAllSelected();
            }
        }

        private void BTN_AddContainer_Click(object sender, EventArgs e)
        {
            string type = ((Button)sender).Text.Substring(4);
            InsertContainer(type, this.RelationalID);
        }

        private void InsertContainer(string type, string val)
        {
            KeyValue openKey = null;
            KeyValue closeKey = null;
            string openkv = "";
            string closekv = "";
            numContainers++;
            switch (type)
            {
                case "Envelope":
                    openkv = "FE010001";
                    closekv = "FE010002";
                    numContainers = 1;
                    break;
                case "Array":
                    openkv = "FE030001";
                    closekv = "FE030002";
                    break;
                case "Entity":
                    openkv = "FE020001";
                    closekv = "FE020002";
                    break;
                default:
                    break;
            }

            foreach (KeyValue k in keyValues)
            {
                if (k.KeyID == openkv)
                {
                    openKey = k;
                }
                if (k.KeyID == closekv)
                {
                    closeKey = k;
                }
            }
            if (this.selectedContainer == null)
            {
                RelationalContainer rc = new RelationalContainer(openKey, closeKey, this, this, 0, val);
                PAN_Relational.Controls.Add(rc);
                ClearAllSelected();
                ((RelationalItem2)rc.Controls[0]).SetSelected(true);
                BTN_Envelope.Enabled = false;
                BTN_Array.Enabled = true;
                BTN_Entity.Enabled = true;
                RefreshPresets();
            }
            else
            {
                if (!this.selectedChild.CloseBracket)
                {
                    RelationalContainer rc = new RelationalContainer(openKey, closeKey, this.selectedContainer, this, this.selectedContainer.Level + 1, val);
                    this.selectedContainer.AddContainer(rc);
                    ClearAllSelected();
                    ((RelationalItem2)rc.Controls[0]).SetSelected(true);
                }
                else
                {
                    if (this.selectedContainer.parent != this)
                    {
                        RelationalContainer rc = new RelationalContainer(openKey, closeKey, this.selectedContainer.parent, this, this.selectedContainer.Level, val);
                        ((RelationalContainer)this.selectedContainer.parent).selectedChild = this.selectedContainer;
                        ((RelationalContainer)this.selectedContainer.parent).AddContainer(rc);
                        ClearAllSelected();
                        ((RelationalItem2)rc.Controls[0]).SetSelected(true);
                    }
                }
            }
        }

        private void BTN_Insert_Click(object sender, EventArgs e)
        {
            if (activeLB != presetLB)
            {
                if (!BTN_Envelope.Enabled)
                {
                    if (this.selectedChild != null)
                    {
                        KeyValue inkey = null;
                        foreach (KeyValue k in keyValues)
                        {
                            if (activeLB.SelectedItem.ToString().Contains("0x" + k.KeyID))
                            {
                                inkey = k;
                                AddToRecent(activeLB.SelectedItem.ToString());
                                break;
                            }
                        }
                        this.selectedChild.parent.AddKey(inkey, "");
                    }
                    else
                    {
                        MessageBox.Show("No position selected to insert.");
                    }
                }
                else
                {
                    MessageBox.Show("Single keys can only be added inside a relational block.  Please add an envelope or preset first.");
                }
            }
            else
            {
                if (presetLB.SelectedItem != null)
                {
                    foreach (XmlNode ps in presets)
                    {
                        if (ps.SelectSingleNode("Type").InnerText == parent.dm_name && ps.SelectSingleNode("Name").InnerText == presetLB.SelectedItem.ToString())
                        {
                            bool isEnvelope = ps.SelectSingleNode("Data").InnerText.StartsWith("FE010001") ? true : false;
                            if (isEnvelope && !BTN_Envelope.Enabled)
                            {
                                MessageBox.Show("The selected preset is an Envelope block and cannot be inserted inside an existing block");
                            }
                            else if (!isEnvelope && BTN_Envelope.Enabled)
                            {
                                MessageBox.Show("The selected preset is an Array or Entity block and must be insterted inside an existing block");
                            }
                            else
                            {
                                ParseImportMessage(ps.SelectSingleNode("Data").InnerText);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void AddToRecent(string s)
        {
            foreach (GroupBox gb in PAN_Keys.Controls)
            {
                if (gb.Text == "Recent" && (ListBox)gb.Controls[0] != activeLB && gb.Text != "Presets")
                {
                    if (!((ListBox)gb.Controls[0]).Items.Contains(s))
                    {
                        ((ListBox)gb.Controls[0]).Items.Add(s);
                    }
                }
            }
        }

        private void BTN_Template_Click(object sender, EventArgs e)
        {
            InsertContainer("Envelope", this.RelationalID);
        }

        public void RefreshArrayCount()
        {

            arrayCount = 0;
            foreach (RelationalContainer rc in PAN_Relational.Controls)
            {
                rc.UpdateCount(updateValues);
            }

            BTN_Envelope.Enabled = PAN_Relational.Controls.Count == 0 ? true : false;
            BTN_Array.Enabled = PAN_Relational.Controls.Count == 0 ? false : true;
            BTN_Entity.Enabled = PAN_Relational.Controls.Count == 0 ? false : true;
            RefreshPresets();
            if (BTN_Envelope.Enabled)
            {
                this.selectedChild = null;
                this.selectedContainer = null;
            }
        }

        public void RefreshPresets()
        {
            presetLB.Items.Clear();
            foreach (XmlNode ps in presets)
            {
                if (BTN_Envelope.Enabled)
                {
                    if (ps.SelectSingleNode("Type").InnerText == parent.dm_name && ps.SelectSingleNode("BlockType").InnerText == "Envelope")
                    {
                        presetLB.Items.Add(ps.SelectSingleNode("Name").InnerText);
                    }

                }
                else
                {
                    if (ps.SelectSingleNode("Type").InnerText == parent.dm_name && ps.SelectSingleNode("BlockType").InnerText != "Envelope")
                    {
                        presetLB.Items.Add(ps.SelectSingleNode("Name").InnerText);
                    }
                }
            }
        }

        public void RelationalIDChanged(string val)
        {
            foreach (KeyValue k in relationalTypes)
            {
                if (k.KeyID.StartsWith("FD") && k.AttributeName == val)
                {
                    RelationalID = k.KeyID;
                    envLSB = k.KeyID.Substring(6);
                    break;
                }
            }
        }

        private void ParseImportMessage(string mes)
        {
            //updateValues = false;
            string extractedMessage = mes;
            int keylength = 8;
            string zeroes = "00000000";
            bool preGEBL = false;
            try
            {
                while (extractedMessage.Length > 0)
                {
                    try
                    {
                        //extract the first key ID from the message based on the expected length of the key
                        string currentKey = extractedMessage.Substring(0, keylength);
                        bool keyfound = false;
                        //compare the extracted key to all the keys in the list of loaded values
                        foreach (KeyValue kv in keyValues)
                        {
                            string paddedkey = (zeroes + kv.KeyID).Substring((zeroes + kv.KeyID).Length - keylength);
                            if (paddedkey == currentKey)
                            {
                                keyfound = true;
                                string value = "";
                                string retvalue;
                                int messagelength = 0;
                                //If key is found, use length and type data associated with this key to extract the correct number of
                                //payload bytes and format them appropriately
                                if (kv.Length > 0)
                                {
                                    value = extractedMessage.Substring(keylength, kv.Length);
                                    retvalue = kv.Process(value);
                                    messagelength = kv.Length;
                                }
                                else //if a string value, convert each hex digit to a char and string them together until null character is reached
                                {
                                    messagelength = 0;
                                    string _string = "";
                                    for (int i = keylength; i < extractedMessage.Length; i += 2)
                                    {
                                        if (extractedMessage.Substring(i, 2) == "00")
                                        {
                                            messagelength = i - (keylength - 2);
                                            break;
                                        }
                                        else
                                        {
                                            messagelength += 2;
                                            _string += Convert.ToChar(int.Parse(extractedMessage.Substring(i, 2), NumberStyles.AllowHexSpecifier));
                                        }
                                    }
                                    retvalue = _string;//extractedMessage.Substring(4, messagelength);
                                }
                                //
                                extractedMessage = extractedMessage.Substring(keylength + messagelength);
                                switch (kv.KeyID)
                                {
                                    case "FE010001":
                                        this.RelationalID = retvalue;
                                        InsertContainer("Envelope", retvalue);
                                        break;
                                    case "FE020001":
                                        InsertContainer("Entity", retvalue);
                                        break;
                                    case "FE030001":
                                        InsertContainer("Array", retvalue);
                                        if (retvalue.EndsWith("0001"))
                                        {
                                            preGEBL = true;
                                        }
                                        break;
                                    case "FE010002":
                                        EndOfBlock();
                                        break;
                                    case "FE020002":
                                        EndOfBlock();
                                        break;
                                    case "FE030002":
                                        EndOfBlock();
                                        break;
                                    default:
                                        this.selectedChild.parent.AddKey(kv, retvalue);
                                        break;

                                }
                            }
                        }
                        if (!keyfound)
                        {
                            //LB_WIDE.Items.Add("Key Not Found (0x" + currentKey + ")");
                            extractedMessage = "";
                        }
                    }
                    catch
                    {
                        //savedExtractedMessage = extractedMessage;
                        extractedMessage = "";
                    }

                }
                this.CB_GEBL.Checked = preGEBL;
            }
            catch
            {
                MessageBox.Show("Specified input is not a recognized relational cycle for the selected platform.");
            }
        }

        private void EndOfBlock()
        {
            this.selectedContainer.ClearAllSelected();
            if (this.selectedChild.CloseBracket)
            {
                if (this.selectedContainer.parent != this)
                {
                    ((RelationalContainer)this.selectedContainer.parent).ClearAllSelected();
                    ((RelationalItem2)this.selectedContainer.Parent.Controls[this.selectedContainer.Parent.Controls.Count - 1]).SetSelected(true);
                }
            }
            else
            {
                ((RelationalItem2)this.selectedContainer.Controls[this.selectedContainer.Controls.Count - 1]).SetSelected(true);
            }
        }

        private void BTN_Import_Click(object sender, EventArgs e)
        {
            try
            {
                PAN_Relational.Controls.Clear();
                this.selectedChild = null;
                this.selectedContainer = null;
                string extractedMessage = TB_Import.Text.ToUpper().Replace(".", "").Replace(",", "").Replace("'", "");
                if (extractedMessage.Contains("WIDE"))
                {
                    extractedMessage = LUAtoMQTT(extractedMessage);
                }
                string mes = "";
                try
                {
                    mes = extractedMessage.Substring(extractedMessage.IndexOf("FE010001"));
                }
                catch
                {
                    if (extractedMessage.IndexOf("900580") >= 0)
                    {
                        mes = "FE010001FD020001" + extractedMessage.Substring(extractedMessage.IndexOf("900580") + 10) + "FE010002FD020001";
                    }
                }
                updateValues = false;
                ParseImportMessage(mes);
                updateValues = true;
            }
            catch
            {
                MessageBox.Show("Specified input is not a recognized relational cycle.");
            }
        }

        private string LUAtoMQTT(string mes)
        {
            string mqttmes = "";
            string[] messages = mes.Replace("(", "").Replace(")", "").Split('}');
            for (int i = 0; i < messages.Length; i++)
            {
                try
                {
                    messages[i] = messages[i].Substring(messages[i].IndexOf('{'));
                    if (messages[i].Contains("FE010001"))
                    {
                        messages[i] = messages[i].Substring(messages[i].IndexOf("FE010001"));
                    }
                    else
                    {
                        messages[i] = messages[i].Substring(7);
                    }
                    mqttmes += messages[i];
                }
                catch
                {
                }
            }
            return mqttmes;
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BuildKVPString()
        {
            cyclestring = "";

            foreach (RelationalContainer rc in PAN_Relational.Controls)
            {
                cyclestring += rc.KVPMessage();
            }
            if (numContainers == 1)
            {
                if (MessageBox.Show("No internal array detected.  Would you like to export only the KVPs and not the Relational structure?", "Envelope Only", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    cyclestring = cyclestring.Substring(16, cyclestring.Length - 32);
                }
                else
                {
                }
            }

        }

        private void BTN_MQTT_Click(object sender, EventArgs e)
        {
            BuildKVPString();
            cyclestring = "FF3333" + cyclestring;
            cyclestring = (cyclestring.Length / 2).ToString("X4") + cyclestring;
            cyclestring = Regex.Replace(cyclestring, ".{2}", "$0,");
            cyclestring = cyclestring.Substring(0, cyclestring.Length - 1);
            Clipboard.SetText(cyclestring);
            MessageBox.Show("MQTT payload copied to clipboard");
        }

        private void BTN_LUA_Click(object sender, EventArgs e)
        {
            BuildKVPString();
            string mes = "target_node = " + parent.GetNodeID() + " -- Set this value to the target node ID\r\n";
            cyclestring = Regex.Replace(cyclestring, ".{90}", "$0,");
            string[] parts = cyclestring.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                string trid = i == 0 ? "3333" : "";
                int lastmes = i == parts.Length - 1 ? 128 : 0;
                parts[i] = "9005" + (lastmes + i).ToString("X2") + trid + parts[i];
                parts[i] = parts[i] = Regex.Replace(parts[i], ".{2}", "$0,");
                parts[i] = parts[i].Substring(0, parts[i].Length - 1);
                parts[i] = "wide:SendMessage(target_node,4,{'" + parts[i].Replace(",", "','") + "'})\r\n";
                mes += parts[i];
            }

            Clipboard.SetText(mes);
            MessageBox.Show("Reveal LUA script copied to clipboard");

        }

        private void BTN_Send_Click(object sender, EventArgs e)
        {
            BuildKVPString();
            parent.SendExternal(cyclestring, true, true);
        }

        public void EnablePreset(bool enable)
        {
            BTN_Preset.Enabled = enable;
        }

        private void BTN_Preset_Click(object sender, EventArgs e)
        {
            bool addNode = true;
            SavePreset sp = new SavePreset();
            sp.ShowDialog();
            if (sp.PresetName != "")
            {
                foreach (XmlNode ps in presets)
                {
                    if (ps.SelectSingleNode("Name").InnerText == sp.PresetName && ps.SelectSingleNode("Type").InnerText == parent.dm_name)
                    {
                        if (MessageBox.Show("Preset with this name already exists.  Overwrite?", "Duplicate Name", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ps.ParentNode.RemoveChild(ps);
                            xml.Save("SymbIOT_Presets.xml");
                        }
                        else
                        {
                            addNode = false;
                        }
                    }
                }
                if (addNode)
                {
                    string mes = selectedContainer.KVPMessage();
                    //XmlDocument xml = new XmlDocument();
                    //xml.Load("SymbIOT_Presets.xml");
                    XmlNode node = xml.CreateNode(XmlNodeType.Element, "Preset", null);
                    XmlNode nodeName = xml.CreateElement("Name");
                    nodeName.InnerText = sp.PresetName;
                    XmlNode nodeType = xml.CreateElement("Type");
                    nodeType.InnerText = parent.dm_name;
                    XmlNode nodeBlockType = xml.CreateElement("BlockType");
                    nodeBlockType.InnerText = selectedContainer.type;
                    XmlNode nodeData = xml.CreateElement("Data");
                    nodeData.InnerText = mes;
                    node.AppendChild(nodeName);
                    node.AppendChild(nodeType);
                    node.AppendChild(nodeBlockType);
                    node.AppendChild(nodeData);
                    xml.DocumentElement.AppendChild(node);
                    xml.Save("SymbIOT_Presets.xml");

                    LoadXML();
                }
            }

        }

        private void BTN_Clear_Click(object sender, EventArgs e)
        {
            TB_Import.Text = "";
        }

        public bool GetGEBLCB()
        {
            return CB_GEBL.Checked;
        }

        private void CB_GEBL_CheckedChanged(object sender, EventArgs e)
        {
            RefreshArrayCount();
        }

        private void PAN_Relational_Scroll(object sender, ScrollEventArgs e)
        {
            scrollpos = PAN_Relational.VerticalScroll.Value;
        }

        public int GetVerticalScrollPos()
        {
            return scrollpos;
        }

        public void SetVerticalScrollPos(int pos)
        {
            PAN_Relational.VerticalScroll.Value = pos;
        }

        private void BTN_DelPreset_Click(object sender, EventArgs e)
        {
            if (presetLB.SelectedIndex >= 0)
            {
                foreach (XmlNode ps in presets)
                {
                    if (ps.SelectSingleNode("Name").InnerText == presetLB.SelectedItem.ToString() && ps.SelectSingleNode("Type").InnerText == parent.dm_name)
                    {
                        if (MessageBox.Show("Really delete Preset '" + presetLB.SelectedItem + "'?", "Delete Preset", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ps.ParentNode.RemoveChild(ps);
                            xml.Save("SymbIOT_Presets.xml");
                            LoadXML();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No preset selected to delete.");
            }
        }

        private void CB_FullDM_CheckedChanged(object sender, EventArgs e)
        {
            while (PAN_Keys.Controls.Count > 2)
            {
                PAN_Keys.Controls.RemoveAt(2);
            }

            LoadLBs(CB_FullDM.Checked);
        }

        private void BTN_JSON_Click(object sender, EventArgs e)
        {
            cyclestring = "";

            foreach (RelationalContainer rc in PAN_Relational.Controls)
            {
                cyclestring += rc.JSONMessage();
            }

            if (cyclestring != "")
            {
                Clipboard.SetText(cyclestring);
                MessageBox.Show("JSON string copied to clipboard");
            }
            else
            {
                MessageBox.Show("No data to export");
            }

        }

        private void BTN_Swap_Click(object sender, EventArgs e)
        {
            foreach (RelationalContainer rc in PAN_Relational.Controls)
            {
                rc.SwapCavities();
            }
        }

        private void RelationalBuilder2_Resize(object sender, EventArgs e)
        {
            PAN_Buttons.Top = Math.Max(this.Height - 68, 1);
            PAN_Relational.Height = Math.Max(this.Height - 141, 1);
            PAN_Keys.Height = Math.Max(this.Height - 78, 1);
        }

        private void RB_Sort_CheckedChanged(object sender, EventArgs e)
        {
            CB_FullDM_CheckedChanged(this, null);
            if (RB_Name.Checked)
            {
                foreach (Control c in PAN_Keys.Controls)
                {
                    if (((ListBox)c.Controls[0]) != presetLB)
                    {
                        ((ListBox)c.Controls[0]).Sorted = true;
                    }
                }
            }
        }
    }
}
