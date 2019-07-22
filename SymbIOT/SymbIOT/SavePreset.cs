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
    public partial class SavePreset : Form
    {
        public string PresetName;

        public SavePreset()
        {
            InitializeComponent();
            PresetName = "";
        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BTN_Save_Click(object sender, EventArgs e)
        {
            if (TB_Name.Text != "")
            {
                PresetName = TB_Name.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a name for the Preset");
            }
        }
    }
}
