﻿using System;
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
        public const byte API_NUMBER = 0;//TODO: change to API value
        /// <summary>
        /// Opcodes parsed by this form
        /// </summary>
        /// 

        private DataTable results;
        private string curfilename;
        private List<string> responses;


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

            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("OTA Result");

            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = results;

            TB_LogDir.Text = Directory.GetCurrentDirectory();

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
              /*  try
                {
                    int linkstate = int.Parse(s.Substring(s.IndexOf("linkstate") + 10, 1));
                    int claimstate = int.Parse(s.Substring(s.IndexOf("claimed") + 8, 1));
                    lock (lockObj)
                    {
                        iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).LinkState = linkstate;
                        iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).ClaimState = claimstate;
                    }
                }
                catch { }*/
            }
            if (data.ContentAsString.StartsWith("mqtt_in_data:"))
            {
                //MQTT data in the Trace just comes as raw hex regardless of message format, so need to conver it to ASCII to get the topic string
                string[] parts = data.ContentAsString.Replace(" ", "").Split(':');
                string sb = "";
               /* for (int i = 0; i < parts[2].Length; i += 2)
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
                }*/
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

        private void BTN_LogDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                TB_LogDir.Text = fbd.SelectedPath;
            }
        }

        private void SetText()
        {
           // foreach (string s in responses)
           // {
               // string[] parts = s.Split('\t');
                //int listindex = 0;
           DataRow resultRow = results.NewRow();

            // results.Rows.Add(TB_IP.Text + TB_Payload.Text + "Pass12");
          
            resultRow["IP Address"] = TB_IP;
            resultRow["OTA Payload"] = TB_Payload;
            resultRow["OTA Result"] = "ff";
            results.Rows.Add(resultRow);

                try
                {
                    using (StreamWriter sw = File.AppendText(curfilename))
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + TB_IP + "," +
                            TB_Payload.Text + "," +
                            "Pass0");
                    }
                }
                catch { }
           // }
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

        private void BT_Payload_Click(object sender, EventArgs e)
        {
            
            if (!File.Exists(TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
            {
                curfilename = TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                using (StreamWriter sw = File.CreateText(curfilename))
                {
                    sw.WriteLine("Time,IP,Payload,Result");
                }
            }
            SetText();
        }

      
    }
}
