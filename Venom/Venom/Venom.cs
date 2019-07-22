using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.Net.NetworkInformation;
using System.Timers;
using System.IO;
using System.Threading.Tasks;

namespace VenomNamespace
{
    public partial class Venom : WideInterface
    {
        public static int pingtimeout = 4000; //amout of time to wait before considering a ping to have been dropped
        public static int pingperiod = 10000; //amount of time to wait between ping attempts

        string pingresp = "";
        string ota_pay = "";
        bool mqttresp = false;
        System.Timers.Timer timer;

        public delegate void SetTextCallback();
        public SetTextCallback settextcallback;
        public int listindex;

        static object lockObj = new object();

        private DataTable results;
        private BindingSource sbind = new BindingSource();

        private List<IPData> iplist;
        private List<string> responses;

        private string curfilename;

        public const byte API_NUMBER = 0;//TODO: change to API value
        /// <summary>
        /// Opcodes parsed by this form
        /// </summary>
        public enum OPCODES
        {
            //Include your opcodes in here
            //SET_VALUE = 1
        }


        //to access wideLocal use base.WideLocal or simple WideLocal
        public Venom(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            //Add your constructor operations here

            //AutoSave: Load and Save your CheckBoxes, TextBoxes and ComboBoxes last conditions
            //Default is true, this line can be removed
            AutoSave = true;
            pingresp = "";
            timer = new System.Timers.Timer(pingperiod);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            settextcallback = new SetTextCallback(SetText);
            listindex = 0;

            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("Avg Resp Time");
            results.Columns.Add("# Missed Pings");
            results.Columns.Add("Ping Response %");
            results.Columns.Add("Link State");
            results.Columns.Add("Claim State");
            results.Columns.Add("Cloud Disconnect Count");
            results.Columns.Add("Current Connected Time");
            results.Columns.Add("Current Disconnected Time");
            results.Columns.Add("Wifi Resync Statistics");

            sbind.DataSource = results;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = results;
            DGV_Data.DataSource = sbind;

            TB_LogDir.Text = Directory.GetCurrentDirectory();

            iplist = new List<IPData>();
            responses = new List<string>();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (LB_IPs.Items.Count > 0)
            {
                PingIPList();
                //listindex++;
                //listindex = listindex >= LB_IPs.Items.Count ? 0 : listindex;
            }
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
            List<RevealPacket> revealpkt_list = new List<RevealPacket>();
            revealpkt_list.ParseRevelationPacket(data);
            //A Revelation packet can contain more than one Reveal Message.
            foreach (RevealPacket reveal_pkt in revealpkt_list)
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
        /// Parse messages from the MQTT connection.
        /// </summary>
        /// <param name="data">The data from the mqtt connected appliance.</param>
        public override void parseMqttMessages(ExtendedMqttMsgPublish data)
        {
            RevealPacket reveal_pkt = new RevealPacket();
            if (reveal_pkt.ParseMqttPacket(data))
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
        /// Parse messages from the Udp connection.
        /// </summary>
        /// <param name="data">The data from the udp connected appliance.</param>
        public override void parseUdpMessages(ExtendedUdpMessage data)
        {


        }

        //This command does not come with Lucas's template.  You will have to add it manually.
        public override void parseTraceMessages(ExtendedTracePacket data)
        {
            //base.parseTraceMessages(data);
            if (data.ContentAsString.Contains("linkstate"))
            {
                string s = data.ContentAsString;
                try
                {
                    int linkstate = int.Parse(s.Substring(s.IndexOf("linkstate") + 10, 1));
                    int claimstate = int.Parse(s.Substring(s.IndexOf("claimed") + 8, 1));
                    lock (lockObj)
                    {
                        iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).LinkState = linkstate;
                        iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).ClaimState = claimstate;
                    }
                }
                catch { }
            }
            if (data.ContentAsString.StartsWith("mqtt_in_data:"))
            {
                //MQTT data in the Trace just comes as raw hex regardless of message format, so need to conver it to ASCII to get the topic string
                string[] parts = data.ContentAsString.Replace(" ", "").Split(':');
                string sb = "";
                for (int i = 0; i < parts[2].Length; i += 2)
                {
                    string hs = parts[2].Substring(i, 2);
                    sb += Convert.ToChar(Convert.ToUInt32(hs, 16));
                }

                //Locate the MQTT Statistics message in the Trace and grab the four wifi reset reason bytes from it
                if (sb.Contains("\"statistics\": \""))
                {
                    try
                    {
                        sb = sb.Replace("\"statistics\": \"", "@");
                        string[] stats = sb.Split('@');
                        string[] wifireset = stats[1].Split(',');
                        string resetreason = "";
                        for (int j = 0; j < 4; j++)
                        {
                            resetreason += wifireset[j] + (j == 3 ? "" : ";");
                        }
                        lock (lockObj)
                        {
                            iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).WifiResyncStatistics = resetreason;
                        }
                    }
                    catch { }
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

        //Not used right now, was used when pinging was done serially rather than asynchronously
        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == nameOrAddress);
                if (cai != null)
                {
                    mqttresp = cai.IsMqttConnected;
                }
                else
                {
                    mqttresp = false;
                }
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress, pingtimeout);
                pingable = reply.Status == IPStatus.Success;
                if (pingable)
                {

                    pingresp = reply.Address + "\t" + reply.RoundtripTime + "ms" + "\t" + mqttresp.ToString();

                }
                else
                {
                    pingresp = nameOrAddress + "\t" + reply.Status.ToString() + "\t" + mqttresp.ToString();

                }
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
                mqttresp = false;
                pingresp = nameOrAddress + "\t" + "FAIL" + "\t" + mqttresp.ToString();

            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void PingIPList()
        {
            string s = LB_IPs.Items[listindex].ToString();
            {
                //PingHost(s);
                DateTime startime = DateTime.Now;
                PingHostsAsync();
                while (DateTime.Now < startime.AddMilliseconds(pingtimeout))
                {
                    // wait for responses to finish
                }
                Invoke(settextcallback);

                pingresp = "";
            }
        }

