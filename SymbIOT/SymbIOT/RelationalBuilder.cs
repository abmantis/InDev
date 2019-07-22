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

namespace SymbIOT
{
    public partial class RelationalBuilder : Form
    {
        public List<RelationalItem> relationalItems;
        public List<Cycle> cycles;
        public string tempKey;
        public string timeKey;
        public string probeAmtKey;
        public string doneKey;
        public string powKey;
        public KeyValue cpltKey;
        public string cyclekeys;

        public SymbIOT parent;

        private string cycleString;
        public string CycleString
        {
            get { return cycleString; }
        }

        public RelationalBuilder(SymbIOT p, List<Cycle> cy, string temp, string time, string probe, string done, string pow, KeyValue cplt)
        {
            InitializeComponent();
            relationalItems = new List<RelationalItem>();
            cycles = cy;
            tempKey = temp;
            timeKey = time;
            probeAmtKey = probe;
            cpltKey = cplt;
            doneKey = done;
            powKey = pow;
            cyclekeys = "";
            parent = p;

            foreach (Cycle c in cycles)
            {
                if (!cyclekeys.Contains(c.KeyID))
                {
                    cyclekeys += c.KeyID + ";";
                }
            }
        }

        public void BuildCycle()
        {
            cycleString = "FE010001FD020001FE010003";
            cycleString += RB_Start.Checked ? "05" : "06";
            cycleString += "FE02000102010000";
            cycleString += "FE020003" + StringtoHex(TB_Name.Text);
            cycleString = TB_URL.Text != "" ? cycleString + "02030008" + StringtoHex(TB_URL.Text) : cycleString;
            cycleString += "FE02000401";
            cycleString += "FE03000102010001";
            foreach (RelationalItem ri in relationalItems)
            {
                cycleString += ri.InnerStep;
            }
            cycleString += "FE03000202010001FE02000202010000FE010002FD020001";
        }

        public void ReSort()
        {
            PAN_Steps.Controls.Clear();
            relationalItems.Sort((x, y) => x.Index.CompareTo(y.Index));
            for (int i = 0; i < relationalItems.Count; i++)
            {
                relationalItems[i].Index = i;
                relationalItems[i].Left = 0;
                relationalItems[i].Top = relationalItems[i].Height * i;
                if (i == 0)
                {
                    if (relationalItems.Count > 1)
                    {
                        relationalItems[i].SetUpDownButtons(false, true);
                    }
                    else
                    {
                        relationalItems[i].SetUpDownButtons(false, false);
                    }
                }
                else if (i == relationalItems.Count - 1)
                {
                    relationalItems[i].SetUpDownButtons(true, false);
                }
                else
                {
                    relationalItems[i].SetUpDownButtons(true, true);
                }
                PAN_Steps.Controls.Add(relationalItems[i]);
                relationalItems[i].UpdateInnerStep(this, null);  
            }
        }

