using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SymbIOT
{
    public partial class RelationalItem2 : UserControl
    {
        KeyValue keyvalue;
        KeyValue upperKV;
        KeyValue lowerKV;
        public RelationalContainer parent;
        RelationalBuilder2 topParent;
        Scripter scriptParent;
        Panel panParent;
        string indent = "";

        public bool Selected;
        public bool CloseBracket;
        public bool OpenBracket;
        public bool IsSend;
        bool isScript;

        public string KeyName
        {
            get { return LBL_Name.Text; }
        }

        public string KeyValue
        {
            get { return keyvalue.KeyID; }
        }

        public RelationalItem2(KeyValue kv, RelationalContainer parent, RelationalBuilder2 topparent, string val)
        {
            InitializeComponent();
            isScript = false;
            this.parent = parent;
            this.topParent = topparent;
            keyvalue = kv;
            upperKV = kv;
            lowerKV = kv;
            CloseBracket = false;
            OpenBracket = false;
            CBO_Type.Visible = false;
            indent = new String(' ', parent.Level*2);
            SetSelected(false);
            if (kv.Instance.StartsWith("Relational") && kv.AttributeName == "Open" || kv.AttributeName == "Close")
            {
                try
                {
                    CBO_Type.SelectedIndex = val != "" && !val.StartsWith("FD") ? int.Parse(val.Substring(0, 2), NumberStyles.AllowHexSpecifier) - 1 : 0;
                }
                catch
                {
                    CBO_Type.SelectedIndex = 0;
                }
                switch (kv.AttributeName)
                {
                    case "Open":
                        LBL_Name.Text = indent + (kv.Instance.Contains("Array") ? "[" : "{");
                        CBO_Type.Visible = true;
                        OpenBracket = true;
                        break;
                    case "Close":
                        LBL_Name.Text = indent + (kv.Instance.Contains("Array") ? "]" : "}");
                        CloseBracket = true;
                        break;
                    default:
                        break;
                }

                parent.type = kv.Instance.Split('_')[1];
                parent.datatype = val != "" && !val.StartsWith("FD") ? val.Substring(0, 2) : "01";

                if (parent.type == "Envelope" && CBO_Type.Visible)
                {
                    CBO_Type.Items.Clear();
                    for (int i = 0; i < topparent.relationalTypes.Count; i++)
                    {
                        CBO_Type.Items.Add(topparent.relationalTypes[i].AttributeName + " (0x" + topparent.relationalTypes[i].KeyID + ")");
                        if (topparent.relationalTypes[i].KeyID == "FD020001")
                        {
                            CBO_Type.SelectedIndex = i;
                        }
                    }
                }
                else
                {
                    /*if (SymbIOT.dm_name == "Cooking")
                    {
                        //CBO_Type.Visible = false;
                        //parent.datatype = "02";
                    }*/
                }
                
                TB_Value.Visible = true;
                CBO_Value.Visible = false;
                TB_Value.Text = val;
            }
            else
            {
                LBL_Name.Text = indent + kv.Instance + "." + kv.AttributeName;
                if (kv.EnumName != null)
                {
                    TB_Value.Visible = false;
                    CBO_Value.Visible = true;
                    for (int j = 0; j < kv.EnumName.Enums.Count; j++)
                    {
                        if (kv.EnumName.Enums[j] != null)
                        {
                            CBO_Value.Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j]);
                            if (val == kv.EnumName.Enums[j].ToString())
                            {
                                CBO_Value.SelectedItem = j.ToString("X2") + "=" + kv.EnumName.Enums[j];
                            }
                        }
                    }
                    /*foreach (string s in kv.EnumName.Enums)
                    {
                        if (s != null)
                        {
                            CBO_Value.Items.Add(s);
                        }
                    }
                    if (val != null)
                    {
                        CBO_Value.SelectedItem = val;
                    }*/
                }
                else
                {
                    TB_Value.Visible = true;
                    CBO_Value.Visible = false;
                    if (val != null)
                    {
                        TB_Value.Text = val;
                    }
                }
            }
        }

        public RelationalItem2(KeyValue kv, Scripter parent, Panel pan, string val,bool send)
        {
            InitializeComponent();
            isScript = true;
            //this.parent = parent;
            //this.topParent = topparent;
            this.scriptParent = parent;
            this.panParent = pan;
            keyvalue = kv;
            upperKV = kv;
            lowerKV = kv;
            CloseBracket = false;
            OpenBracket = false;
            CBO_Type.Visible = false;
            TB_Value.Left = TB_Value.Left - CBO_Type.Width;
            CBO_Value.Left = CBO_Value.Left - CBO_Type.Width;
            this.Width = this.Width - CBO_Type.Width;
            //indent = new String(' ', parent.Level * 2);
            SetSelected(false);
            IsSend = send;
            
            {
                LBL_Name.Text = indent + kv.Instance + "." + kv.AttributeName;
                if (kv.EnumName != null)
                {
                    TB_Value.Visible = false;
                    CBO_Value.Visible = true;
                    for (int j = 0; j < kv.EnumName.Enums.Count; j++)
                    {
                        if (kv.EnumName.Enums[j] != null)
                        {
                            CBO_Value.Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j]);
                            if (val == kv.EnumName.Enums[j].ToString() || val == j.ToString("X2"))
                            {
                                CBO_Value.SelectedItem = j.ToString("X2") + "=" + kv.EnumName.Enums[j];
                            }
                        }
                    }
                    /*foreach (string s in kv.EnumName.Enums)
                    {
                        if (s != null)
                        {
                            CBO_Value.Items.Add(s);
                        }
                    }
                    if (val != null)
                    {
                        CBO_Value.SelectedItem = val;
                    }*/
                }
                else
                {
                    TB_Value.Visible = true;
                    CBO_Value.Visible = false;
                    if (val != null)
                    {
                        TB_Value.Text = val;
                    }
                }
            }
        }

        public void SetSelected(bool sel)
        {
            Selected = sel;
            if (Selected)
            {
                BTN_Remove.Visible = true;
                BTN_Up.Visible = true;
                BTN_Down.Visible = true;
                if (isScript)
                {
                    this.scriptParent.selectedChild = this;
                }
                else
                {
                    this.parent.selectedChild = this;
                    this.topParent.selectedChild = this;
                    this.topParent.selectedContainer = this.parent;
                }

                if (isScript)
                {
                    try
                    {
                        this.BorderStyle = BorderStyle.FixedSingle;
                        BTN_Up.Enabled = this.panParent.Controls.GetChildIndex(this) > 0 ? true : false;
                        BTN_Down.Enabled = this.panParent.Controls.GetChildIndex(this) < this.panParent.Controls.Count - 1 ? true : false;

                    }
                    catch
                    {
                        BTN_Up.Enabled = false;
                        BTN_Down.Enabled = false;
                    }
                }
                else
                {
                    try
                    {
                        if (CloseBracket || OpenBracket)
                        {
                            this.parent.BorderStyle = BorderStyle.FixedSingle;
                            BTN_Up.Enabled = this.parent.Parent.Controls.GetChildIndex(this.parent) > 1 ? true : false;
                            BTN_Down.Enabled = this.parent.Parent.Controls.GetChildIndex(this.parent) < this.parent.Parent.Controls.Count - 2 ? true : false;
                            this.parent.selectedChild = CloseBracket ? null : this;
                            topParent.EnablePreset(true);
                        }
                        else
                        {
                            this.BorderStyle = BorderStyle.FixedSingle;
                            BTN_Up.Enabled = this.parent.Controls.GetChildIndex(this) > 1 ? true : false;
                            BTN_Down.Enabled = this.parent.Controls.GetChildIndex(this) < this.parent.Controls.Count - 2 ? true : false;
                            topParent.EnablePreset(false);
                        }
                    }
                    catch
                    {
                        BTN_Up.Enabled = false;
                        BTN_Down.Enabled = false;
                        topParent.EnablePreset(false);
                    }
                }
            }
            else
            {
                BTN_Down.Visible = false;
                BTN_Up.Visible = false;
                BTN_Remove.Visible = false;
                this.BorderStyle = BorderStyle.None;
                if (!isScript)
                {
                    this.parent.BorderStyle = BorderStyle.None;
                    topParent.EnablePreset(false);
                }
            }
        }

        private void RelationalItem2_Click(object sender, EventArgs e)
        {
            if (isScript)
            {               
                foreach (Control c in panParent.Controls)
                {
                    ((RelationalItem2)c).SetSelected(false);
                }
                this.SetSelected(true);
            }
            else
            {
                int pos = this.topParent.GetVerticalScrollPos();
                this.topParent.ClearAllSelected();
                this.SetSelected(true);
                this.topParent.SetVerticalScrollPos(pos);
            }

        }

        private void BTN_Remove_Click(object sender, EventArgs e)
        {            
            this.SetSelected(false);
            if (isScript)
            {
                this.scriptParent.selectedChild = null;
                this.panParent.Controls.Remove(this);
                int height = 0;
                foreach (Control c in panParent.Controls)
                {
                    c.Top = height;
                    height += c.Height;
                }
            }
            else
            {
                this.parent.selectedChild = null;
                this.topParent.selectedChild = null;
                this.topParent.selectedContainer = null;
                this.parent.RemoveChild(this);
            }
        }

        public void SetValue(string s)
        {
            if (CBO_Value.Visible)
            {
                CBO_Value.SelectedItem = this.keyvalue.Process(s);
            }
            else
            {
                TB_Value.Text = this.keyvalue.Process(s);
            }
        }

        private void BTN_Up_Click(object sender, EventArgs e)
        {
            if (isScript)
            {
                panParent.Controls.SetChildIndex(this, panParent.Controls.GetChildIndex(this) - 1);
                int height = 0;
                foreach (Control c in panParent.Controls)
                {
                    c.Top = height;
                    height += c.Height;
                }
            }
            else
            {
                this.parent.MoveChild(this, 1);
            }
            this.SetSelected(true);
        }

        private void BTN_Down_Click(object sender, EventArgs e)
        {
            if (isScript)
            {
                panParent.Controls.SetChildIndex(this, panParent.Controls.GetChildIndex(this) + 1);
                int height = 0;
                foreach (Control c in panParent.Controls)
                {
                    c.Top = height;
                    height += c.Height;
                }
            }
            else
            {
                this.parent.MoveChild(this, -1);
            }
            this.SetSelected(true);
        }

        public string KVPValue(bool numericenum)
        {           
            string mes = "";
            try
            {
                string outvalue = CBO_Value.Visible ? CBO_Value.SelectedItem.ToString().Substring(3) : TB_Value.Text;
                outvalue = CBO_Value.Visible && numericenum ? CBO_Value.SelectedItem.ToString().Substring(0, 2) : outvalue;
                mes = outvalue;
            }
            catch
            {
                mes = "";
            }            

            return mes;
        }

        public string KVPMessage()
        {
            string mes = "";
            try
            {
                string outvalue = CBO_Value.Visible ? CBO_Value.SelectedItem.ToString().Substring(3) : TB_Value.Text;
                mes = keyvalue.OutValue(outvalue);
            }
            catch
            {
                mes = "";
            }            

            return mes;
        }

        public string JSONMessage()
        {
            string mes = "";
            string val = "";
            try
            {
                val = CBO_Value.Visible ? CBO_Value.SelectedItem.ToString() : TB_Value.Text;
            }
            catch
            {
                val = "";
            }
            if (OpenBracket || CloseBracket)
            {
                mes = new String(' ', parent.Level*2) + LBL_Name.Text + "\r\n";
            }
            else
            {
                mes = new String(' ', parent.Level*2 + 4) + LBL_Name.Text + " = " + "\"" + val + "\"" + "\r\n";
            }

            return mes;
        }

        private void CBO_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OpenBracket)
            {
                if (this.parent.type == "Envelope")
                {
                    this.TB_Value.Text = CBO_Type.SelectedItem.ToString().Split('(')[1].Substring(2, 8);
                    this.topParent.RelationalIDChanged(CBO_Type.SelectedItem.ToString().Split(' ')[0]);
                }
                else
                {
                    this.parent.datatype = (CBO_Type.SelectedIndex + 1).ToString("X2");
                }
                this.topParent.RefreshArrayCount();
            }
        }

        public void SetCBOVisible(bool visible)
        {
            this.CBO_Type.Visible = visible;
        }

        public void SetAltCavityKV(KeyValue kv, bool lower)
        {
            if (lower)
            {
                lowerKV = kv;
            }
            else
            {
                upperKV = kv;
            }
        }

        public void SwapCavities()
        {
            if (!(OpenBracket || CloseBracket))
            {
                if (keyvalue == upperKV)
                {
                    keyvalue = lowerKV;
                }
                else
                {
                    keyvalue = upperKV;
                }

                LBL_Name.Text = indent + keyvalue.Instance + "." + keyvalue.AttributeName;
            }
        }
    }
}