        private void SetText()
        {
            foreach (string s in responses)
            {
                string[] parts = s.Split('\t');
                IPData ipd = iplist.FirstOrDefault(x => x.IPAddress == parts[0]);
                listindex = iplist.IndexOf(ipd);
                int rtt = parts[1].EndsWith("ms") ? int.Parse(parts[1].Substring(0, parts[1].Length - 2)) : pingtimeout;
                iplist[listindex].AddReply(rtt);
                if (parts[2] == "FALSE" && iplist[listindex].LinkState != -1)
                {
                    iplist[listindex].LinkState = 0;
                }
                results.Rows[listindex]["Avg Resp Time"] = iplist[listindex].Average;
                results.Rows[listindex]["# Missed Pings"] = iplist[listindex].DropCount;
                results.Rows[listindex]["Ping Response %"] = iplist[listindex].UptimePct;
                results.Rows[listindex]["Link State"] = iplist[listindex].LinkState;
                results.Rows[listindex]["Claim State"] = iplist[listindex].ClaimState;
                results.Rows[listindex]["Cloud Disconnect Count"] = iplist[listindex].MQTTDropCount;
                results.Rows[listindex]["Current Connected Time"] = iplist[listindex].ConnectedTime.ToString("g");
                results.Rows[listindex]["Current Disconnected Time"] = iplist[listindex].DisconnectedTime.ToString("g");
                results.Rows[listindex]["Wifi Resync Statistics"] = iplist[listindex].WifiResyncStatistics;

                try
                {
                    using (StreamWriter sw = File.AppendText(curfilename))
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," +
                            iplist[listindex].MACAddress + "," +
                            rtt + "," +
                            iplist[listindex].DropCount + "," +
                            iplist[listindex].UptimePct + "," +
                            iplist[listindex].LinkState + "," +
                            iplist[listindex].ClaimState + "," +
                            iplist[listindex].MQTTDropCount + "," +
                            iplist[listindex].ConnectedTime.ToString("g") + "," +
                            iplist[listindex].DisconnectedTime.ToString("g") + "," +
                            iplist[listindex].WifiResyncStatistics);
                    }
                }
                catch { }
            }
            /*//LB_Response.Items.Add(pingresp);
            string[] parts = pingresp.Split('\t');
            int rtt = parts[1].EndsWith("ms") ? int.Parse(parts[1].Substring(0,parts[1].Length-2)) : pingtimeout;
            iplist[listindex].AddReply(rtt,mqttresp);
            results.Rows[listindex]["Avg Resp Time"] = iplist[listindex].Average;
            results.Rows[listindex]["# Missed Packets"] = iplist[listindex].DropCount;
            results.Rows[listindex]["Uptime %"] = iplist[listindex].Uptime;
            results.Rows[listindex]["MQTT Drop Count"] = iplist[listindex].MQTTDropCount;
            results.Rows[listindex]["MQTT Uptime %"] = iplist[listindex].MQTTUptime;*/

