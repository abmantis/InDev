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
            CB_Product.Items.AddRange(new object[] {"NAR Cooking",
                                                    "EMEA Cooking", "NAR Laundry", "Other (any remote cycle)"});
            CB_Variant.Items.AddRange(new object[] {"HMI","ACU", "WiFi", "Expansion"});
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

        private void ResetForm()
        {
            TB_DWN.Text = "";
            TB_UP.Text = "";
            TB_IP.Text = "";
            TB_Other.Text = "";
            CB_Product.ResetText();
            CB_Variant.ResetText();
            CB_NoCyc.Checked = false;
            CB_NoTTF.Checked = false;
            CB_NoGen.Checked = false;
            CB_Save.Checked = false;
        }
        public void CB_Variant_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CB_Product.Text.Equals("Other (any remote cycle)"))
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
            bool check = true;
            string var = CB_Variant.Text;
            string prod = CB_Product.Text;
            //Check to make sure we have both upgrade and downgrade bundles and set type if we do
            if (!TB_UP.Text.Equals(""))
            {
                if (!TB_DWN.Text.Equals(""))
                    up = TB_UP.Text;
                
                else
                    check = false;
            }
            else
                check = false;

            if (!TB_DWN.Text.Equals(""))
            {
                if (!TB_UP.Text.Equals(""))
                    dwn = TB_DWN.Text;
                
                else
                    check = false;
            }
            else
                check = false;

            if (string.IsNullOrEmpty(var))
                check = false;
            else
                node = CB_Variant.Text;

            if (string.IsNullOrEmpty(prod))
                check = false;
            if (prod.Contains("Other"))
            {
                if (TB_Other.Text.Equals("") || TB_Other.Text.Contains("Paste"))
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
            string pay;
            if (name.Contains("Downgrade"))
            {
                pay = dwn;
                newip.Type = "DOWNGRADE";
            }
            else
            {
                pay = up;
                newip.Type = "UPGRADE";
            }
            if (parent.LB_IPs.Items.Count == 0)
            {
                if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                    parent.LB_IPs.Items.Add(cai.IPAddress);

                if (CB_Product.Text.Contains("Cooking") || CB_Product.Text.Contains("Other"))
                {
                    newip.Set = "0008FF33330203000A";
                    newip.Prod = "Cooking";
                    newip.Cncl = "0008FF3333030F000201";
                }

                else
                {
                    newip.Set = "0008FF333302020009";
                    newip.Cncl = "0008FF33330307000101";
                    newip.Prod = "Laundry";
                }

                parent.iplist.Add(newip);
            }

            //Update window for added IP
            DataRow dr = parent.results.NewRow();

            dr["IP Address"] = newip.IPAddress;
            dr["OTA Payload"] = pay;
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

            if (CB_Product.Text.Equals("NAR Cooking"))
                mqttpay = "001BFF33330310000C02030D00010000005A0310000106E6030F000202"; // Standard bake 350 for upper oven for 1.5 minute;
                //mqttpay = "001BFF33330310000C02030D00010000003C0310000106E6030F000202"; // Standard bake 350 for upper oven for 1 minute;
            else if (CB_Product.Text.Equals("EMEA Cooking"))
                mqttpay = "001BFF33330B02001B0104090001028F04060001000000780408000202"; // Standard bake for Speed Oven (MWO bake instead of upper oven)          
            else if (CB_Product.Text.Equals("NAR Laundry"))
                mqttpay = "0026FF333305050006010505001503050500180305050014000505000A010505000D000307000102"; // Standard wash cycle for Janus washer (wash cavity)
            else
                mqttpay = CB_Product.Text;
            //
            //See if anything is skipped
            if (CB_NoCyc.Checked)
                parent.skipcyc = true;
            if (CB_NoTTF.Checked)
                parent.skipttf = true;
            if (CB_NoGen.Checked)
                parent.skipgen = true;


            try
            {
                ClearAll();
                parent.glblip = TB_IP.Text;
                for (int i = 0; i < parent.TESTCASEMAX; i++)
                {
                    switch (i)
                    {
                        case 0:
                            BuildList(cai, "RQM 131835 OTA : Remote : User Starts Cycle from App", mqttpay);
                            break;
                        case 1:
                            BuildList(cai, "RQM 131837 OTA : Remote : User Changes Settings from App", mqttpay);
                            break;
                        case 2:
                            BuildList(cai, "RQM 131839 OTA : Remote : User Starts Cycle from App", mqttpay);
                            break;
                        case 3:
                            BuildList(cai, "RQM 131841 OTA : Remote : User Changes Settings from App", mqttpay);
                            break;
                        case 4:
                            BuildList(cai, "RQM 131812 OTA : Generic : Forced Update : Unit in Idle State", mqttpay);
                            break;
                        case 5:
                            BuildList(cai, "RQM 154635 OTA : Generic : Forced Update : Downgrade : Unit in Idle State", mqttpay);
                            break;
                        case 6:
                            BuildList(cai, "RQM 131844 OTA : Generic : Forced Update : Upgrade : Model/Serial Consistency Check", mqttpay);
                            break;
                        case 7:
                            BuildList(cai, "RQM 131845 OTA : Generic : Forced Update : Upgrade : Version Number Check", mqttpay);
                            break;
                        case 8:
                            BuildList(cai, "RQM 131846 OTA : Generic : Forced Update : Downgrade : Model/Serial Consistency Check", mqttpay);
                            break;
                        case 9:
                            BuildList(cai, "RQM 131847 OTA : Generic : Forced Update : Downgrade : Version Number Check", mqttpay);
                            break;
                        case 10:
                            BuildList(cai, "RQM 131849 OTA : Generic : Forced Update : Upgrade : CCURI Check", mqttpay);
                            break;
                        case 11:
                            BuildList(cai, "RQM 131850 OTA : Generic : Forced Update : Downgrade : CCURI Check", mqttpay);
                            break;
                        case 12:
                            BuildList(cai, "RQM 131851 OTA : Generic : Check Provision State", mqttpay);
                            break;
                        case 13:
                            BuildList(cai, "RQM 131852 OTA : Generic : Check Claimed Status", mqttpay);
                            break;
                        case 14:
                            BuildList(cai, "RQM 186300 OTA : Generic : Consumer is informed of the update status on app", mqttpay);
                            break;
                        case 15:
                            if (node == "HMI")
                                BuildList(cai, "RQM 131821 OTA : Generic : Forced Update : HMI Update", mqttpay);
                            if (node == "WiFi")
                                BuildList(cai, "RQM 132549 OTA : Generic : Forced Update : Wifi Radio", mqttpay);
                            if (node == "Expansion")
                                BuildList(cai, "RQM 132550 OTA : Generic : Forced Update : All Updatable Modules Updated", mqttpay);
                            if (node == "ACU")
                                BuildList(cai, "RQM 131822 OTA : Generic : Forced Update : ACU Update", mqttpay);
                            break;
                        case 16:
                            BuildList(cai, "RQM 131863 OTA : Generic : RSSI Strong Signal", mqttpay);
                            break;
                        case 17:
                            BuildList(cai, "RQM 186529 OTA : Generic : Post Condition : After OTA is successful OTAs are still possible (Appliances are able to receive and apply OTAs)", mqttpay);
                            break;
                        case 18:
                            BuildList(cai, "RQM 154667 OTA : Generic : Forced Update : ISPPartNumber check", mqttpay);
                            break;
                        case 19:
                            BuildList(cai, "RQM 132552 OTA : TTF : Download Times Out After 5 Attempts", mqttpay);
                            break;
                        case 20:
                            BuildList(cai, "RQM 131865 OTA : TTF : Invalid URL", mqttpay);
                            break;
                        case 21:
                            BuildList(cai, "RQM 131854 OTA : TTF : Incorrect CRC", mqttpay); 
                            break;
                        case 22:
                            BuildList(cai, "RQM 131862 OTA : TTF : Forced OTA Payload Sent Multiple Times", mqttpay);
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
            if (!CB_Save.Checked)
                ResetForm(); //ENABLE WHEN UPLOADING TO STORE

            CB_NoCyc.Checked = false;
            CB_NoTTF.Checked = false;
            CB_NoGen.Checked = false;
        }

        private void BTN_Clr_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
    }
}