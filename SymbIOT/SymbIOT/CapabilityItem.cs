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
    public partial class CapabilityItem : UserControl
    {
        private KeyValueProperty _kvp;
        public KeyValueProperty KVP
        {
            get { return _kvp; }
        }

        public string SetSelectedValue;

        public string SelectedItem
        {
            get 
            {
                string output = "";
                if (SetSelectedValue == "")
                {
                    if (CBO_Value.Visible)
                    {
                        output = CBO_Value.SelectedItem.ToString();
                    }
                    else
                    {
                        if (TB_Value.Visible)
                        {
                            output = TB_Value.Text;
                        }
                        else
                        {
                            output = NUD_Value.Value.ToString();
                        }
                    }
                }
                else
                {
                    output = SetSelectedValue;
                }

                return output;
            }
        }

        public CapabilityItem(KeyValueProperty kvp)
        {
            InitializeComponent();
            CBO_Value.Visible = false;
            NUD_Value.Visible = false;
            TB_Value.Visible = false;
            _kvp = kvp;
            LBL_KeyName.Text = _kvp.KeyValue.AttributeName;
            if (_kvp.List.Count > 0 || _kvp.IsEnum)
            {
                CBO_Value.Visible = true;
            }
            else
            {
                NUD_Value.Visible = true;
                //TB_Value.Visible = true;
            }
            SetCBOValues();
            SetSelectedValue = "";
        }

        private void SetCBOValues()
        {
            if (CBO_Value.Visible)
            {
                CBO_Value.Items.Clear();
                if (!_kvp.IsEnum)
                {
                    if (_kvp.List.Count > 0)
                    {
                        foreach (string s in _kvp.List)
                        {
                            string listitem = s;
                            try
                            {
                                listitem = _kvp.KeyValue.Process(int.Parse(listitem).ToString("X" + _kvp.KeyValue.Length.ToString()));
                            }
                            catch
                            {
                            }
                            CBO_Value.Items.Add(listitem);
                            if (_kvp.Default == s)
                            {
                                CBO_Value.SelectedItem = listitem;
                            }
                        }
                    }
                    else
                    {
                        double min = double.Parse(_kvp.Min);
                        double max = double.Parse(_kvp.Max);
                        double def = 0;
                        try
                        {
                            def = double.Parse(_kvp.Default);
                        }
                        catch
                        {
                            def = Convert.ToDouble(bool.Parse(_kvp.Default));
                        }
                        int step = 1;
                        if (_kvp.KeyValue.Format == "equation")
                        {
                            min = SymbIOT.tempUnits == 1 ? Math.Round((min / 10) * 9 / 5 + 32) : Math.Round(min / 10);
                            max = SymbIOT.tempUnits == 1 ? Math.Round((max / 10) * 9 / 5 + 32) : Math.Round(max / 10);
                            def = SymbIOT.tempUnits == 1 ? Math.Round((def / 10) * 9 / 5 + 32) : Math.Round(def / 10);
                            step = SymbIOT.tempUnits == 1 ? int.Parse(_kvp.StepF) : int.Parse(_kvp.StepC);
                        }
                        else
                        {
                            step = int.Parse(_kvp.Step);
                        }
                        for (int i = (int)min; i <= (int)max; i += step)
                        {
                            if (_kvp.KeyValue.Format == "time")
                            {
                                CBO_Value.Items.Add(_kvp.KeyValue.Process(i.ToString("X8")));
                            }
                            else
                            {
                                CBO_Value.Items.Add(i);
                            }
                            if (i == (int)def)
                            {
                                CBO_Value.SelectedIndex = CBO_Value.Items.Count - 1;
                            }

                        }
                    }
                }
                else
                {
                    foreach (int id in _kvp.ValidEnums)
                    {
                        string val = _kvp.KeyValue.Process(id.ToString("X2"));
                        CBO_Value.Items.Add(id.ToString("X2") + "=" + val);
                        if (_kvp.Default == val || _kvp.Default.EndsWith(val))
                        {
                            CBO_Value.SelectedIndex = CBO_Value.Items.Count - 1;
                        }
                    }
                }
            }
            else
            {
                decimal min = decimal.Parse(_kvp.Min);
                decimal max = decimal.Parse(_kvp.Max);
                decimal def = 0;
                try
                {
                    def = decimal.Parse(_kvp.Default);
                }
                catch
                {
                    def = Convert.ToDecimal(bool.Parse(_kvp.Default));
                }
                int step = 1;
                if (_kvp.KeyValue.Format == "time")
                {
                    TB_Value.Visible = true;
                }
                if (_kvp.KeyValue.Format == "equation")
                {
                    min = SymbIOT.tempUnits == 1 ? Math.Round((min / 10) * 9 / 5 + 32) : Math.Round(min / 10);
                    max = SymbIOT.tempUnits == 1 ? Math.Round((max / 10) * 9 / 5 + 32) : Math.Round(max / 10);
                    def = SymbIOT.tempUnits == 1 ? Math.Round((def / 10) * 9 / 5 + 32) : Math.Round(def / 10);
                    step = SymbIOT.tempUnits == 1 ? int.Parse(_kvp.StepF) : int.Parse(_kvp.StepC);
                }
                else
                {
                    step = int.Parse(_kvp.Step);
                }
                NUD_Value.Minimum = Math.Min(min, def);
                NUD_Value.Maximum = Math.Max(max, def);
                NUD_Value.Value = def;
                NUD_Value.Increment = step;
                if (TB_Value.Visible)
                {
                    TB_Value.Text = _kvp.KeyValue.Process(((int)NUD_Value.Value).ToString("X4"));
                }
            }
        }

        private void NUD_Value_ValueChanged(object sender, EventArgs e)
        {
            if (TB_Value.Visible)
            {
                TB_Value.Text = _kvp.KeyValue.Process(((int)NUD_Value.Value).ToString("X4"));
            }
        }

        private void TB_Value_Leave(object sender, EventArgs e)
        {
            try
            {
                NUD_Value.Value = int.Parse(_kvp.KeyValue.ConvertTime(TB_Value.Text),NumberStyles.AllowHexSpecifier);
            }
            catch
            {
                NUD_Value.Value = NUD_Value.Minimum;
            }
        }

        private void TB_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && TB_Value.Focused)
            {
                TB_Value_Leave(null, null);
            }
        }

        public string ReturnRandom()
        {
            string output = "";
            if (SetSelectedValue == "")
            {
                Random rnd = new Random();
                if (CBO_Value.Visible)
                {
                    output = CBO_Value.Items[rnd.Next(0, CBO_Value.Items.Count)].ToString();                    
                }
                else
                {
                    if (TB_Value.Visible)
                    {
                        output = TB_Value.Text;
                    }
                    else
                    {
                        int numsteps = (int)Math.Round((NUD_Value.Maximum - NUD_Value.Minimum)/NUD_Value.Increment);
                        int mult = rnd.Next(0, numsteps);
                        output = (NUD_Value.Minimum + mult*NUD_Value.Increment).ToString();
                    }
                }
            }
            else
            {
                output = SetSelectedValue;
            }

            return output;
        }
    }
}
