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
        private BindingSource sbind = new BindingSource();
        //private BindingSource sbind = new BindingSource();
        string up = "";
        string dwn = "";
        string node = "";

        public AutoGen(Venom ParentForm, WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            parent = ParentForm;
            CB_Variant.Items.AddRange(new object[] {"NAR Cooking",
                                                    "EMEA Cooking", "NAR Laundry", "Other (any remote cycle)"});
            // Generate tables
            sbind.DataSource = parent.results;
            parent.DGV_Data.AutoGenerateColumns = true;
            parent.DGV_Data.DataSource = parent.results;
            parent.DGV_Data.DataSource = sbind;

            // Do not allow columns to be sorted
            foreach (DataGridViewColumn column in parent.DGV_Data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void ResetForm(string type)
        {
            if (type == "all")
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

                CB_HMI.Checked = false;
                CB_ACU.Checked = false;
                CB_WIFI.Checked = false;
                CB_EXP.Checked = false;

                CB_HMI.Enabled = true;
                CB_ACU.Enabled = true;
                CB_WIFI.Enabled = true;
                CB_EXP.Enabled = true;

                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;

                TB_HMI_DWN.Enabled = false;
                TB_HMI_UP.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_ACU_DWN.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_EXP_DWN.Enabled = false;
                TB_EXP_UP.Enabled = false;
                TB_WIFI_DWN.Enabled = false;
                TB_WIFI_UP.Enabled = false;

                TB_HMI_DWN.Visible = false;
                TB_HMI_UP.Visible = false;
                TB_ACU_DWN.Visible = false;
                TB_ACU_UP.Visible = false;
                TB_EXP_DWN.Visible = false;
                TB_EXP_UP.Visible = false;
                TB_WIFI_DWN.Visible = false;
                TB_WIFI_UP.Visible = false;
            }
            if (type == "hmi")
            {
                TB_ACU_DWN.Text = "";
                TB_ACU_UP.Text = "";
                TB_EXP_DWN.Text = "";
                TB_EXP_UP.Text = "";
                TB_WIFI_DWN.Text = "";
                TB_WIFI_UP.Text = "";

                TB_HMI_DWN.Enabled = true;
                TB_HMI_UP.Enabled = true;
                TB_ACU_UP.Enabled = false;
                TB_ACU_DWN.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_EXP_DWN.Enabled = false;
                TB_EXP_UP.Enabled = false;
                TB_WIFI_DWN.Enabled = false;
                TB_WIFI_UP.Enabled = false;

                TB_HMI_DWN.Visible = true;
                TB_HMI_UP.Visible = true;
                TB_ACU_DWN.Visible = false;
                TB_ACU_UP.Visible = false;
                TB_EXP_DWN.Visible = false;
                TB_EXP_UP.Visible = false;
                TB_WIFI_DWN.Visible = false;
                TB_WIFI_UP.Visible = false;

                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;

                CB_ACU.Checked = false;
                CB_WIFI.Checked = false;
                CB_EXP.Checked = false;
            }
            if (type == "acu")
            {
                TB_HMI_DWN.Text = "";
                TB_HMI_UP.Text = "";
                TB_EXP_DWN.Text = "";
                TB_EXP_UP.Text = "";
                TB_WIFI_DWN.Text = "";
                TB_WIFI_UP.Text = "";

                TB_HMI_DWN.Enabled = false;
                TB_HMI_UP.Enabled = false;
                TB_ACU_UP.Enabled = true;
                TB_ACU_DWN.Enabled = true;
                TB_EXP_DWN.Enabled = false;
                TB_EXP_UP.Enabled = false;
                TB_WIFI_DWN.Enabled = false;
                TB_WIFI_UP.Enabled = false;

                TB_HMI_DWN.Visible = false;
                TB_HMI_UP.Visible = false;
                TB_ACU_DWN.Visible = true;
                TB_ACU_UP.Visible = true;
                TB_EXP_DWN.Visible = false;
                TB_EXP_UP.Visible = false;
                TB_WIFI_DWN.Visible = false;
                TB_WIFI_UP.Visible = false;

                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;

                CB_HMI.Checked = false;
                CB_WIFI.Checked = false;
                CB_EXP.Checked = false;
            }
            if (type == "wifi")
            {
                TB_ACU_DWN.Text = "";
                TB_ACU_UP.Text = "";
                TB_EXP_DWN.Text = "";
                TB_EXP_UP.Text = "";
                TB_HMI_DWN.Text = "";
                TB_HMI_UP.Text = "";

                TB_HMI_DWN.Enabled = false;
                TB_HMI_UP.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_ACU_DWN.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_EXP_DWN.Enabled = false;
                TB_EXP_UP.Enabled = false;
                TB_WIFI_DWN.Enabled = true;
                TB_WIFI_UP.Enabled = true;

                TB_HMI_DWN.Visible = false;
                TB_HMI_UP.Visible = false;
                TB_ACU_DWN.Visible = false;
                TB_ACU_UP.Visible = false;
                TB_EXP_DWN.Visible = false;
                TB_EXP_UP.Visible = false;
                TB_WIFI_DWN.Visible = true;
                TB_WIFI_UP.Visible = true;

                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = false;
                label13.Visible = false;

                CB_ACU.Checked = false;
                CB_HMI.Checked = false;
                CB_EXP.Checked = false;
            }
            if (type == "exp")
            {
                TB_ACU_DWN.Text = "";
                TB_ACU_UP.Text = "";
                TB_HMI_DWN.Text = "";
                TB_HMI_UP.Text = "";
                TB_WIFI_DWN.Text = "";
                TB_WIFI_UP.Text = "";

                TB_HMI_DWN.Enabled = false;
                TB_HMI_UP.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_ACU_DWN.Enabled = false;
                TB_ACU_UP.Enabled = false;
                TB_EXP_DWN.Enabled = true;
                TB_EXP_UP.Enabled = true;
                TB_WIFI_DWN.Enabled = false;
                TB_WIFI_UP.Enabled = false;

                TB_HMI_DWN.Visible = false;
                TB_HMI_UP.Visible = false;
                TB_ACU_DWN.Visible = false;
                TB_ACU_UP.Visible = false;
                TB_EXP_DWN.Visible = true;
                TB_EXP_UP.Visible = true;
                TB_WIFI_DWN.Visible = false;
                TB_WIFI_UP.Visible = false;

                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = true;
                label13.Visible = true;

                CB_ACU.Checked = false;
                CB_WIFI.Checked = false;
                CB_HMI.Checked = false;
            }

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
        private void ClearAll()
        {
            parent.results.Clear();
            parent.iplist.Clear();
            parent.LB_IPs.Items.Clear();
            parent.DGV_Data.Refresh();
        }
        private bool CheckFill()
        {
            bool check = false;

            //Check to make sure we have both upgrade and downgrade bundles and set type if we do
            if (!TB_HMI_UP.Text.Equals(""))
            {
                if (!TB_HMI_DWN.Text.Equals(""))
                {
                    check = true;
                    up = TB_HMI_UP.Text;
                }
                else
                    check = false;
            }

            if (!TB_HMI_DWN.Text.Equals(""))
            {
                if (!TB_HMI_UP.Text.Equals(""))
                {
                    check = true;
                    dwn = TB_HMI_DWN.Text;
                    node = "HMI";
                }
                else
                    check = false;
            }

            if (!TB_ACU_UP.Text.Equals(""))
            {
                if (!TB_ACU_DWN.Text.Equals(""))
                {
                    check = true;
                    up = TB_ACU_UP.Text;
                }
                else
                    check = false;
            }

            if (!TB_ACU_DWN.Text.Equals(""))
            {
                if (!TB_ACU_UP.Text.Equals(""))
                {
                    check = true;
                    dwn = TB_ACU_DWN.Text;
                    node = "ACU";
                }
                else
                    check = false;
            }

            if (!TB_WIFI_UP.Text.Equals(""))
            {
                if (!TB_WIFI_DWN.Text.Equals(""))
                {
                    check = true;
                    up = TB_WIFI_UP.Text;
                }
                else
                    check = false;
            }

            if (!TB_WIFI_DWN.Text.Equals(""))
            {
                if (!TB_WIFI_UP.Text.Equals(""))
                {
                    check = true;
                    dwn = TB_WIFI_DWN.Text;
                    node = "WIFI";
                }
                else
                    check = false;
            }

            if (!TB_EXP_UP.Text.Equals(""))
            {
                if (!TB_EXP_DWN.Text.Equals(""))
                {
                    check = true;
                    up = TB_EXP_UP.Text;
                }
                else
                    check = false;
            }

            if (!TB_EXP_DWN.Text.Equals(""))
            {
                if (!TB_EXP_UP.Text.Equals(""))
                {
                    check = true;
                    dwn = TB_EXP_DWN.Text;
                    node = "EXP";
                }
                else
                    check = false;
            }

            return check;
        }
        private void BuildList(ConnectedApplianceInfo cai, string name, string mqttpay)
        {
            IPData newip = new IPData(cai.IPAddress, up);
            newip.MAC = cai.MacAddress;
            newip.Node = node;
            newip.MQTTPay = mqttpay;
            newip.TabIndex = parent.AUTOINDEX;
            newip.Name = name;
            newip.Down = dwn;

            if (name.Contains("Downgrade"))
                newip.Type = "DOWNGRADE";
            else
                newip.Type = "UPGRADE";

            if (parent.LB_IPs.Items.Count == 0)
            {
                if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                    parent.LB_IPs.Items.Add(cai.IPAddress);

                parent.iplist.Add(newip);
            }


            //Update window for added IP
            DataRow dr = parent.results.NewRow();

            dr["IP Address"] = newip.IPAddress;
            dr["OTA Payload"] = newip.Payload;
            dr["Delivery Method"] = "MQTT";
            dr["OTA Type"] = newip.Type;
            dr["Node"] = newip.Node;
            dr["Name"] = newip.Name;
            dr["OTA Result"] = "PENDING";
            parent.results.Rows.Add(dr);
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

            if (!CheckFill())
            {
                DialogResult checkresult = MessageBox.Show("You have NOT provided both an upgrade and downgrade OTA version. " +
                                                            "Please check each text box and retry again.", "Error: Missing an Upgrade or Downgrade Payload",
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string mqttpay = "";
            //Set MQTT payload to start standard bake 350 for whatever product or allow user to input one            

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
                ClearAll();
                for (int i = 0; i < parent.TESTCASEMAX; i++)
                {
                    switch (i)
                    {
                        case 0:
                            BuildList(cai, "RQM 131812 OTA : Generic : Forced Update : Unit in Idle State", mqttpay);
                            break;
                        case 1:
                            BuildList(cai, "RQM 154635 OTA : Generic : Forced Update : Downgrade : Unit in Idle State", mqttpay);
                            break;
                        case 4:
                            BuildList(cai, "RQM 131835 OTA : Downloading : User Starts Cycle from App", mqttpay);
                            break;
                        case 5:
                            BuildList(cai, "RQM 131837 OTA : Downloading : User Changes Settings from App", mqttpay);
                            break;
                        case 6:
                            BuildList(cai, "RQM 131839 OTA : Programming & Progress Status : User Starts Cycle from App", mqttpay);
                            break;
                        case 8:
                            BuildList(cai, "RQM 131841 OTA : Programming & Progress Status : User Changes Settings from App", mqttpay);
                            break;
                        case 9:
                            BuildList(cai, "RQM 131844 OTA : Generic : Forced Update : Upgrade : Model/Serial Consistency Check", mqttpay);
                            break;
                        case 10:
                            BuildList(cai, "RQM 131845 OTA : Generic : Forced Update : Upgrade : Version Number Check", mqttpay);
                            break;
                        case 11:
                            BuildList(cai, "RQM 131846 OTA : Generic : Forced Update : Downgrade : Model/Serial Consistency Check", mqttpay);
                            break;
                        case 12:
                            BuildList(cai, "RQM 131847 OTA : Generic : Forced Update : Downgrade : Version Number Check", mqttpay);
                            break;
                        case 13:
                            BuildList(cai, "RQM 131849 OTA : Generic : Forced Update : Upgrade : CCURI Check", mqttpay);
                            break;
                        case 14:
                            BuildList(cai, "RQM 131850 OTA : Generic : Forced Update : Downgrade : CCURI Check", mqttpay);
                            break;
                        case 15:
                            BuildList(cai, "RQM 131851 OTA : Post Update : Check Provision State", mqttpay);
                            break;
                        case 16:
                            BuildList(cai, "RQM 131852 OTA : Post Update : Check Claimed Status", mqttpay);
                            break;
                        case 17:
                            BuildList(cai, "RQM 131854 OTA : Preconditions : Incorrect CRC", mqttpay);
                            break;
                        case 18:
                            BuildList(cai, "RQM 131862 OTA : Generic : Forced OTA Payload Sent Multiple Times", mqttpay);
                            break;
                        case 19:
                            BuildList(cai, "RQM 131863 OTA : Downloading : RSSI Strong Signal", mqttpay);
                            break;
                        case 20:
                            BuildList(cai, "RQM 131865 OTA : Preconditions : Invalid URL", mqttpay);
                            break;
                        case 21:
                            BuildList(cai, "RQM 154667 OTA : Generic : Forced Update : ISPPartNumber check", mqttpay);
                            break;
                        case 22:
                            BuildList(cai, "RQM 132552 OTA : Downloading : Download Times Out After 5 Attempts", mqttpay);
                            break;
                        case 23:
                            BuildList(cai, "RQM 132837 OTA : Preconditions : CRC Validation", mqttpay);
                            break;
                        case 24:
                            BuildList(cai, "RQM 133399 OTA : Post Update : Connection Reset", mqttpay);
                            break;
                        case 25:
                            if (node == "HMI")
                                BuildList(cai, "RQM 131821 OTA : Generic : Forced Update : HMI Update", mqttpay);
                            break;
                        case 26:
                            if (node == "WIFI")
                                BuildList(cai, "RQM 132549 OTA : Generic : Forced Update : Wifi Radio", mqttpay);
                            break;
                        case 27:
                            if (node == "EXP")
                                BuildList(cai, "RQM 132550 OTA : Generic : Forced Update : All Updatable Modules Updated", mqttpay);
                            break;
                        case 28:
                            if (node == "ACU")
                                BuildList(cai, "RQM 131822 OTA : Generic : Forced Update : ACU Update", mqttpay);
                            break;

                        default:
                            break;
                    }
                }
                parent.DGV_Data.Refresh();
                parent.autogen = true;
                parent.BTN_MakeList.Enabled = false;
                parent.BTN_Import.Enabled = false;
                parent.SizeCol();
            }
            catch
            {
            }
        }
        private void AutoGen_Load(object sender, EventArgs e)
        {
            //ResetForm("all"); //ENABLE WHEN UPLOADING TO STORE
        }
        private void CB_HMI_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_HMI.Checked)
                ResetForm("hmi");
        }
        private void CB_ACU_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_ACU.Checked)
                ResetForm("acu");
        }
        private void CB_WIFI_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_WIFI.Checked)
                ResetForm("wifi");
        }
        private void CB_EXP_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EXP.Checked)
                ResetForm("exp");
        }

    }
}