            DGV_Data.Refresh();
            responses.Clear();

            /*try
            {
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," + rtt + "," + mqttresp);                
            }
            catch { }*/
        }

        private void BTN_Ping_Click(object sender, EventArgs e)
        {
            if (BTN_Ping.Text == "Start Auto Ping")
            {
                if (!File.Exists(TB_LogDir.Text + "\\" + "PingLog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
                {
                    curfilename = TB_LogDir.Text + "\\" + "PingLog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                    using (StreamWriter sw = File.CreateText(curfilename))
                    {
                        sw.WriteLine("Time,IP,OTAPayload,Ping Response Time (ms), # Missed Pings, Ping Response %, Link State, Claim State, Cloud Disconnect Count, Current Connected Time, Current Disconnected Time, Wifi Resync Statistics");
                    }
                }
                timer.Enabled = true;
                BTN_Ping.Text = "Stop Ping";
                BTN_Add.Enabled = false;
                BTN_Remove.Enabled = false;
            }
            else
            {
                try
                {
                    //sw.Close();
                }
                catch { }
                timer.Enabled = false;
                BTN_Ping.Text = "Start Auto Ping";
                BTN_Add.Enabled = true;
                BTN_Remove.Enabled = true;
            }
        }

        //Not used right now, had a button to send a manual ping but removed it
        private void BTN_Manual_Click(object sender, EventArgs e)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            PingHost(TB_IP.Text);
            SetText();
        }

        private void BTN_LogDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                TB_LogDir.Text = fbd.SelectedPath;
            }
        }

        void PingHostsAsync()
        {
            //responses.Clear();
            foreach (IPData ipd in iplist)
            {
                string ip = ipd.IPAddress;

                Ping p = new Ping();
                p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
                p.SendAsync(ip, pingtimeout, ip);
            }
        }

        //If the product responds to a ping, set the ping data and also check its MQTT connection from the Trace.  Also re-enable the Trace if it has been lost due to disconnection/reboot/power loss
        void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null)
            {
                lock (lockObj)
                {
                    //gets the list of appliances from WifiBasic
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    //Selects the appliance based on IP address
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);
                    //If an appliance with the specified IP address is found in the list...
                    if (cai != null)
                    {
                        mqttresp = cai.IsTraceOn; //check if Trace is currently enabled
                        if (!cai.IsTraceOn)
                        {
                            //if it's not and Revelation is also not enabled, enable Revelation
                            if (!cai.IsRevelationConnected)
                            {
                                WifiLocal.ConnectTo(cai);
                            }
                            else
                            {
                                //If Revelation is enabled, enable Trace
                                WifiLocal.EnableTrace(cai, true);
                            }
                            mqttresp = false;
                        }
                        else
                        {
                            //If the Trace is enabled and Revelation is also connected, close the Revelation connection
                            if (cai.IsRevelationConnected)
                            {
                                WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                            }
                            mqttresp = true;
                        }
                    }
                    // Else if the IP address is not found in the WifiBasic list, connection has been lost
                    else
                    {
                        mqttresp = false;
                    }
                    if (e.Reply.Status == IPStatus.Success)
                    {
                        pingresp = ip + "\t" + e.Reply.RoundtripTime + "ms" + "\t" + mqttresp.ToString();
                    }
                    else
                    {
                        pingresp = ip + "\t" + e.Reply.Status.ToString() + "\t" + mqttresp.ToString();
                    }
                    responses.Add(pingresp);
                }
            }
        }
        private void LB_IPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            TB_IP.Text = LB_IPs.SelectedItem.ToString();
        }
        private void BTN_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                iplist.RemoveAt(LB_IPs.SelectedIndex);
                results.Rows.RemoveAt(LB_IPs.SelectedIndex);
                LB_IPs.Items.Remove(LB_IPs.SelectedItem);
            }
            catch { }
        }

        private void BTN_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);
                    if (cai != null)
                    {
                        LB_IPs.Items.Add(cai.IPAddress);
                        ota_pay = TB_Payload.Text;
                        IPData newip = new IPData(cai.IPAddress, cai.MacAddress);
                        iplist.Add(newip);
                        DataRow dr = results.NewRow();
                        dr["IP Address"] = newip.IPAddress;
                        dr["MAC Address"] = newip.MACAddress;
                        results.Rows.Add(dr);
                    }
                }
            }
            catch
            {
            }
        }
    }
}