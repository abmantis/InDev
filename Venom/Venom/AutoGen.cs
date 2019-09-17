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
        }

        private void BTN_Gen_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will automatically create a new test plan run from the current IP. " +
                "This will then clear the current payload list and update the table accordingly. " +
                "Press Yes to Create or No to Cancel.", "Verify Full Clear and Auto Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                parent.results.Clear();
                parent.responses.Clear();
                parent.iplist.Clear();
                parent.LB_IPs.Items.Clear();
                parent.DGV_Data.Refresh();
                //plist.DGV_Data.Refresh();

                //Parse OTA payload into byte array for sending via MQTT
                string localpay = "";

                if (RB_NAR_C.Checked)
                    localpay = "001BFF33330310000C02030D00010000003C0310000106E6030F000202"; // Standard bake 350 for upper oven for 1 minute;
                if (RB_EMEA_C.Checked)
                    localpay = "001BFF33330B02001B0104090001028F04060001000000780408000202"; // Standard bake for Speed Oven (MWO bake instead of upper oven)          ;
                if (RB_Laundry.Checked)
                    localpay = "0026FF333305050006010505001503050500180305050014000505000A010505000D000307000102"; // Standard wash cycle for Janus washer (wash cavity)
                
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);

                try
                {
                    if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                        parent.LB_IPs.Items.Add(cai.IPAddress);

                    IPData newip = new IPData(cai.IPAddress, localpay);
                    parent.iplist.Add(newip);
                    newip.MAC = cai.MacAddress;
                    //newip.Type = CB_Variant.Text + " " + CB_Type.Text;

                    // Update window for added IP
                    DataRow dr = parent.results.NewRow();

                    dr["IP Address"] = newip.IPAddress;
                    dr["OTA Payload"] = newip.Payload;
                    dr["Delivery Method"] = "MQTT";
                    dr["OTA Type"] = newip.Type;
                    dr["OTA Result"] = "PENDING";
                    parent.results.Rows.Add(dr);

                }
                catch
                {
                }
            }
        }

        private void RB_NAR_C_CheckedChanged(object sender, EventArgs e)
        {
            RB_EMEA_C.Checked = false;
            RB_Laundry.Checked = false;
           // RB_NAR_C.Checked = true;

        }

        private void RB_EMEA_C_CheckedChanged(object sender, EventArgs e)
        {
            RB_NAR_C.Checked = false;
            RB_Laundry.Checked = false;
           // RB_EMEA_C.Checked = true;
        }

        private void RB_Laundry_CheckedChanged(object sender, EventArgs e)
        {
            RB_NAR_C.Checked = false;
            RB_EMEA_C.Checked = false;
           // RB_Laundry.Checked = true;
        }
    }
}
