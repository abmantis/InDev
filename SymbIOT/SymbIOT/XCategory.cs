using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbIOT
{
    public partial class XCategory : Form
    {
        public SymbIOT parent;
        //private List<Panel> pans;
        public XCategory(List<KeyValue> kv, SymbIOT parent)
        {
            InitializeComponent();
            this.parent = parent;
            int curtab = 0;
            foreach (KeyValue k in kv)
            {
                if (k.IsUsed)
                {
                    bool found = false;
                    string instance = k.Instance.Split('_')[0];
                    instance = instance.StartsWith("Config") ? "Config" : instance;
                    for (int i = 0; i < TAB_Keys.TabPages.Count; i++)
                    {
                        if (TAB_Keys.TabPages[i].Text == instance)
                        {
                            found = true;
                            curtab = i;
                        }
                    }
                    if (!found)
                    {
                        TAB_Keys.TabPages.Add(instance);
                        curtab = TAB_Keys.TabPages.Count - 1;
                        TAB_Keys.TabPages[curtab].AutoScroll = true;
                    }

                    
                        XCatItem xc = new XCatItem(k, this);
                        xc.Top = TAB_Keys.TabPages[curtab].Controls.Count * xc.Height;
                        xc.Left = 0;
                        TAB_Keys.TabPages[curtab].Controls.Add(xc);
                   
                }
            }
        }

        public void RefreshAll()
        {
            foreach (TabPage tp in TAB_Keys.TabPages)
            {
                foreach (XCatItem x in tp.Controls)
                {
                    x.RefreshValue();
                }
            }
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void BTN_Refresh_Click(object sender, EventArgs e)
        {
            foreach (XCatItem x in TAB_Keys.TabPages[TAB_Keys.SelectedIndex].Controls)
            {
                x.BTN_Get_Click(this, null);
            }
        }

        private void BTN_SetAll_Click(object sender, EventArgs e)
        {
            string outvalue = "";
            foreach (XCatItem x in TAB_Keys.TabPages[TAB_Keys.SelectedIndex].Controls)
            {
                if (x.Checked)
                {
                    outvalue += x.GetOutValue();
                }
            }
            parent.SendExternal(outvalue, false, true);
        }

        private void BTN_ClearAll_Click(object sender, EventArgs e)
        {
            foreach (XCatItem x in TAB_Keys.TabPages[TAB_Keys.SelectedIndex].Controls)
            {
                x.Checked = false;
            }
        }

        public void Clear()
        {
            TAB_Keys.TabPages.Clear();
        }

        private void BTN_ClearRcv_Click(object sender, EventArgs e)
        {
            foreach (XCatItem x in TAB_Keys.TabPages[TAB_Keys.SelectedIndex].Controls)
            {
                x.ClearRcv();
            }

        }

        private void XCategory_Resize(object sender, EventArgs e)
        {
            PAN_Buttons.Top = Math.Max(this.Height - 65, 1);
            TAB_Keys.Height = Math.Max(this.Height - 66, 1);
        }
    }
}
