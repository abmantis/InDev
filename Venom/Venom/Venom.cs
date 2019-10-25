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
    public partial class Venom : WideInterface
    {
        public const byte API_NUMBER = 0;
        public static int WAITTIME = 2000;
        public static int ATTEMPTMAX = 10;
        public static int TMAX = 120 * 60000; //OTA max thread time is 2 hours
        public static int REBMAX = 2 * 60000;
        public static int RECONWAIT = 10 * 60000;
        public static int CYCLEWAIT = 1 * 60000;

        // Entities used to store log and window data
        public DataTable results;
        public BindingSource sbind = new BindingSource();

        // Entities used to store the list of targeted IPs and their progress
        public List<IPData> iplist;
        public List<string> responses;
        public List<ManualResetEventSlim> signal;

        public string curfilename;
        public PayList plist;
        public bool cancel_request = false;

        static object lockObj = new object();
        static object writeobj = new object();
        static object cancelobj = new object();

        // Used for importing API144 DDM (to identify cycles)
        public List<KeyValue> keyValues;
        public List<Enumeration> enumerations;
        public List<Enumeration> allenums;
        public List<Cycle> cyclesU;
        public List<Cycle> cyclesL;
        public List<Cycle> cyclesM;
        public List<Cycle> cyclesW;
        public List<Cycle> cyclesD;
        public List<List<Cycle>> cycles;

        public int kvapi;
        public string dm_name;
        public string implementation;
        public bool usesCookTimeOp;
        public string[] categoryList = { "Cooking", "Dish", "Laundry", "Refrigeration", "Small Appliance" };
        public enum OPCODES { }

        //to access wideLocal use base.WideLocal or simple WideLocal
        public Venom(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            AutoSave = true;
            keyValues = new List<KeyValue>();
            enumerations = new List<Enumeration>();
            allenums = new List<Enumeration>();
            kvapi = 144;
            implementation = "";
            dm_name = "";
            usesCookTimeOp = true;

            // Set DDM holder entities
            cycles = new List<List<Cycle>>();
            cyclesU = new List<Cycle>();
            cyclesL = new List<Cycle>();
            cyclesM = new List<Cycle>();
            cyclesW = new List<Cycle>();
            cyclesD = new List<Cycle>();

            // Build log and window table
            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("Delivery Method");
            results.Columns.Add("OTA Type");
            results.Columns.Add("Node");
            results.Columns.Add("Name");
            results.Columns.Add("OTA Result");

            // Generate tables
            sbind.DataSource = results;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = results;
            DGV_Data.DataSource = sbind;
            //DataGridViewColumn column = DGV_Data.Columns[2];
            //column.Width = 60;
            //DataGridViewColumn column2 = DGV_Data.Columns[4];
            //column2.Width = 40;
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
                    // Process OTA-related messages
                    string sb = System.Text.Encoding.ASCII.GetString(data.Message);
                    lock (writeobj)
                    {
                        ProcessPayload(sb, data.Source.ToString(), "MQTT Message");
                    }
                    break;

                 /*case "iot-2/evt/subscribe/fmt/json":  
                    // Process connection-related messages
                     if (iplist.Count > 0)
                     {
                         string pay = System.Text.Encoding.ASCII.GetString(data.Message);
                        if (pay.Contains("version"))
                        {
                            if (iplist.FirstOrDefault(x => x.IPAddress == data.Source.ToString()) != null)
                            {                                
                                string[] stats = pay.Split(':');

                                if (stats[1].Equals("0"))
                                {
                                    // Allow a moment for widebox to catch up to the logs
                                    Wait(1000);
                                    // Send connection message to the IP that raised the event
                                    byte[] paybytes = Encoding.ASCII.GetBytes("{\"sublist\":[1,144,147]}");
                                    string[] ipad = data.Source.ToString().Split('.');
                                    byte[] ipbytes = new byte[4];
                                    for (int j = 0; j < 4; j++)
                                    {
                                        ipbytes[j] = byte.Parse(ipad[j]);
                                    }
                                    SendMQTT(ipbytes, "iot-2/evt/subscribe/fmt/json", paybytes);
                                    return;
                                }

                                else
                                {   // Already connected so see if this thread is waiting for permission to send next OTA payload
                                    foreach (var member in iplist)
                                    {
                                        if (member.IPAddress.ToString().Equals(data.Source.ToString()))
                                        {
                                            if (!member.Result.Contains("PENDING"))
                                                continue;
                                            else
                                            {
                                                member.Signal.Set();
                                                break;
                                            }
                                        }
                                    }
                                }                                
                            }
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
                    if (member.IPAddress.ToString().Equals(ip))
                    {
                        if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                            continue;
                        else if (member.Result.Contains("Programming"))
                            break;
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
                    if (member.IPAddress.ToString().Equals(ip))
                    {
                        if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
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
                    if (member.IPAddress.ToString().Equals(ip))
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
                        if (member.IPAddress.ToString().Equals(ip))
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
               // IPData ipd = iplist.FirstOrDefault(x => x.IPAddress == parts[0]);
               // listindex = iplist.IndexOf(ipd);

                // Update window with OTA result
                //results.Rows[listindex]["Delivery Method"] = iplist[listindex].Delivery;
                //results.Rows[listindex]["OTA Type"] = iplist[listindex].Type;
                //results.Rows[listindex]["Node"] = iplist[listindex].Node;
                results.Rows[listindex]["OTA Result"] = iplist[listindex].Result;

                if (type.Equals("status"))
                {
                    try
                    {
                        using (StreamWriter sw = File.AppendText(curfilename))
                        {
                            //sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + parts[0] + "," +
                            sw.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "," + "By thread with lock " + iplist[listindex].Signal.WaitHandle.Handle + "," + parts[0] + "," +
                            iplist[listindex].MAC + "," + source + "," +
                            iplist[listindex].Payload + "," +
                            iplist[listindex].Delivery + "," +
                            iplist[listindex].Type + "," +
                            iplist[listindex].Node + "," +
                            iplist[listindex].Name + "," +
                            iplist[listindex].Result);    
                            
                        }
                    }
                    catch { }
                }
            }

            // Push result update
            DGV_Data.Refresh();
            //responses.Clear();
        }
       
        private void BTN_Payload_Click(object sender, EventArgs e)
        {
            if (BTN_Payload.Text == "Run Test List")
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
                        catch
                        {
                            MessageBox.Show("The chosen directory path does not exist. Please browse to a path that DOES exist and try again.", "Error: Directory Path Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                // Begin processing all IPs on list
                if (LB_IPs.Items.Count > 0)

                //DisableWhileRunning();
                {

                    BTN_Payload.Text = "Stop Running";
                    LED_Internet.SetColor(Color.DarkGray);
                    BTN_Add.Enabled = false;
                    BTN_Remove.Enabled = false;
                    BTN_Clr.Enabled = false;
                    TB_LogDir.Enabled = false;
                    TB_Loop.Enabled = false;
                    LB_IPs.Enabled = false;
                    BTN_Import.Enabled = false;
                    BTN_MakeList.Enabled = false;
                    BTN_LogDir.Enabled = false;
                    // RB_MQTT.Enabled = false;
                    //RB_Reveal.Enabled = false;
                    cancel_request = false;
                    ProcessIP();
                }
                

                else
                {
                    MessageBox.Show("No IP Address in IP List. Please populate list and try again.", "Error: No IP in List",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("This will dispose of all running threads and end the OTA list execution. Are you sure you want to exit?",
                                                        "Verify Exiting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    BTN_Payload.Text = "Run Test List";
                    BTN_Add.Enabled = true;
                    BTN_Remove.Enabled = true;
                    BTN_Clr.Enabled = true;
                    TB_LogDir.Enabled = true;
                    TB_Loop.Enabled = true;
                    LB_IPs.Enabled = true;
                    BTN_Import.Enabled = true;
                    BTN_MakeList.Enabled = true;
                    BTN_LogDir.Enabled = true;
                    // RB_MQTT.Enabled = true;
                    //RB_Reveal.Enabled = true;
                    responses.Clear();
                    cancel_request = true;
                    //Console.WriteLine("Thread with corresponding lock " + Thread.CurrentThread.Name + " closed.");
                    //return;
                    //IPData ipd = iplist.FirstOrDefault(x => x.IPAddress == parts[0]);
                    //int listindex = iplist.IndexOf(ipd);
                    try
                    {
                        foreach (var member in iplist)
                        {
                            //if (iplist[member.TabIndex].Signal.)
                            //{
                            Console.WriteLine("Thread with corresponding lock " + iplist[member.TabIndex].Signal.WaitHandle.Handle + " closed.");
                            iplist[member.TabIndex].Signal.Set();

                            //return;
                            // }
                            //Thread.CurrentThread.Interrupt();
                        }
                        //Console.WriteLine("Thread with corresponding lock " + Thread.CurrentThread.Name + " closed.");
                        return;
                        //EnableWhileRunning();
                        //return;
                    }

                    catch
                    {
                        return;
                    }


                }
                else
                    return;
            }               
                
        } 

        public void SendMQTT(byte[] ipbytes, string topic, byte[] paybytes, string mac)
        {
            System.Net.IPAddress ip = new System.Net.IPAddress(ipbytes);

            // From byte array to string
            string ips = Encoding.UTF8.GetString(ipbytes, 0, ipbytes.Length);

            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ips);
            
            //If IP Address exists, send to that IP
           // if (cai != null)
                //Semd payload
                WifiLocal.SendMqttMessage(ip, topic, paybytes);
            /*else
            {
                //Otherwise IP changed, use MAC address to map to new IP
                ConnectedApplianceInfo cai_m = cio.FirstOrDefault(x => x.MacAddress == mac);
                if (cai != null)
                {
                    string[] ipad = cai_m.IPAddress.Split('.');
                    byte[] n_ipbytes = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        n_ipbytes[j] = byte.Parse(ipad[j]);
                    }
                    System.Net.IPAddress n_ip = new System.Net.IPAddress(n_ipbytes);
                    WifiLocal.SendMqttMessage(n_ip, topic, paybytes);
                }
                else
                {
                    MessageBox.Show("OTA target IP Address of " + cai.IPAddress + "was changed and unable to be remapped. Ending OTA attempts and " +
                        "closing corresponding thread.", "Error: Unable to change IP Address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Thread.CurrentThread.Abort();
                    return;
                }
            }*/

        }

        public void SendRevelation(string ips, byte[] paybytes)
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
                     Wait(2000);
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
                        Wait(2000);
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
        //Wait 
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
        private static void EndThread(object source, ElapsedEventArgs args)
        {
            Console.WriteLine("End Thread was called by " + Thread.CurrentThread.Name;
            //Thread.CurrentThread.Interrupt();
            //EnableWhileRunning();
        }
        public void RunCycle(IPData ipd, byte[] ipbytes)
        {
            //Send subscribe message before sending cycle
            byte[] paybytes = Encoding.ASCII.GetBytes("{\"sublist\":[1,144,147]}");
            SendMQTT(ipbytes, "iot-2/cmd/subscribe/fmt/json", paybytes, ipd.MAC);
            Wait(5000);

            //paybytes = Encoding.ASCII.GetBytes(ipd.MQTTPay);for (int i = 0; i < bytes.Length; i++)

            byte[] bytes = new byte[ipd.MQTTPay.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(ipd.MQTTPay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
                
            }
                        
            SendMQTT(ipbytes, "iot-2/cmd/cc_SetKvp/fmt/binary", bytes, ipd.MAC);
            //Wait(ipd.Wait);
            Wait(CYCLEWAIT);
        }
        public void RunTask(string ip, ManualResetEventSlim sig, string ipindex)
        {
            int loop = int.Parse(TB_Loop.Text);
            //if (loop < 1)
                //loop = 1;

            for (int i = 0; i <= loop; i++)
            {
                int looptestREMOVE = 0;
                //if (TaskQ.Peek().ToString().Contains(ip))
                foreach (IPData ipd in iplist)
                {

                    if (ipd.IPAddress.Equals(ip) && Thread.CurrentThread.Name.ToString().Equals(ipindex))
                    {
                        ipd.IPIndex = Int32.Parse(ipindex);
                        ipd.Signal = sig;
                        ipd.TabIndex = iplist.IndexOf(ipd);
                        //string[] item = TaskQ.Dequeue().ToString().Split('\t');

                        //Force each thread to live only two hours (process somehow got stuck)
                        System.Timers.Timer timer = new System.Timers.Timer();
                        timer.Interval = TMAX;
                        timer.Elapsed += new ElapsedEventHandler(EndThread);
                        timer.Start();

                        //Parse payload into byte array
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
                        {
                            if (ipd.Type.Equals("Cycle"))
                                
                                RunCycle(ipd, ipbytes);
                            
                            else
                            
                                SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, ipd.MAC);                            
                        }

                        else
                        {
                            SendRevelation(ipd.IPAddress, paybytes);
                        }

                        // Spinwait until our result gets updated to a pass or fail
                        //while (!results.Rows[index][4].ToString().Contains("PASS") || !results.Rows[index][4].ToString().Contains("FAIL")) ;
                        //SpinWait.SpinUntil(() => isCompleted == true);
                        Console.WriteLine("This is the info before calling this lock " + ipd.Signal.WaitHandle.Handle + " with this thread name " + Thread.CurrentThread.Name +
                            " and this IP Index (from thread order) " + ipd.IPIndex + " for this IP Address " + ipd.IPAddress + ".");
                        // Set wait signal to be unlocked by thread after job is complete (result seen from log)
                        sig.Wait();
                        lock (cancelobj)
                        {
                            if (cancel_request)
                            {
                                Console.WriteLine("Thread with corresponding lock " + ipd.Signal.WaitHandle.Handle + " and name "
                                                 + Thread.CurrentThread.Name + " closed.");
                                //Thread.CurrentThread.Abort();
                                return;
                            }
                        }
                        if (!ipd.Type.Equals("Cycle"))
                        {
                            Wait(RECONWAIT); //Wait one minute for cycle to run
                            byte[] subbytes = Encoding.ASCII.GetBytes("{\"sublist\":[1,144,147]}");
                            SendMQTT(ipbytes, "iot-2/cmd/subscribe/fmt/json", subbytes, ipd.MAC);
                            Wait(REBMAX); //12 minute timeout to allow worst case time for reconnecting to MQTT broker after booting out of IAP
                        }
                        Console.WriteLine("Thread " + Thread.CurrentThread.Name + " ran to conclusion.");
                        //break;
                    }
                    else
                    {
                        continue;
                    }

                }
                
                Console.WriteLine("Loop count is " + looptestREMOVE + " out of " + loop + ".");
                looptestREMOVE++;
            }   
        } 
        
        void ProcessIP()
        {
            //TaskQ.Clear();

            foreach (IPData ipd in iplist)
            //Parallel.ForEach(iplist, ipd =>
            {
                if (cancel_request)
                    return;
                // Enable Trace for IP address in list
                if (!TraceConnect(ipd.IPAddress, ipd.Payload, ipd.Type, ipd.Delivery))
                    return;
                //string taskentry = "";
                //lock (lockObj)
                //{


                //}
                //taskentry = ipd.IPAddress + "\t" + ipd.Payload + "\t" + ipd.Type + "\t" + ipd.Delivery;
                //TaskQ.Enqueue(taskentry);
            }

            for (int i = 0; i < LB_IPs.Items.Count; i++)
            {
                string ip = LB_IPs.Items[i].ToString();
                string number = "";
                ManualResetEventSlim sig = new ManualResetEventSlim();
                signal.Add(sig);
                //Console.WriteLine("This is the info on generating the lock " + sig  +
                //       " and this IP Index (from thread order) " + i + " for this IP Address " + ip + ".");

                Thread th = new Thread(() => RunTask(ip, sig, number));
                th.Name = i.ToString();
                number = th.Name;
                th.IsBackground = true;
                th.Start();
                //th.Join();   
                /*new Thread(() =>
                {
                    //Thread.CurrentThread.IsBackground = true;
                    Name = i.ToString();
                    RunTask(ip, sig, i.ToString());
                }).Start();*/
            }
            // });
        }

        bool RevelationConnect(ConnectedApplianceInfo cai)
        {
            int traceattempt = 0;

            while (traceattempt < ATTEMPTMAX)
            {
                try
                {
                    if (cancel_request)
                        return false;

                    traceattempt++;
                    if (cai != null)
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
                        else
                        {
                            //If the Trace is enabled and Revelation is also connected, close the Revelation connection
                            if (cai.IsRevelationConnected)
                            {
                                WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                                //WifiLocal.Close(cai);
                                Wait(2000);
                            }
                            return true;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Trace was not able to start.  Please verify the socket can be " +
                                "opened and it is not in use (UITracer is not running). You may need to close" +
                                "Widebox and try again.", "Error: Unable to start Trace",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //WifiLocal.CloseAll(true);
                        return false;
                    }

                }


                catch
                {
                    MessageBox.Show("Trace was not able to start.  Please verify the socket can be " +
                                    "opened and it is not in use (UITracer is not running). You may need to close" +
                                "Widebox and try again.", "Error: Unable to start Trace",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //WifiLocal.CloseAll(true);
                    return false;
                }
            }


            return false;
        }

        bool TraceConnect(string ip, string pay, string type, string delivery) 
        {
            //gets the list of appliances from WifiBasic
            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;

            //Selects the appliance based on IP address
            ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);
            //CycleWifi(cai);
            Wait(2000);
            //If an appliance with the specified IP address is found in the list
            if (!RevelationConnect(cai))
            {
                if (cancel_request)
                    return false;
                //First round of attempts failed, close data / open data and try again
                CycleWifi(cai);

                if (!RevelationConnect(cai))
                {
                    MessageBox.Show("Revelation/Trace was not able to start even after retry. Please close Venom and WideBox and try again.",
                                    "Error: WideBox issue prevents running Plugin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            responses.Add(ip + "\t" + pay + "\t" + delivery + "\t" + type + "\t" + "PENDING");

            return true;

        }
       
        public bool CycleWifi(ConnectedApplianceInfo cai)
        {
            /*using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }*/
            string localIP = WifiLocal.Localhost.ToString();
            try
            {
                // Close all WifiBasic connections
                WifiLocal.CloseAll(true);
                //WifiLocal.Close(cai);
                Wait(2000);
                // Get new cert to restart WifiBasic connections
                CertManager.CertificateManager certMgr = new CertManager.CertificateManager();

                // Restart Wifi Connection
                if (certMgr.IsLocalValid)
                {
                    WifiLocal.SetWifi(System.Net.IPAddress.Parse(localIP), certMgr.GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020));
                    Wait(5000);
                    return true;
                }
            }

            catch
            {
                //WifiLocal.SetWifi(System.Net.IPAddress.Parse(cai.IPAddress), new CertManager.CertificateManager().GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020));
                return false;
            }

            return false;
            
        }

        private void BTN_Add_Click(object sender, EventArgs e)
        {
           /* string localpay = ""; // TB_Payload.Text;
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
            //RB_Reveal.Checked = false;*/
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
            //LED_Internet.SetColor(Color.DarkGray);
            //SetLED(TB_IP.Text);
        }

        private void BTN_MakeList_Click(object sender, EventArgs e)
        {
            try
            {
                plist.Show();
            }
            catch
            {

                plist = new PayList(this, this.WideLocal, this.WifiLocal);
                plist.Show();
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
                    var ipskipped = new List<string>();

                    while (!reader.EndOfStream)
                    {
                        value = reader.ReadLine().Split('\t');
                        System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                        ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == value[0]);
                        //CycleWifi(cai);
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
                            newip.Node = value[2];
                            newip.Type = value[3];
                            newip.MQTTPay = value[4];
                            newip.Name = value[5];
                           // newip.Wait = int.Parse(value[7]);

                            // Update window for added IP
                            DataRow dr = results.NewRow();
                            dr["IP Address"] = newip.IPAddress;
                            dr["OTA Payload"] = newip.Payload;
                            dr["Delivery Method"] = "MQTT";
                            dr["OTA Type"] = newip.Type;
                            dr["Node"] = newip.Node;
                            dr["Name"] = newip.Name;
                            dr["OTA Result"] = "PENDING";
                            results.Rows.Add(dr);
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
                        if (String.IsNullOrWhiteSpace(badips))
                            badips = "EMPTY LINES";
                        MessageBox.Show("Import skipped a total of " + skipped + " line(s) for IP Address(es) " +
                             badips + " due to NOT being listed in Wifibasic OR the import file having EMPTY lines. Please verify the IP Address(es) or empty lines " +
                            "that were skipped are connected and listed in Wifibasic or expected, then retry importing.", "Error: WifiBasic IP Address(es) Not Found or File Has Empty Lines",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //Convert string representation of data types into a standard format
        private string SwitchLength(string type)
        {
            string length = "";
            switch (type)
            {
                case "String":
                    length = "string";
                    break;
                case "IntegerUnsigned8Bit":
                    length = "uint8";
                    break;
                case "BEIntegerUnsigned16Bit":
                    length = "uint16";
                    break;
                case "BEIntegerSigned16Bit":
                    length = "int16";
                    break;
                case "BEInteger16Bit":
                    length = "int16";
                    break;
                case "BEIntegerUnsigned32Bit":
                    length = "uint32";
                    break;
                case "BEIntegerSigned32Bit":
                    length = "int32";
                    break;
                case "BEInteger32Bit":
                    length = "int32";
                    break;
                case "IntegerSigned8Bit":
                    length = "int8";
                    break;
                case "Integer8Bit":
                    length = "int8";
                    break;
                case "Boolean":
                    length = "boolean";
                    break;
                case "enum":
                    length = "uint8";
                    break;
                default:
                    length = "string";
                    break;
            }
            return length;
        }
        private bool XLSKeyValues(string filename)
        {
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=No'", filename);
            string query;// = string.Format("SELECT * FROM [{0}$]", "Keys");
            DataTable data = new DataTable();
            DataTable dt = new DataTable();
            string[] excelSheets = null;
            bool loaded = true;
            //string implementation = "";

            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    //OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    //adapter.Fill(data);
                    con.Close();
                }

                excelSheets = new string[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString().Replace("'", "").Replace("$", "");
                    i++;
                }

                string selection = "";
                if (Array.IndexOf(excelSheets, "Definitions") >= 0)
                {
                    selection = "Definitions";
                }
                else
                {
                    TabSelect ts = new TabSelect("Select the tab containing the KVP definitions", excelSheets);
                    ts.ShowDialog();
                    selection = ts.SelectedTab;
                }

                query = string.Format("SELECT * FROM [{0}$]", selection);

                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    //dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(data);
                    con.Close();
                }

                keyValues.Clear();

                if (selection == "Keys")
                {
                    bool start = false;

                    foreach (DataRow d in data.Rows)
                    {
                        if (start && d[0].ToString() != "")
                        {
                            KeyValue kv = new KeyValue(d[0].ToString(), "", "", "", d[2].ToString(), d[10].ToString(), d[7].ToString().Substring(2), kvapi, "", "");
                            keyValues.Add(kv);
                        }
                        if (d[0].ToString() == "Class")
                        {
                            start = true;
                        }
                    }
                }
                else
                {
                    int instanceCol = -1;
                    int lengthCol = -1;
                    int keyCol = -1;
                    int platCol = 0;
                    int descCol = 0;
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        if (data.Rows[0][j].ToString() == "Instance") { instanceCol = j; }
                        if (data.Rows[0][j].ToString() == "Payload Data Type") { lengthCol = j; }
                        if (data.Rows[1][j].ToString() == "Hex") { keyCol = j; }
                        if (data.Rows[0][j].ToString() == "Implementation") { platCol = j; }
                        if (data.Rows[0][j].ToString() == "Description") { descCol = j; }
                    }

                    if (instanceCol > 0 && lengthCol > 0 && keyCol > 0)
                    {
                        if (platCol > 0)
                        {
                            string[] plats = new string[data.Columns.Count - platCol];
                            //plats[0] = "All";
                            for (int m = 0; m < plats.Length; m++)
                            {
                                plats[m] = data.Rows[1][platCol + m].ToString().Replace("\n", "");
                            }

                            TabSelect ts1 = new TabSelect("Select the platform implementation", plats);
                            ts1.ShowDialog();
                            implementation = ts1.SelectedTab;

                            if (!(implementation == "All" || implementation == ""))
                            {
                                platCol = platCol + ts1.SelectedIndex;
                            }
                            else
                            {
                                platCol = 0;
                            }
                        }

                        for (int k = 2; k < data.Rows.Count; k++)
                        {
                            if (data.Rows[k][lengthCol].ToString() != ""/* && (data.Rows[k][platCol].ToString() == "X" || platCol == 0)*/)
                            {
                                KeyValue kv = new KeyValue(selection, data.Rows[k][instanceCol - 2].ToString(), data.Rows[k][instanceCol - 1].ToString(), data.Rows[k][instanceCol].ToString(), data.Rows[k][instanceCol].ToString() + "_" + data.Rows[k][instanceCol + 1].ToString(), SwitchLength(data.Rows[k][lengthCol].ToString()), data.Rows[k][keyCol].ToString().Substring(2), kvapi, data.Rows[k][descCol].ToString(), data.Rows[k][descCol - 1].ToString());
                                if (kv.KeyID != "00000000")
                                {
                                    kv.isSet = data.Rows[k][keyCol + 2].ToString() == "TRUE" || data.Rows[k][keyCol + 2].ToString() == "1" ? true : false;
                                    keyValues.Add(kv);
                                    kv.IsUsed = data.Rows[k][platCol].ToString() == "X" ? true : false;
                                    kv.IsState = data.Rows[k][descCol - 2].ToString() == "TRUE" || data.Rows[k][descCol - 2].ToString() == "1" ? true : false;
                                    if (kv.KeyID == "030D0002")
                                    {
                                        usesCookTimeOp = kv.IsUsed;
                                    }
                                }
                            }
                        }



                    }
                    else
                    {
                        MessageBox.Show("Unable to load key values from selected file");
                        loaded = false;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to load key values from selected file.  Make sure the Data Model file has not been modified and you have either Microsoft Excel or the Office System Driver (found in the Help link) installed.");
                loaded = false;
            }

            if (loaded)
            {
                string selection2 = "";
                if (Array.IndexOf(excelSheets, "Enumerations") >= 0)
                {
                    selection2 = "Enumerations";
                }
                else
                {
                    TabSelect ts2 = new TabSelect("Select the tab containing the Enumerations", excelSheets);
                    ts2.ShowDialog();
                    selection2 = ts2.SelectedTab;
                }

                query = string.Format("SELECT * FROM [{0}$]", selection2);
                DataTable data2 = new DataTable();

                try
                {
                    using (OleDbConnection con = new OleDbConnection(connectionString))
                    {
                        con.Open();
                        OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                        adapter.Fill(data2);
                    }

                    int platCol = -1;
                    if (!(implementation == "All" || implementation == ""))
                    {
                        for (int i = 0; i < data2.Columns.Count; i++)
                        {
                            if (data2.Rows[1][i].ToString().Replace("\n", "") == implementation)
                            {
                                platCol = i;
                                i = data2.Columns.Count;
                            }
                        }
                    }

                    enumerations.Clear();

                    if (kvapi == 144 || kvapi == 147)
                    {
                        string curEntity = "";
                        string curAtt = "";
                        string fixedname = "";
                        Enumeration curEnum = new Enumeration();
                        Enumeration cloneEnum = new Enumeration();

                        for (int j = 2; j < data2.Rows.Count; j++)
                        {
                            if (data2.Rows[j][0].ToString() != "")
                            {
                                curEntity = data2.Rows[j][0].ToString();
                            }
                            if (data2.Rows[j][1].ToString() != "")
                            {
                                curAtt = data2.Rows[j][1].ToString();
                                /*foreach (KeyValue kv in keyValues)
                                {
                                    if (kv.Entity == curEntity && kv.DisplayName.Substring(kv.DisplayName.LastIndexOf('_') + 1) == curAtt)
                                    {
                                        kv.EnumName = curEnum;
                                        curEnum.UsedBy += curEntity + "." + curAtt + ";";
                                        cloneEnum.UsedBy += curEntity + "." + curAtt + ";";
                                    }
                                }*/
                            }
                            if (!(data2.Rows[j][2].ToString() == ""))
                            {
                                //curEnum.Name = data2.Rows[j][2].ToString();
                                bool found = false;

                                foreach (Enumeration e in enumerations)
                                {
                                    fixedname = data2.Rows[j][2].ToString().IndexOf(':') > 0 ? data2.Rows[j][2].ToString().Substring(0, data2.Rows[j][2].ToString().IndexOf(':')) : data2.Rows[j][2].ToString();
                                    if (e.Name == curEntity + "." + fixedname)
                                    {
                                        found = true;
                                        curEnum = e;
                                        cloneEnum = allenums[enumerations.IndexOf(e)];
                                        curEnum.UsedBy = curEnum.UsedBy.Contains(curEntity + "." + curAtt) ? curEnum.UsedBy : curEnum.UsedBy + curEntity + "." + curAtt + ";";
                                        cloneEnum.UsedBy = cloneEnum.UsedBy.Contains(curEntity + "." + curAtt) ? cloneEnum.UsedBy : cloneEnum.UsedBy + curEntity + "." + curAtt + ";";
                                        //if (platCol < 0 || data2.Rows[j][platCol].ToString() == "X")
                                        {
                                            curEnum.insertEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][4].ToString());
                                            curEnum.enableEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][platCol].ToString() == "X");
                                        }
                                        foreach (KeyValue kv in keyValues)
                                        {
                                            if ((kv.Entity == curEntity || kv.Instance == curEntity) && kv.DisplayName.Substring(kv.DisplayName.LastIndexOf('_') + 1) == curAtt)
                                            {
                                                kv.EnumName = curEnum;
                                                //curEnum.UsedBy = curEnum.UsedBy.Contains(curEntity + "." + curAtt) ? curEnum.UsedBy : curEnum.UsedBy + curEntity + "." + curAtt + ";";
                                                //cloneEnum.UsedBy = cloneEnum.UsedBy.Contains(curEntity + "." + curAtt) ? cloneEnum.UsedBy : cloneEnum.UsedBy + curEntity + "." + curAtt + ";";
                                            }
                                        }
                                    }
                                }
                                if (!found)
                                {
                                    curEnum = new Enumeration(curEntity + "." + data2.Rows[j][2].ToString());
                                    cloneEnum = new Enumeration(curEntity + "." + data2.Rows[j][2].ToString());
                                    //if (platCol < 0 || data2.Rows[j][platCol].ToString() == "X")
                                    {
                                        curEnum.insertEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][4].ToString());
                                        curEnum.enableEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][platCol].ToString() == "X");
                                        foreach (KeyValue kv in keyValues)
                                        {
                                            if ((kv.Entity == curEntity || kv.Instance == curEntity) && kv.DisplayName.Substring(kv.DisplayName.LastIndexOf('_') + 1) == curAtt)
                                            {
                                                kv.EnumName = curEnum;
                                                curEnum.UsedBy = curEnum.UsedBy.Contains(curEntity + "." + curAtt) ? curEnum.UsedBy : curEnum.UsedBy + curEntity + "." + curAtt + ";";
                                                cloneEnum.UsedBy = cloneEnum.UsedBy.Contains(curEntity + "." + curAtt) ? cloneEnum.UsedBy : cloneEnum.UsedBy + curEntity + "." + curAtt + ";";
                                            }
                                        }
                                    }
                                    cloneEnum.insertEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][4].ToString());
                                    enumerations.Add(curEnum);
                                    allenums.Add(cloneEnum);
                                }
                            }
                            else
                            {
                                if (data2.Rows[j][3].ToString() != "")
                                {
                                    // if (platCol < 0 || data2.Rows[j][platCol].ToString() == "X")
                                    {
                                        curEnum.insertEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][4].ToString());
                                        curEnum.enableEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][platCol].ToString() == "X");
                                        foreach (KeyValue kv in keyValues)
                                        {
                                            if ((kv.Entity == curEntity || kv.Instance == curEntity) && kv.DisplayName.Substring(kv.DisplayName.LastIndexOf('_') + 1) == curAtt)
                                            {
                                                kv.EnumName = curEnum;
                                                curEnum.UsedBy = curEnum.UsedBy.Contains(curEntity + "." + curAtt) ? curEnum.UsedBy : curEnum.UsedBy + curEntity + "." + curAtt + ";";
                                                cloneEnum.UsedBy = cloneEnum.UsedBy.Contains(curEntity + "." + curAtt) ? cloneEnum.UsedBy : cloneEnum.UsedBy + curEntity + "." + curAtt + ";";
                                            }
                                        }
                                    }
                                    cloneEnum.insertEnum(int.Parse(data2.Rows[j][3].ToString()), data2.Rows[j][4].ToString());
                                }

                            }

                        }
                    }
                    else
                    {
                        bool start = false;
                        Enumeration en;
                        for (int col = 0; col < data2.Columns.Count; col += 3)
                        {
                            en = new Enumeration();
                            start = true;
                            for (int row = 0; row < data2.Rows.Count; row++)
                            {
                                if (data2.Rows[row][col].ToString() == "")
                                {
                                    start = true;
                                    if (en.Enums.Count > 0 && en.Name != null)
                                    {
                                        enumerations.Add(en);
                                    }
                                    en = new Enumeration();
                                }
                                else
                                {
                                    if (start)
                                    {
                                        en.Name = data2.Rows[row][col].ToString();
                                        start = false;
                                    }
                                    else
                                    {
                                        en.insertEnum(int.Parse(data2.Rows[row][col].ToString().Substring(2, 2), NumberStyles.AllowHexSpecifier), data2.Rows[row][col + 1].ToString());
                                        if (row == data2.Rows.Count - 1)// if this enumeration goes to the bottom of the file
                                        {
                                            enumerations.Add(en);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    CorrectEnums();

                }
                catch
                {
                    MessageBox.Show("Could not load Enumerations from selected file");
                }
            }
            return loaded;
        }

        private void JSONKeyValues(string filename)
        {
            string ddm = new StreamReader(filename).ReadToEnd();
            ddm = ddm.Replace("\"", "");
            ddm = ddm.Replace(" ", "");
            ddm = ddm.Replace("\n", "");
            int id = ddm.IndexOf("_id:") + "_id:".Length;
            implementation = ddm.Substring(id, ddm.IndexOf(",", id) - id);
            implementation = Regex.Match(implementation, @"(?!_)[^_]*_[^_]*$").Value;
            id = ddm.IndexOf("Category:") + "Category:".Length;
            dm_name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ddm.Substring(id, ddm.IndexOf(",", id) - id).ToLower());
            ddm = ddm.Substring(ddm.IndexOf("attributes:[") + "attributes:[".Length);
            int level = 0;
            char[] chars = ddm.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '{')
                {
                    level++;
                    if (level > 1)
                    {
                        chars[i] = '[';
                    }
                }
                if (chars[i] == '}')
                {
                    level--;
                    if (level > 0)
                    {
                        chars[i] = ']';
                    }
                }

            }
            ddm = new string(chars);
            ddm = ddm.Replace("},", "`");
            string[] keys = ddm.Split('`');
            foreach (string a in keys)
            {
                string b = a.Substring(1);
                if (b.Contains("["))
                {
                    while (b.IndexOf(':', b.IndexOf("[")) > 0 && (b.IndexOf(':', b.IndexOf("["))) < b.IndexOf(']'))
                    {
                        try
                        {
                            int c = b.IndexOf(':', b.IndexOf("["));
                            b = b.Remove(c, 1);
                            b = b.Insert(c, ";");
                            c = b.IndexOf(',', b.IndexOf("["));
                            if (c < b.IndexOf(']'))
                            {
                                b = b.Remove(c, 1);
                                b = b.Insert(c, ";");
                            }
                        }
                        catch { }
                    }
                }
                string[] attributes = b.Split(',');
                Dictionary<string, string> attdict = new Dictionary<string, string>();
                foreach (string att in attributes)
                {
                    try
                    {
                        string[] pair = att.Split(':');
                        attdict.Add(pair[0], pair[1]);
                    }
                    catch { }
                }

                try
                {
                    if (attdict["Key"] != "0x00000000")
                    {
                        KeyValue k = new KeyValue("Definitions", attdict["M2MAttributeName"].Substring(0, attdict["M2MAttributeName"].IndexOf("_")), "", attdict["Instance"], attdict["Instance"] + "_" + attdict["AttributeName"], SwitchLength(attdict["DataType"]), attdict["Key"].Substring(2), 144, "", "");
                        k.isSet = attdict["DeviceIO"] == "RW" || attdict["DeviceIO"] == "WO";
                        k.IsUsed = true;
                        if (attdict["DataType"] == "enum")
                        {
                            string[] enums = attdict["EnumValues"].Substring(1, attdict["EnumValues"].Length - 2).Split(';');
                            if (enums[0] != "")
                            {
                                Enumeration e = new Enumeration();
                                for (int i = 0; i < enums.Length; i += 2)
                                {
                                    e.insertEnum(int.Parse(enums[i]), enums[i + 1]);
                                    e.enableEnum(int.Parse(enums[i]), true);
                                }
                                enumerations.Add(e);
                                k.EnumName = e;
                            }
                        }
                        keyValues.Add(k);
                    }
                }
                catch { }
            }
            CorrectEnums();
        }

        //For key values with multiple possible enum sets, cycle through the one currently assigned to it and see if any are used, otherwise swap it with an enum that is used
        private void CorrectEnums()
        {
            bool swapped = false;
            for (int i = 0; i < keyValues.Count; i++)
            {
                if (keyValues[i].IsUsed && keyValues[i].EnumName != null)
                {
                    bool used = false;
                    foreach (bool b in keyValues[i].EnumName.IsUsed)
                    {
                        if (b || swapped)
                        {
                            used = true;
                            swapped = false;
                            break;
                        }
                    }
                    if (!used)
                    {
                        foreach (Enumeration en in enumerations)
                        {
                            if (en.UsedBy.Contains(keyValues[i].AttributeName) && keyValues[i].EnumName != en)
                            {
                                keyValues[i].EnumName = en;
                                i--;
                                swapped = true;
                                break;
                            }
                        }
                    }
                }
                if (keyValues[i].EnumName != null)
                {
                    keyValues[i].EnumName.TrimEnums();
                }
            }
        }
        private void BTN_Auto_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will automatically create a new test plan run from the current IP list. " +
                "This will then clear the current payload list and update the table accordingly. " +
                "Press Yes to Create or No to Cancel.", "Verify Full Clear and Auto Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                /*foreach (IPData ipd in iplist)
                {
                    if (ipd.Type.Equals("HMI")) ;

                    if (ipd.Type.Equals("ACU")) ;

                    if (ipd.Type.Equals("WiFi")) ;

                    if (ipd.Type.Equals("Multi")) ;

                }*/

                /* OpenFileDialog ofd = new OpenFileDialog();
                 ofd.Filter = "Data Model/DDM Definitions (*.xlsx, *.json)|*.xlsx; *.json";
                 ofd.ShowDialog();
                 if (ofd.FileName != "")
                 {
                     keyValues.Clear();
                     enumerations.Clear();
                     //settings.Clear();
                     //setLabels.Clear();
                     cyclesL.Clear();
                     cyclesM.Clear();
                     cyclesU.Clear();
                    // PAN_Settings.Controls.Clear();
                     kvapi = 144;
                     bool loaded = true;
                     if (ofd.FileName.EndsWith("xlsx"))
                     {
                         loaded = XLSKeyValues(ofd.FileName);
                         try
                         {
                             dm_name = ofd.SafeFileName.Substring(0, ofd.SafeFileName.LastIndexOf(' ')).Replace("API 144 Data Model Definition - ", "");
                         }
                         catch
                         {
                             dm_name = "";
                         }
                     }
                     else
                     {
                         JSONKeyValues(ofd.FileName);
                     }
                     if (loaded)
                     {                        
                         //xc = new XCategory(keyValues, this);

                         if (Array.IndexOf(categoryList, dm_name) < 0)
                         {
                             TabSelect dm = new TabSelect("Please select Data Model category", categoryList);
                             dm.ShowDialog();
                             dm_name = dm.SelectedTab;
                         }

                         //TB_Cap.Text = dm_name + " - " + implementation;
                         switch (dm_name)
                         {
                             case "Laundry":
                                 cycles.Add(cyclesW);
                                 cycles.Add(cyclesD);
                                 break;
                             default:
                                 cycles.Add(cyclesU);
                                 cycles.Add(cyclesL);
                                 cycles.Add(cyclesM);
                                 break;
                         }*/

                /*SetupLabels();
                CyclesFromDM(keyValues);
                SettingsLabels();
                SwitchCycles(0);
            }

            }*/

            }
        }

        private void Venom_Load(object sender, EventArgs e)
        {
            TB_Loop.Text = "0";
        }

        /*private byte[] SetStartDisplay(bool start, bool modify)
        {
            try
            {
                //string zeroes = "00000000";
                string startdisp = start ? "02" : "03";
                startdisp = modify ? "04" : startdisp;
                string timestartdisp = start ? "02" : "05";
                int endfunc = CBO_End.SelectedIndex < 0 ? 0 : CBO_End.SelectedIndex;

                string mes = cycles[TAB_Platform_Cooking.SelectedIndex][CBO_Cycles.SelectedIndex].KeyID + cycles[TAB_Platform_Cooking.SelectedIndex][CBO_Cycles.SelectedIndex].Enum.ToString("X2");
                mes = TB_Temp.Enabled && TB_Temp.Text != "" && LBL_SetTemp.Text == "Temp" ? mes + tempKey + FtoCx10(TB_Temp.Text) : mes;
                mes = TB_Temp.Enabled && TB_Temp.Text != "" && LBL_SetTemp.Text == "Power" ? mes + powKey + int.Parse(TB_Temp.Text).ToString("X2") : mes;

                mes = TB_Time.Enabled && TB_Time.Text != "" ? mes + timeKey + ((int)(double.Parse(TB_Time.Text) * 60)).ToString("X8") : mes;
                mes = TB_Time.Enabled && TB_Time.Text != "" && !modify && usesCookTimeOp ? mes + timeOpKey + timestartdisp : mes;

                mes = TB_Probe.Enabled && TB_Probe.Text != "" && TAB_Platform_Cooking.SelectedIndex != 2 ? mes + probeAmtKey + FtoCx10(TB_Probe.Text) : mes;
                mes =  TB_Probe.Text != "" && TAB_Platform_Cooking.SelectedIndex == 2 ? mes + probeAmtKey + int.Parse(TB_Probe.Text.ToString()).ToString("X4") : mes;
                //mes = CBO_Amt.Enabled && CBO_Amt.SelectedItem != null
                mes = CBO_End.SelectedItem != null ? mes + cpltKey + endfunc.ToString("X2") : mes;

                mes = TB_Delay.Enabled && TB_Delay.Text != "" && TAB_Platform_Cooking.SelectedIndex != 2 ? mes + delayKey + (int.Parse(TB_Delay.Text) * 60).ToString("X8") : mes;
                mes = CBO_Doneness.Visible && CBO_Doneness.SelectedItem != null ? mes + doneKey + CBO_Doneness.SelectedIndex.ToString("X2") : mes;

                mes += opKey + startdisp;
                mes = CBO_Cycles.SelectedItem.ToString().Contains("Sabbath") ? mes += sabKey + "01" : mes;

                byte[] bytes = MakeBytes(mes, true);

                return bytes;
            }
            catch
            {
                MessageBox.Show("Input contains invalid arguments");
                return null;
            }

        }*/

    }
}
