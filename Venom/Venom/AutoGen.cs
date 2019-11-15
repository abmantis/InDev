using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.IO;
using System.Globalization;

namespace VenomNamespace
{
    public partial class AutoGen : WideInterface
    {
        public Venom parent;
        public PayList plist;
        public int TESTCASEMAX = 50;
        //private BindingSource sbind = new BindingSource();
        List<IPData> iplist = new List<IPData>();

        public AutoGen(Venom ParentForm, WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {          
            InitializeComponent();
            parent = ParentForm;
            ResetForm();
            CB_Variant.Items.AddRange(new object[] {"NAR Cooking",
                                                    "EMEA Cooking", "NAR Laundry", "Other (any remote cycle)"});

        }

        private void ResetForm()
        {
            TB_ACU_DWN.Text = "";
            TB_ACU_UP.Text = "";
            TB_EXP_DWN.Text = "";
            TB_EXP_UP.Text = "";
            TB_HMI_DWN.Text = "";
            TB_HMI_UP.Text = "";
            TB_IP.Text = "";
            TB_Other.Text = "";
            TB_WIFI_DWN.Text = "";
            TB_WIFI_UP.Text = "";
            CB_Variant.ResetText();
        }
        public void CB_Variant_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CB_Variant.Text.Equals("Other (any remote cycle)"))
            {
                TB_Other.Enabled = true;
                TB_Other.Text = "Paste MQTT Payload Here";
                TB_Other.Visible = true;
            }
            else
            {
                TB_Other.Enabled = false;
                TB_Other.Visible = false;

                if (!TB_Other.Text.Equals(""))
                    TB_Other.Text = "";
            }

        }
        private void BTN_Gen_Click(object sender, EventArgs e)
        {
            //  DialogResult dialogResult = MessageBox.Show("This will automatically create a new test plan run from the current IP. " +
            //  "This will then clear the current payload list and update the table accordingly. " +
            //  "Press Yes to Create or No to Cancel.", "Verify Full Clear and Auto Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if (dialogResult == DialogResult.Yes)
            // {

            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);

