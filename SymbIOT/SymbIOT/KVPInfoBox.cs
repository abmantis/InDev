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
    public partial class KVPInfoBox : Form
    {
        public KVPInfoBox(KeyValue kv)
        {
            InitializeComponent();

            this.LBL_KVPName.Text = kv.DisplayName;
            this.RTB_Description.Text = kv.Description;
            this.TB_Key.Text = kv.KeyID;
            this.TB_Size.Text = kv.LengthString;
            this.TB_Format.Text = kv.Format == "equation" ? "temperature" : kv.Format;

            if (kv.EnumName != null)
            {
                LBL_Enums.Visible = true;
                CBO_Enums.Visible = true;
                for (int j = 0; j < kv.EnumName.Enums.Count; j++)
                {
                    if (kv.EnumName.Enums[j] != null && kv.EnumName.IsUsed[j])
                    {
                        CBO_Enums.Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j].ToString());
                    }
                }
            }
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
