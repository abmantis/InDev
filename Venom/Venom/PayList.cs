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
    public partial class PayList : WideInterface
    {
        public Venom parent;
        public AutoGen auto;
        private BindingSource sbind = new BindingSource();
        public PayList(Venom ParentForm, WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            parent = ParentForm;
            CB_Type.Items.AddRange(new object[] {"Upgrade",
                        "Downgrade"});
            CB_Variant.Items.AddRange(new object[] {"HMI",
                        "ACU", "WiFi", "Multi"});

            // Generate tables
            sbind.DataSource = parent.results;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = parent.results;
            DGV_Data.DataSource = sbind;
        }
        
        private void BTN_Add_Click(object sender, EventArgs e)
        {
            string localpay = TB_Payload.Text;
            string localdeliver = "MQTT"; // CHANGE IF ADDING REVELATION BACK

            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IPDisplay.Text);

            if (cai != null)
            {
                //if (localdeliver.Equals("MQTT") && !cai.IsMqttConnected) // Add back if tracking MQTT or Revelation
                if (!cai.IsMqttConnected)
                {
                    DialogResult dialogResult = MessageBox.Show("You have selected the OTA delivery method as MQTT but the MQTT connection" +
                                                                " for the entered IP Address of " + TB_IPDisplay.Text + " is not currently connected." +
                                                                " If this is acceptable, click Yes to Continue. Otherwise, click No and setup the" +
                                                                " MQTT connection then try adding the IP Address again.",
                                                                "Error: MQTT Delivery but Device is not the MQTT Broker.",
                                                                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dialogResult == DialogResult.No)
                        return;
                }

                try
                {
                    if (parent.iplist.FirstOrDefault(x => x.IPAddress == TB_IPDisplay.Text) == null)
                        parent.LB_IPs.Items.Add(cai.IPAddress);

                    IPData newip = new IPData(cai.IPAddress, localpay);
                    parent.iplist.Add(newip);
                    newip.MAC = cai.MacAddress;
                    newip.Type = CB_Variant.Text + " " + CB_Type.Text;

                    // Update window for added IP
                    DataRow dr = parent.results.NewRow();

                    dr["IP Address"] = newip.IPAddress;
                    dr["OTA Payload"] = newip.Payload;
                    dr["Delivery Method"] = localdeliver;
                    dr["OTA Type"] = newip.Type;
                    dr["OTA Result"] = "PENDING";
                    parent.results.Rows.Add(dr);

                }
                catch
                {
                }
            }
        
            else
            {
                // Else if the IP address is not found in the WifiBasic list                        
                MessageBox.Show("No IP Address was found in WifiBasic. Please choose a new IP Address or Retry.", "Error: WifiBasic IP Address Not Found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BTN_Remove_Click(object sender, EventArgs e)
        {
            try
            {                
                int found = 0;

                foreach (DataGridViewRow row in parent.DGV_Data.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(parent.DGV_Data.Rows[DGV_Data.CurrentCell.RowIndex].Cells[0].Value.ToString()))                    
                        found++;
                    if (found > 1)
                        break;
                }

                if (found <= 1)                
                    parent.LB_IPs.Items.RemoveAt(DGV_Data.CurrentCell.RowIndex);
                
                parent.iplist.RemoveAt(DGV_Data.CurrentCell.RowIndex);
                parent.results.Rows.RemoveAt(DGV_Data.CurrentCell.RowIndex);
            }
            catch { }
        }

        private void BTN_Clear_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will clear all IPs and their results from all windows. Press Yes to Clear or No to Cancel.",
                                                        "Verify Full Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                parent.results.Clear();
                parent.responses.Clear();
                parent.iplist.Clear();
                parent.LB_IPs.Items.Clear();
                DGV_Data.Refresh();
            }
        }

        private void BTN_Save_Click(object sender, EventArgs e)
        {
            //Write info to log
            if (!File.Exists(parent.TB_LogDir.Text + "\\" + "Payload_List_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
            {
                {
                    // Verify directory exists, if not, throw exception
                    string curfilename = parent.TB_LogDir.Text + "\\" + "Payload_List_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                    try
                    {
                        using (StreamWriter sw = File.CreateText(curfilename))
                        {
                            sw.WriteLine("IP\tPayload\tType\tResult");
                            foreach (DataRow row in parent.results.Rows)
                            {
                                sw.WriteLine(row.ItemArray[0].ToString() + "\t" +
                                    row.ItemArray[1].ToString() + "\t" +
                                    row.ItemArray[3].ToString() + "\t" +
                                    row.ItemArray[4].ToString());
                            }
                        }
                        MessageBox.Show("Saved payload list at " + curfilename + ".", "Export: Payload List Saving.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("The chosen directory path does not exist. Please browse to a path that DOES exist and try again.", "Error: Directory Path Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

       
        private void BTN_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BTN_Auto_Click(object sender, EventArgs e)
        {
            try
            {
                auto.Show();
            }
            catch
            {

                auto = new AutoGen(parent, this.WideLocal, this.WifiLocal);
                auto.Show();
            }

            
        }
    }
}