            if (cai == null)
            {
                MessageBox.Show("Target IP Address of " + TB_IP.Text + " is not currently listed within Wifibasic. " +
                    "Please verify the IP Address is connected and listed in Wifibasic, then retry generation.", "Error: WifiBasic IP Address Not Found",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }


            bool check = false;

            List<string> types = new List<string>();
            List<string> sources = new List<string>();
            //Check to make sure we have both upgrade and downgrade bundles and set type if we do
            if (!TB_HMI_UP.Text.Equals(""))
            {
                if (!TB_HMI_DWN.Text.Equals(""))
                {
                    check = true;
                    types.Add("HMI\tUpgrade");
                    sources.Add(TB_HMI_UP.Text);
                }
                else
                    check = false;
            }

            if (!TB_HMI_DWN.Text.Equals(""))
            {
                if (!TB_HMI_UP.Text.Equals(""))
                {
                    check = true;
                    types.Add("HMI\tDowngrade");
                    sources.Add(TB_HMI_DWN.Text);
                }
                else
                    check = false;
            }

            if (!TB_ACU_UP.Text.Equals(""))
            {
                if (!TB_ACU_DWN.Text.Equals(""))
                {
                    check = true;
                    types.Add("ACU\tUpgrade");
                    sources.Add(TB_ACU_UP.Text);
                }
                else
                    check = false;
            }

            if (!TB_ACU_DWN.Text.Equals(""))
            {
                if (!TB_ACU_UP.Text.Equals(""))
                {
                    check = true;
                    types.Add("ACU\tDowngrade");
                    sources.Add(TB_ACU_DWN.Text);
                }
                else
                    check = false;
            }

            if (!TB_WIFI_UP.Text.Equals(""))
            {
                if (!TB_WIFI_DWN.Text.Equals(""))
                {
                    check = true;
                    types.Add("Wifi\tUpgrade");
                    sources.Add(TB_WIFI_UP.Text);
                }
                else
                    check = false;
            }

            if (!TB_WIFI_DWN.Text.Equals(""))
            {
                if (!TB_WIFI_UP.Text.Equals(""))
                {
                    check = true;
                    types.Add("Wifi\tDowngrade");
                    sources.Add(TB_WIFI_DWN.Text);
                }
                else
                    check = false;
            }

            if (!TB_EXP_UP.Text.Equals(""))
            {
                if (!TB_EXP_DWN.Text.Equals(""))
                {
                    check = true;
                    types.Add("Expansion\tUpgrade");
                    sources.Add(TB_EXP_UP.Text);
                }
                else
                    check = false;
            }

            if (!TB_EXP_DWN.Text.Equals(""))
            {
                if (!TB_EXP_UP.Text.Equals(""))
                {
                    check = true;
                    types.Add("Expansion\tDowngrade");
                    sources.Add(TB_EXP_DWN.Text);
                }
                else
                    check = false;
            }

            if (!check)
            {
                DialogResult checkresult = MessageBox.Show("You have NOT provided both an upgrade and downgrade OTA version. " +
                                                            "Please check each text box and retry again.", "Error: Missing an Upgrade or Downgrade Payload",
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                types.Clear();
                return;
            }

            //parent.results.Clear();
            // parent.responses.Clear();
            // parent.iplist.Clear();
            // parent.LB_IPs.Items.Clear();
            // parent.DGV_Data.Refresh();
            //plist.DGV_Data.Refresh();

            //Set MQTT payload to start standard bake 350 for whatever product or allow user to input one
            string mqttpay = "";

            if (CB_Variant.Text.Equals("NAR Cooking"))
                mqttpay = "001BFF33330310000C02030D00010000003C0310000106E6030F000202"; // Standard bake 350 for upper oven for 1 minute;
            else if (CB_Variant.Text.Equals("EMEA Cooking"))
                mqttpay = "001BFF33330B02001B0104090001028F04060001000000780408000202"; // Standard bake for Speed Oven (MWO bake instead of upper oven)          ;
            else if (CB_Variant.Text.Equals("NAR Laundry"))
                mqttpay = "0026FF333305050006010505001503050500180305050014000505000A010505000D000307000102"; // Standard wash cycle for Janus washer (wash cavity)
            else
                mqttpay = CB_Variant.Text;

            //mqttpay = mqttpay.Substring(16, mqttpay.Length - 32);

            try
            {
                //if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                //parent.LB_IPs.Items.Add(cai.IPAddress);

                int listindex = 0;
                foreach (String type in types)
                {
                    IPData newip = new IPData(cai.IPAddress, sources[listindex]);
                    string[] parts = type.Split('\t');
                    newip.MAC = cai.MacAddress;
                    newip.Node = parts[0];
                    newip.Type = parts[1];
                    newip.MQTTPay = mqttpay;
                    iplist.Add(newip);

                    /* Update window for added IP
                    DataRow dr = parent.results.NewRow();

                    dr["IP Address"] = newip.IPAddress;
                    dr["OTA Payload"] = sources[count];
                    dr["Delivery Method"] = "MQTT";
                    dr["OTA Type"] = type;
                    dr["OTA Result"] = "PENDING";
                    parent.results.Rows.Add(dr);*/
                    listindex++;
                }
                //Write info to log
                if (!File.Exists(parent.TB_LogDir.Text + "\\" + "AUTOGEN_Payload_List_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
                {

                    // Verify directory exists, if not, throw exception
                    string curfilename = parent.TB_LogDir.Text + "\\" + "AUTOGEN_Payload_List_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                    try
                    {
                        using (StreamWriter sw = File.CreateText(curfilename))
                        {
                            string content = "";
                            listindex = 0;
                            sw.WriteLine("IP\tOTA_Payload\tNode\tType\tCycle_Payload\tName\tMAC\tCycle_Wait\tWait_Type");
                            /* foreach (IPData ipd in iplist)
                             {
                                 sw.WriteLine(ipd.IPAddress + "\t" +
                                              ipd.Payload + "\t" +
                                              ipd.Node + "\t" +
                                              ipd.Type + "\t" +
                                              ipd.MQTTPay);
                             }*/
                            for (int i = 0; i < (sources.Count / 2); i++)
                            {
                                for (int count = 0; count < TESTCASEMAX; count++)
                                {

                                    switch (count)
                                    {
                                        //Start of - 131812 - OTA : Generic : Forced Update : Unit in Idle State
                                        //Set OTA upgrade
                                        case 0:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131812 - OTA : Generic : Forced Update : Unit in Idle State - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 1:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131812 - OTA : Generic : Forced Update : Unit in Idle State - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131812 - OTA : Generic : Forced Update : Unit in Idle State

                                        //Start of - 131813 - OTA : Generic : Forced Update : Unit in Programming State
                                        //Set Programming state -TODOGET THIS WORKING
                                        /* case 2: 
                                             content = iplist[listindex].IPAddress + "\t" +
                                                       "No OTA Payload - Non-OTA Cycle" + "\t" + //Not a ota do not save
                                                       iplist[listindex].Node + "\t" +
                                                       "Cycle" + "\t" +
                                                       iplist[listindex].MQTTPay + "\t" +
                                                       "RQM 131813 - OTA : Generic : Forced Update : Unit in Programming State - Part 1" + "\t" +
                                                       iplist[listindex].MAC + "\t" +
                                                       "25" + "\t" +
                                                       "";
                                             break;

                                         //Set OTA upgrade
                                         case 3:
                                             content = iplist[listindex].IPAddress + "\t" +
                                                       iplist[listindex].Payload + "\t" +
                                                       iplist[listindex].Node + "\t" +
                                                       iplist[listindex].Type + "\t" +
                                                       "NA" + "\t" + //Not a cycle do not save
                                                       "RQM 131813 - OTA : Generic : Forced Update : Unit in Programming State - Part 2" + "\t" +
                                                       iplist[listindex].MAC + "\t" +
                                                       "0" + "\t" +
                                                       "";
                                             break;
                                         //Set OTA  downgrade (return to SOP)
                                         case 4:
                                             content = iplist[listindex+1].IPAddress + "\t" +
                                                       iplist[listindex+1].Payload + "\t" +
                                                       iplist[listindex+1].Node + "\t" +
                                                       iplist[listindex+1].Type + "\t" +
                                                       "NA" + "\t" + //Not a cycle do not save
                                                       "RQM 131813 - OTA : Generic : Forced Update : Unit in Programming State - Part 3" + "\t" +
                                                       iplist[listindex].MAC + "\t" +
                                                       "0" + "\t" +
                                                       "";
                                             break;*/
                                        //End of - 131813 - OTA : Generic : Forced Update : Unit in Programming State

                                        //Start of - 131814 - OTA : Generic : Forced Update : Unit in Running State
                                        case 5:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      "No OTA Payload - Non-OTA Cycle" + "\t" + //Not an ota do not save
                                                      iplist[listindex].Node + "\t" +
                                                      "Cycle" + "\t" +
                                                      iplist[listindex].MQTTPay + "\t" +
                                                      "RQM 131814 - OTA : Generic : Forced Update : Unit in Running State - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "25" + "\t" +
                                                      "";
                                            break;

                                        //Set OTA upgrade
                                        case 6:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131814 - OTA : Generic : Forced Update : Unit in Running State - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 7:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131814 - OTA : Generic : Forced Update : Unit in Running State  - Part 3" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131814 - OTA : Generic : Forced Update : Unit in Running State

                                        //Start of - 131815 - OTA : Generic : Forced Update : Unit in Post-Cycle Running State
                                        case 8:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      "No OTA Payload - Non-OTA Cycle" + "\t" + //Not an ota do not save
                                                      iplist[listindex].Node + "\t" +
                                                      "Cycle" + "\t" +
                                                      iplist[listindex].MQTTPay + "\t" +
                                                      "RQM 131815 - OTA : Generic : Forced Update : Unit in Post-Cycle Running State - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "70" + "\t" +
                                                      "";
                                            break;

                                        //Set OTA upgrade
                                        case 9:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131815 - OTA : Generic : Forced Update : Unit in Post-Cycle Running State - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 10:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131815 - OTA : Generic : Forced Update : Unit in Post-Cycle Running State  - Part 3" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131815 - OTA : Generic : Forced Update : Unit in Post-Cycle Running State


                                        //Start of - 131816 - OTA : Generic : Forced Update : Unit in Non-Running Complete State
                                        case 11:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      "No OTA Payload - Non-OTA Cycle" + "\t" + //Not an ota do not save
                                                      iplist[listindex].Node + "\t" +
                                                      "Cycle" + "\t" +
                                                      iplist[listindex].MQTTPay + "\t" +
                                                      "RQM 131816 - OTA : Generic : Forced Update : Unit in Non-Running Complete State - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "360" + "\t" +
                                                      "";
                                            break;

                                        //Set OTA upgrade
                                        case 12:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131816 - OTA : Generic : Forced Update : Unit in Non-Running Complete State - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 13:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131816 - OTA : Generic : Forced Update : Unit in Non-Running Complete State  - Part 3" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131815 - OTA : Generic : Forced Update : Unit in Non-Running Complete State


                                        //Start of - 131817 - OTA : Generic : Forced Update : Unit in Low Power Idle State
                                        case 14:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      "No OTA Payload - Wait Till Low Power Idle State" + "\t" + //Not an ota do not save
                                                      iplist[listindex].Node + "\t" +
                                                      "Wait" + "\t" +
                                                      "NA" + "\t" +
                                                      "RQM 131817 - OTA : Generic : Forced Update : Unit in Low Power Idle State - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "360" + "\t" +
                                                      "";
                                            break;

                                        //Set OTA upgrade
                                        case 15:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131817 - OTA : Generic : Forced Update : Unit in Low Power Idle State - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 16:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131817 - OTA : Generic : Forced Update : Unit in Low Power Idle State  - Part 3" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131817 - OTA : Generic : Forced Update : Unit in Low Power Idle State

                                        //Start of - 131821 - OTA : Generic : Forced Update : HMI Update
                                        //Set OTA upgrade
                                        case 17:
                                            if (!TB_HMI_UP.Text.Equals(""))
                                                content = iplist[listindex].IPAddress + "\t" +
                                                      TB_HMI_UP.Text + "\t" +
                                                      "HMI" + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131821 - OTA : Generic : Forced Update : HMI Update - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            else
                                                content = null;
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 18:
                                            if (!TB_HMI_DWN.Text.Equals(""))
                                                content = iplist[listindex + 1].IPAddress + "\t" +
                                                      TB_HMI_DWN.Text + "\t" +
                                                      "HMI" + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131821 - OTA : Generic : Forced Update : HMI Update - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            else
                                                content = null;
                                            break;
                                        //End of - 131821 - OTA : Generic : Forced Update : HMI Update

                                        //Start of - 131822 - OTA : Generic : Forced Update : ACU Update
                                        //Set OTA upgrade
                                        case 19:
                                            if (!TB_ACU_UP.Text.Equals(""))
                                                content = iplist[listindex].IPAddress + "\t" +
                                                      TB_ACU_UP.Text + "\t" +
                                                      "ACU" + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131822 - OTA : Generic : Forced Update : ACU Update - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            else
                                                content = null;
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 20:
                                            if (!TB_ACU_DWN.Text.Equals(""))
                                                content = iplist[listindex + 1].IPAddress + "\t" +
                                                      TB_ACU_DWN.Text + "\t" +
                                                      "ACU" + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131822 - OTA : Generic : Forced Update : ACU Update - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            else
                                                content = null;
                                            break;
                                        //End of - 131822 - OTA : Generic : Forced Update : ACU Update

                                        //Start of - 131835 - OTA : Downloading : Forced Update : User starts cycle from App
                                        //Set OTA upgrade
                                        case 21:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      iplist[listindex].MQTTPay + "\t" +
                                                      "RQM 131835 - OTA : Downloading : Forced Update : User Starts Cycle from App - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "1" + "\t" +
                                                      "Cycle";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 22:
                                            content = iplist[listindex+1].IPAddress + "\t" +
                                                      iplist[listindex+1].Payload + "\t" +
                                                      iplist[listindex+1].Node + "\t" +
                                                      iplist[listindex+1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131835 - OTA : Downloading : Forced Update : User Starts Cycle from App - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131835 - OTA : Downloading : Forced Update : User starts cycle from App

                                        //Start of - 131837 - OTA : Downloading : Forced Update : User Changes Settings from App
                                        //Set OTA upgrade
                                        case 23:
                                            content = iplist[listindex].IPAddress + "\t" +
                                                      iplist[listindex].Payload + "\t" +
                                                      iplist[listindex].Node + "\t" +
                                                      iplist[listindex].Type + "\t" +
                                                      "0021FF333301070001323031392D30312D30315431393A33323A34382B30303A303000" + "\t" +
                                                      "RQM 131837 - OTA : Downloading : Forced Update : User Changes Settings from App - Part 1" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "1" + "\t" +
                                                      "Setting";
                                            break;
                                        //Set OTA  downgrade (return to SOP)
                                        case 24:
                                            content = iplist[listindex + 1].IPAddress + "\t" +
                                                      iplist[listindex + 1].Payload + "\t" +
                                                      iplist[listindex + 1].Node + "\t" +
                                                      iplist[listindex + 1].Type + "\t" +
                                                      "NA" + "\t" + //Not a cycle do not save
                                                      "RQM 131837 - OTA : Downloading : Forced Update : User Changes Settings from App - Part 2" + "\t" +
                                                      iplist[listindex].MAC + "\t" +
                                                      "0" + "\t" +
                                                      "";
                                            break;
                                        //End of - 131837 - OTA : Downloading : Forced Update : User Changes Settings from App

                                        default:
                                            content = null;
                                            break;

                                    }

                                    if (content != null)
                                        sw.WriteLine(content);

                                }

                                //Bound checking to control max possible bundle sources is iterated max number of node times
                                if ((listindex / 2) + 1 < sources.Count / 2)
                                    listindex += 2;
                            }

                        }

                        MessageBox.Show("AutoGeneration has been completed for " + TB_IP.Text + " with the payload list saved at " + curfilename + ".", "AutoGenerate: Generation Complete.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        iplist.Clear();
                        sources.Clear();
                        types.Clear();

                    }
                    catch
                    {
                        MessageBox.Show("The chosen directory path does not exist. Please browse to a path that DOES exist and try again.", "Error: Directory Path Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }



            }
            catch
            {
            }
            //}
        }

        private void AutoGen_Load(object sender, EventArgs e)
        {
            //ResetForm(); //ENABLE WHEN UPLOADING TO STORE
        }
    }
}
