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
        private BindingSource sbind = new BindingSource();

        public AutoGen(Venom ParentForm, WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            parent = ParentForm;
            CB_Variant.Items.AddRange(new object[] {"NAR Cooking",
                                                    "EMEA Cooking", "NAR Laundry", "Other (any remote cycle)"});            
        }
        public void CB_Variant_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CB_Variant.Text.Equals("Other (any remote cycle)"))
            {
                TB_Other.Enabled = true;
                TB_Other.Visible = true;
                TB_Other.Text = "Paste MQTT Payload Here";
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
            DialogResult dialogResult = MessageBox.Show("This will automatically create a new test plan run from the current IP. " +
                "This will then clear the current payload list and update the table accordingly. " +
                "Press Yes to Create or No to Cancel.", "Verify Full Clear and Auto Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (dialogResult == DialogResult.Yes)
            {
                bool check = false;
                List<string> types = new List<string>();
                List<string> sources = new List<string>();
                //Check to make sure we have both upgrade and downgrade bundles and set type if we do
                if (!TB_HMI_UP.Text.Equals(""))
                {
                    if (!TB_HMI_DWN.Text.Equals(""))
                    {
                        check = true;
                        types.Add("HMI");
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
                        types.Add("HMI");
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
                        types.Add("ACU");
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
                        types.Add("ACU");
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
                        types.Add("Wifi");
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
                        types.Add("Wifi");
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
                        types.Add("Expansion");
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
                        types.Add("Expansion");
                        sources.Add(TB_EXP_DWN.Text);
                    }
                    else
                        check = false;
                }

                if (!check)
                {
                    DialogResult checkresult = MessageBox.Show("You have NOT provided both an upgrade and downgrade OTA version. " +
                                                                "Please check each text box and retry again.","Error: Missing an Upgrade or Downgrade Payload",
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                    types.Clear();
                    return;
                }

                parent.results.Clear();
                parent.responses.Clear();
                parent.iplist.Clear();
                parent.LB_IPs.Items.Clear();
                parent.DGV_Data.Refresh();
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

                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);

                try
                {
                    if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                        parent.LB_IPs.Items.Add(cai.IPAddress);

                    int count = 0;
                    foreach (String type in types)
                    {
                        IPData newip = new IPData(cai.IPAddress, sources[count]);
                        parent.iplist.Add(newip);
                        newip.MAC = cai.MacAddress;
                        newip.Type = CB_Variant.Text + " " + type;
                        newip.Cycle = mqttpay;

                        // Update window for added IP
                        DataRow dr = parent.results.NewRow();

                        dr["IP Address"] = newip.IPAddress;
                        dr["OTA Payload"] = sources[count];
                        dr["Delivery Method"] = "MQTT";
                        dr["OTA Type"] = type;
                        dr["OTA Result"] = "PENDING";
                        parent.results.Rows.Add(dr);
                        count++;
                    }
                    MessageBox.Show("AutoGeneration has been completed for " + TB_IP.Text + ".", "AutoGenerate: Generation Complete.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                }
            }
        }

       
    }
}
