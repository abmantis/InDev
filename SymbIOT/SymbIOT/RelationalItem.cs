using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbIOT
{
    public partial class RelationalItem : UserControl
    {
        public List<Cycle> cycles;
        public string tempKey;
        public string timeKey;
        public string probeAmtKey;
        public string doneKey;
        public string powKey;
        public string[] zonemsg;
        public string[] zones;
        public string[] dones;
        public KeyValue cpltKey;
        private RelationalBuilder parent;

        private int index;
        public int Index
        {
            get { return index; }
            set
            { 
                index = value;
                LBL_Step.Text = "Step " + (index+1).ToString();
            }
        }
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
        }

        private string innerStep;
        public string InnerStep
        {
            get { return innerStep; }
        }

        public RelationalItem(RelationalBuilder par, List<Cycle> cy, string temp, string time, string probe, string done, string pow, KeyValue cplt, int idx)
        {
            InitializeComponent();
            parent = par;
            cycles = cy;
            tempKey = temp;
            timeKey = time;
            probeAmtKey = probe;
            cpltKey = cplt;
            doneKey = done;
            powKey = pow;
            index = idx;
            zonemsg = new string[10];
            zones = new string[10];
            TB_Message.Enabled = false;
            isValid = false;

            zones[0] = "None";
            for (int j = 1; j < 10; j++)
            {
                zones[j] = j.ToString();//Zones
            }

            if (doneKey != "")
            {
                LBL_SetTemp.Text = "Power:";
                LBL_SetTempF.Text = "%";
                LBL_SetProbe.Text = "Amount:";
                LBL_SetProbeF.Text = "";
                LBL_Zone.Text = "Done:";
                LBL_Zone.Visible = true;
                //TB_Zone.Visible = true;
                CBO_Amt.Visible = true;
                CBO_ZoneDoneness.Visible = true;

                string d = "";
                foreach (KeyValue k in parent.parent.keyValues)                
                {
                    if (k.KeyID == doneKey)
                    {
                        
                        for (int j = 0; j <= k.EnumName.Enums.Count; j++)
                        {
                            try
                            {
                                if (k.EnumName.Enums[j] != null)
                                {
                                    d += k.EnumName.Enums[j] + ";";//Doneness
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                d = d.Substring(0, d.Length - 1);
                dones = d.Split(';');
                CBO_ZoneDoneness.Items.AddRange(dones);
                
            }
            else
            {
                CBO_ZoneDoneness.Visible = false;                              
                CBO_Amt.Visible = false;
            }

            LBL_Step.Text = "Step " + (index+1).ToString();
            innerStep = "";
            foreach (Cycle c in cycles)
            {
                CBO_Cycle.Items.Add(c.Name);
            }
            for (int i = 0; i < cplt.EnumName.Enums.Count; i++)
            {
                if (cplt.EnumName.Enums[i] != null)
                {
                    CBO_End.Items.Add(cplt.EnumName.Enums[i].ToString());
                }
            }
        }

        public void SetAttributes(int cidx, string []mes, string temp, string time, string probe, int endidx,string zone)
        {
            this.CBO_Cycle.SelectedIndex = cidx;
            //this.TB_Message.Text = mes;
            this.zonemsg = mes;
            this.TB_Temp.Text = temp;
            this.TB_Time.Text = time;            
            this.CBO_End.SelectedIndex = endidx;
            CBO_ZoneDoneness.Visible = false;
            if (doneKey == "")
            {
                this.TB_Probe.Text = probe;                
            }
            else
            {
                try
                {
                    CBO_Amt.SelectedItem = probe;
                    if (mes.All(x => x == null))
                    {
                        CBO_ZoneDoneness.SelectedIndex = int.Parse(zone);
                    }
                }
                catch { }
                CBO_ZoneDoneness.Visible = true;
            }

            if (!mes.All(x => x == null))
            {
                //this.TB_Zone.Text = zone;
                try
                {
                    for (int i = 0; i < mes.Length; i++)
                    {
                        if (mes[i] != null)
                        {
                            CBO_ZoneDoneness.SelectedIndex = i;
                            TB_Message.Enabled = true;
                        }
                    }
                }
                catch
                {
                    CBO_ZoneDoneness.SelectedIndex = -1;
                }
                CBO_ZoneDoneness.Visible = true;
                this.RB_Text.Checked = true;
                this.RB_Cycle.Checked = false;
            }

            this.UpdateInnerStep(this, null);
        }

        private void RB_Cycle_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_Cycle.Checked)
            {
                CBO_Cycle.Visible = true;
                TB_Message.Visible = false;
                LBL_Cycle.Text = "Cycle:";
                TB_Temp.Enabled = true;
                TB_Probe.Enabled = true;
                TB_Time.Enabled = true;
                CBO_Amt.Enabled = true;
                if (doneKey == "")
                {
                    LBL_Zone.Visible = false;
                    TB_Zone.Visible = false;
                    CBO_ZoneDoneness.Visible = false;
                }
                else
                {
                    CBO_ZoneDoneness.Items.Clear();
                    CBO_ZoneDoneness.Items.AddRange(dones);
                    LBL_Zone.Visible = true;
                    //TB_Zone.Visible = true;
                    LBL_Zone.Text = "Done:";
                    CBO_ZoneDoneness.Visible = true;
                }
            }
            else
            {
                CBO_Cycle.Visible = false;
                TB_Message.Visible = true;
                LBL_Cycle.Text = "Msg:";
                TB_Temp.Enabled = false;
                TB_Probe.Enabled = false;
                TB_Time.Enabled = false;
                CBO_Amt.Enabled = false;
                LBL_Zone.Visible = true;
                //TB_Zone.Visible = true;
                LBL_Zone.Text = "Zone:";
                CBO_ZoneDoneness.Items.Clear();
                CBO_ZoneDoneness.Items.AddRange(zones);
                CBO_ZoneDoneness.Visible = true;
                //CBO_ZoneDoneness.SelectedIndex = 0;
                for (int i = 0; i < zonemsg.Length; i++)
                {
                    if (zonemsg[i] != null)
                    {
                        CBO_ZoneDoneness.SelectedIndex = i;
                    }
                }
            }
        }

        public void UpdateInnerStep(object sender, EventArgs e)
        {            
            try
            {
                if (parent.parent.selectedTab != 2 || CBO_Cycle.SelectedItem.ToString().StartsWith("Convect"))
                {
                    LBL_SetTemp.Text = "Temp:";
                    LBL_SetTempF.Text = "F";                    
                }
                else
                {
                    LBL_SetTemp.Text = "Power:";
                    LBL_SetTempF.Text = "%";                    
                }                
            }
            catch { }
            try
            {
                if (RB_Text.Checked)
                {
                    zonemsg[CBO_ZoneDoneness.SelectedIndex] = TB_Message.Text;
                }
            }
            catch { }
            innerStep = "FE020001020101" + (index + 1).ToString("X2");
            try
            {
                if (RB_Cycle.Checked)
                {
                    innerStep = CBO_Cycle.SelectedIndex >= 0 ? innerStep + cycles[CBO_Cycle.SelectedIndex].KeyID + cycles[CBO_Cycle.SelectedIndex].Enum.ToString("X2") : innerStep;
                    innerStep = TB_Temp.Text != "" && LBL_SetTemp.Text == "Temp:" ? innerStep + tempKey + FtoCx10(TB_Temp.Text) : innerStep;
                    innerStep = TB_Temp.Text != "" && LBL_SetTemp.Text == "Power:" ? innerStep + powKey + (int.Parse(TB_Temp.Text)).ToString("X2") : innerStep;
                    innerStep = TB_Probe.Text != "" && LBL_SetProbe.Text == "Probe:" ? innerStep + probeAmtKey + FtoCx10(TB_Probe.Text) : innerStep;
                    innerStep = LBL_SetProbe.Text == "Amount:" && CBO_Amt.SelectedItem != null && CBO_Amt.Enabled ? innerStep + probeAmtKey + (int.Parse(CBO_Amt.SelectedItem.ToString())).ToString("X4") : innerStep;
                    innerStep = LBL_Zone.Text == "Done:" && CBO_ZoneDoneness.SelectedIndex >= 0 ? innerStep + doneKey + CBO_ZoneDoneness.SelectedIndex.ToString("X2") : innerStep;
                    innerStep = TB_Time.Text != "" ? innerStep + timeKey + ((int)(double.Parse(TB_Time.Text) * 60)).ToString("X8") : innerStep;
                }
                else
                {
                    for (int i = 0; i < zonemsg.Length; i++)
                    {
                        if (zonemsg[i] != null && zonemsg[i] != "")
                        {
                            if (i == 0)
                            {
                                innerStep += (parent.SysChecked() ? "02030009" : parent.parent.v1msgKey) + StringtoHex(zonemsg[i]);
                            }
                            else
                            {
                                innerStep += (parent.SysChecked() ? "0203000B" : parent.parent.v2msgKey) + StringtoHex(i.ToString() + "," + zonemsg[i]);
                            }
                        }
                    }
                }
                isValid = true;
            }
            catch { isValid = false; }
            innerStep = CBO_End.SelectedIndex >= 0 ? innerStep + cpltKey.KeyID + (CBO_End.SelectedIndex).ToString("X2") : innerStep;
            innerStep += "FE020002020101" + (index + 1).ToString("X2");

            this.parent.BuildCycle();
        }

        public int Cx10toF(int cx10)
        {
            double a = cx10 / 10;
            double b = a * 9 / 5 + 32;
            return (int)b;
        }

        public string FtoCx10(string f)
        {
            double fs = double.Parse(f);
            double a = fs - 32;
            double b = (a * 5 / 9) * 10;
            return ((int)b).ToString("X4");
        }



        public string StringtoHex(string s)
        {
            string mes = "";
            char[] values = s.ToCharArray();
            foreach (char c in values)
            {
                mes += Convert.ToInt32(c).ToString("X2");
            }
            mes += "00";
            return mes;
        }

        public void SetUpDownButtons(bool buttonup, bool buttondown)
        {
            BTN_Up.Enabled = buttonup;
            BTN_Down.Enabled = buttondown;
        }

        private void BTN_Up_Click(object sender, EventArgs e)
        {
            this.parent.relationalItems[index - 1].Index = index;
            this.index = index - 1;
            this.parent.ReSort();
        }

        private void BTN_Down_Click(object sender, EventArgs e)
        {
            this.parent.relationalItems[index + 1].Index = index;
            this.index = index + 1;
            this.parent.ReSort();
        }

        private void BTN_Delete_Click(object sender, EventArgs e)
        {
            this.parent.relationalItems.Remove(this);
            this.parent.ReSort();
        }

        private void CBO_Cycle_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cycle c = cycles[CBO_Cycle.SelectedIndex];
            if (c.DefAmt != 0)
            {
                CBO_Amt.Items.Clear();
                if (c.ValAmt == "")
                {
                    for (int i = c.MinAmt; i <= c.MaxAmt; i += c.StepAmt)
                    {
                        CBO_Amt.Items.Add(i.ToString());
                    }
                }
                else
                {
                    string[] vals = c.ValAmt.Split(',');
                    for (int i = 0; i < vals.Length; i++)
                    {
                        CBO_Amt.Items.Add(vals[i]);
                    }
                }
                CBO_Amt.SelectedItem = c.DefAmt.ToString();
                CBO_Amt.Enabled = true;
            }
            else
            {
                CBO_Amt.Items.Clear();
                CBO_Amt.Enabled = false;
            }
            UpdateInnerStep(this, null);
        }

        private void CBO_Doneness_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RB_Text.Checked)
            {
                TB_Message.Text = zonemsg[CBO_ZoneDoneness.SelectedIndex];
                TB_Message.Enabled = true;
            }
        }
    }
}
