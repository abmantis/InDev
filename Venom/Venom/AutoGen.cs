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
                                                    "EMEA Cooking", "NAR Laundry", "EMEA Laundry","Other (any remote cycle)"});
            CB_Variant.Items.AddRange(new object[] {"HMI","ACU", "WiFi", "Expansion"});
            CB_Type.Items.AddRange(new object[] { "Indigo", "Gen4" });

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
            CB_Type.ResetText();
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
        /*private void ClearAll()
        {
            parent.results.Clear();
            parent.iplist.Clear();
            parent.LB_IPs.Items.Clear();
            parent.DGV_Data.Refresh();
        }*/
        private bool CheckFill()
        {
            bool check = true;
            string var = CB_Variant.Text;
            string prod = CB_Product.Text;
            string wtype = CB_Type.Text;

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

            if (string.IsNullOrEmpty(wtype))
                check = false;

            if (prod.Contains("Other"))
            {
                if (TB_Other.Text.Equals("") || TB_Other.Text.Contains("Paste"))
                    check = false;
            }

            return check;
        }
        private void BuildList(ConnectedApplianceInfo cai, string name, string mqttpay, int i)
        {
            IPData newip = new IPData(cai.IPAddress, up);
            newip.MAC = cai.MacAddress;
            newip.Node = node;
            newip.MQTTPay = mqttpay;
            newip.TabIndex = parent.AUTOINDEX;
            newip.Name = name;
            newip.Down = dwn;
            newip.Result = "PENDING";


            string wtype = CB_Type.Text;
            if (wtype.Contains("Indigo"))
                parent.indigo = true;
            else
                parent.tourma = true;

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
                    newip.Prod = "Cooking";

                    if (parent.indigo)
                    {
                        //00.08.FF.33.33.03.0F.00.02.01
                        newip.Cncl = "0008FF3333030F000201";
                        newip.Set = "0008FF33330203000A0A";
                    }
                    
                    else
                    {
                        if (CB_Product.Text.Contains("EMEA"))
                        {
                            newip.Cncl = "0008FF33330408000201";
                            newip.Set = "0008FF33330203000A0A";
                        }
                        else
                        {
                            //00.08.FF.33.33.03.0F.00.02.01
                            newip.Cncl = "0008FF3333030F000201";
                            //00.08.FF.33.33.02.03.00.0A.0A
                            newip.Set = "0008FF33330203000A0A";
                        }
                    }
                }

                else
                {
                    newip.Set = "0008FF333302020009";
                    newip.Cncl = "0008FF33330307000101";
                    if (CB_Product.Text.Contains("EMEA Laundry"))
                        newip.Prod = "EMEA Laundry";
                    else
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
            dr["OTA Result"] = newip.Result;
            parent.results.Rows.Add(dr);

            if (parent.PendCheck(i))
            {
                newip.Result = "Skipped by user.";

                parent.results.Rows[i]["OTA Result"] = newip.Result;

                parent.DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
            }

            if (parent.tourma && (i == 0 || i == 1))
            {
                newip.Result = "Test case skipped when using Gen4.";

                parent.results.Rows[i]["OTA Result"] = newip.Result;

                parent.DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
            }

            if (parent.indigo && (i == 0 || i == 1) && CB_Product.Text.Contains("EMEA Laundry"))
            {
                newip.Result = "Test case skipped when using EMEA products.";

                parent.results.Rows[i]["OTA Result"] = newip.Result;

                parent.DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
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

            if (!CheckFill())
            {
                DialogResult checkresult = MessageBox.Show("You have NOT filled out all required text boxes and drop downs. " +
                                                            "Please check each text box and drop down and retry again.", "Error: Form Improperly Filled Out",
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (parent.LB_IPs.Items.Count != 0)
                parent.ResetForm(true);

            string mqttpay = "";
            //Set MQTT payload to start standard bake 350 for whatever product or allow user to input one            

            if (CB_Product.Text.Equals("NAR Cooking"))
                mqttpay = "001BFF33330310000C02030D00010000005A0310000106E6030F000202"; // Standard bake 350 for upper oven for 1.5 minute;
            else if (CB_Product.Text.Equals("EMEA Cooking"))
                mqttpay = "001BFF33330B02001B0104090001028F04060001000000780408000202"; // Standard bake for Speed Oven (MWO bake instead of upper oven)          
            else if (CB_Product.Text.Equals("NAR Laundry"))
                mqttpay = "0026FF333305050006010505001503050500180305050014000505000A010505000D000307000102"; // Standard wash cycle for Janus washer (wash cavity)
            else
                mqttpay = CB_Product.Text;

            string allskip = "";

            //See if anything is skipped
            if (CB_NoCyc.Checked)
            {
                allskip += "Remote cycle ";
                parent.skipcyc = true;
            }
            if (CB_NoTTF.Checked)
            {
                if (!String.IsNullOrEmpty(allskip))
                    allskip += "and ";
                allskip += "Tests to Fail ";
                parent.skipttf = true;
            }
            if (CB_NoGen.Checked)
            {
                if (!String.IsNullOrEmpty(allskip))
                    allskip += "and ";
                allskip += "Generic ";
                parent.skipgen = true;
            }

            if (!String.IsNullOrEmpty(allskip))
            {
                DialogResult dialogResult = MessageBox.Show("Please verify skipping " + allskip + "test case(s)." + '\n' +
                                                            "Press Yes to Confirm or No to Cancel.", "Verify Skip and Auto Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
                if (dialogResult != DialogResult.Yes)
                    return;
            }
            try
            {
                //ClearAll();
                parent.glblip = TB_IP.Text;
                for (int i = 0; i < parent.TESTCASEMAX; i++)
                {
                    switch (i)
                    {
                        case 0:
                            BuildList(cai, "RQM 131835 OTA : Remote : User Starts Cycle from App in Download", mqttpay, i);
                            break;
                        case 1:
                            BuildList(cai, "RQM 131837 OTA : Remote : User Changes Settings from App in Download", mqttpay, i);
                            break;
                        case 2:
                            BuildList(cai, "RQM 131839 OTA : Remote : User Starts Cycle from App in IAP", mqttpay, i);
                            break;
                        case 3:
                            BuildList(cai, "RQM 131841 OTA : Remote : User Changes Settings from App in IAP", mqttpay, i);
                            break;
                        case 4:
                            BuildList(cai, "RQM 131812 OTA : Generic : Forced Update : Unit in Idle State", mqttpay, i);
                            break;
                        case 5:
                            BuildList(cai, "RQM 154635 OTA : Generic : Forced Update : Downgrade : Unit in Idle State", mqttpay, i);
                            break;
                        case 6:
                            BuildList(cai, "RQM 131844 OTA : Generic : Forced Update : Upgrade : Model/Serial Consistency Check", mqttpay, i);
                            break;
                        case 7:
                            BuildList(cai, "RQM 131845 OTA : Generic : Forced Update : Upgrade : Version Number Check", mqttpay, i);
                            break;
                        case 8:
                            BuildList(cai, "RQM 131846 OTA : Generic : Forced Update : Downgrade : Model/Serial Consistency Check", mqttpay, i);
                            break;
                        case 9:
                            BuildList(cai, "RQM 131847 OTA : Generic : Forced Update : Downgrade : Version Number Check", mqttpay, i);
                            break;
                        case 10:
                            BuildList(cai, "RQM 131849 OTA : Generic : Forced Update : Upgrade : CCURI Check", mqttpay, i);
                            break;
                        case 11:
                            BuildList(cai, "RQM 131850 OTA : Generic : Forced Update : Downgrade : CCURI Check", mqttpay, i);
                            break;
                        case 12:
                            BuildList(cai, "RQM 131851 OTA : Generic : Check Provision State", mqttpay, i);
                            break;
                        case 13:
                            BuildList(cai, "RQM 131852 OTA : Generic : Check Claimed Status", mqttpay, i);
                            break;
                        case 14:
                            BuildList(cai, "RQM 186300 OTA : Generic : Consumer is informed of the update status on app", mqttpay, i);
                            break;
                        case 15:
                            if (node == "HMI")
                                BuildList(cai, "RQM 131821 OTA : Generic : Forced Update : HMI Update", mqttpay, i);
                            if (node == "WiFi")
                                BuildList(cai, "RQM 132549 OTA : Generic : Forced Update : Wifi Radio", mqttpay, i);
                            if (node == "Expansion")
                                BuildList(cai, "RQM 132550 OTA : Generic : Forced Update : All Updatable Modules Updated", mqttpay, i);
                            if (node == "ACU")
                                BuildList(cai, "RQM 131822 OTA : Generic : Forced Update : ACU Update", mqttpay, i);
                            break;
                        case 16:
                            BuildList(cai, "RQM 131863 OTA : Generic : RSSI Strong Signal", mqttpay, i);
                            break;
                        case 17:
                            BuildList(cai, "RQM 186529 OTA : Generic : Post Condition : After OTA is successful OTAs are still possible (Appliances are able to receive and apply OTAs)", mqttpay, i);
                            break;
                        case 18:
                            BuildList(cai, "RQM 154667 OTA : Generic : Forced Update : ISPPartNumber check", mqttpay, i);
                            break;
                        case 19:
                            BuildList(cai, "RQM 132552 OTA : TTF : Download Times Out After 5 Attempts", mqttpay, i);
                            break;
                        case 20:
                            BuildList(cai, "RQM 131865 OTA : TTF : Invalid URL", mqttpay, i);
                            break;
                        case 21:
                            BuildList(cai, "RQM 131854 OTA : TTF : Incorrect CRC", mqttpay, i); 
                            break;
                        case 22:
                            BuildList(cai, "RQM 131862 OTA : TTF : Forced OTA Payload Sent Multiple Times", mqttpay, i);
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
            /*catch
            {
            }*/

            catch (Exception f)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom BTN_Gen():  Message and Stacktrace were ");
                LogException(f, true);
                return;
            }
        }
        private void AutoGen_Load(object sender, EventArgs e)
        {
            if (!CB_Save.Checked)
                ResetForm();
        }
        private void BTN_Clr_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will clear the entire AutoGen form. Please confirm you want to clear the entire form.", "Verify Clearing form", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;
            ResetForm();
        }
    }
}