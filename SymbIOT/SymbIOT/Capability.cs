using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;

namespace SymbIOT
{
    public partial class Capability : Form
    {
        SymbIOT parent;
        List<List<Cycle>> cycles;
        List<KeyValue> keyValues;
        List<ListBox> LB_Cycle;
        string capabilityname;

        public Capability(SymbIOT p, List<List<Cycle>> c, List<KeyValue> k)
        {
            InitializeComponent();
            parent = p;
            cycles = c;
            keyValues = k;
            LB_Cycle = new List<ListBox>(3);
            for (int tabs = 0; tabs < cycles.Count; tabs++)
            {
                ListBox clb = new ListBox();
                clb.SelectionMode = SelectionMode.MultiExtended;
                clb.Size = new System.Drawing.Size(252, 284);
                clb.Location = new System.Drawing.Point(0,0);
                clb.SelectedIndexChanged +=new EventHandler(CLB_Cycles_SelectedIndexChanged);
                LB_Cycle.Add(clb);
                TAB_Cavities.TabPages[tabs].Controls.Add(LB_Cycle[tabs]);
            }
            if (parent.capabilityLoaded)
            {
                this.LoadListBox();
            }
        }

        private void BTN_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                capabilityname = ofd.SafeFileName.Replace(".json", "");
                StreamReader sr = new StreamReader(ofd.FileName);
                JObject jo = JObject.Parse(sr.ReadToEnd());
                if (parent.dm_name == "Cooking")
                {
                    string[] capcavities = { "OvenUpperCavity", "OvenLowerCavity", "MwoCavity" };
                    for (int tab = 0; tab < capcavities.Length; tab++)
                    {
                        try
                        {
                            JToken jt = jo.SelectToken("Capability." + capcavities[tab] + ".CapabilityData", true);
                            cycles[tab].Clear();
                            JEnumerable<JToken> jc = jt.Children();
                            foreach (JProperty jto in jc)
                            {
                                if (jto.Name.Contains("BroilAndGrill"))
                                {
                                }
                                string ckvp = "";
                                int cenum = 0;
                                try
                                {
                                    ckvp = ((int)jto.SelectToken("$..ConnectivityKey")).ToString("X8");
                                    cenum = (int)jto.SelectToken("$..ConnectivityValue");
                                }
                                catch
                                {
                                    IEnumerable<KeyValue> matches = keyValues.Where(k => jto.Name.StartsWith(k.CapabilityName));
                                    KeyValue kto = keyValues.FirstOrDefault(k => jto.Name.StartsWith(k.CapabilityName));
                                    int longestcap = kto.CapabilityName.Length;
                                    foreach (KeyValue kvl in matches) //need to do it this way because CommonMode and CommonModeProbe both match the same StartsWith
                                    {
                                        if (kvl.CapabilityName.Length > longestcap)
                                        {
                                            kto = kvl;
                                            longestcap = kvl.CapabilityName.Length;
                                        }
                                    }
                                    ckvp = kto.KeyID;
                                    try
                                    {
                                        for (int count = 0; count < kto.EnumName.Enums.Count; count++)
                                        {
                                            if (kto.EnumName.Enums[count] != null)
                                            {
                                                string na = kto.EnumName.ToCamelCase(kto.EnumName.Name.Replace(kto.Instance + ".", ""), false) + kto.EnumName.LCS + kto.EnumName.Enums[count];
                                                if (kto.CapabilityName + (kto.EnumName.Enums[count].ToString().StartsWith(kto.EnumName.LCS) ? "" : kto.EnumName.LCS) + kto.EnumName.Enums[count] == jto.Name
                                                    || kto.CapabilityName + kto.EnumName.Enums[count] == jto.Name
                                                    || jto.Name.EndsWith(kto.EnumName.ToCamelCase(kto.EnumName.Name.Replace(kto.Instance + ".", ""), false) + kto.EnumName.LCS + kto.EnumName.Enums[count]))
                                                {
                                                    cenum = count;
                                                    count = kto.EnumName.Enums.Count;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        int.TryParse(jto.Name.Replace(kto.CapabilityName, ""), out cenum);
                                    }
                                }
                                Cycle c = new Cycle(keyValues.FirstOrDefault(z => z.KeyID == ckvp), cenum);

                                c.OperationKey = keyValues.FirstOrDefault(k => (k.CapabilityName.StartsWith(capcavities[tab].Replace("Cavity", "")) && k.CapabilityName.EndsWith("SetOperations")));
                                c.StateKey = keyValues.FirstOrDefault(k => (k.CapabilityName.StartsWith(capcavities[tab].Replace("Cavity", "")) && k.CapabilityName.EndsWith("StatusState")));
                                c.CycleTimeRemainingKey = keyValues.FirstOrDefault(k => (k.CapabilityName.StartsWith(capcavities[tab].Replace("Cavity", "")) && k.CapabilityName.EndsWith("CookTimeRemaining")));
                                c.DisplayTempKey = keyValues.FirstOrDefault(k => (k.CapabilityName.StartsWith(capcavities[tab].Replace("Cavity", "")) && k.CapabilityName.EndsWith("DisplayTemp")));

                                cycles[tab].Add(c);
                                JToken required = jto.SelectToken("$..Required", false);
                                //JToken noneditable = jto.SelectToken("$..NonEditable", false);
                                JToken optional = jto.SelectToken("$..Optional", false);

                                AddOptionKeys(c, required, true);
                                AddOptionKeys(c, optional, false);
                                /*try
                                {
                                    JEnumerable<JToken> jop = optional.Children();
                                    foreach (JToken joo in jop)
                                    {
                                        string opt = joo.ToString().Replace("\"", "").Replace("}", "{").Replace(",", ":").Replace("\r\n", "").Replace(" ", "");
                                        string[] optparts = opt.Split('{');
                                        KeyValueProperty reqkvp = new KeyValueProperty(keyValues.FirstOrDefault(x => x.CapabilityName == optparts[0].Replace(":", "")));
                                        c.AddOptionKey(reqkvp, false);
                                        JEnumerable<JToken> jret = joo.Children();
                                        foreach (JProperty jretp in jret.Children())
                                        {
                                            if (jretp.Name.EndsWith("Range"))
                                            {
                                                foreach (JProperty jtrp in jretp.Value.Children())
                                                {                
                                                    switch (jtrp.Name)
                                                    {
                                                        case "Min":
                                                            reqkvp.Min = jtrp.Value.ToString();
                                                            break;
                                                        case "Max":
                                                            reqkvp.Max = jtrp.Value.ToString();
                                                            break;
                                                        case "Default":
                                                            reqkvp.Default = jtrp.Value.ToString();
                                                            break;
                                                        case "StepF":
                                                            reqkvp.StepF = jtrp.Value.ToString();
                                                            break;
                                                        case "StepC":
                                                            reqkvp.StepC = jtrp.Value.ToString();
                                                            break;
                                                        case "Step":
                                                            reqkvp.Step = jtrp.Value.ToString();
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            if (jretp.Name == "Enumeration")
                                            {
                                                reqkvp.IsEnum = true;
                                                string[] enums = jretp.Value.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                                                foreach (string s in enums)
                                                {
                                                    for (int en = 0; en < reqkvp.KeyValue.EnumName.Enums.Count; en++)
                                                    {
                                                        try
                                                        {
                                                            if (reqkvp.KeyValue.EnumName.Enums[en].ToString() == s.Trim() || 
                                                                reqkvp.KeyValue.EnumName.LCS + reqkvp.KeyValue.EnumName.Enums[en].ToString() == s.Trim())
                                                            {
                                                                reqkvp.AddValidEnum(en);
                                                                //if (reqkvp.Default == s.Trim())
                                                                //{
                                                                //    reqkvp.Default = en.ToString();
                                                                //}
                                                            }
                                                        }
                                                        catch { en = 256; }
                                                    }
                                                }
                                            }
                                            if (jretp.Name == "Default")
                                            {
                                                reqkvp.Default = jretp.Value.ToString();
                                            }
                                        }
                                    }
                                }
                                catch { }*/

                            }
                        }
                        catch { }
                    }

                    LoadListBox();
                    parent.capabilityLoaded = true;
                }
            }
            
        }

        private void AddOptionKeys(Cycle c, JToken options, bool required)
        {
            try
            {
                JEnumerable<JToken> jre = options.Children();
                foreach (JToken jor in jre)
                {
                    string req = jor.ToString().Replace("\"", "").Replace("}", "{").Replace(",", ":").Replace("\r\n", "").Replace(" ", "");
                    string[] reqparts = req.Split('{');
                    KeyValueProperty reqkvp = new KeyValueProperty(keyValues.FirstOrDefault(x => x.CapabilityName == reqparts[0].Replace(":", "")),required);
                    c.AddOptionKey(reqkvp, required);
                    JEnumerable<JToken> jret = jor.Children();
                    foreach (JProperty jretp in jret.Children())
                    {
                        if (jretp.Name.EndsWith("Range"))
                        {
                            foreach (JProperty jtrp in jretp.Value.Children())
                            {
                                switch (jtrp.Name)
                                {
                                    case "Min":
                                        reqkvp.Min = jtrp.Value.ToString();
                                        break;
                                    case "Max":
                                        reqkvp.Max = jtrp.Value.ToString();
                                        break;
                                    case "Default":
                                        reqkvp.Default = jtrp.Value.ToString();
                                        break;
                                    case "StepF":
                                        reqkvp.StepF = jtrp.Value.ToString();
                                        break;
                                    case "StepC":
                                        reqkvp.StepC = jtrp.Value.ToString();
                                        break;
                                    case "Step":
                                        reqkvp.Step = jtrp.Value.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (jretp.Name == "Enumeration")
                        {
                            reqkvp.IsEnum = true;
                            string[] enums = jretp.Value.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                            string lcs = reqkvp.KeyValue.EnumName.LCS;
                            foreach (string s in enums)
                            {
                                for (int en = 0; en < reqkvp.KeyValue.EnumName.Enums.Count; en++)
                                {
                                    try
                                    {
                                        if (reqkvp.KeyValue.EnumName.Enums[en] != null)
                                        {
                                            if (reqkvp.KeyValue.EnumName.Enums[en].ToString() == s.Trim() ||
                                                s.Trim().EndsWith(lcs + reqkvp.KeyValue.EnumName.Enums[en].ToString()))
                                            {
                                                reqkvp.AddValidEnum(en);
                                                /*if (reqkvp.Default == s.Trim())
                                                {
                                                    reqkvp.Default = en.ToString();
                                                }*/
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        if (reqkvp.ValidEnums.Count == 0 && lcs != "")
                                        {
                                            lcs = "";
                                            en = 0;
                                        }
                                        else
                                        {
                                            en = 256;
                                        }
                                    }
                                }
                            }
                        }
                        if (jretp.Name == "Default")
                        {
                            reqkvp.Default = jretp.Value.ToString();
                        }
                        if (jretp.Name == "List")
                        {
                            string[] list = jretp.Value.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                            foreach (string s in list)
                            {
                                string listitem = s.Trim();                                
                                reqkvp.AddToList(listitem);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadListBox()
        {
            for (int tab = 0; tab < LB_Cycle.Count; tab++)
            {
                bool hascycles = false;
                LB_Cycle[tab].Items.Clear();
                try
                {
                    foreach (Cycle c in cycles[tab])
                    {
                        LB_Cycle[tab].Items.Add(c.KeyValue.AttributeName + "_" + c.Name);
                        hascycles = true;
                    }
                }
                catch
                {
                    hascycles = false;
                }
                if (hascycles)
                {
                    TAB_Cavities.SelectedIndex = tab;
                }
            }
        }

        private void CLB_Cycles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LBL_CycleName.Text = LB_Cycle[TAB_Cavities.SelectedIndex].SelectedItem.ToString();
            RefreshPAN_CycleCommands(cycles[TAB_Cavities.SelectedIndex][LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndex]);
        }

        private void RefreshPAN_CycleCommands(Cycle c)
        {
            PAN_CycleCommands.Controls.Clear();
            foreach (KeyValueProperty kvp in c.RequiredKeys)
            {
                CapabilityItem capi = new CapabilityItem(kvp);
                capi.Top = PAN_CycleCommands.Controls.Count * capi.Height;
                capi.Left = 0;
                PAN_CycleCommands.Controls.Add(capi);
            }
            foreach (KeyValueProperty kvpo in c.OptionalKeys)
            {
                CapabilityItem capi = new CapabilityItem(kvpo);
                capi.Top = PAN_CycleCommands.Controls.Count * capi.Height;
                capi.Left = 0;
                PAN_CycleCommands.Controls.Add(capi);
            }
        }

        private void BTN_Start_Click(object sender, EventArgs e)
        {
            Cycle c = cycles[TAB_Cavities.SelectedIndex][LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndex];
            string output = cycles[TAB_Cavities.SelectedIndex][LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndex].KeyValue.OutValue(cycles[TAB_Cavities.SelectedIndex][LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndex].KeyValue.Process(cycles[TAB_Cavities.SelectedIndex][LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndex].Enum.ToString("X2")));
            foreach (CapabilityItem ci in PAN_CycleCommands.Controls)
            {
                string outvalue = ci.KVP.IsEnum ? ci.SelectedItem.Substring(3) : ci.SelectedItem;
                if (ci.KVP.Required)
                {
                    output += ci.KVP.KeyValue.OutValue(outvalue);
                }
                else
                {
                    if (ci.KVP.KeyValue.OutValue(outvalue) != ci.KVP.KeyValue.OutValue(ci.KVP.Default))
                    {
                        output += ci.KVP.KeyValue.OutValue(outvalue);
                    }
                }
            }
            output += c.OperationKey.KeyID + ((Button)sender).Tag.ToString();
            parent.SendExternal(output, false, true);
        }

        XmlDocument CancelScript(Cycle cycle, List<string> setkeys, int waittime)
        {
            List<string> outkeys = new List<string>();

            outkeys.Add(cycle.OperationKey.KeyID + ",01");

            return CreateScript(cycle.Name + " Cancel", outkeys, setkeys, waittime);
        }

        XmlDocument CreateScript(string docname, List<string> outkeys, List<string> inkeys, int waittime)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Script", null);
            XmlNode name = doc.CreateNode(XmlNodeType.Attribute, "Name", null);
            name.Value = docname;
            node.Attributes.SetNamedItem(name);

            foreach (string s in outkeys)
            {
                string[] keys = s.Split(',');
                XmlNode outkey = doc.CreateElement("OutKey");
                XmlNode kvp = doc.CreateElement("KVP");
                kvp.InnerText = keys[0];
                XmlNode val = doc.CreateElement("Value");
                val.InnerText = keys[1];
                outkey.AppendChild(kvp);
                outkey.AppendChild(val);
                node.AppendChild(outkey);
            }
            foreach (string s in inkeys)
            {
                string[] keys = s.Split(',');
                XmlNode outkey = doc.CreateElement("InKey");
                XmlNode kvp = doc.CreateElement("KVP");
                kvp.InnerText = keys[0];
                XmlNode val = doc.CreateElement("Value");
                val.InnerText = keys[1];
                outkey.AppendChild(kvp);
                outkey.AppendChild(val);
                node.AppendChild(outkey);
            }
            XmlNode wait = doc.CreateElement("Wait");
            wait.InnerText = waittime.ToString();
            node.AppendChild(wait);
            doc.AppendChild(node);

            return doc;
        }

        private string SetTargetTemp(KeyValueProperty k)
        {
            double min = double.Parse(k.Min);
            double max = double.Parse(k.Max);
            double def = double.Parse(k.Default);
            min = SymbIOT.tempUnits == 1 ? Math.Round((min / 10) * 9 / 5 + 32) : Math.Round(min / 10);
            max = SymbIOT.tempUnits == 1 ? Math.Round((max / 10) * 9 / 5 + 32) : Math.Round(max / 10);
            def = SymbIOT.tempUnits == 1 ? Math.Round((def / 10) * 9 / 5 + 32) : Math.Round(def / 10);

            string temp = def.ToString();
            if (RB_TempDefault.Checked)
            {
                temp = def.ToString();
            }
            else if (RB_TempMax.Checked)
            {
                temp = max.ToString();
            }
            else if (RB_TempMin.Checked)
            {
                temp = min.ToString();
            }
            else if (RB_TempRandom.Checked)
            {   
                int step = SymbIOT.tempUnits == 1 ? int.Parse(k.StepF) : int.Parse(k.StepC);

                int numsteps = (int)Math.Round((max - min) / step);
                Random rnd = new Random();
                int mult = rnd.Next(0, numsteps);
                temp = (min + step * mult).ToString();
            }

            return temp;
        }

        private void BTN_Generate_Click(object sender, EventArgs e)
        {
            if (LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndices.Count > 0)
            {
                XmlDocument batch = new XmlDocument();
                XmlNode node = batch.CreateNode(XmlNodeType.Element, "Batch", null);
                batch.AppendChild(node);

                Cycle curcycle;
                int stepcount = 0;
                foreach (int i in LB_Cycle[TAB_Cavities.SelectedIndex].SelectedIndices)
                {
                    curcycle = cycles[TAB_Cavities.SelectedIndex][i];
                    List<string> inkeys = new List<string>();
                    List<string> outkeys = new List<string>();
                    List<string> cancelkeys = new List<string>();
                    outkeys.Add(curcycle.KeyID + "," + (curcycle.KeyValue.EnumName != null ? curcycle.Enum.ToString("X2") : curcycle.CCID.ToString()));
                    inkeys.Add(curcycle.KeyID + "," + (curcycle.KeyValue.EnumName != null ? curcycle.Enum.ToString("X2") : curcycle.CCID.ToString()));
                    cancelkeys.Add(curcycle.KeyID + "," + "00");

                    foreach (KeyValueProperty kvpr in curcycle.RequiredKeys)
                    {
                        CapabilityItem capi = new CapabilityItem(kvpr);

                        int cooktime;
                        if (CB_CookTime.Checked && int.TryParse(TB_CookTime.Text, out cooktime) && capi.KVP.KeyValue.CapabilityName.EndsWith("CookTimeSet"))
                        {
                            capi.SetSelectedValue = capi.KVP.KeyValue.Process((cooktime*60).ToString("X8"));
                        }

                        else if (capi.KVP.KeyValue.CapabilityName.EndsWith("TargetTemp"))
                        {
                            capi.SetSelectedValue = SetTargetTemp(capi.KVP);
                        }
                        else
                        {
                            if (CB_Randomize.Checked)
                            {
                                capi.SetSelectedValue = capi.ReturnRandom();
                            }
                        }

                        if (!(capi.KVP.KeyValue.Format == "time" && capi.SelectedItem == "0:00:00"))
                        {
                            outkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? capi.SelectedItem.Substring(0, 2) : capi.SelectedItem));
                            inkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? capi.SelectedItem.Substring(0, 2) : capi.SelectedItem));
                            cancelkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? "00" : capi.KVP.KeyValue.Process(0.ToString("X" + capi.KVP.KeyValue.Length))));
                        }
                        
                    }
                    if (!CB_Optional.Checked)
                    {
                        foreach (KeyValueProperty kvpr in curcycle.OptionalKeys)
                        {
                            CapabilityItem capi = new CapabilityItem(kvpr);

                            int cooktime;
                            int delaytime;
                            if (CB_CookTime.Checked && int.TryParse(TB_CookTime.Text, out cooktime) && capi.KVP.KeyValue.CapabilityName.EndsWith("CookTimeSet"))
                            {
                                capi.SetSelectedValue = capi.KVP.KeyValue.Process((cooktime * 60).ToString("X8"));
                            }                            
                            else if (CB_DelayTime.Checked && int.TryParse(TB_DelayTime.Text, out delaytime) && capi.KVP.KeyValue.CapabilityName.EndsWith("DelayTime"))
                            {
                                capi.SetSelectedValue = capi.KVP.KeyValue.Process((delaytime * 60).ToString("X8"));
                            }
                            else
                            {
                                if (CB_Randomize.Checked)
                                {
                                    capi.SetSelectedValue = capi.ReturnRandom();
                                }
                            }

                            if (!(capi.KVP.KeyValue.Format == "time" && capi.SelectedItem == "0:00:00"))
                            {
                                outkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? capi.SelectedItem.Substring(0, 2) : capi.SelectedItem));
                                inkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? capi.SelectedItem.Substring(0, 2) : capi.SelectedItem));
                                cancelkeys.Add(capi.KVP.KeyValue.KeyID + "," + (capi.KVP.IsEnum ? "00" : capi.KVP.KeyValue.Process(0.ToString("X" + capi.KVP.KeyValue.Length))));
                            }

                        }
                    }

                    outkeys.Add(curcycle.OperationKey.KeyID + "," + "02");
                    inkeys.Add(curcycle.OperationKey.KeyID + "," + "02");
                    inkeys.Add(curcycle.StateKey.KeyID + ",");
                    inkeys.Add(curcycle.DisplayTempKey.KeyID + ",");
                    cancelkeys.Add(curcycle.StateKey.KeyID + "," + "00");
                    cancelkeys.Add(curcycle.DisplayTempKey.KeyID + "," + curcycle.DisplayTempKey.Process("0000"));

                    int canceltime = 10000;
                    if (CB_Cancel.Checked && int.TryParse(TB_CancelTime.Text, out canceltime))
                    {
                        canceltime = canceltime * 1000;
                    }
                    stepcount++;
                    XmlNode newscript = batch.ImportNode(CreateScript(stepcount.ToString("00") + ". " + curcycle.Name, outkeys, inkeys, canceltime).SelectSingleNode("Script"), true);
                    batch.SelectSingleNode("Batch").AppendChild(newscript);
                    if (CB_Cancel.Checked)
                    {
                        stepcount++;
                        outkeys.Clear();
                        outkeys.Add(curcycle.OperationKey.KeyID + ",01");
                        XmlNode cancelscript = batch.ImportNode(CreateScript(stepcount.ToString("00") + ". " + curcycle.Name + " Cancel", outkeys, cancelkeys, 10000).SelectSingleNode("Script"), true);
                        batch.SelectSingleNode("Batch").AppendChild(cancelscript);
                    }
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "SymbIOT_Script-" + capabilityname + ".xml";
                sfd.Filter = "Script Files (*.xml)|*.xml";
                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK && sfd.FileName != "")
                {                    
                    batch.Save(sfd.FileName);
                }
            }
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}