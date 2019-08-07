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

namespace VenomNamespace
{
    public partial class PayList : Form
    {
        public Venom parent;
        public ConnectedApplianceInfo cai;
        private BindingSource sbind = new BindingSource();
        public PayList(ConnectedApplianceInfo conapp, Venom ParentForm)
        {
            InitializeComponent();
            parent = ParentForm;
            cai = conapp;
            CB_Type.Items.AddRange(new object[] {"Upgrade",
                        "Downgrade"});
            CB_Variant.Items.AddRange(new object[] {"HMI",
                        "ACU", "WiFi", "Multi"});
            // Generate tables
            sbind.DataSource = parent.results;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = parent.results;
            DGV_Data.DataSource = sbind;

            TB_IPDisplay.Text = cai.IPAddress.ToString();

        }

        
        private void BTN_Add_Click(object sender, EventArgs e)
        {
            string localpay = TB_Payload.Text;
            string localdeliver = "MQTT"; // CHANGE IF ADDING REVELATION BACK
            
            try
            {
                if (parent.iplist.FirstOrDefault(x => x.IPAddress == parent.TB_IP.Text) == null)
                    parent.LB_IPs.Items.Add(cai.IPAddress);

                IPData newip = new IPData(cai.IPAddress, localpay, localdeliver);
                parent.iplist.Add(newip);
                newip.MAC = cai.MacAddress;
                newip.Type = CB_Variant.Text + " " + CB_Type.Text;

                // Update window for added IP
                DataRow dr = parent.results.NewRow();
                dr["IP Address"] = newip.IPAddress;
                dr["OTA Payload"] = newip.Payload;
                dr["Delivery Method"] = newip.Delivery;
                dr["OTA Type"] = newip.Type;
                dr["OTA Result"] = "PENDING";
                parent.results.Rows.Add(dr);

            }
            catch
            {
            }
        }

        private void BTN_Remove_Click(object sender, EventArgs e)
        {
            try            
            {
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
                            foreach (DataRow row in parent.results.Rows)
                            {
                                sw.WriteLine(row.ItemArray[0].ToString() + "," +
                                    row.ItemArray[1].ToString() + "," +
                                    row.ItemArray[3].ToString() + "," +
                                    row.ItemArray[4].ToString());
                            }
                        }
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
    }
}
