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
    public partial class XCatItem : UserControl
    {

        KeyValue keyvalue;
        XCategory parent;
        
        public bool Checked
        {
            get { return CB_XCat.Checked; }
            set { CB_XCat.Checked = value; }
        }

        public XCatItem(KeyValue kv, XCategory parent)
        {
            InitializeComponent();
            this.parent = parent;
            keyvalue = kv;
            LBL_Name.Text = kv.AttributeName;            

            if (kv.EnumName != null)
            {
                TB_Value.Visible = true;
                CBO_Value.Visible = false;
                TB_SetValue.Visible = false;
                CBO_SetValue.Visible = true;
                for (int j = 0; j < kv.EnumName.Enums.Count; j++)
                {
                    if (kv.EnumName.Enums[j] != null && kv.EnumName.IsUsed[j])
                    {
                        CBO_Value.Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j].ToString());
                        CBO_SetValue.Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j].ToString());
                    }
                }
                if (kv.RcvValue != null && kv.RcvValue != "")
                {
                    CBO_Value.SelectedItem = kv.RawValue + "=" + kv.RcvValue;
                    TB_Value.Text = kv.RawValue + "=" + kv.RcvValue;
                }
                if (kv.SetValue != null && kv.SetValue != "")
                {
                    CBO_SetValue.SelectedItem = kv.SetRawValue + "=" + kv.SetValue;
                }
            }
            else
            {
                TB_Value.Visible = true;
                CBO_Value.Visible = false;
                TB_SetValue.Visible = true;
                CBO_SetValue.Visible = false;
                if (kv.RcvValue != null)
                {
                    TB_Value.Text = kv.RcvValue;
                }
                if (kv.SetValue != null)
                {
                    TB_SetValue.Text = kv.SetValue;
                }
            }

            if (!kv.isSet)
            {
                this.BTN_Set.Visible = false;
                this.TB_SetValue.Visible = false;
                this.CBO_SetValue.Visible = false;
            }

            string ttstring = "Key ID = 0x" + keyvalue.KeyID + "\r\nDescription: " + keyvalue.Description;
            TT_Label.SetToolTip(LBL_Name, ttstring);
            
        }

        public void RefreshValue()
        {
            if (keyvalue.RcvValue != null && keyvalue.RcvValue != "")
            {
                string tt = "Raw Value:\nHex: 0x" + keyvalue.RawValue;
                tt = keyvalue.Length > 0 ? tt + "\nInt: " + long.Parse(keyvalue.RawValue, NumberStyles.AllowHexSpecifier).ToString() : tt;
                if (CBO_Value.Visible)
                {
                    CBO_Value.SelectedItem = keyvalue.RawValue + "=" + keyvalue.RcvValue;
                    TT_Label.SetToolTip(CBO_Value, tt);
                }
                else
                {
                    TB_Value.Text = keyvalue.EnumName != null ? keyvalue.RawValue + "=" + keyvalue.RcvValue : keyvalue.RcvValue;
                    TT_Label.SetToolTip(TB_Value, tt);
                }
            }
            if (keyvalue.SetValue != null && keyvalue.SetValue != "")
            {
                string tt = "Raw Value:\nHex: 0x" + keyvalue.SetRawValue;
                tt = keyvalue.Length > 0 ? tt +"\nInt: " + long.Parse(keyvalue.SetRawValue, NumberStyles.AllowHexSpecifier).ToString() : tt;
                if (CBO_SetValue.Visible)
                {
                    CBO_SetValue.SelectedItem = keyvalue.SetRawValue + "=" + keyvalue.SetValue;
                    TT_Label.SetToolTip(CBO_SetValue, tt);
                }
                else
                {
                    TB_SetValue.Text = keyvalue.SetValue;
                    TT_Label.SetToolTip(TB_SetValue, tt);
                }
            }
        }

        public void BTN_Get_Click(object sender, EventArgs e)
        {
            parent.parent.SendExternal(keyvalue.KeyID, false, false);
            parent.RefreshAll();
        }

        private void BTN_Set_Click(object sender, EventArgs e)
        {
            try
            {
                string outvalue = CBO_SetValue.Visible && CBO_SetValue.SelectedItem != null ? CBO_SetValue.SelectedItem.ToString().Substring(3) : TB_SetValue.Text;
                if (outvalue != "")
                {
                    parent.parent.SendExternal(keyvalue.OutValue(outvalue), false, true);
                    parent.RefreshAll();
                }
                else
                {
                    MessageBox.Show("Invalid value.");
                }
            }
            catch
            {
                MessageBox.Show("Invalid value.");
            }
        }

        public string GetOutValue()
        {
            if (keyvalue.isSet)
            {
                try
                {
                    string outvalue = CBO_SetValue.Visible && CBO_SetValue.SelectedItem != null ? CBO_SetValue.SelectedItem.ToString().Substring(3) : TB_SetValue.Text;
                    if (outvalue != "")
                    {
                        return keyvalue.OutValue(outvalue);
                    }
                    else
                    {
                        MessageBox.Show("Invalid value on key " + keyvalue.AttributeName);
                        return "";
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid value on key " + keyvalue.AttributeName);
                    return "";
                }
            }
            return "";
        }

        public void ClearRcv()
        {
            TB_Value.Text = "";
            CBO_Value.SelectedItem = "";
            keyvalue.RcvValue = "";
            keyvalue.RawValue = "";
        }

        private void LLBL_Info_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            KVPInfoBox kvpi = new KVPInfoBox(keyvalue);
            kvpi.ShowDialog();
        }
    }
}
