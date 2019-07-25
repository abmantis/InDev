using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.IO;
using System.Threading;

namespace VenomNamespace
{
    public partial class Venom : WideInterface
    {
        public const byte API_NUMBER = 0;//TODO: change to API value
        /// <summary>
        /// Opcodes parsed by this form
        /// </summary>
        /// 

        private DataTable results;
        private BindingSource sbind = new BindingSource();
        private string curfilename;
        private List<IPData> iplist;
        private List<string> responses;

        public string statusb = "PENDING";
        public System.Net.IPAddress ip;
        public delegate void SetTextCallback();
        public SetTextCallback settextcallback;
        public int listindex;

        static object lockObj = new object();

        bool mqttresp = false;

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
            //Add your constructor operations here
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            //AutoSave: Load and Save your CheckBoxes, TextBoxes and ComboBoxes last conditions
            //Default is true, this line can be removed
            AutoSave = true;
            //To facilitate the debug of your form you can trace your exception or print messages using LogException function
            //  LogException("My Error Message");
            //  or
            //  try { //your code } catch (Exception ex) { LogException(ex,false); }
            //Avoid to do a to much operations during the construction time to have a faster open time and avoid opening the form errors.
            //Errors during the construction time are hard to debug because your object are not instantiated yet.

            settextcallback = new SetTextCallback(SetText);

            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("OTA Result");

            sbind.DataSource = results;
            listindex = 0;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = results;
            DGV_Data.DataSource = sbind;

            TB_LogDir.Text = Directory.GetCurrentDirectory();

            iplist = new List<IPData>();
            responses = new List<string>();
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
              /*  string text = reveal_pkt.API.ToString("X2") + "," + reveal_pkt.OpCode.ToString("X2");
                foreach (byte b in reveal_pkt.PayLoad)
                {
                    text += "," + b.ToString("X2");
                }
                TB_Log.Text = text;*/

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

            switch (data.Topic)
            {
                case "iot-2/evt/subscribe/fmt/json":
                    string pay = System.Text.Encoding.ASCII.GetString(data.Message);
                   // setLEDs((byte)4, (pay.Contains("1") ? (byte)3 : (byte)2));
                   // setLEDs((byte)3, (byte)2);
                    break;
                default:
                    break;
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
            /*if (data.ContentAsString.Contains("linkstate"))
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
            }*/
            // Console.WriteLine("Data " + data);
            double statusval = 0;

            if (data.ContentAsString.StartsWith("mqtt_out_data:"))
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
                // if (sb.Contains("\"status\": ["))
                if (sb.Contains("\"status\""))
                {
                    try
                    {
                        sb = sb.Replace("\"status\":[", "@");
                        string[] stats = sb.Split('@');
                        sb = "";
                        parts = stats[1].Split(',');
                        for (int i = 0; i < parts[0].Length; i++)
                            //for (int i = 0; !statusb[i].Equals(","); i++)
                            sb += parts[i];

                        statusval = Char.GetNumericValue(Char.Parse(sb));
                        //for (int i = 0; i < )
                       // Console.WriteLine("Status BPre" + DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + statusb);
                        if (statusval == 0 || statusval == 1)
                        {
                            statusb = "OTA Status Result was " + statusval.ToString() + " PASS.";

                           // Console.WriteLine("Status BWork" + DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + statusb);
                        }

                        else {
                            statusb = "OTA Status Result was " + statusval.ToString() + " FAIL.";

                        //Console.WriteLine("Status BFail" + DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + statusb);
                             }

                        //lock (lockObj)
                        // {
                        iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).Result = statusb;
                        
                        // Write info to widebox window
                        //Invoke(settextcallback);
                        SetText();

                        responses.Clear();
                        statusb = "PENDING";
                        // }
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
       /* private void setLEDs(byte opCode, byte payload)
        {
            switch (payload)
                {
                    case 1:
                        LED_Internet.SetColor(Color.Red);
                        break;
                    case 2:
                        LED_Internet.SetColor(Color.Yellow);
                        break;
                    case 3:
                        LED_Internet.SetColor(Color.LightGreen);
                        break;
                    default:
                        LED_Internet.SetColor(Color.DarkGray);
                        break;
                }

        }*/

        private void BTN_LogDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                TB_LogDir.Text = fbd.SelectedPath;
            }

            // ADD IF FOLDER DOESN'T EXIST *********************
        }