        private void BTN_Add_Click(object sender, EventArgs e)
        {
            RelationalItem ri = new RelationalItem(this,cycles,tempKey,timeKey,probeAmtKey,doneKey,powKey,cpltKey,relationalItems.Count);
            relationalItems.Add(ri);
            ReSort();
            PAN_Steps.ScrollControlIntoView(ri);
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

        public string HextoString(string h)
        {
            string _string = "";
            for (int i = 0; i < h.Length; i += 2)
            {
                if (h.Substring(i, 2) == "00")
                {                    
                    break;
                }
                else
                {
                    _string += Convert.ToChar(int.Parse(h.Substring(i, 2), NumberStyles.AllowHexSpecifier));
                }
            }

            return _string;
        }

        public int Cx10toF(int cx10)
        {
            double a = cx10 / 10;
            double b = a * 9 / 5 + 32;
            return (int)b;
        }

        private void BTN_Copy_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            foreach (RelationalItem ri in relationalItems)
            {
                if (!ri.IsValid)
                {
                    MessageBox.Show("Invalid parameters in Step " + (ri.Index+1).ToString());
                    isValid = false;
                }
            }
            if (isValid)
            {
                BuildCycle();
                string outputString = "FF3333" + cycleString;
                outputString = (outputString.Length / 2).ToString("X4") + outputString;
                if (CB_Delimit.Checked)
                {
                    outputString = Regex.Replace(outputString, ".{2}", "$0,");
                    outputString = outputString.Substring(0, outputString.Length - 1);
                }
                Clipboard.SetText(outputString);
                MessageBox.Show("MQTT payload copied to clipboard");
            }
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BTN_Import_Click(object sender, EventArgs e)
        {
            string mes = TB_Import.Text.ToUpper().Replace(".","").Replace(",","");
            try
            {
                mes = mes.Substring(mes.IndexOf("FE010001FD020001"));
                mes = mes.Substring(mes.IndexOf("FE020003")+8);
                TB_Name.Text = HextoString(mes);
                if (mes.IndexOf("02030008") > 0)
                {
                    mes = mes.Substring(mes.IndexOf("02030008") + 8);
                    TB_URL.Text = HextoString(mes);
                }
                mes = mes.Substring(mes.IndexOf("FE020001")+8);
                mes = mes.Replace("FE020001", "~");

                PAN_Steps.Controls.Clear();
                relationalItems.Clear();

                string[] steps = mes.Split('~');
                for (int i = 0; i < steps.Length; i++)
                {
                    int cidx = -1;
                    string []msg = new string[10];
                    string temp = "";
                    string time = "";
                    string zone = "";
                    string probe = "";
                    //string pow = "";
                    //string done = "";
                    int eidx = -1;

                    steps[i] = steps[i].Substring(8);
                    steps[i] = steps[i].Substring(0,steps[i].IndexOf("FE020002"));
                    while (steps[i].Length > 0)
                    {
                        string key = steps[i].Substring(0, 8);
                        steps[i] = steps[i].Substring(8);
                        if (key == tempKey)
                        {
                            temp = Cx10toF(int.Parse(steps[i].Substring(0, 4), NumberStyles.AllowHexSpecifier)).ToString();
                            steps[i] = steps[i].Substring(4);
                        }
                        if (key == powKey)
                        {
                            temp = (int.Parse(steps[i].Substring(0, 2), NumberStyles.AllowHexSpecifier)).ToString();
                            steps[i] = steps[i].Substring(2);
                        }
                        if(key == doneKey)
                        {
                            zone = (int.Parse(steps[i].Substring(0, 2), NumberStyles.AllowHexSpecifier)).ToString();
                            steps[i] = steps[i].Substring(2);
                        }
                        if (key == timeKey)
                        {
                            time = (int.Parse(steps[i].Substring(0, 8), NumberStyles.AllowHexSpecifier) / 60).ToString();
                            steps[i] = steps[i].Substring(8);
                        }
                        if (key == probeAmtKey)
                        {
                            if (doneKey == "")
                            {
                                probe = (int.Parse(steps[i].Substring(0, 4), NumberStyles.AllowHexSpecifier)).ToString();
                                steps[i] = steps[i].Substring(4);
                            }
                            else
                            {
                                probe = Cx10toF(int.Parse(steps[i].Substring(0, 4), NumberStyles.AllowHexSpecifier)).ToString();
                                steps[i] = steps[i].Substring(4);
                            }
                        }
                        if (key == "02030009" || key == "0203000B" || key == parent.v1msgKey || key == parent.v2msgKey)
                        {
                            string hexmsg = steps[i].Substring(0, steps[i].IndexOf("00") + 2);
                            if (hexmsg.Length % 2 != 0)//in case the last character ends in 0
                            {
                                hexmsg = steps[i].Substring(0, steps[i].IndexOf("00") + 3);
                            }

                            string ms = HextoString(hexmsg);
                            if (key == "0203000B" || key == parent.v2msgKey)
                            {
                                string[] parts = ms.Split(',');
                                msg[int.Parse(parts[0])] = parts[1];
                                //zone = parts[0];
                            }
                            else
                            {
                                msg[0] = ms;
                            }
                            steps[i] = steps[i].Substring(hexmsg.Length);
                        }
                        if (key == cpltKey.KeyID)
                        {
                            eidx = int.Parse(steps[i].Substring(0, 2), NumberStyles.AllowHexSpecifier);
                            steps[i] = steps[i].Substring(2);
                        }
                        if (cyclekeys.Contains(key))
                        {
                            for(int j = 0; j < cycles.Count; j++)
                            {
                                if (key == cycles[j].KeyID && steps[i].Substring(0, 2) == cycles[j].Enum.ToString("X2"))
                                {
                                    cidx = j;
                                    break;
                                }
                            }
                            steps[i] = steps[i].Substring(2);
                        }
                    }
                    this.BTN_Add_Click(this, null);
                    this.relationalItems[relationalItems.Count - 1].SetAttributes(cidx, msg, temp, time, probe,eidx,zone);

                }
                PAN_Steps.ScrollControlIntoView(relationalItems[0]);
            }
            catch
            {
                MessageBox.Show("Specified input is not a recognized relational cycle for the selected platform/cavity.");
            }
        }

        private void BTN_Send_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            foreach (RelationalItem ri in relationalItems)
            {
                if (!ri.IsValid)
                {
                    MessageBox.Show("Invalid parameters in Step " + (ri.Index+1).ToString());
                    isValid = false;
                }
            }
            if (isValid)
            {
                BuildCycle();
                parent.SendExternal(cycleString, true, true);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RelationalItem ri in relationalItems)
            {
                ri.UpdateInnerStep(this, null);
            }
        }

        public bool SysChecked()
        {
            return RB_Sys.Checked;
        }
    }
}
