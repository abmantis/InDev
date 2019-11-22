using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data.OleDb;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace VenomNamespace
{
    public partial class MQTT_URL : WideInterface
    {
        public const byte API_NUMBER = 0;
        public static int ATTEMPTMAX = 10;
        public string choice = "";
        public string mqtt_url = "TRY AGAIN";
        public enum OPCODES { }

        //to access wideLocal use base.WideLocal or simple WideLocal
        public MQTT_URL(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
           

    }
        
        /// <summary>
        /// Parse message from WideBoxInterface
        /// </summary>
        /// <param name="data">Simple Whirlpool packet data with extended info (TimeStamp and IsValid)</param>
        public override void parseSimpleWhirlpoolMessage(ExtendedSimpleWhirlpoolPacket data)
        {
            RevealPacket reveal_pkt = new RevealPacket();
            if (reveal_pkt.ParseSimpleWhirlpoolMessage(data))
            {              
                switch (reveal_pkt.API)
                {
                    case API_NUMBER:   //parse opcodes to this API (already without the feedback bit, i.e. 0x25: reveal_pkt.OpCode = 5 , reveal_pkt.IsFeedback = true )
                        switch ((OPCODES)reveal_pkt.OpCode)
                        {
                            /*
                             * Create a opcode enumeration to parse specific opcode
                             * case OPCODES.SET_VALUE:
                             *     if(reveal_pkt.IsFeedback)
                             *     {
                             *          textBox1.Text = reveal_pkt.PayLoad[0].ToString();
                             *     }
                             *     break;
                             *   */
                            default:

                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Parse messages from the device between the bus and the PC
        /// </summary>
        /// <param name="message">string containing a message from the device</param>
        /// <param name="data">data from the device message</param>
        public override void parseDeviceMessage(DeviceMessage message)
        {
            //Using DeviceMesageString to parse a Device message
            //Checking if message income is Data Flow State message
            if (DeviceMessageStrings.CheckDeviceString(DeviceMessageStrings.DataFlowState, message.Message))
            {
                //Get the Data from the message into a string list, See DeviceMessageStrings.(message) for info about the data
                List<string> values = DeviceMessageStrings.GetDeviceStringParameters(wbox.DeviceMessageStrings.DataFlowState, message.Message);
                //textBoxDataConnected.Text = values[0].ToUpperInvariant() == "ON" ? "Data OFF" : "Data ON";
            }
        }

        /// <summary>
        /// Parse messages from the Revelation connection.
        /// </summary>
        /// <param name="data">The data from the revelation connected appliance.</param>
        public override void parseRevelationMessages(ExtendedRevelationPacket data)
        {
            
        }

        /// <summary>
        /// Parse messages from the MQTT connection.
        /// </summary>
        /// <param name="data">The data from the mqtt connected appliance.</param>
        public override void parseMqttMessages(ExtendedMqttMsgPublish data)
        {            

        }

        /// <summary>
        /// Parse messages from the Udp connection.
        /// </summary>
        /// <param name="data">The data from the udp connected appliance.</param>
        public override void parseUdpMessages(ExtendedUdpMessage data)
        {

        }

        public override void parseTraceMessages(ExtendedTracePacket data)
        {
            // Filter on relevant OTA topics only psm[mqtt_url]=[wa.applianceconnect.net]
            if (data.ContentAsString.StartsWith("psm") && data.ContentAsString.Contains("mqtt_url"))
            {
                //MQTT data in the Trace just comes as raw hex regardless of message format, so need to convert it to ASCII to get the payload
                string[] parts = data.ContentAsString.Replace("[", "").Split('=');
                mqtt_url = "";
                for (int i = 0; i < parts[1].Length-1; i++)
                {
                    string hs = parts[1].Substring(i, 1);
                    mqtt_url += hs;
                }
            }
        }

        #region WIRED BUS Message functions
        /// <summary>
        /// Send a wide Message over the bus
        /// </summary>
        /// <param name="destination">Address to send the message</param>
        /// <param name="sap">sap to transport layer defined in Wide docs (1/2-TDD/UDD 3-Gmcl 4-Reveal)</param>
        /// <param name="data">wide payload</param>
        public void SendWideMessage(byte destination, byte sap, byte[] data)
        {
            WideLocal.SendWideMsg(destination, sap, data);
        }

        /// <summary>
        /// Send a Reveal Message over the bus.
        /// Packet Source address will be ignored as the WideBox address that will be used
        /// To set WideBox address send the WideBoxConstants.CMD_GET_WIDE_BOX_ADDR using the WideLocal.SendCommand
        /// </summary>
        /// <param name="pkt">Reveal packet containing the command or feedback to be sent</param>
        public void SendRevealMessage(RevealPacket pkt)
        {
            WideLocal.SendMessage(pkt.Destination, WideBoxConstants.REVEAL_SAP, pkt.ToMessagePayload());
        }

        /// <summary>
        /// Send a Reveal Message over the bus.
        /// </summary>
        /// <param name="destination">Address to send the message</param>
        /// <param name="api">Reveal Api</param>
        /// <param name="opcode">Reveal Opcode</param>
        /// <param name="isFeedback">Set true if the message is a feedback</param>
        /// <param name="payload">payload bytes</param>
        public void SendRevealMessage(byte destination, byte api, byte opcode, bool isFeedback, byte[] payload)
        {
            RevealPacket pkt = new RevealPacket(api, opcode, 0, destination, isFeedback, payload);
            WideLocal.SendMessage(pkt.Destination, WideBoxConstants.REVEAL_SAP, pkt.ToMessagePayload());
        }
        #endregion

        #region WIRELESS Message functions
        /// <summary>
        /// Send a Reveal Message over the Revelation connection.
        /// </summary>
        /// <param name="connectedAppliance">The connected Applicance</param>
        /// <param name="destination">Address to send the message</param>
        /// <param name="api">Reveal Api</param>
        /// <param name="opcode">Reveal Opcode</param>
        /// <param name="isFeedback">Set true if the message is a feedback</param>
        /// <param name="payload">payload bytes</param>
        public void SendRevealMessageOverRevelation(ConnectedApplianceInfo connectedAppliance, byte destination, byte api, byte opcode, bool isFeedback, byte[] payload)
        {
            RevealPacket pkt = new RevealPacket(api, opcode, 0, destination, isFeedback, payload);
            WifiLocal.SendRevelationMessage(connectedAppliance, pkt.ToRevelation());
        }

        /// <summary>
        /// Send a Reveal Message over the Revelation connection.
        /// Use WifiLocal.ConnectedAppliances to get the list of current scanned and/or connected appliances.
        /// </summary>
        /// <param name="connectedAppliance">The connected Applicance</param>
        /// <param name="pkt">Reveal packet containing the command or feedback to be sent</param>
        public void SendRevealMessageOverRevelation(ConnectedApplianceInfo connectedAppliance, RevealPacket pkt)
        {
            WifiLocal.SendRevelationMessage(connectedAppliance, pkt.ToRevelation());
        }

        /// <summary>
        /// Send a Reveal Message List over the Revelation connection.
        /// Use WifiLocal.ConnectedAppliances to get the list of current scanned and/or connected appliances.
        /// </summary>
        /// <param name="connectedAppliance">The connected Applicance</param>
        /// <param name="pktList">Reveal packet list containing the commands and/or feedbacks to be sent</param>
        public void SendRevealMessageOverRevelation(ConnectedApplianceInfo connectedAppliance, List<RevealPacket> pktList)
        {
            WifiLocal.SendRevelationMessage(connectedAppliance, pktList.ToRevelation());
        }

        /// <summary>
        /// Send a Reveal Message over the Mqtt connection.
        /// Use WifiLocal.ConnectedAppliances to get the list of current scanned and/or connected appliances.
        /// </summary>
        /// <param name="connectedAppliance">The connected Applicance</param>
        /// <param name="pkt">Reveal packet containing the command or feedback to be sent</param>
        public void SendRevealMessageOverMQTT(ConnectedApplianceInfo connectedAppliance, RevealPacket pkt)
        {
            WifiLocal.SendMqttMessage(connectedAppliance, pkt.ToMqtt());
        }

        /// <summary>
        /// Send a Reveal Message over the Mqtt connection.
        /// Use WifiLocal.ConnectedAppliances to get the list of current scanned and/or connected appliances.
        /// </summary>
        /// <param name="connectedAppliance">The connected Applicance</param>
        /// <param name="destination">Address to send the message</param>
        /// <param name="api">Reveal Api</param>
        /// <param name="opcode">Reveal Opcode</param>
        /// <param name="isFeedback">Set true if the message is a feedback</param>
        /// <param name="payload">payload bytes</param>
        public void SendRevealMessageOverMQTT(ConnectedApplianceInfo connectedAppliance, byte destination, byte api, byte opcode, bool isFeedback, byte[] payload)
        {
            RevealPacket pkt = new RevealPacket(api, opcode, 0, destination, isFeedback, payload);
            WifiLocal.SendMqttMessage(connectedAppliance, pkt.ToMqtt());
        }

        #endregion
       
        public bool Valid()
        {
            if (CB_Custom.Checked == true)
                return true;
            if (CB_EMEAP.Checked == true)
                return true;
            if (CB_NARS.Checked == true)
                return true;
            if (CB_NARP.Checked == true)
                return true;

            return false;
        }
        public string PaySelection()
        {
            if (BTN_GET.Text == "Running")
                return "{\"system\":\"readpsm:whr,mfgdata,mqtt_url\"}";
            if (CB_Custom.Checked == true)
                return "{\"system\":\"mqtt_url:" + TB_Custom.Text + "\"}";
            if (CB_EMEAP.Checked == true)
                return "{\"system\":\"mqtt_url:159.8.169.212\"}";
            if (CB_NARS.Checked == true)
                return "{\"system\":\"mqtt_url:wa.applianceconnect.net\"}"; 
            if (CB_NARP.Checked == true)
                return "{\"system\":\"mqtt_url:169.45.2.20\"}";

            return "";
        }
        public string OrgSelection()
        {
            //if (BTN_GET.Text == "Stop Running")
                //return "{\"system\":\"readpsm: whr,mfgdata,mqtt_org_id\"}";
            if (CB_EMEAP.Checked == true)
                return "{\"system\":\"mqtt_org_id:xsc5ga\"}";
            if (CB_NARS.Checked == true)
                return "{\"system\":\"mqtt_org_id:leni51\"}";
            if (CB_NARP.Checked == true)
                return "{\"system\":\"mqtt_org_id:rychla\"}";

            return "";
        }
        private void BTN_Payload_Click(object sender, EventArgs e)
        {
            if (BTN_Payload.Text == "Set")
            {
                try
                {
                    DialogResult dialogurl;
                    if (!Valid())
                    {
                        DialogResult dialogResult = MessageBox.Show("No selection made. Please choose a selection from the check boxes.",
                                                            "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!CB_Suppress.Checked)
                    {
                        if (MessageBox.Show("Before continuing, please verify that IP Address of " + TB_IP.Text + " is currently" +
                                                               " listed within WifiBasic. If not, press 'No' on this window and open WifiBasic then press 'Data Start'" +
                                                               " and finally, press 'Scan Appliances' to populate the list in WifiBasic.", "Verify IP Address is Listed",
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                            return;
                    }
                    string set = "set";
                    bool result = false;
                    BTN_Payload.Text = "Running";
                    BTN_Payload.Enabled = false;
                    CB_EMEAP.Enabled = false;
                    CB_NARS.Enabled = false;
                    CB_NARP.Enabled = false;
                    CB_Custom.Enabled = false;
                    CB_Org.Enabled = false;
                    BTN_Reset.Enabled = false;
                    TB_IP.Enabled = false;
                    BTN_GET.Enabled = false;
                    //CycleWifi();
                    //Wait(2000);
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);
                    
                    byte[] paybytes = Encoding.ASCII.GetBytes(PaySelection());
                    byte[] orgbytes = Encoding.ASCII.GetBytes(OrgSelection());

                    if (cai != null)
                    {
                        if (RevelationConnect(set, cai))
                        {
                            result = SendRevelation(TB_IP.Text, paybytes, orgbytes);//, cai);
                            Wait(2000);
                        }
                        if (result)
                            dialogurl = MessageBox.Show("The MQTT URl for " + TB_IP.Text + " WAS CHANGED successfuly." +
                                                        " Request completed. Closing all open connections.", "Set MQTT URL",
                                                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            dialogurl = MessageBox.Show("The MQTT URl for " + TB_IP.Text + " was NOT CHANGED successfuly." +
                                                                                    " Request completed. Closing all open connections.", "Set MQTT URL",
                                                                                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset(false);
                        BTN_Payload.Enabled = true;
                        TB_IP.Enabled = true;
                        BTN_Reset.Enabled = true;
                        BTN_GET.Enabled = true;
                        WifiLocal.CloseAll(true);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                            " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Reset(false);
                        return;
                    }
                }
                
                catch
                {
                    Reset(false);
                    return;
                }
                
            }
            else
            {
                try
                {
                    Reset(false);
                    TB_IP.Enabled = true;
                    BTN_Reset.Enabled = true;
                    return;
                }

                catch
                {
                    Reset(false);
                    return;
                }
                               
            }
                
        }         
        public bool SendRevelation(string ips, byte[] paybytes, byte[] orgbytes)//, ConnectedApplianceInfo cai)
        {
            int revattempt = 0;
            bool revconnect = false;
            // Check CAI and set IP address
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ips);

            var myDestination = WifiLocal.ConnectedAppliances.FirstOrDefault(i => i.IPAddress.Equals(ips));

            // See if Revelation is Connected and attempt to connect until it is
            while (!revconnect && (revattempt < ATTEMPTMAX))
            {
                try
                {
                    revattempt++;

                    // Send Revelation message(s)
                    if (myDestination != null && cai.IsRevelationConnected)
                    {
                        WifiLocal.SendRevelationMessage(myDestination, new RevelationPacket()
                        {
                            API = 0xF0,
                            Opcode = 00,
                            Payload = paybytes,
                        });
                        Wait(2000);
                        if (CB_Org.Checked)
                        {
                            WifiLocal.SendRevelationMessage(myDestination, new RevelationPacket()
                            {
                                API = 0xF0,
                                Opcode = 00,
                                Payload = orgbytes,
                            });
                            Wait(2000);
                        }
                        revconnect = true;
                    }

                    // Close revelation
                    if (revconnect)
                    {
                        WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                        Wait(2000);
                        return true;
                    }

                }
                catch
                {
                    MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                           " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                        " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;

            /*bool revconnect = false;
            int revattempt = 0;
            //var myDestination = WifiLocal.ConnectedAppliances.FirstOrDefault(i => i.IPAddress.Equals(ips));
            // See if Revelation is Connected and attempt to connect until it is

            Wait(2000);
            try
            {
                revattempt++;

                // Send Revelation message(s)
                if (cai != null && cai.IsRevelationConnected)
                {
                    WifiLocal.SendRevelationMessage(cai, new RevelationPacket()
                    {
                        API = 0xF0,
                        Opcode = 00,
                        Payload = paybytes,
                    });
                    Wait(2000);
                    if (CB_Org.Checked)
                    {
                        WifiLocal.SendRevelationMessage(cai, new RevelationPacket()
                        {
                            API = 0xF0,
                            Opcode = 00,
                            Payload = orgbytes,
                        });
                        Wait(2000);
                    }
                    revconnect = true;
                }

                // Close revelation
                if (revconnect)
                    return true;

            }
            catch
            {
                MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                       " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }



            MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                        " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;*/
        }        
        public void Wait(int timeout)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(timeout);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }
        bool RevelationConnect(string sent, ConnectedApplianceInfo cai)
        {

            int revattempt = 0;
            while (revattempt < ATTEMPTMAX)
            {
                revattempt++;
                try
                {
                    if (cai != null)
                    {
                        if (sent == "set")
                        {
                            if (!cai.IsRevelationConnected)
                            {
                                WifiLocal.ConnectTo(cai);
                                Wait(2000);
                            }


                            if (!cai.IsRevelationConnected)
                            {
                                continue;
                            }


                            if (cai.IsRevelationConnected)
                                return true;
                        }


                        if (sent == "get")
                        {
                            if (!cai.IsTraceOn)
                            {
                                //if it's not and Revelation is also not enabled, enable Revelation
                                if (!cai.IsRevelationConnected)
                                {
                                    WifiLocal.ConnectTo(cai);
                                    Wait(2000);
                                }
                                if (cai.IsRevelationConnected)
                                {
                                    //If Revelation is enabled, enable Trace
                                    WifiLocal.EnableTrace(cai, true);
                                    Wait(2000);
                                }
                            }

                            if (cai.IsTraceOn)
                                    return true;
                        }
                            
                    }            
                                        
                    else
                    {
                        MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                        " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }


                catch
                {
                    if (sent == "get")
                        MessageBox.Show("Connection failed. Verify that UITracer is NOT RUNNING and is Closed. You may need to close" +
                                "Widebox and try again.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                        MessageBox.Show("Revelation connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                        " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            /*if (CycleWifi())
            {
                try
                {
                        if (!cai.IsRevelationConnected)
                        {
                            WifiLocal.ConnectTo(cai);
                            Wait(2000);
                        }

                    if (cai.IsRevelationConnected)
                        return true;

                    else
                    {
                        MessageBox.Show("Revelation connection failed. You may need to close" +
                                   " Widebox and/or power cycle the product and try again.", "Error: Unable to connect.",
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    

                }
                catch { return false; }
            }*/

            MessageBox.Show("Revelation connection failed. You may need to close" +
                                " Widebox and/or power cycle the product and try again.", "Error: Unable to connect.",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }
        public bool CycleWifi()
        {
            string localIP = WifiLocal.Localhost.ToString();
            try
            {
                // Close all WifiBasic connections
                WifiLocal.CloseAll(true);
                //WifiLocal.Close(cai);
                Wait(4000);
                // Get new cert to restart WifiBasic connections
                CertManager.CertificateManager certMgr = new CertManager.CertificateManager();

                // Restart Wifi Connection
                if (certMgr.IsLocalValid)
                {
                    WifiLocal.SetWifi(System.Net.IPAddress.Parse(localIP), certMgr.GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020));
                    Wait(7000);
                    WifiLocal.ScanConnectedAppliances(false, null);
                    return true;
                }
            }

            catch
            {
                return false;
            }

            return false;

        }
        private void MQTT_URL_Load(object sender, EventArgs e)
        {
            TB_IP.Text = "";
            Reset(true);
        }
        private void CB_NARS_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_NARS.Checked)
            {
                BTN_GET.Enabled = false;
                CB_EMEAP.Enabled = false;
                CB_NARP.Enabled = false;
                CB_Custom.Enabled = false;
                choice = "nars";
            }
            else
            {
                BTN_GET.Enabled = true;
                CB_Custom.Enabled = true;
                CB_EMEAP.Enabled = true;
                CB_NARP.Enabled = true;
            }
            
        }
        private void CB_NARP_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_NARP.Checked)
            {
                BTN_GET.Enabled = false;
                CB_EMEAP.Enabled = false;
                CB_NARS.Enabled = false;
                CB_Custom.Enabled = false;
                choice = "narp";
            }
            else
            {
                BTN_GET.Enabled = true;
                CB_Custom.Enabled = true;
                CB_EMEAP.Enabled = true;
                CB_NARS.Enabled = true;
            }
        }
        private void CB_EMEAP_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EMEAP.Checked)
            {
                BTN_GET.Enabled = false;
                CB_NARS.Enabled = false;
                CB_NARP.Enabled = false;
                CB_Custom.Enabled = false;
                choice = "emeap";
            }
            else
            {
                BTN_GET.Enabled = true;
                CB_Custom.Enabled = true;
                CB_NARP.Enabled = true;
                CB_NARS.Enabled = true;
            }
        }
        private void CB_Custom_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_Custom.Checked)
            {
                BTN_GET.Enabled = false;
                CB_EMEAP.Enabled = false;
                CB_NARP.Enabled = false;
                CB_NARS.Enabled = false;
                CB_Org.Enabled = false;
                TB_Custom.Text = "";
                TB_Custom.Visible = true;
                choice = "custom";
            }
            else
            {
                BTN_GET.Enabled = true;
                CB_EMEAP.Enabled = true;
                CB_NARP.Enabled = true;
                CB_NARS.Enabled = true;
                CB_Org.Enabled = true;
                TB_Custom.Text = "";
                TB_Custom.Visible = false;
            }
        }
        public void Reset(bool parm)
        {
            CB_EMEAP.Enabled = true;
            CB_NARP.Enabled = true;
            CB_NARS.Enabled = true;
            CB_Custom.Enabled = true;
            CB_Org.Enabled = true;
            CB_Org.Checked = false;
            CB_EMEAP.Checked = false;
            CB_NARP.Checked = false;
            CB_NARS.Checked = false;
            CB_Custom.Checked = false;
            TB_Custom.Text = "";
            TB_Custom.Visible = false;
            BTN_GET.Enabled = true;
            BTN_Payload.Enabled = true;
            BTN_GET.Text = "Get";
            BTN_Payload.Text = "Set";
            choice = "";
            TB_IP.Enabled = true;
            if (parm)
            {
                TB_IP.Text = "";
                CB_Suppress.Checked = false;
            }
        }
        private void BTN_Reset_Click(object sender, EventArgs e)
        {
            Reset(true);
        }
        private void BTN_GET_Click(object sender, EventArgs e)
        {
            if (BTN_GET.Text == "Get")
            {
                try
                {
                    DialogResult dialogurl;
                    if (!CB_Suppress.Checked)
                    {                        
                        if (MessageBox.Show("Before continuing, please verify that IP Address of " + TB_IP.Text + " is currently" +
                                                              " listed within WifiBasic and UITracer is NOT running. If not, press 'No' on this window and open WifiBasic then press 'Data Start'" +
                                                              " and finally, press 'Scan Appliances' to populate the list in WifiBasic.", "Verify IP Address is Listed",
                                                              MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)

                        return;
                    }
                    string get = "get";
                    BTN_GET.Text = "Running";
                    BTN_GET.Enabled = false;
                    CB_EMEAP.Enabled = false;
                    CB_NARS.Enabled = false;
                    CB_NARP.Enabled = false;
                    CB_Custom.Enabled = false;
                    CB_Org.Enabled = false;
                    CB_Org.Checked = false;
                    BTN_Reset.Enabled = false;
                    TB_IP.Enabled = false;
                    BTN_Payload.Enabled = false;
                    //CycleWifi();
                    //Wait(2000);
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);
                    byte[] orgbytes = Encoding.ASCII.GetBytes(OrgSelection());
                    byte[] paybytes = Encoding.ASCII.GetBytes(PaySelection());

                    if (cai != null)
                    {
                        if (RevelationConnect(get, cai))
                        {
                            if (SendRevelation(TB_IP.Text, paybytes, orgbytes))
                            {
                                for (int i = 0; i < ATTEMPTMAX; i++)
                                {
                                    if (mqtt_url.Contains("."))
                                        break;
                                    else
                                        Wait(1000);
                                }
                            }                            
                        }                            

                        if (mqtt_url != "")
                            dialogurl = MessageBox.Show("The MQTT URL for " + TB_IP.Text + " is currently " + mqtt_url + "."
                                                    + " Request completed. Closing all open connections.", "Current MQTT URL",
                                                                   MessageBoxButtons.OK, MessageBoxIcon.Information);

                        else
                            dialogurl = MessageBox.Show("The MQTT URl for " + TB_IP.Text + " was NOT returned successfuly." +
                                                                                    " Request completed. Closing all open connections.", "Get MQTT URL",
                                                                                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset(false);
                        mqtt_url = "";
                        TB_IP.Enabled = true;
                        BTN_Reset.Enabled = true;
                        BTN_Payload.Enabled = true;
                        WifiLocal.CloseAll(true);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Connection failed. Verify IP Address of " + TB_IP.Text + " has been correctly typed" +
                                            " and that the IP Address is listed within WifiBasic.", "Error: Unable to connect.",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Reset(false);
                        return;
                    }
                }

                catch
                {
                    Reset(false);
                    mqtt_url = "";
                    TB_IP.Enabled = true;
                    BTN_Reset.Enabled = true;
                    BTN_Payload.Enabled = true;
                    return;
                }

            }
            else
            {
                try
                {
                    Reset(false);
                    TB_IP.Enabled = true;
                    BTN_Reset.Enabled = true;
                    return;
                }

                catch
                {
                    Reset(false);
                    return;
                }

            }
        }
        private void CB_Org_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_Org.Checked)
            {
                BTN_GET.Enabled = false;
                CB_Custom.Enabled = false;
                CB_Custom.Checked = false;
                TB_Custom.Text = "";
                TB_Custom.Visible = false;
            }
            else
            {
                BTN_GET.Enabled = true;
                CB_Custom.Enabled = true;
            }
        }
    }
}
