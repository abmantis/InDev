using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VenomNamespace
{
    public partial class Venom : WideInterface
    {
        public const byte API_NUMBER = 0;
        /// <summary>
        /// Opcodes parsed by this form
        /// </summary>
        /// 
        public static int WAITTIME = 2000;
        public static int ATTEMPTMAX = 10;
        // Entities used to store log and window data
        public DataTable results;
        public BindingSource sbind = new BindingSource();
        // Entities used to store the list of targeted IPs and their progress
        public List<IPData> iplist;
        public List<string> responses;
        public List<ManualResetEventSlim> signal;
        //public Queue TaskQ = new Queue();
        public string curfilename;
        public int listindex;
        public PayList plist;
        //public delegate void SetTextCallback();
        //public SetTextCallback settextcallback;

        static object lockObj = new object();
        static object writeobj = new object();

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

            //settextcallback = new SetTextCallback(SetText);
            // Build log and window table
            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("Delivery Method");
            results.Columns.Add("OTA Type");
            results.Columns.Add("OTA Result");

            // Generate tables
            sbind.DataSource = results;
            listindex = 0;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = results;
            DGV_Data.DataSource = sbind;

            TB_LogDir.Text = Directory.GetCurrentDirectory();

            // Generate lists
            iplist = new List<IPData>();
            responses = new List<string>();
            signal = new List<ManualResetEventSlim>();

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
            switch (data.Topic) 
            {
                case "iot-2/evt/isp/fmt/json":

                    // Take data message array and convert to hex string, then to ascii and concatinate to one string
                    //sb = string.Concat(Array.ConvertAll(data.Message, b => Convert.ToChar((Convert.ToUInt32(b.ToString("X2"), 16)))));
                    string sb = System.Text.Encoding.ASCII.GetString(data.Message);
                    lock (writeobj)
                    {
                        ProcessPayload(sb, data.Source.ToString(), "MQTT Message");
                    }
                    break;

                /* case "iot-2/evt/subscribe/fmt/json":    // Not currently implemented
                     if (iplist.Count > 0)
                     {
                         string pay = System.Text.Encoding.ASCII.GetString(data.Message);
                         if (pay.Contains("version"))
                         {
                             string s = "";
                             s.Replace("\"version\":", "@");
                             string[] stats = s.Split('@');
                             if (stats[1].Equals("1"))
                                 iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).MQTT = 3;
                             else
                                 iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()).MQTT = 2;
                         }
                     }
                     break;*/

                case "iot-2/evt/cc_Kvp/fmt/binary":
                    string savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2")));

                    if (savedExtractedMessage.Equals("000A035D4983AC0109000502"))
                        lock (writeobj)
                        {
                            ProcessPayload("Programming", data.Source.ToString(), "MQTT Message");
                        }
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

        public override void parseTraceMessages(ExtendedTracePacket data)
        {   
            // Filter on relevant OTA topics only
            if (data.ContentAsString.StartsWith("mqtt_out_data:") || data.ContentAsString.StartsWith("mqtt_in_data:"))
            {
                //MQTT data in the Trace just comes as raw hex regardless of message format, so need to convert it to ASCII to get the payload
                string[] parts = data.ContentAsString.Replace(" ", "").Split(':');
                string sb = "";
                for (int i = 0; i < parts[2].Length; i += 2)
                {
                    string hs = parts[2].Substring(i, 2);
                    sb += Convert.ToChar(Convert.ToUInt32(hs, 16));
                }
                lock (writeobj)
                {
                    ProcessPayload(sb, data.Source.ToString(), "Trace Message");
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
       public void SetLED(string ip)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);
            if (cai != null)
            {
                if (!cai.IsMqttConnected)
                    LED_Internet.SetColor(Color.Red);

                else
                    LED_Internet.SetColor(Color.LightGreen);
            }
            else
            {
                // Else if the IP address is not found in the WifiBasic list                        
                MessageBox.Show("No IP Address was found in WifiBasic. Please choose a new IP Address or Retry.", "Error: WifiBasic IP Address Not Found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        public void ProcessPayload(string sb, string ip, string source)
        {
            // Locate if OTA payload has been sent and update status
            if (sb.Contains("\"update\""))
            {
                // Lookup status reason byte pass or fail reason

                foreach (var member in iplist)
                {
                    if (iplist.FirstOrDefault(x => x.IPAddress == ip) != null)
                    {
                        if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                            continue;
                        else if (member.Result.Contains("Programming"))
                            continue;
                        else
                        {
                            // Write info to widebox window
                            iplist[member.TabIndex].Result = "Downloading";
                            SetText("update", source, member.TabIndex);
                            break;
                        }
                    }
                }


            }

            if (sb.Contains("Programming") || sb.Contains("IAP_MODE"))
            {
                // Lookup status reason byte pass or fail reason

                foreach (var member in iplist)
                {
                    if (iplist.FirstOrDefault(x => x.IPAddress == ip) != null)
                    {
                        if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                            continue;
                        else if (member.Result.Contains("Downloading")) 
                            continue;
                        else
                        {
                            // Write info to widebox window
                            iplist[member.TabIndex].Result = "Programming";
                            SetText("update", source, member.TabIndex);
                            break;
                        }
                    }
                }
            }

            if (sb.Contains("\"progress\"") && source.Equals("MQTT Message"))
            {
                sb = sb.Replace("\"progress\":[", "@");
                string[] stats = sb.Split('@');
                string[] parts = stats[1].Split(',');
                sb = "";

                foreach (var member in iplist)
                {
                    if (iplist.FirstOrDefault(x => x.IPAddress == ip) != null)
                    {
                        if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                            continue;
                        else
                        {
                            // Write info to widebox window
                            if (!parts[6].Equals("0"))
                                iplist[member.TabIndex].Result = "Programming";
                            else
                                iplist[member.TabIndex].Result = "Downloading";

                            SetText("update", source, member.TabIndex);
                            break;
                        }
                    }
                }

            }

            //Locate the MQTT status message in the Trace and grab the reason byte immediately after it
            if (sb.Contains("\"status\""))
            {
                try
                {
                    // Overwrite status portion to have a point of reference directly next to status reason byte
                    sb = sb.Replace("\"status\":[", "@");
                    string[] stats = sb.Split('@');
                    string[] parts = stats[1].Split(',');
                    sb = "";

                    for (int i = 0; i < parts[0].Length; i++)
                        sb += parts[i];

                    // Convert status reason byte to a numeric value
                    double statusval = Char.GetNumericValue(Char.Parse(sb));

                    // Lookup status reason byte pass or fail reason
                    foreach (var member in iplist)
                    {
                        if (iplist.FirstOrDefault(x => x.IPAddress == ip) != null)
                        {
                            if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                                continue;
                            else
                            {
                                // Write info to widebox window
                                iplist[member.TabIndex].Result = StatusLookup(statusval);
                                SetText("status", source, member.TabIndex);
                                iplist[member.TabIndex].Signal.Set();
                                break;
                            }
                        }
                    }                                                       

                }
                catch { }
            }
        }
        public string StatusLookup(double statusval)
        {
            string statusb = "";
            // Status enumerations for various error or success states of OTA result
            switch (statusval)
            {
                case 0:
                    statusb = "PASS " + statusval.ToString() + " PA_UPDATE_SUCCESS.";
                    break;
                case 1:
                    statusb = "PASS " + statusval.ToString() + " PA_UPDATE_SUCCESS_REBOOT_NEEDED.";
                    break;
                case 2:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_MEMORY_ALLOCATION.";
                    break;
                case 3:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_INVALID_INPUT.";
                    break;
                case 4:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_RETRIEVING_FILE_FROM_SERVER.";
                    break;
                case 5:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_CRC_MISMATCH.";
                    break;
                case 6:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_BDF_PARSING.";
                    break;
                case 7:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_WAIT_FOR_RESPONSE_TIMED_OUT.";
                    break;
                case 8:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_SENDING_MESSAGE.";
                    break;
                case 9:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_MODEL_DOES_NOT_MATCH.";
                    break;
                case 10:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_INVALID_NODE.";
                    break;
                case 11:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_NODE_IS_NOT_UPDATEABLE.";
                    break;
                case 12:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_ECM_UPDATE_FAILED.";
                    break;
                case 13:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PART_NUMBER_MISMATCH.";
                    break;
                case 14:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_MC200_UPDATE_FAILED_BUT_CONTINUE_UPDATE.";
                    break;
                case 15:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_NOT_ENOUGH_MEMORY_TO_DOWNLOAD_ALL_RESOURCES.";
                    break;
                case 16:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_LENGTH_ERROR_IN_DESCRIPTOR_FILE.";
                    break;
                case 17:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_POST_UPDATE_VERIFICATION_FAILED.";
                    break;
                case 18:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FILETYPE_IS_NOT_SUPPORTED.";
                    break;
                case 19:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FILETYPE_REQUIRES_PROG_ADDR_IN_UBD.";
                    break;
                case 20:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PROG_ADDR_NOT_IN_FIT_TABLE.";
                    break;
                case 21:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PROG_END_ADDR_EXCEED_FIT_TABLE.";
                    break;
                case 22:
                    statusb = "FAIL " + statusval.ToString() + " PA_NEED_APPLICATION_PERMISSION_BEFORE_UPDATE.";
                    break;
                case 23:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FILE_EXTENSION_NOT_SUPPORTED.";
                    break;
                case 24:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FILE_EXTENSION_NOT_DSA.";
                    break;
                case 25:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_DSA_VERIFICATION_FAILED.";
                    break;
                case 26:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_APPLIANCE_PART_NUMBER_MISMATCH.";
                    break;
                case 27:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_UBD_GOT_CORRUPTED.";
                    break;
                case 28:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_IAP_FAILURE.";
                    break;
                case 29:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_SCRIPT_FAILURE.";
                    break;
                case 30:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_UNABLE_CHDIR_TO_USB.";
                    break;
                case 31:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_UNABLE_CHDIR.";
                    break;
                case 32:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_UNABLE_CHMOD.";
                    break;
                case 33:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_NO_BDF_IN_PSM.";
                    break;
                case 34:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_RESTART_DURING_DOWNLOAD.";
                    break;
                case 35:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_RESTART_WAITING_FOR_APPL_PERMISSION.";
                    break;
                case 36:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_RESTART_DURING_IAP.";
                    break;
                case 37:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_UPDATING_WIFI_SW.";
                    break;
                case 38:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_UPDATING_RADIO_SW.";
                    break;
                case 39:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_ALL_NECESSARY_IAP_NODE_NOT_PRESENT_IN_WMSP.";
                    break;
                case 40:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_ALL_NECESSARY_IAP_NODE_NOT_PRESENT_IN_WMSP_AFTER_SPEED_CHANGE.";
                    break;
                case 41:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_ERASE_NO_RESP.";
                    break;
                case 42:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_FAILED_ERASING_FLASH.";
                    break;
                case 43:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PASSIVE_HANDLER_IS_NULL.";
                    break;
                case 80:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PASSIVE_UPDATE_FAILURE_RANGE_BEGIN.";
                    break;
                case 99:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_PASSIVE_UPDATE_FAILURE_RANGE_END.";
                    break;
                case 100:
                    statusb = "FAIL " + statusval.ToString() + " PA_ERROR_UNKNOWN.";
                    break;
                default:
                    break;
            }

            return statusb;
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

        public void SetText(string type, string source, int listindex)
        {
            // Process each progress entity for each IP added
            foreach (string s in responses)
            {
                // Seperate on tabs and pull the IP address
                string[] parts = s.Split('\t');
                IPData ipd = iplist.FirstOrDefault(x => x.IPAddress == parts[0]);
                listindex = iplist.IndexOf(ipd);

                // Update window with OTA result
                results.Rows[listindex]["Delivery Method"] = iplist[listindex].Delivery;
                results.Rows[listindex]["OTA Type"] = iplist[listindex].Type;
                results.Rows[listindex]["OTA Result"] = iplist[listindex].Result;

                if (type.Equals("status"))
                {
                    try
                    {
                        using (StreamWriter sw = File.AppendText(curfilename))
                        {
                            sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," +
                            iplist[listindex].MAC + "," + source + "," +
                            iplist[listindex].Payload + "," +
                            iplist[listindex].Delivery + "," +
                            iplist[listindex].Type + "," +
                            iplist[listindex].Result);                            
                        }
                    }
                    catch { }
                }
            }

            // Push result update
            DGV_Data.Refresh();

        }

        private void BTN_Payload_Click(object sender, EventArgs e)
        {
            if (BTN_Payload.Text == "Send Payload")
            {
                //Write info to log
                if (!File.Exists(TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
                {
                    {
                        // Verify directory exists, if not, throw exception
                        curfilename = TB_LogDir.Text + "\\" + "OTALog_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                        try
                        {                            
                            using (StreamWriter sw = File.CreateText(curfilename))
                            {
                                sw.WriteLine("Time,IP,MAC,Log Source,Payload,Method,Type,Result");
                            }
                        }
                        catch {
                            MessageBox.Show("The chosen directory path does not exist. Please browse to a path that DOES exist and try again.", "Error: Directory Path Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                BTN_Payload.Text = "Stop Running";
                LED_Internet.SetColor(Color.DarkGray);
                BTN_Add.Enabled = false;
                BTN_Remove.Enabled = false;
                BTN_Clr.Enabled = false;
               // RB_MQTT.Enabled = false;
                //RB_Reveal.Enabled = false;

                // Begin processing all IPs on list
                if (LB_IPs.Items.Count > 0)                
                    ProcessIP();                
            }
            else
            {
                BTN_Payload.Text = "Send Payload";
                BTN_Add.Enabled = true;
                BTN_Remove.Enabled = true;
                BTN_Clr.Enabled = true;
               // RB_MQTT.Enabled = true;
                //RB_Reveal.Enabled = true;
                responses.Clear();
            }
        }

        public void SendMQTT(byte[] ipbytes, byte[] paybytes)
        {
            System.Net.IPAddress ip = new System.Net.IPAddress(ipbytes);

            //Semd payload
            WifiLocal.SendMqttMessage(ip, "iot-2/cmd/isp/fmt/json", paybytes);
        }

        public void SendReveal(string ips, byte[] paybytes)
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
                    // Connect revelation
                     WifiLocal.ConnectTo(cai);
                     Wait();
                     revattempt++;
                    
                    // Send Revelation message
                    if (myDestination != null && cai.IsRevelationConnected)
                    {
                        WifiLocal.SendRevelationMessage(myDestination, new RevelationPacket()
                        {
                            API = 0xF1,
                            Opcode = 00,
                            Payload = paybytes,
                        });
                        revconnect = true;
                    }

                    // Close revelation
                    if (revconnect)
                    {
                        WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                        //WifiLocal.Close(cai);
                        Wait();
                        revattempt = 0;
                    }

                    if (revattempt >= ATTEMPTMAX)
                    {
                        CycleWifi(cai);
                        revattempt = 0;
                    }
                }
                catch { }
            }
        }
        public void Wait()
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(WAITTIME);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }
        public void RunTask(string ip, ManualResetEventSlim sig, int ipindex)
        {
            //if (TaskQ.Peek().ToString().Contains(ip))
            foreach (IPData ipd in iplist)
            {
                ipd.IPIndex = ipindex;
                ipd.Signal = sig;
                ipd.TabIndex = iplist.IndexOf(ipd);

                if (ipd.IPAddress.Equals(ip))
                {
                    //string[] item = TaskQ.Dequeue().ToString().Split('\t');                                                                       

                    //Parse OTA payload into byte array for sending via MQTT
                    byte[] paybytes = Encoding.ASCII.GetBytes(ipd.Payload);

                    //Prepare IP address for sending via MQTT
                    string[] ipad = ipd.IPAddress.Split('.');
                    byte[] ipbytes = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        ipbytes[j] = byte.Parse(ipad[j]);
                    }
                    // Figure out a way to schedule how the threads march through iplist, we already filter on ip so only one thread in at a time
                    // See if sending over MQTT or Revelation
                    if (ipd.Delivery.Equals("MQTT"))
                        SendMQTT(ipbytes, paybytes);

                    else
                        SendReveal(ipd.IPAddress, paybytes);

                    // Spinwait until our result gets updated to a pass or fail
                    //while (!results.Rows[index][4].ToString().Contains("PASS") || !results.Rows[index][4].ToString().Contains("FAIL")) ;
                    //SpinWait.SpinUntil(() => isCompleted == true);
                    sig.Wait();
                }
                else
                {
                    continue;
                }
            }
                
        }

   
        void ProcessIP()
        {
            //TaskQ.Clear();
            foreach (IPData ipd in iplist)
            //Parallel.ForEach(iplist, ipd =>
            {
                //string taskentry = "";
                lock (lockObj)
                {
                    // Enable Trace for IP address in list
                    TraceConnect(ipd.IPAddress, ipd.Payload, ipd.Type, ipd.Delivery);
                }
                //taskentry = ipd.IPAddress + "\t" + ipd.Payload + "\t" + ipd.Type + "\t" + ipd.Delivery;
                //TaskQ.Enqueue(taskentry);
            }

            for (int i = 0; i < LB_IPs.Items.Count; i++)
            {
                string ip = LB_IPs.Items[i].ToString();
                ManualResetEventSlim sig = new ManualResetEventSlim();
                signal.Add(sig);
                Thread th = new Thread(() => RunTask(ip, sig, i));
                th.Start();
            }
           // });
        }
        void TraceConnect(string ip, string pay, string type, string delivery)
        {
            bool mqttresp = false;
            int traceattempt = 0;
            //gets the list of appliances from WifiBasic
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;

            //Selects the appliance based on IP address
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);

                //If an appliance with the specified IP address is found in the list...
                while (!mqttresp && (traceattempt < ATTEMPTMAX))
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
                                Wait();
                                traceattempt++;
                            }
                            if (cai.IsRevelationConnected)
                            {
                                //If Revelation is enabled, enable Trace
                                WifiLocal.EnableTrace(cai, true);
                                Wait();
                                traceattempt++;
                            }
                        }
                         else
                         {
                            //If the Trace is enabled and Revelation is also connected, close the Revelation connection
                            if (cai.IsRevelationConnected)
                            {
                                WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                                //WifiLocal.Close(cai);
                                Wait();
                             }
                            mqttresp = true;
                         }
                    }                      
                }

            if (mqttresp)
                // Update progress information   
                responses.Add(ip + "\t" + pay + "\t" + delivery + "\t" + type + "\t" + "PENDING");
            else
            {
                MessageBox.Show("Revelation/Trace was not able to start. Restarting connection attempts.", "Error: Unable to start Trace",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CycleWifi(cai);
                traceattempt = 0;

                /*if (!CycleWifi(System.Net.IPAddress.Parse(cai.IPAddress)))
                {
                    MessageBox.Show("Revelation/Trace was not able to start even after retry. Please close Venom and WideBox and try again.", "Error: WideBox issue prevents running Plugin",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }*/

            }

        }

        public void CycleWifi(ConnectedApplianceInfo cai)    //TODO GET THIS WORKING
        {
            // Close all WifiBasic connections
            //WifiLocal.CloseAll(true);
            WifiLocal.Close(cai);

            // Get new cert to restart WifiBasic connections
           // var cert = new CertManager.CertificateManager().GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020);

            // Restart Wifi Connection
            //WifiLocal.SetWifi(System.Net.IPAddress.Parse(cai.IPAddress), new CertManager.CertificateManager().GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020));
        }

        private void BTN_Add_Click(object sender, EventArgs e)
        {
            string localpay = ""; // TB_Payload.Text;
            string localdeliver = "";
            RB_MQTT.Checked = true;         // Disable this when actually using RBs for Revelation and MQTT
            // A delivery method must be selected
            if (!RB_MQTT.Checked && !RB_Reveal.Checked)
            {
                // Else if the IP address is not found in the WifiBasic list                        
                MessageBox.Show("No delivery method was selected. Please choose either the Revelation or MQTT button for a delivery method.", "Error: Payload Delivery Method Not Found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set delivery method
            if (RB_MQTT.Checked)
            {
                localdeliver = "MQTT";
                //SetLED(TB_IP.Text);
            }
            else
                localdeliver = "Revelation";

            try
            {
                if (iplist.FirstOrDefault(x => x.IPAddress == TB_IP.Text) == null)
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);

                    // Will only run if IP address is first time added
                    if (cai != null)
                    {
                        if (localdeliver.Equals("MQTT") && !cai.IsMqttConnected)
                        {
                            DialogResult dialogResult = MessageBox.Show("You have selected the OTA delivery method as MQTT but the MQTT connection" +
                                                                        " for the entered IP Address of " + TB_IP.Text + " is not currently connected." +
                                                                        " If this is acceptable, click Yes to Continue. Otherwise, click No and setup the" +
                                                                        " MQTT connection then try adding the IP Address again.",
                                                                        "Error: MQTT Delivery but Device is not the MQTT Broker.",
                                                                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (dialogResult == DialogResult.No)
                                return;
                        }
                        
                         // Add IP to list of IPs
                         LB_IPs.Items.Add(cai.IPAddress);
                         IPData newip = new IPData(cai.IPAddress, localpay);
                         iplist.Add(newip);
                         newip.MAC = cai.MacAddress;

                         // Update window for added IP
                         DataRow dr = results.NewRow();
                         dr["IP Address"] = newip.IPAddress;
                         dr["OTA Payload"] = newip.Payload;
                         dr["Delivery Method"] = localdeliver;
                         dr["OTA Result"] = "PENDING";
                         results.Rows.Add(dr);
                                
                    
                    }
                    else
                    {
                        // Else if the IP address is not found in the WifiBasic list                        
                        MessageBox.Show("No IP Address was found in WifiBasic. Please choose a new IP Address or Retry.", "Error: WifiBasic IP Address Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }

            //RB_MQTT.Checked = false;            //Enable these when actually using RBs for Revelation and MQTT
            //RB_Reveal.Checked = false;
        }

        private void BTN_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                /* foreach (DataRow row in results.Rows)
                 {
                     if (row["IP Address"].ToString().Equals(TB_IP.Text))
                         results.Rows.Remove(row);
                 }*/
                DialogResult dialogResult = MessageBox.Show("This will clear all payloads for the chosen IP " + LB_IPs.SelectedItem.ToString() + ". Press Yes to Remove or No to Cancel.",
                                                        "Verify IP Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    for (int i = results.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = results.Rows[i];
                        if (dr["IP Address"].ToString() == LB_IPs.SelectedItem.ToString())
                            results.Rows.Remove(dr);
                    }
                    results.AcceptChanges();

                    iplist.RemoveAll(x => x.IPAddress == LB_IPs.SelectedItem.ToString());
                    LB_IPs.Items.Remove(LB_IPs.SelectedItem);
                }

            }
            catch { }
        }
        
        private void BTN_Clr_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will clear all IPs and their results from all windows. Press Yes to Clear or No to Cancel.",
                                                        "Verify Full Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                results.Clear();
                responses.Clear();
                iplist.Clear();
                LED_Internet.SetColor(Color.DarkGray);
                LB_IPs.Items.Clear();
                DGV_Data.Refresh();
                // Stop anything that is still running
                //Environment.Exit(Environment.ExitCode);
            }
            
        }

        private void BTN_MQTT_Click(object sender, EventArgs e)
        {
            // Send Subscribe message over MQTT to test MQTT connection
            //WifiLocal.SendMqttMessage(System.Net.IPAddress.Parse(TB_IP.Text), "iot-2/evt/subscribe/fmt/json", Encoding.ASCII.GetBytes("{\"sublist\":[1,144,147]}"));
            LED_Internet.SetColor(Color.DarkGray);
            SetLED(TB_IP.Text);
        }

        private void BTN_MakeList_Click(object sender, EventArgs e)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == TB_IP.Text);

            // Will only run if IP address is first time added
            if (cai != null)
            {
                //if (localdeliver.Equals("MQTT") && !cai.IsMqttConnected) // Add back if tracking MQTT or Revelation
                if (!cai.IsMqttConnected)
                {
                    DialogResult dialogResult = MessageBox.Show("You have selected the OTA delivery method as MQTT but the MQTT connection" +
                                                                " for the entered IP Address of " + TB_IP.Text + " is not currently connected." +
                                                                " If this is acceptable, click Yes to Continue. Otherwise, click No and setup the" +
                                                                " MQTT connection then try adding the IP Address again.",
                                                                "Error: MQTT Delivery but Device is not the MQTT Broker.",
                                                                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dialogResult == DialogResult.No)
                        return;
                }

                try
                {
                    plist.Show();
                }
                catch
                {
                    plist = new PayList(cai, this);
                    plist.Show();
                }

            }
            else
            {
                // Else if the IP address is not found in the WifiBasic list                        
                MessageBox.Show("No IP Address was found in WifiBasic. Please choose a new IP Address or Retry.", "Error: WifiBasic IP Address Not Found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BTN_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //Read the contents of the file into a stream
                var fileStream = ofd.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string line = reader.ReadLine();
                    string[] value; // = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                    int skipped = 0;
                    int index = 0;
                    var ipskipped = new List<string>();

                    while (!reader.EndOfStream)
                    {
                        value = reader.ReadLine().Split('\t');
                        System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                        ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == value[0]);

                        if (cai != null)
                        {
                            if (iplist.FirstOrDefault(x => x.IPAddress == value[0]) == null && !cai.IsMqttConnected)
                            {
                                DialogResult dialogResult = MessageBox.Show("You have selected the OTA delivery method as MQTT but the MQTT connection" +
                                                                            " for the entered IP Address of " + cai.IPAddress + " is not currently connected." +
                                                                            " If this is acceptable, click Yes to Continue. Otherwise, click No and setup the" +
                                                                            " MQTT connection then try adding the IP Address again.",
                                                                            "Error: MQTT Delivery but Device is not the MQTT Broker.",
                                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                if (dialogResult == DialogResult.No)
                                    return;
                            }

                            if (iplist.FirstOrDefault(x => x.IPAddress == value[0]) == null)
                                LB_IPs.Items.Add(cai.IPAddress);

                            IPData newip = new IPData(cai.IPAddress, value[1]); //Add Revelation logic if supported
                            iplist.Add(newip);
                            newip.MAC = cai.MacAddress;
                            newip.Type = value[2];

                            // Update window for added IP
                            DataRow dr = results.NewRow();
                            dr["IP Address"] = newip.IPAddress;
                            dr["OTA Payload"] = newip.Payload;
                            dr["Delivery Method"] = "MQTT";
                            dr["OTA Type"] = newip.Type;
                            dr["OTA Result"] = "PENDING";
                            results.Rows.Add(dr);
                            index++;
                        }
                        else
                        {
                            skipped++;
                            if (ipskipped.FirstOrDefault(x => x.ToString() == value[0]) == null)
                                ipskipped.Add(value[0]);
                        }

                    }

                    if (skipped > 0)
                    {
                        string badips ="";
                        foreach (string ips in ipskipped)
                            badips = badips + "  " + ips;
                        MessageBox.Show("Import skipped a total of " + skipped + " line(s) for IP Address(es) " +
                             badips + " due to NOT being listed in Wifibasic. Please verify the IP Address(es) " +
                            "that were skipped are connected and listed in Wifibasic, then retry importing.", "Error: WifiBasic IP Address(es) Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
