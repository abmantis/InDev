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
    public partial class TabSelect : Form
    {

        private string selected_tab;
        public string SelectedTab
        {
            get { return selected_tab; }
        }

        private int selected_index;
        public int SelectedIndex
        {
            get { return selected_index; }
        }

        public TabSelect(string header, string[] sheet_names)
        {
            InitializeComponent();
            LBL_Header.Text = header;

            foreach (string s in sheet_names)
            {
                if (s != "")
                {
                    LB_Tabs.Items.Add(s);
                }
                if (((s.Contains("Enum") && header.Contains("Enum")) || (s.Contains("Def") && header.Contains("KVP")) || (s == "All" && header.Contains("platform"))) && !s.Contains("Print_Area"))
                {
                    LB_Tabs.SelectedIndex = LB_Tabs.Items.Count - 1;
                }
            }
        }

        private void BTN_Select_Click(object sender, EventArgs e)
        {
            if (LB_Tabs.SelectedItem != null)
            {
                selected_tab = LB_Tabs.SelectedItem.ToString();
                selected_index = LB_Tabs.SelectedIndex;
                this.Close();
            }
            else
            {
                MessageBox.Show(LBL_Header.Text);
            }
        }
    }
}