        private void SetText()
        {
            // foreach (string s in responses)
            // {
            // string[] parts = s.Split('\t');
            //int listindex = 0;
            /*DataRow resultRow = results.NewRow();

             // results.Rows.Add(TB_IP.Text + TB_Payload.Text + "Pass12");

             resultRow["IP Address"] = TB_IP.Text;
             resultRow["OTA Payload"] = TB_Payload.Text;
             resultRow["OTA Result"] = statusb;
             results.Rows.Add(resultRow);

                 try
                 {
                     using (StreamWriter sw = File.AppendText(curfilename))
                     {
                         sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + TB_IP.Text + "," +
                             TB_Payload.Text + "," +
                             statusb);
                     }
                 }
                 catch { }
            // }*/

            foreach (string s in responses)
            {
                string[] parts = s.Split('\t');
                IPData ipd = iplist.FirstOrDefault(x => x.IPAddress == parts[0]);
                listindex = iplist.IndexOf(ipd);

                results.Rows[listindex]["OTA Result"] = iplist[listindex].Result;
                /*//LB_Response.Items.Add(pingresp);
                string[] parts = pingresp.Split('\t');
                int rtt = parts[1].EndsWith("ms") ? int.Parse(parts[1].Substring(0,parts[1].Length-2)) : pingtimeout;
                iplist[listindex].AddReply(rtt,mqttresp);
                
                results.Rows[listindex]["# Missed Packets"] = iplist[listindex].DropCount;
                results.Rows[listindex]["Uptime %"] = iplist[listindex].Uptime;
                results.Rows[listindex]["MQTT Drop Count"] = iplist[listindex].MQTTDropCount;
                results.Rows[listindex]["MQTT Uptime %"] = iplist[listindex].MQTTUptime;*/

                try
                {
                    using (StreamWriter sw = File.AppendText(curfilename))
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," +
                            iplist[listindex].Payload + "," +
                            iplist[listindex].Result);
                    }
                }
                catch { }
            }

            DGV_Data.Refresh();

            /*try
            {
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," + rtt + "," + mqttresp);                
            }
            catch { }*/
        }

        private void BTN_Payload_Click(object sender, EventArgs e)
        {
            if (BTN_Payload.Text == "Send Payload")
            {
                //Write info to log
                if (!File.Exists(TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
                {
                    curfilename = TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                    using (StreamWriter sw = File.CreateText(curfilename))
                    {
                        sw.WriteLine("Time,IP,Payload,Result");
                    }
                }
                BTN_Payload.Text = "Stop Running";
                BTN_Add.Enabled = false;
                BTN_Remove.Enabled = false;

                if (LB_IPs.Items.Count > 0)
                
                    ProcessIP();
                
            }
            else
            {
                BTN_Payload.Text = "Send Payload";
                BTN_Add.Enabled = true;
                BTN_Remove.Enabled = true;
            }
        }

        public void SendMQTT(byte[] ipbytes, byte[] paybytes)
        {
            // ADD IF MQTT NOT SELECTEC AND ADD REVELATION SEND OPTION ****************
            ip = new System.Net.IPAddress(ipbytes);

            //Semd payload
            WifiLocal.SendMqttMessage(ip, "iot-2/cmd/isp/fmt/json", paybytes);
        }
        public void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }

        void ProcessIP()
        {
            foreach (IPData ipd in iplist)
            {
                string ip = ipd.IPAddress;
                string pay = ipd.Payload;

                TraceConnect(ip, pay);

                //Parse OTA payload into byte array for sending via MQTT
                byte[] paybytes = Encoding.ASCII.GetBytes(pay);


                //Prepare IP address for sending via MQTT
                string[] ipad = ip.Split('.');
                byte[] ipbytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    ipbytes[j] = byte.Parse(ipad[j]);
                }

                SendMQTT(ipbytes, paybytes);

                //Invoke(settextcallback);

               // SetText();

            }
        }
        //Check trace status
        void TraceConnect(string ip, string pay)
        {
            
           // lock (lockObj)
           // {
                    //gets the list of appliances from WifiBasic
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    //Selects the appliance based on IP address
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);
                //If an appliance with the specified IP address is found in the list...
                while (!mqttresp)
                {
                    if (cai != null)
                    {
                        mqttresp = cai.IsTraceOn; //check if Trace is currently enabled
                        if (!cai.IsTraceOn)
                        {
                            //if it's not and Revelation is also not enabled, enable Revelation
                            if (!cai.IsRevelationConnected)
                            {
                                WifiLocal.ConnectTo(cai);
                                Wait(1000);
                            }
                            if (cai.IsRevelationConnected)
                            {
                                //If Revelation is enabled, enable Trace
                                WifiLocal.EnableTrace(cai, true);
                                Wait(1000);
                        }
                            mqttresp = false;
                        }
                        else
                        {
                            //If the Trace is enabled and Revelation is also connected, close the Revelation connection
                            if (cai.IsRevelationConnected)
                            {
                                WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                            Wait(1000);
                        }
                            mqttresp = true;
                        }
                    }
                    // Else if the IP address is not found in the WifiBasic list, connection has been lost
                    else

                        mqttresp = false;
                }


            responses.Add(ip + "\t" + pay + "\t" + statusb);
            // }

        }
        private void LB_IPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            TB_IP.Text = LB_IPs.SelectedItem.ToString();
        }


        private void BTN_Add_Click(object sender, EventArgs e)
        {
            string localpay = TB_Payload.Text;
            try
            {
                if (iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);
                    if (cai != null)
                    {
                        LB_IPs.Items.Add(cai.IPAddress);
                        IPData newip = new IPData(cai.IPAddress, localpay);
                        iplist.Add(newip);
                        DataRow dr = results.NewRow();
                        dr["IP Address"] = newip.IPAddress;
                        dr["OTA Payload"] = newip.Payload;
                        dr["OTA Result"] = statusb;
                        results.Rows.Add(dr);
                    }
                }
            }
            catch
            {
            }
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
    }
}
