﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using WideBoxLib;
using WirelessLib;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Drawing;

namespace VenomNamespace
{
    public partial class Venom : WideInterface
    {
        public int AUTOINDEX = 0;
        public int AUTOCNT = 6; //Amount of times OTA payload combinations (upgrade --> downgrade) aka X OTAs will be sent total
        public int TESTCASEMAX = 26; //Max test cases that can be automated, can be changed if some skipped
        public int NODECASEMAX = 23; //Max automated test case per node
        public static int ATTEMPTMAX = 3;
        public static int MQTTMAX = 25;
        public static int TTFCNT = 4;
        public static int CYCGO = 0;    //Cycles fire on autogen iteration i=0
        public static int TMAX = 90 * 60000; //OTA max thread time in ms before the thread needs to end (got stuck)
        public static int CYCWAIT = 1 * 30000; //Amount of time to let cycle run
        public static int TTFWAIT = 1 * 30000; //Amount of time to wait for TTF result
        public static int RECONWAIT = 1 * 60000; //MQTT max reconnect timer
        public static int LASTITER = 5;
        public const byte API_NUMBER = 0;
        public int timeout = 0; //REMOVE ME
        
        //Global timer
        public Stopwatch g_time = new Stopwatch();

        // Entities used to store log and window data
        public DataTable results;
        public BindingSource sbind = new BindingSource();

        // Entities used to store the list of targeted IPs and their progress
        public List<IPData> iplist;
        public List<Thread> waits;
        public PayList plist;
        public AutoGen auto;

        public Random rand = new Random();

        public bool tbeat = false;
        public bool mbeat = false;
        public bool ttf = false;
        public bool cyc = false;
        public bool rerun = false;
        public bool autogen = false;
        public bool cycstart = false;
        public bool cancel_request = false;
        public bool skipcyc = false;
        public bool skipttf = false;
        public bool skipgen = false;
        public bool multdload = false;
        public bool indigo = false;
        public bool tourma = false;
        public bool tfound = false;
        public bool symcert = false;

        public string curfilename;
        public string ccuri = "";
        public string vers = "";
        public string ispp = "";
        public string prov = "";
        public string clm = "";
        public string rssi = "";
        public string glblip = "";

        public int autottl = 0;
        public int timeleft = 0;
        public int gstatus = 0;
        public int retcnt = 0;
        public int dlcnt = 0;
        public int prog = 0;
        public int thread_done = 0; //Count of threads that have no more tasks
        public int glob_i = 0;

        public LinkedList<int> gllist = null;

        static object lockObj = new object();
        static object writeobj = new object();
        static object setobj = new object();

        public enum OPCODES { }

        //to access wideLocal use base.WideLocal or simple WideLocal
        public Venom(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            AutoSave = true;


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
            TB_LogDir.Text = Directory.GetCurrentDirectory();

            // Do not allow columns to be sorted
            foreach (DataGridViewColumn column in DGV_Data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Generate lists
            iplist = new List<IPData>();
            //signal = new List<ManualResetEventSlim>();
            waits = new List<Thread>();


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
            string savedExtractedMessage = "";
            if (BTN_Payload.Text != "Stop Running") //Only parse when Venom is running
                return;
            switch (data.Topic)
            {
                

                case "iot-2/evt/isp/fmt/json":
                    string sb = System.Text.Encoding.ASCII.GetString(data.Message);
                    // Process OTA-related messages
                    if (autogen)
                    {
                        if (sb.Contains("progress"))    //Count progress messages sent
                        {
                            prog++;
                            if (tourma)
                            {
                                //RTB_Diag.AppendText("Progress sb is " + sb + Environment.NewLine); RTB_Diag.ScrollToCaret();
                                ProcessPayload(sb, data.Source.ToString(), "MQTT Message", "NA");
                            }
                            return;
                        }

                        if (sb.Contains("tatus"))   //Ignore status messages on Indigo
                        {
                            if (tourma)
                            {
                                //RTB_Diag.AppendText("Status sb is " + sb + Environment.NewLine); RTB_Diag.ScrollToCaret();
                                ProcessPayload(sb, data.Source.ToString(), "MQTT Message", "NA");
                            }

                            return;
                        }

                        if (!mbeat && data.Source.ToString().Equals(glblip))
                        {
                            mbeat = true;
                            lock (writeobj)
                            {
                                ProcessPayload(sb, data.Source.ToString(), "mbeat", "NA");
                            }
                        }
                    }
                    break;

                case "iot-2/evt/cc_Kvp/fmt/binary":
                    savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2")));

                    if (savedExtractedMessage.Equals("000A035D4983AC0109000502"))   //this is wrong
                        lock (writeobj)
                        {
                            ProcessPayload("Programming", data.Source.ToString(), "MQTT Message", "NA");
                        }
                    //Indigo RSSI
                    if (autogen && indigo && !ttf && savedExtractedMessage.Contains("1020001"))
                    {
                        string pdata = savedExtractedMessage.Substring(savedExtractedMessage.Length - 2);
                        string un_data = pdata;
                        int val;
                        if (un_data.Length == 2 && int.Parse(un_data, NumberStyles.AllowHexSpecifier) > 127)
                        {
                            un_data = un_data.PadLeft(4, 'F');
                        }
                        val = unchecked((short)Convert.ToUInt16(un_data, 16));
                        if (val < 0)
                        {
                            lock (writeobj)
                            {
                                ProcessPayload("rssi", data.Source.ToString(), "MQTT Message", val.ToString());
                            }
                        }
                    }
                    break;

                 case "iot-2/evt/cc_SetKvpResult/fmt/binary":
                    //savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2")));
                    if (cyc && autogen)
                        SetResult(data.Message[data.Message.Length - 1]);
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
            if (BTN_Payload.Text != "Stop Running") //Only parse when Venom is running
                return;
            // Filter on relevant OTA topics only
            if (data.ContentAsString.StartsWith("mqtt_out_data:") || data.ContentAsString.StartsWith("mqtt_in_data:")) //may want to use || data.ContentAsString.StartsWith("mqtt-out:") to allow tourma trace to release locks
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
                    ProcessPayload(sb, data.Source.ToString(), "Trace Message", data.ContentAsString);
                }
                return;
            }

            //tbeat check TOURMA
            if (autogen && tourma && !tbeat && data.ContentAsString.StartsWith("hs:"))
            {
                //hs:
                //
                string[] split = data.ContentAsString.Split(':');
                string[] parts = split[1].Split(' ');
                if (IsAllNumeric(parts[0].Replace(" ", "")))
                {
                    if (!ttf)
                        tbeat = true;
                    if (tourma)
                    {
                        //TAKE ME OUT
                        /*LogException("Venom tbeat set as " + tbeat);
                        LogException("Venom sb was " + data.ContentAsString);*/
                    }
                    //RTB_Diag.AppendText("tbeat calling ProcessPayload with sb= " + data.ContentAsString + Environment.NewLine);
                    lock (writeobj)
                    {
                        ProcessPayload("tbeat", data.Source.ToString(), "Trace Message", data.ContentAsString);
                    }
                }

                return;    
            }

            //tbeat check INDIGO
            if (autogen && indigo && !tbeat && data.ContentAsString.StartsWith("web_reveal.c:main:14") && data.ContentAsString.Contains("linkstate"))
            {
                tbeat = true;
                lock (writeobj)
                {
                    ProcessPayload("tbeat", data.Source.ToString(), "Trace Message", data.ContentAsString);
                }

                return;
            }

            //mbeat check  INDIGO
            if (autogen && indigo && !mbeat && data.ContentAsString.Contains("cc="))
            {
                mbeat = true;
                lock (writeobj)
                {
                    ProcessPayload("mbeat", data.Source.ToString(), "Trace Message", data.ContentAsString);
                }

                return;
            }

            //Tourma TTfs
            if (autogen && tourma && ttf && data.ContentAsString.StartsWith("Open session failed URL:"))
                retcnt++;

            if (autogen && tourma && ttf && multdload && data.ContentAsString.StartsWith("runFirmwareUpdatePA"))
            {
                //RTB_Diag.AppendText("dlcnt increment from " + dlcnt + " to dlcnt++" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                dlcnt++;
                multdload = false;

            }

            //Indigo TTFs
            if (autogen && indigo && ttf && data.ContentAsString.StartsWith("paimage__getFromUrl(252): WGET failed, retrying"))
                retcnt++;

            if (autogen && indigo && ttf && multdload)
            {
                /*if (data.ContentAsString.StartsWith("WR entering handleFirmwareUpdateApi(), launching download thread if isp not running")
                    || data.ContentAsString.StartsWith("Unable to start OTA"))
                multcnt++;*/
                if (data.ContentAsString.StartsWith("Image percent download complete:"))
                {
                    string[] str = data.ContentAsString.Replace(" ", "").Split(':');

                    if (gllist == null)
                    {
                        gllist = new LinkedList<int>();
                        gllist.AddFirst(Int32.Parse(str[1]));
                        //Console.WriteLine("gllist is " );
                    }
                    else
                        gllist.AddLast(Int32.Parse(str[1]));

                }
                
                return;
            }

            //Indigo vers
            if (autogen && indigo && !ttf && !cyc && data.ContentAsString.StartsWith("Writing applianceUpdateVersion="))
            {
                string[] str = data.ContentAsString.Split('=');
                string[] parts = str[1].Split(' ');
                //Console.WriteLine("writing applianceupdatevers is trying to write vers as " + parts[0] + " from the old global vers " + vers);
                //RTB_Diag.AppendText("writing applianceupdatevers is trying to write vers as " + parts[0] + " from the old global vers " + vers + Environment.NewLine); RTB_Diag.ScrollToCaret();

                vers = parts[0];

                return;
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
        
        public void ProcessPayload(string sb, string ip, string source, string raw)
        {
            string call = "";
            try
            {
                if (sb.Equals("tbeat"))
                {
                    call = "tbeat";
                    
                    string[] parts = raw.Split(' ');
                    string[] split = parts[6].Split('[');

                    //Shared on both
                    if (!ttf)
                        ccuri = split[1].Replace("]", "");

                    if (indigo) //specialized Indigo format
                    {
                        split = parts[4].Split('[');
                        clm = split[1].Replace("]", "");
                        split = parts[3].Split('[');
                        prov = split[1].Replace("]", "");
                    }
                    else //specialized tourma format
                    {            
                        if (!ttf)
                        {
                            //hs:134448 claimed=2 linkstate=3 tt=1[0] commprot=0 appstate=13 cc_uri[API144_COOKING_V22] cat[10] wise[286250] rssi[-52] v[15.7.0] store[0/10] isp[0] wcars[0-00000000-5ed44fe9]
                            split = parts[9].Split('[');
                            rssi = split[1].Replace("]", "");
                            split = parts[1].Split('=');
                            clm = split[1];
                            split = parts[2].Split('=');
                            prov = split[1];

                            //TAKE ME OUT
                            /*LogException("Venom rssi is " + rssi);
                            LogException("Venom prov is " + prov);
                            LogException("Venom clm is " + clm);
                            LogException("Venom ccuri is " + ccuri);*/
                        }
                        else
                        {
                            split = parts[12].Split('[');
                            string val = split[1].Replace("]", "");
                            //RTB_Diag.AppendText("Found isp= " + val + Environment.NewLine);
                            if (val == "1")
                            {
                                //RTB_Diag.AppendText("Found isp=1 in tbeat during TTF" + Environment.NewLine);
                                tfound = true;
                                tbeat = true;
                            }
                            return;
                        }
                    }
                    //Console.WriteLine("ccuri is " + ccuri);
                    //Console.WriteLine("clm is " + clm);
                    //Console.WriteLine("prov is " + prov);


                    //RTB_Diag.AppendText("ccuri is " + ccuri + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    //RTB_Diag.AppendText("clm is " + clm + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    //RTB_Diag.AppendText("prov is " + prov + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    if (tourma)
                    {
                        //Console.WriteLine("rssi is " + rssi);
                        //RTB_Diag.AppendText("rssi is " + rssi + Environment.NewLine); RTB_Diag.ScrollToCaret();                        
                    }
                    //Console.WriteLine("Entire string was " + sb);

                    return;
                }

                if (indigo && sb.Equals("rssi"))
                {
                    call = "indigo rssi";
                    rssi = raw;
                    return;
                }

                if (indigo && sb.Equals("mbeat"))  //Indigo mbeat from Trace call
                {
                    call = "indigo mbeat";
                    //88:e7: 12:03:f5: 55,WOC75EC0HS,2345678,7 | 1.193.0,cat = 13,cc = API144_COOKING_V40,prov = 1,grp = 0,ls = 3
                    mbeat = true;
                    string[] parts = raw.Replace(" ", "").Split(',');
                    string[] split = parts[3].Split('|');
                    vers = split[0];
                    //Console.WriteLine("vers is " + vers);
                    //Console.WriteLine("Trace mbeat string was " + raw);
                    //RTB_Diag.AppendText("vers is " + vers + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    //RTB_Diag.AppendText("Trace mbeat string was " + raw + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    //RTB_Diag.AppendText("Trace mbeat unlocked " + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    if (iplist[AUTOINDEX].Signal != null)
                        iplist[AUTOINDEX].Signal.Set();
                    return;
                }

                if (source.Contains("mbeat"))   //mbeat from MQTT call
                {
                    call = "mbeat";
                    mbeat = true;
                    string[] parts = sb.Replace("[", "").Split(':');
                    parts[2].Replace("]", "");
                    string[] split = parts[2].Split(',');
                    ispp = split[0].Replace("\"", "");
                    ispp = ispp.Replace("]", "");
                    vers = split[1].Replace("\"", "");
                    vers = vers.Replace("]", "");
                    //Console.WriteLine("ispp is " + ispp);
                    //Console.WriteLine("vers is " + vers);
                    //RTB_Diag.AppendText("ispp is " + ispp + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    //RTB_Diag.AppendText("vers is " + vers + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    //Console.WriteLine("Entire string was " + sb);
                    //RTB_Diag.AppendText("mbeat unlocked " + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    if (iplist[AUTOINDEX].Signal != null)
                        iplist[AUTOINDEX].Signal.Set();

                    return;
                }

                // Locate if OTA payload has been sent and update status
                if (sb.Contains("\"update\"") && !ttf)
                {
                    // Lookup status reason byte pass or fail reason
                    call = "update no ttf";
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
                    call = "programming or IAP_MODE";
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

                if (sb.Contains("\"progress\"") && source.Equals("MQTT Message")) //may want to use && tourma to allow trace message to change to prog
                {
                    call = "MQTT progress";
                    if (indigo && ttf)  //protect Indigo specific logic
                        return;
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

                //Locate the MQTT status message and check result
                if (sb.Contains("\"status\""))
                {
                    call = "status";
                    //{ "@0,"W12345678"] }
                    // Overwrite status portion to have a point of reference directly next to status reason byte
                    sb = sb.Replace("\"status\":[", "@");
                    string[] stats = sb.Split('@');
                    string[] parts = stats[1].Split(',');
                    sb = "";

                    for (int i = 0; i < parts[0].Length; i++)
                        sb += parts[i];

                    // Convert status reason byte to a numeric value
                    int statusval = Int32.Parse(sb);
                    //RTB_Diag.AppendText("statusval was " + statusval + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    call = "status and " + statusval;
                    //Save to global for auto test execution
                    if (autogen)
                    {
                        string[] strpp = parts[1].Split('"');
                        //Console.WriteLine("ispp was " + ispp + " and is now changed to " + strpp[1]);
                        //RTB_Diag.AppendText("ispp was " + ispp + " and is now changed to " + strpp[1] + Environment.NewLine); RTB_Diag.ScrollToCaret();

                        ispp = strpp[1];
                        gstatus = statusval;
                        call = "status and " + gstatus + " and ispp " + ispp;
                    }

                    // Lookup status reason byte pass or fail reason
                    foreach (var member in iplist)
                    {
                        if (member.IPAddress.ToString().Equals(ip))
                        {
                            if (member.Result.Contains("PASS") || member.Result.Contains("FAIL"))
                                continue;
                            else if (member.Result.Contains("PENDING"))
                            {
                                iplist[member.TabIndex].Result = "FAIL - MQTT Message was never sent in Trace log for final result.";
                                SetText("status", source, member.TabIndex);
                                break;
                            }
                            else
                            {
                                // Write info to widebox window
                                iplist[member.TabIndex].Result = StatusLookup(statusval);
                                SetText("status", source, member.TabIndex);
                                break;
                            }
                        }
                    }

                }

            }

            catch(Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom ProcessPayload(): Local value 'raw' as " + raw + " and 'source' as " + source
                            + " sb was " + sb + " and call was " + call + " Message and Stacktrace were ");
                LogException(e, true);
                return;
            }

        }
        public string StatusLookup(int statusval)
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
            lock (setobj)
            {
                try
                {
                    if (source == "Final")
                    {
                        int total;
                        if (autogen)
                            total = autottl;
                        else
                            total = iplist.Count();
                        string[] results = type.Split('\t');
                        using (StreamWriter sw = File.AppendText(curfilename))
                        {
                            sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + ", " +
                                total + " OTA Update(s) ran with a total running time of " + results[0] +
                                "  that resulted in an average run time per OTA Update of " + results[1]
                                + ".");
                        }

                        return;
                    }

                    if (!autogen)
                        results.Rows[listindex]["OTA Result"] = iplist[listindex].Result;

                    if (autogen && type.Equals("update"))
                    {
                        InvLabel("auto", iplist[AUTOINDEX].Result);
                        if (cyc && iplist[AUTOINDEX].Result.Contains("Programming"))
                        {
                            if (iplist[AUTOINDEX].Signal != null)
                                iplist[AUTOINDEX].Signal.Set();
                            //RTB_Diag.AppendText("SetText unlock of cyc called during programming" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                        }
                    }

                    if (autogen && type.Equals("status"))
                    {
                        string[] res = iplist[AUTOINDEX].Result.Split(' ');
                        InvLabel("auto", res[0]);
                    }

                    if (type.Equals("auto"))
                    {
                        string[] res = iplist[AUTOINDEX].Result.Split(' ');
                        if (DGV_Data.Rows[listindex].Cells[6].Value.ToString().Contains(res[0]))                        
                            return;
                        if (DGV_Data.Rows[listindex].Cells[6].Value.ToString().Contains("PASS") && res[0].Contains("timeout"))
                        {
                            //Console.WriteLine("timeout called " + ++timeout + " times.");
                            //RTB_Diag.AppendText("timeout called " + ++timeout + " times." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            return;
                        }
                        string pay = iplist[AUTOINDEX].Payload;
                        string otype = "";
                        if (iplist[AUTOINDEX].Next == "UPGRADE")
                        {
                            pay = iplist[AUTOINDEX].Down;
                            otype = "DOWNGRADE";
                        }
                        else
                            otype = "UPGRADE";

                        using (StreamWriter sw = File.AppendText(curfilename))
                        {
                            sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + iplist[AUTOINDEX].IPAddress + "," +
                            iplist[AUTOINDEX].MAC + "," + //source + "," +
                            pay + "," +
                            iplist[AUTOINDEX].Delivery + "," +
                            otype + "," +
                            iplist[AUTOINDEX].Node + "," +
                            DGV_Data.Rows[listindex].Cells[5].Value.ToString() + "," +
                            iplist[AUTOINDEX].Result); ;
                        }
                        Invoke((MethodInvoker)delegate
                        {
                            results.Rows[listindex]["OTA Result"] = iplist[AUTOINDEX].Result;
                            DGV_Data.Refresh();
                        });

                        return;
                    }

                    if (type.Equals("status") && !iplist[listindex].Written)
                    {
                        if (!autogen)
                        {
                            using (StreamWriter sw = File.AppendText(curfilename))
                            {
                                sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + iplist[listindex].IPAddress + "," +
                                iplist[listindex].MAC + "," + //source + "," +
                                iplist[listindex].Payload + "," +
                                iplist[listindex].Delivery + "," +
                                iplist[listindex].Type + "," +
                                iplist[listindex].Node + "," +
                                iplist[listindex].Name + "," +
                                iplist[listindex].Result);
                            }
                            Invoke((MethodInvoker)delegate
                            {
                                DGV_Data.Refresh();
                            });

                            iplist[listindex].Written = true;
                        }

                        if (iplist[listindex].Signal != null)
                            iplist[listindex].Signal.Set();

                        long duration = g_time.ElapsedMilliseconds;
                        TimeSpan t = TimeSpan.FromMilliseconds(duration);
                        string s_dur = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
                        //Console.WriteLine("Thread release signal was sent at " + s_dur + ".");
                        //RTB_Diag.AppendText("Thread release signal was sent at " + s_dur + "." + Environment.NewLine); RTB_Diag.ScrollToCaret();
                    }
                }
                catch (Exception e)
                {
                    /*MessageBox.Show("Catastrophic SetText error.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                    //ystem.Net.IPAddress ip = new System.Net.IPAddress(ipbytes);
                    /*LogException("Venom SetText Exception with local value 'type' as " + type + " and 'source' as " + source
                        + " and Message " + e.Message + " and Stacktrace " + e.StackTrace);*/

                    LogException("Venom SetText(): Local value 'type' as " + type + " and 'source' as " + source
                                + " Message and Stacktrace were ");
                    LogException(e, true);
                    return;
                }
            }

        }
        private void BTN_Payload_Click(object sender, EventArgs e)
        {
            // Begin processing all IPs on list
            if (LB_IPs.Items.Count > 0 && iplist.Count > 0)
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
                                    sw.WriteLine("Time,IP,MAC,Payload,Delivery Source,Type,Node,Name,Result");
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
                    if (autogen)
                    {

                        DialogResult dialogResult = MessageBox.Show("The AutoGen Test Execution REQUIRES the product to be Idle(Standby) with Remote Enable ON." +
                                                                "You MUST be on the lowest [SOP] version with an UPGRADE version set. " +
                                                                "If these are ALL true, press Yes to continue or No to exit.",
                                                                "Verify Start State", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (dialogResult == DialogResult.No)
                            return;
                    }

                    BTN_Payload.Text = "Stop Running";
                    BTN_Remove.Enabled = false;
                    BTN_Clr.Enabled = false;
                    TB_LogDir.Enabled = false;
                    LB_IPs.Enabled = false;
                    BTN_Import.Enabled = false;
                    BTN_MakeList.Enabled = false;
                    BTN_Auto.Enabled = false;
                    BTN_LogDir.Enabled = false;
                    cancel_request = false;
                    RTB_Diag.Clear();
                    g_time.Start();
                    if (autogen)
                        LabelSet(true);
                    if (rerun)
                    {
                        if (autogen)
                        {
                            int j = 0;
                            if (tourma)
                            {
                                results.Rows[j]["OTA Result"] = "Test case skipped when using Gen4.";

                                DGV_Data.Rows[j].Cells[6].Style.BackColor = Color.Yellow;
                                results.Rows[j+1]["OTA Result"] = "Test case skipped when using Gen4.";

                                DGV_Data.Rows[j+1].Cells[6].Style.BackColor = Color.Yellow;
                                j = 2;
                            }
                            if (indigo && iplist[AUTOINDEX].Prod == "EMEA Laundry")
                            {
                                results.Rows[j]["OTA Result"] = "Test case skipped when using EMEA Laundry products.";

                                DGV_Data.Rows[j].Cells[6].Style.BackColor = Color.Yellow;
                                results.Rows[j + 1]["OTA Result"] = "Test case skipped when using EMEA Laundry products.";

                                DGV_Data.Rows[j + 1].Cells[6].Style.BackColor = Color.Yellow;
                                j = 2;
                            }
                            for (int i = j; i < NODECASEMAX; i++)
                            {

                                if (!results.Rows[i]["OTA Result"].ToString().Contains("PENDING"))
                                    results.Rows[i]["OTA Result"] = "PENDING";

                                DGV_Data.Rows[i].Cells[6].Style.BackColor = default(Color);
                            }
                            iplist[AUTOINDEX].Result = "PENDING";
                            iplist[AUTOINDEX].Model = "";
                            iplist[AUTOINDEX].Serial = "";
                            ResetGlobal();
                            StopTimer();
                        }
                        else
                        {
                            for (int i = 0; i < iplist.Count(); i++)
                            {
                                if (iplist[i].Result != "PENDING")
                                {
                                    iplist[i].Written = false;
                                    iplist[i].Result = "PENDING";
                                    results.Rows[i]["OTA Result"] = "PENDING";
                                }
                            }
                        }

                        DGV_Data.Refresh();
                    }
                    //After first time press, force reset results in DGV (only reset if ResetForm(true) is called)
                    rerun = true;

                    ProcessIP();
                }

                else
                {
                    DialogResult dialogResult = MessageBox.Show("This will dispose of all running threads and end the OTA list execution. Are you sure you want to exit?",
                                                            "Verify Exiting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        BTN_Payload.Text = "Run Test List";
                        //BTN_Remove.Enabled = true;
                        BTN_Clr.Enabled = true;
                        TB_LogDir.Enabled = true;
                        LB_IPs.Enabled = true;
                        if (!autogen)
                        {
                            BTN_Import.Enabled = true;
                            BTN_MakeList.Enabled = true;
                        }
                        BTN_Auto.Enabled = true;
                        BTN_LogDir.Enabled = true;
                        cancel_request = true;
                        g_time.Stop();
                        g_time.Reset();
                        if (autogen)
                            LabelSet(false);    //Can change all above to just ResetForm(true);
                        try
                        {
                            if (autogen)
                            {
                                StopTimer();
                                InvLabel("auto", "PENDING");
                                InvLabel("ud", "PENDING");
                                FailLeft(0, false);
                                ResetTarget(0);
                                ResetTarget(2);
                                ResetTarget(4);
                                if (iplist[AUTOINDEX].Signal != null)
                                    iplist[AUTOINDEX].Signal.Set();

                                iplist[AUTOINDEX].Result = "PENDING";
                            }
                            else
                            {
                                int stop = iplist.Count();
                                for (int i_base = 0; i_base < stop; i_base++)
                                {

                                    if (iplist[i_base].Result.Contains("PASS") || iplist[i_base].Result.Contains("FAIL"))
                                    {
                                        if (iplist[i_base].Signal == null)
                                            continue;
                                        if (!iplist[i_base].Signal.IsSet)
                                            iplist[i_base].Signal.Set();
                                    }
                                    else
                                    {
                                        iplist[i_base].Result = "Cancelled by User.";
                                        results.Rows[i_base]["OTA Result"] = iplist[i_base].Result;
                                        if (iplist[i_base].Signal == null)
                                            continue;
                                        if (!iplist[i_base].Signal.IsSet)
                                            iplist[i_base].Signal.Set();
                                    }
                                }
                            }


                            Invoke((MethodInvoker)delegate
                            {
                                DGV_Data.Refresh();
                            });

                            foreach (Thread thread in waits)
                            {
                                //Wake sleeping wait threads that will no longer be used
                                if (thread.IsAlive)
                                    thread.Interrupt();
                            }

                            // Close all WifiBasic connections
                            WifiLocal.CloseAll(true);
                            Wait(2000);

                            return;
                        }

                        /*catch
                        {
                            MessageBox.Show("Catastrophic thread closure error. Closing all threads.", "Error: Threads Failed to Close",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //Environment.Exit(1);
                            return;
                        }*/
                        catch (Exception f)
                        {
                            /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                                + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                            LogException("Venom BTN_Payload(): Message and Stacktrace were ");
                            LogException(f, true);
                            return;
                        }

                    }
                    else //Dialog was Yes
                        return;
                }

            }

            else
            {
                if (iplist.Count > 0)
                    MessageBox.Show("No IP Address in IP List. Please populate list and try again.", "Error: No IP in List",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("DataGrid is empty or has become corrupted. Please try again.", "Error: DataGrid corrupted",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public bool IsAllNumeric(string value)
        {
            return value.All(char.IsNumber);
        }
        public void LabelSet(bool type)
        {
            Invoke((MethodInvoker)delegate
            {
                if (type)
                {
                    LBL_Auto.Visible = true;
                    LBL_OTA.Visible = true;
                    LBL_UD.Visible = true;
                    LBL_VAR.Visible = true;
                    LBL_Phase.Visible = true;
                    LBL_i.Visible = true;
                }
                else
                {
                    LBL_Auto.Visible = false;
                    LBL_OTA.Visible = false;
                    LBL_UD.Visible = false;
                    LBL_VAR.Visible = false;
                    LBL_Phase.Visible = false;
                    LBL_i.Visible = false;
                    LBL_Time.Visible = false;
                    LBL_Rmn.Visible = false;
                }
            });
        }
        public void InvLabel(string source, string text)
        {
            Invoke((MethodInvoker)delegate
            {
                if (source == "auto")
                    LBL_Auto.Text = text;
                if (source == "ud")
                    LBL_UD.Text = text;
                if (source == "phase")
                    LBL_i.Text = text;
                if (source == "result")
                {
                    string[] tmp = text.Split(',');
                    results.Rows[int.Parse(tmp[1])]["OTA Result"] = tmp[0];
                }
                DGV_Data.Refresh();
            });
        }
        public void InvColor(int index, string type)
        {
            Invoke((MethodInvoker)delegate
            {
                if (type == "yll")
                    DGV_Data.Rows[index].Cells[6].Style.BackColor = Color.Yellow;
                if (type == "red")
                    DGV_Data.Rows[index].Cells[6].Style.BackColor = Color.Red;
                if (type == "grn")
                    DGV_Data.Rows[index].Cells[6].Style.BackColor = Color.Green;
                if (type == "res")
                    DGV_Data.Rows[index].Cells[6].Style.BackColor = default(Color);

                if (type =="remhi")
                {
                    for (int i = 0; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = Color.White;
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = Color.Black;
                    }
                    
                }
                if (type == "genhi")
                {
                    for (int i = 4; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = Color.White;
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = Color.Black;
                    }
                }
                if (type == "ttfhi")
                {
                    for (int i = 19; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = Color.White;
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = Color.Black;
                    }
                }

                if (type == "uremhi")
                {
                    for (int i = 0; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = default(Color);
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = default(Color);
                    }
                }
                if (type == "ugenhi")
                {
                    for (int i = 4; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = default(Color);
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = default(Color);
                    }
                }
                if (type == "uttfhi")
                {
                    for (int i = 19; i < index; i++)
                    {
                        DGV_Data.Rows[i].Cells[5].Style.ForeColor = default(Color);
                        DGV_Data.Rows[i].Cells[5].Style.BackColor = default(Color);
                    }
                }

                DGV_Data.Refresh();
            });
        }
        public bool SendMQTT(byte[] ipbytes, string topic, byte[] paybytes, ConnectedApplianceInfo cai, IPData ipd)
        {
            try
            {
                System.Net.IPAddress ip = new System.Net.IPAddress(ipbytes);

                if (cancel_request)
                {
                    //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                    //RTB_Diag.AppendText("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    return true;
                }

                //If IP Address exists, send to that IP
                if (cai != null)
                //Send payload
                {
                    WifiLocal.SendMqttMessage(ip, topic, paybytes);
                    //Wait(2000);
                    return true;

                }
                else
                    return false;
            }
            /*catch
            {
                MessageBox.Show("Catastrophic SendMQTT error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom SendMQTT():  Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }

        }
        public void Wait(int timeout)
        {
            try
            {
                Thread thread = new Thread(delegate ()
                {
                    try
                    {
                        Thread.Sleep(timeout);
                    }
                    catch (ThreadInterruptedException)
                    {
                        //Console.WriteLine("Thread with ID " + Thread.CurrentThread.ManagedThreadId + " interrupted.");
                        //RTB_Diag.AppendText("Thread with ID " + Thread.CurrentThread.ManagedThreadId + " interrupted." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    }
                });
                waits.Add(thread);
                thread.Start();
                while (thread.IsAlive)
                {
                    if (cancel_request)
                        return;
                    Application.DoEvents();
                }
            }

            /*catch
            {
                MessageBox.Show("Catastrophic Wait error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom SendMQTT():  Message and Stacktrace were ");
                LogException(e, true);
                return ;
            }
        }
        public void printAllNode()
        {
            //Node temp = head;
            LinkedListNode<int> head = gllist.First;
            LinkedListNode<int> temp = head;
            // if the list is empty
            if (head == null)
            {
                //Console.WriteLine("Nothing to print in the list");
                //RTB_Diag.AppendText("Nothing to print in the list" + Environment.NewLine); RTB_Diag.ScrollToCaret();

            }
            else
            {
                while (temp != null)
                {
                    //RTB_Diag.AppendText(temp.Value + " ");

                    //Console.Write(temp.Value + " ");
                    temp = temp.Next;
                }
                //Console.Write('\n');
                //RTB_Diag.AppendText(Environment.NewLine); RTB_Diag.ScrollToCaret();

            }
        


    }
        private static void ProgressThread(object sender, ElapsedEventArgs e, IPData ipd)
        {
            try
            {
                Console.WriteLine("End Thread was called for signal " + ipd.Signal.WaitHandle.Handle +
                              " and thread was released (set).");

                ipd.Result = "FAIL - Thread timeout maximum reached. Unknown issue caused thread to be stuck. Unable to validate OTA pass or fail.";
                if (ipd.Signal != null)
                    ipd.Signal.Set();
            }
            catch
            {
                /*MessageBox.Show("Catastrophic ProgressThread error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                return;
            }

        }
        public void RemoteOps(ConnectedApplianceInfo cai, IPData ipd, byte[] ipbytes, string type)
        {
            try
            {
                if (clm != "1")
                {
                    //Send subscribe message before sending cycle
                    byte[] paybytes = Encoding.ASCII.GetBytes("{\"sublist\":[1,144,147]}");
                    SendMQTT(ipbytes, "iot-2/cmd/subscribe/fmt/json", paybytes, cai, ipd);
                    Wait(2000);
                }

                string topic = "";
                byte[] bytes = null;
                string pay = "";
                switch (type)
                {
                    case "bright":
                        pay = ipd.Set + "0A";  //Sys set brightness to 10
                        bytes = new byte[pay.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(pay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);

                        }
                        topic = "iot-2/cmd/cc_SetKvp/fmt/binary";
                        break;

                    case "resetb":
                        pay = ipd.Set + "46";  //Sys set brightness to 70
                        bytes = new byte[pay.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(pay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);

                        }
                        topic = "iot-2/cmd/cc_SetKvp/fmt/binary";
                        break;

                    case "rssi":
                        pay = "0005FF01020001";  //XCat get RSSI
                        bytes = new byte[pay.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(pay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);

                        }
                        topic = "iot-2/cmd/cc_GetKvp/fmt/binary";
                        break;

                    case "cyc":
                        bytes = new byte[ipd.MQTTPay.Length / 2];   //Saved from user selection on AutoGen.cs
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(ipd.MQTTPay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);

                        }
                        topic = "iot-2/cmd/cc_SetKvp/fmt/binary";
                        break;

                    case "cncl":
                        pay = ipd.Cncl;   //Remote cancel
                        bytes = new byte[pay.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(pay.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);

                        }
                        topic = "iot-2/cmd/cc_SetKvp/fmt/binary";
                        break;

                    default:
                        break;
                }

                SendMQTT(ipbytes, topic, bytes, cai, ipd);

                StartTimer(CYCWAIT);
                Wait(CYCWAIT);
                StopTimer();

            }
            /*catch
            {
                MessageBox.Show("Catastrophic RemoteOps error.", "Error",
                                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom RemoteOps():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void TTFExec(ConnectedApplianceInfo cai, IPData ipd, byte[] ipbytes, int var)
        {
            try
            {

                if (var == 0)   //Invalid CRC
                {
                    string[] parts = ipd.Payload.Split(',');
                    //TEMP DEV ONLY WHEN DONE SET BACK TO ALL ZEROS
                    if (tourma)
                        parts[0] = "{\"update\":{\"crc32\":\"12345678\",";  //Bad CRC for TTF (even if a valid crc it would mean ubd file is empty i.e. not valid anyway)
                    else
                        parts[0] = "{\"update\":{\"crc32\":\"00000000\",";  //Bad CRC for TTF (even if a valid crc it would mean ubd file is empty i.e. not valid anyway)

                    string pay = parts[0] + parts[1];
                    //RTB_Diag.AppendText("TTF 0, sending bad CRC as " + pay  + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    byte[] paybytes = Encoding.ASCII.GetBytes(pay);

                    SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);

                    ipd.Signal.Reset();
                    ipd.Signal.Wait();

                    if (LBL_Auto.Text.Contains("FAIL") && gstatus == 5)
                    {
                        InvColor(21, "grn");
                        ipd.Result = "PASS - The OTA did not execute (failed) as expected with a Status result equal to " + StatusLookup(gstatus) + ".";
                    }

                    else
                    {
                        InvColor(21, "red");
                        ipd.Result = "FAIL - The final OTA result did not FAIL as expected with a final Status result equal to " + StatusLookup(gstatus) + ".";
                    }
                    InvLabel("auto", "PENDING");
                    SetText("auto", "AutoGen Result", 21);  //Table index for this test case
                    ipd.Result = "";

                    if (tourma) //Let module reboot
                    {
                        StartTimer(75000);
                        Wait(75000);
                        StopTimer();
                    }
                }

                if (var == 1)   //Invalid URL
                {
                    string pay = ipd.Payload.Replace(".com/", ".gov/").Replace(".net/", ".gov/").Replace(".org/", ".gov/");
                    byte[] paybytes = Encoding.ASCII.GetBytes(pay);
                    retcnt = 0;
                    //RTB_Diag.AppendText("TTF 1, sending bad URL as " + pay + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);

                    ipd.Signal.Reset();
                    ipd.Signal.Wait();
                    ipd.Signal.Reset();

                    if (LBL_Auto.Text.Contains("FAIL") && gstatus == 4)
                    {
                        InvColor(20, "grn");
                        ipd.Result = "PASS - The OTA did not execute (failed) as expected with a Status result equal to " + StatusLookup(gstatus) + ".";
                    }

                    else
                    {
                        InvColor(20, "red");
                        ipd.Result = "FAIL - The final OTA result did not FAIL as expected with a final Status result equal to " + StatusLookup(gstatus) + ".";
                    }
                    InvLabel("auto", "PENDING");
                    SetText("auto", "AutoGen Result", 20);  //Table index for this test case
                    ipd.Result = "";

                    if (tourma) //Let module reboot
                    {
                        StartTimer(75000);
                        Wait(75000);
                        StopTimer();
                    }
                }

                if (var == 2)   //Multiple download retry (gathered while var == 1 test case running)
                {
                    int lcnt = retcnt / 2;  //ubd_description file retry was also counted with ubd file (what we intentionally make retry) so count is doubled

                    if (tourma) //not doubled for tourma
                        lcnt = retcnt;

                    if (lcnt == 5 && indigo)  //Count total download attempts for Indigo
                    {
                        InvColor(19, "grn");
                        ipd.Result = "PASS - A total of " + lcnt + " retry attempts were done while trying to download from the invalid url above in RQM 131865.";
                    }

                    else if (lcnt == 3 && tourma)  //Count total download attempts for Tourma
                    {
                        InvColor(19, "grn");
                        ipd.Result = "PASS - A total of " + lcnt + " retry attempts were done while trying to download from the invalid url above in RQM 131865.";
                    }

                    else
                    {
                        InvColor(19, "red");
                        ipd.Result = "FAIL - A total of " + lcnt + " retry attempts were done while trying to download from the invalid url above in RQM 131865.";
                    }

                    SetText("auto", "AutoGen Result", 19);  //Table index for this test case
                    Wait(2000);
                    return;
                }

                if (var == 3)   //Multiple payload sent
                {
                    int stop = rand.Next(4, 6);
                    int intv;
                    byte[] paybytes = Encoding.ASCII.GetBytes(ipd.Payload);

                    for (int j = 0; j < stop; j++)
                    {
                        if (tourma && !multdload && dlcnt == 0)
                        {
                            //RTB_Diag.AppendText("multdload unlocked" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            multdload = true;
                        }
                        intv = rand.Next(200, 2000);
                        Wait(8000 + intv);
                        SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);
                        if (tourma && tbeat && !tfound)
                        {
                            //RTB_Diag.AppendText("tbeat unlocked" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            tbeat = false;
                        }
                    }

                    if (tourma)
                    {
                        if (!tbeat)
                        {
                            /*Console.WriteLine("TTF Programming Thread Wait reached with lock ID " + ipd.Signal.WaitHandle.Handle + ".");

                            RTB_Diag.AppendText("TTF Programming Thread Wait reached with lock ID " + ipd.Signal.WaitHandle.Handle + "." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            if (ipd.Signal != null)
                                ipd.Signal.Wait();

                            if (ipd.Signal != null)
                                ipd.Signal.Reset();*/
                            //RTB_Diag.AppendText("tbeat not seen starting 30s timer." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            StartTimer(30000);
                            Wait(30000);
                            StopTimer();
                            tbeat = true;
                        }
                        if (multdload)
                            multdload = false;
                        if (tfound && dlcnt == 1)  //Check for isp=1 from tbeat
                        {
                            InvColor(22, "grn");
                            ipd.Result = "PASS - A total of " + stop + " OTAs were sent (1 was valid and the rest extra). The log showed that the isp byte was set to 1 (indicating OTA in progress) and " + dlcnt + " OTA began running.";
                        }

                        else if (!tfound)
                        {
                            InvColor(22, "red");
                            ipd.Result = "FAIL - A total of " + stop + " OTAs were sent (1 was valid and the rest extra). The log did NOT show that the isp byte was set to 1 (indicating OTA NOT in progress even though there was).";
                        }
                        else
                        {
                            InvColor(22, "red");
                            ipd.Result = "FAIL - A total of " + stop + " OTAs were sent (1 was valid and the rest extra). The log showed that the isp byte was set to 1 (indicating OTA in progress) and " + dlcnt + " OTA(s) began running.";
                        }
                        SetText("auto", "AutoGen Result", 22);  //Table index for this test case
                        return;
                    }

                    StartTimer(20000);
                    Wait(10000);
                    multdload = true;
                    Wait(10000);
                    StopTimer();

                    multdload = false;


                    if (gllist != null)
                    {
                        //HashSet<string> uniqueNumbers = new HashSet<string>(gllist);

                        //string output = string.Join(" ", uniqueNumbers);
                        //int match = uniqueNumbers.Count;
                        bool pass = true;
                        /*var orderedByAsc = uniqueNumbers.OrderBy(d => d);
                        if (uniqueNumbers.SequenceEqual(orderedByAsc))*/

                        // Traverse the list till last node and return 
                        // false if a node is smaller than or equal 
                        // its next. 
                        for (LinkedListNode<int> t = gllist.First; t.Next != null; t = t.Next)
                        {
                            //Console.WriteLine("Comparing gllist t.value " + t.Value + " to t.next.value " + t.Next.Value);

                            if (!(t.Value <= t.Next.Value))
                            {
                                //Console.WriteLine("Comparing gllist t.value " + t.Value + " to t.next.value " + t.Next.Value);
                                pass = false;
                            }
                        }                      

                                               
                        if (pass)  //If multiple download messages seen in trace, unique numbers will be jumbled (4% , 0% , 5%, 1%, etc.)
                        {
                            InvColor(22, "grn");
                            ipd.Result = "PASS - A total of " + stop + " OTAs were sent (1 was valid and the rest extra). The log showed that download percentages were NOT jumbled (4% , 0% , 5%, 1%, etc.) indicating multiple downloads were NOT running.";
                        }

                        else
                        {
                            InvColor(22, "red");
                            ipd.Result = "FAIL - A total of " + stop + " OTAs were sent (1 was valid and the rest extra). The log showed that download percentages WERE jumbled (4% , 0% , 5%, 1%, etc.) indicating multiple downloads WERE running.";
                            //Console.WriteLine("Multi dload FAILED with unique numbers " + uniqueNumbers + Environment.NewLine);
                            //Console.WriteLine("Multi dload FAILED with orderedasc numbers " + Environment.NewLine);

                            //MessageBox.Show("Multi dload FAILED with unique numbers " + orderedByAsc, "TTF Multi Download fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //RTB_Diag.AppendText("unique numbers " + orderedByAsc + Environment.NewLine + "gllist "); RTB_Diag.ScrollToCaret();

                            printAllNode();
                        }

                        gllist.Clear();
                    }
                    else
                    {
                        InvColor(22, "red");
                        ipd.Result = "FAIL - Unable to store information for multiple OTA payload sent.";
                    }
              
                    SetText("auto", "AutoGen Result", 22);  //Table index for this test case
                    return;
                }

                
            }
            /*catch
            {
                MessageBox.Show("Catastrophic TTFExec error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom TTFExec():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void TTFRun(ConnectedApplianceInfo cai, IPData ipd, byte[] ipbytes)
        {
            try
            {
                ttf = true;
                InvLabel("ud", "TTF");
                //RTB_Diag.AppendText("TTF starting with ttf equal " + ttf + Environment.NewLine); RTB_Diag.ScrollToCaret();

                for (int i = 0; i < TTFCNT; i++)
                {
                    if (cancel_request)
                        return;
                    TTFExec(cai, ipd, ipbytes, i);
                }

                InvLabel("ud", "DOWNGRADE");    //Last TTF sends multiple downgrade payloads, update labels (logs blocked for download event during TTFs)
                InvLabel("auto", "Downloading");
                ipd.Result = "";    //Purge failed results from previous tests as we were forcing fail states
                ttf = false;
                //RTB_Diag.AppendText("TTF ending with ttf equal " + ttf + Environment.NewLine); RTB_Diag.ScrollToCaret();

            }
            /*catch
            {
                MessageBox.Show("Catastrophic TTFRun error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom TTFRun():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public bool CycExec(ConnectedApplianceInfo cai, IPData ipd, byte[] ipbytes, string type, string time)
        {
            try
            {

                if (cancel_request)
                    return false;

                byte[] paybytes = Encoding.ASCII.GetBytes(ipd.Payload);

                if (time.Equals("down"))
                {
                    if (type.Equals("cyc"))   //Only run this one test for Indigo
                    {
                        int next = 0;   //UNDO FOR RELEASE, ONLY KEEP FOR DEV
                        //int next = rand.Next(0, 2); //Random to start cycle or send payload first
                        if (tourma) //skip first test
                            return true;
                        if (indigo && ipd.Prod == "EMEA Laundry")
                            return true;
                        if (next == 0)
                        {
                            RemoteOps(cai, ipd, ipbytes, "cyc");
                            //RTB_Diag.AppendText("Remote cycle sent" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            if (!cycstart)
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was rejected with KVP ACK result of REJECTED-02. Unable to verify test case outcome.");
                                return false;
                            }
                            cycstart = false;

                            if (!SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was accepted with KVP ACK result of ACCEPTED-00 however, the payload was not able to be sent using MQTT. Unable to verify test case outcome.");
                                RemoteOps(cai, ipd, ipbytes, "cncl");
                                return false;
                            }

                            StartTimer(CYCWAIT);
                            Wait(CYCWAIT);
                            StopTimer();

                            RemoteOps(cai, ipd, ipbytes, "cncl");

                            //RTB_Diag.AppendText("Remote cancel sent" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            if (LBL_Auto.Text.Equals("Downloading"))
                            {
                                ipd.LList.AddFirst("PASS - The cycle was accepted with KVP ACK result of ACCEPTED-00. The OTA download continued as expected.");
                                return true;
                            }
                            else
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was accepted with KVP ACK result of ACCEPTED-00 however no download was started. Unable to verify test case outcome.");
                                return false;
                            }
                        }
                        else
                        {
                            if (!SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was accepted with KVP ACK result of ACCEPTED-00 however, the payload was not able to be sent using MQTT. Unable to verify test case outcome.");
                                return false;
                            }

                            StartTimer(CYCWAIT);
                            Wait(CYCWAIT);
                            StopTimer();

                            RemoteOps(cai, ipd, ipbytes, "cyc");

                            //RTB_Diag.AppendText("Remote cycle sent" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            if (!cycstart)
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was rejected with KVP ACK result of REJECTED-02. Unable to verify test case outcome.");
                                return false;
                            }

                            cycstart = false;

                            RemoteOps(cai, ipd, ipbytes, "cncl");

                            //RTB_Diag.AppendText("Remote cancel sent" + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            if (LBL_Auto.Text.Equals("Downloading"))
                            {
                                ipd.LList.AddFirst("PASS - The cycle was accepted with KVP ACK result of ACCEPTED-00. The OTA download continued as expected.");
                                return true;
                            }

                            else
                            {
                                ipd.LList.AddFirst("FAIL - The cycle was accepted with KVP ACK result of ACCEPTED-00 however no download was started. Unable to verify test case outcome.");
                                return false;
                            }
                        }

                    }
                    if (type.Equals("set"))
                    {
                        if (tourma)
                        {

                            if (!SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd)) //start first OTA after skipping download cycle test
                            {
                                ipd.LList.AddFirst("FAIL - The payload was not able to be sent using MQTT. Unable to verify test case outcome.");
                                RemoteOps(cai, ipd, ipbytes, "cncl");
                                return false;
                            }

                            ipd.LList.AddFirst("Test case skipped when using Gen4"); //set first result as skipping very first test case
                            ipd.LList.AddLast("Test case skipped when using Gen4"); //set first result as skipping very first test case
                            return true;
                        }

                        if (indigo && ipd.Prod == "EMEA Laundry")
                        {

                            if (!SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd)) //start first OTA after skipping download cycle test
                            {
                                ipd.LList.AddFirst("FAIL - The payload was not able to be sent using MQTT. Unable to verify test case outcome.");
                                RemoteOps(cai, ipd, ipbytes, "cncl");
                                return false;
                            }

                            ipd.LList.AddFirst("Test case skipped when using EMEA Laundry products"); //set first result as skipping very first test case
                            ipd.LList.AddLast("Test case skipped when using EMEA Laundry products"); //set first result as skipping very first test case
                            return true;
                        }

                        RemoteOps(cai, ipd, ipbytes, "bright");
                        //RTB_Diag.AppendText("bright sent" + Environment.NewLine); RTB_Diag.ScrollToCaret();

                        if (!cycstart && indigo)
                        {
                            ipd.LList.AddLast("FAIL - The setting was rejected with KVP ACK result of REJECTED-02. Unable to verify test case outcome.");
                            return false;
                        }
                        
                        

                        if (LBL_Auto.Text.Equals("Downloading"))
                        {
                            if (indigo) //Different behavior for Indigo v Tourma
                            {
                                cycstart = false;
                                ipd.LList.AddLast("PASS - The setting was accepted with KVP ACK result of ACCEPTED-00. The OTA download continued as expected.");
                                //RemoteOps(cai, ipd, ipbytes, "resetb");
                                return true;
                            }
                            
                            if (tourma && !cycstart)
                            {
                                ipd.LList.AddLast("PASS - The setting was rejected with KVP ACK result of REJECTED-02. The OTA download continued as expected.");
                                //RemoteOps(cai, ipd, ipbytes, "resetb");
                                return true;
                            }
                        }

                        else
                        {
                            ipd.LList.AddLast("FAIL - No download was started. Unable to verify test case outcome.");
                            return false;
                        }

                    }
                }

                if (time.Equals("iap"))
                {
                    if (type.Equals("cyc"))
                    {
                        int next = rand.Next(0, 2); //Random to start cycle or send payload first

                        RemoteOps(cai, ipd, ipbytes, "cyc");

                        if (cycstart)
                        {
                            ipd.LList.AddLast("FAIL - The cycle was accepted with KVP ACK result of ACCEPTED-00 while product in IAP. You MUST explore further as this should not be possible.");
                            return false;
                        }

                        cycstart = false;

                        RemoteOps(cai, ipd, ipbytes, "cncl");

                        if (LBL_Auto.Text.Equals("Programming"))
                        {
                            ipd.LList.AddLast("PASS - The cycle was rejected with KVP ACK result of REJECTED-02. The OTA installation continued as expected.");
                            return true;
                        }

                        else
                        {
                            //RTB_Diag.AppendText("Cyc in IAP failed and label was " + LBL_Auto.Text + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            ipd.LList.AddLast("FAIL - Product did not enter IAP. Unable to verify test case outcome.");
                            return false;
                        }
                    }
                    if (type.Equals("set"))
                    {
                        RemoteOps(cai, ipd, ipbytes, "bright");

                        if (cycstart)
                        {
                            ipd.LList.AddLast("FAIL - The setting was accepted with KVP ACK result of ACCEPTED-00 while product in IAP. You MUST explore further as this should not be possible.");
                            return false;
                        }

                        cycstart = false;


                        if (LBL_Auto.Text.Equals("Programming"))
                        {
                            ipd.LList.AddLast("PASS - The setting was rejected with KVP ACK result of REJECTED-02. The OTA installation continued as expected.");
                            return true;
                        }

                        else
                        {
                            //RTB_Diag.AppendText("Bright in IAP failed and label was " + LBL_Auto.Text + Environment.NewLine); RTB_Diag.ScrollToCaret();

                            ipd.LList.AddLast("FAIL - Product did not enter IAP. Unable to verify test case outcome.");
                            return false;
                        }
                    }
                }

                return false;
            }
            /*catch
            {
                MessageBox.Show("Catastrophic CycExec error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom CycExec():  Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }
        }
        public void SetCyc(IPData ipd, int num, string val, LinkedListNode<string> node)
        {
            try
            {
                InvColor(num, val);
                ipd.Result = node.Value;
                SetText("auto", "AutoGen Result", num);  //Table index for this test case
            }
            /*catch
            {
                MessageBox.Show("Catastrophic SetCyc error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom SetCyc():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public bool CycRun(ConnectedApplianceInfo cai, IPData ipd, byte[] ipbytes)
        {
            try
            {
                cyc = true;
                bool cwait = false;
                ipd.LList = new LinkedList<string>();


                if (CycExec(cai, ipd, ipbytes, "cyc", "down"))
                    cwait = true;
                else
                {
                    cwait = false;
                    InvLabel("auto", "FAIL");
                    ipd.Result = "FAIL - Unable to send OTA payload to product using MQTT. Unable to validate OTA test case results.";
                }
                if (cwait)
                {
                    ipd.Result = "";    //Purge result from previous test
                    CycExec(cai, ipd, ipbytes, "set", "down");
                    //Console.WriteLine("CYCLE Programming Thread Wait reached with lock ID " + ipd.Signal.WaitHandle.Handle + ".");

                    ipd.Result = "";
                    if (ipd.Signal != null)
                    {
                        ipd.Signal.Reset();
                        //RTB_Diag.AppendText("CYCLE Programming Thread Wait reached with lock ID " + ipd.Signal.WaitHandle.Handle + "." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                        ipd.Signal.Wait();

                    }
                    //RTB_Diag.AppendText("CYCLE Programming unlocked." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    Wait(2000);

                    if (ipd.Result.Contains("timeout"))
                    {
                        cwait = false;
                        SetText("status", "Force Close", ipd.TabIndex);
                        SetText("auto", "Force Close", ipd.TabIndex);
                    }

                    else
                    {
                        CycExec(cai, ipd, ipbytes, "cyc", "iap");
                        ipd.Result = "";
                        CycExec(cai, ipd, ipbytes, "set", "iap");
                        ipd.Result = "";
                    }
                }

                if (ipd.Signal != null)
                    ipd.Signal.Reset();

                cyc = false;

                if (cwait)
                    return true;
                else
                    return false;
            }
            /*catch
            {
                MessageBox.Show("Catastrophic CycRun error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/

            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom CycRun():  Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }

        }
        public bool CheckBeat(string type, ConnectedApplianceInfo cai, IPData ipd)
        {
            try
            {
                //Prepare IP address for sending via MQTT
                string[] ipad = ipd.IPAddress.Split('.');
                byte[] ipbytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    ipbytes[j] = byte.Parse(ipad[j]);
                }

                //RTB_Diag.AppendText("CheckBeat started" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                if (type == "init")
                {
                    if (!mbeat)
                    {
                        //RTB_Diag.AppendText("if !mbeat started" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                        byte[] paybytes = Encoding.ASCII.GetBytes("{\"get\": 0 }");
                        SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);
                        Wait(2000);                    
                    }
                    if (tbeat && mbeat)
                    {
                        //RTB_Diag.AppendText("mbeat and tbeat both true" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                        return true;
                    }
                    else
                    {
                        for (int j = 0; j < MQTTMAX; j++)
                        {
                            if (cancel_request)
                                return false;
                            if (!mbeat)
                            {
                                byte[] paybytes = Encoding.ASCII.GetBytes("{\"get\": 0 }");
                                SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);
                            }
                            Wait(5000);
                            if (tbeat && mbeat)
                                return true;
                            else
                                continue;
                        }

                        return false;
                    }

                }

                else
                {
                    tbeat = false;
                    mbeat = false;

                    for (int j = 0; j < MQTTMAX; j++)
                    {
                        if (cancel_request)
                            return false;
                        if (!mbeat)
                        {
                            byte[] paybytes = Encoding.ASCII.GetBytes("{\"get\": 0 }");
                            SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd);
                        }

                        Wait(5000);
                        if (tbeat && mbeat)
                            return true;
                        else
                            continue;
                    }
                }


                return false;
            }
            /*catch
            {
                MessageBox.Show("Catastrophic CheckBeat error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom CheckBeat(): tbeat was " + tbeat + " and mbeat was " + mbeat + " Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }
        }
        public void TestInit(byte[] ipbytes, ConnectedApplianceInfo cai, IPData ipd, int iter)
        {
            try
            {
                //RTB_Diag.AppendText("TestInit started" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                for (int i = 0; i < NODECASEMAX; i++)
                {
                    if (cancel_request)
                        return;
                    //Global values are pulled from trace and mqtt logs by CheckBeat()
                    switch (i)
                    {
                        case 4:    //OTA upgrade in Idle
                                   //No special initialization required
                            break;
                        case 5:    //OTA downgrade in Idle
                                   //No special initialization required
                            break;
                        case 6:    //Upgrade Model/Serial Number check
                            ipd.Model = cai.ModelNumber;
                            ipd.Serial = cai.SerialNumber;
                            //RTB_Diag.AppendText("Upgrade Model / Serial set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 7:    //Upgrade Version Number check
                            int j;
                            string[] stru = cai.VersionNumber.Replace(" ", "").Split('|');
                            bool result = int.TryParse(stru[0], out j);
                            //RTB_Diag.AppendText("Version set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            if (result)
                                ipd.Vers = stru[0];
                            else
                            {
                                if (!ttf && !cyc && !mbeat)
                                {
                                    int tstart = (int)g_time.ElapsedMilliseconds;   //Start timers to detect version already installed
                                    int tstop;
                                    //Console.WriteLine("mbeat wait reached.");
                                    //RTB_Diag.AppendText("mbeat wait reached." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                                    StartTimer(300000); //Wait 5 minutes for heartbeat
                                    ipd.Signal.Wait();
                                    tstop = (int)g_time.ElapsedMilliseconds;
                                    long duration = tstop-tstart;
                                    TimeSpan t = TimeSpan.FromMilliseconds(duration);
                                    string s_dur = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                                t.Hours,
                                                t.Minutes,
                                                t.Seconds,
                                                t.Milliseconds);
                                    //Console.WriteLine("mbeat wait unlocked at time " + s_dur + " and vers = " + vers + ".");
                                    //RTB_Diag.AppendText("mbeat wait unlocked at time " + s_dur + " and vers = " + vers + "." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                                    StopTimer();
                                }
                                if (ipd.Signal != null)
                                    ipd.Signal.Reset();

                                //Console.WriteLine("ipd.Version was " + ipd.Vers + " and vers was " + vers + " before setting ipd.Version = vers");
                                //RTB_Diag.AppendText("ipd.Version was " + ipd.Vers + " and vers was " + vers + " before setting ipd.Version = vers" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                                ipd.Vers = vers;
                            }
                                
                            break;
                        case 8:    //Downgrade Model/Serial Number check
                            ipd.Model = cai.ModelNumber;
                            ipd.Serial = cai.SerialNumber;
                            //RTB_Diag.AppendText("Downgrade Model / Serial set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 9:    //Downgrade Version Number check
                            //Set in case 7
                            break;
                        case 10:    //Upgrade CCURI check
                            ipd.CCURI = ccuri;
                            //RTB_Diag.AppendText("Upgrade CCURI set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 11:    //Downgrade CCURI check
                            ipd.CCURI = ccuri;
                            //RTB_Diag.AppendText("Downgrade CCURI set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 12:    //Provision check
                            ipd.Prov = prov;
                            //RTB_Diag.AppendText("Provision set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 13:    //Claim check
                            ipd.Clm = clm;
                            //RTB_Diag.AppendText("Claim set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 14:    //Progress message check
                                    //No special initialization required
                            break;
                        case 15:    //Node OTA success check
                                    //No special initialization required
                            break;
                        case 16:    //RSSI check
                            if (string.IsNullOrEmpty(rssi))
                                RemoteOps(cai, ipd, ipbytes, "rssi");

                            //RTB_Diag.AppendText("RSSI set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 17:    //OTAs possible after OTA
                                    //No special initialization required
                            break;
                        case 18:    //ApplianceUpdateVersion check
                            ipd.ISPP = ispp;
                            //RTB_Diag.AppendText("ISPP set" + Environment.NewLine); RTB_Diag.ScrollToCaret();
                            break;
                        case 19:    //5 retry check
                                    //No special initialization required
                            break;
                        case 20:    //Invalid URL check
                                    //No special initialization required
                            break;
                        case 21:    //Invalid CRC check
                                    //No special initialization required
                            break;
                        case 22:    //Payload sent multiple times
                                    //No special initialization required
                            break;

                        default:
                            break;
                    }
                }
            }
            /*catch
            {
                MessageBox.Show("Catastrophic Testinit error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom Testinit():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void TestCheck(ConnectedApplianceInfo cai, IPData ipd, int iter)
        {
            try
            {

                bool changed;

                for (int i = 0; i < NODECASEMAX; i++)
                {
                    ipd.Result = "PENDING";

                    if (cancel_request)
                        return;

                    changed = false;

                    switch (i)
                    {
                        case 4:    //Idle OTA upgrade success check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "DOWNGRADE")
                                {
                                    InvColor(i, "grn");
                                    ipd.Result = "PASS - OTA was successfully installed from an Idle(Standby) start state with Status result of PASS."; //OTA result label did not have FAIL so it is PASS
                                    changed = true;
                                }
                            }

                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA was NOT successfully installed from an Idle(Standby) start state with Status result of FAIL."; //OTA result label did not have PASS so it is FAIL
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 5:    //Idle OTA downgrade success check
                            if (!LBL_Auto.Text.Contains("FAIL"))

                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "UPGRADE")
                                {
                                    InvColor(i, "grn");
                                    ipd.Result = "PASS - OTA was successfully installed from an Idle(Standby) start state with Status result of PASS."; //OTA result label did not have FAIL so it is PASS
                                    changed = true;
                                }
                            }

                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA was NOT successfully installed from an Idle(Standby) start state with Status result of FAIL."; //OTA result label did not have PASS so it is FAIL
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 6:    //Upgrade Model/Serial Number check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "DOWNGRADE")
                                {
                                    if (cai.ModelNumber != ipd.Model)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Model Number was different after OTA was applied. " + ipd.Model +
                                                     " was the initial Model Number and " + cai.ModelNumber + " was the final Model Number.";
                                    }

                                    if (cai.SerialNumber != ipd.Serial)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Serial Number was different after OTA was applied. " + ipd.Serial +
                                                     " was the initial Serial Number and " + cai.SerialNumber + " was the final Serial Number.";
                                    }

                                    if (cai.SerialNumber != ipd.Serial && cai.ModelNumber != ipd.Model)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - BOTH Model Number and Serial Number were different after OTA was applied."
                                            + ipd.Model + " was the initial Model Number and " + cai.ModelNumber + " was the final Model Number."
                                            + ipd.Serial + " was the initial Serial Number and " + cai.SerialNumber + " was the final Serial Number.";
                                    }
                                    else
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - BOTH Model Number of " + cai.ModelNumber + " and Serial Number of " + cai.SerialNumber + " were the same after OTA was applied.";
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 7:    //Upgrade Version Number check
                            if (iter == LASTITER)   //Dont run on last iteration / overwrite previous results
                                break;
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                string[] str = cai.VersionNumber.Replace(" ", "").Split('|');
                                int j;
                                string stru = str[0];
                                bool result = int.TryParse(str[0], out j);
                                if (!result)
                                    stru = vers;
                                //Console.WriteLine("Upgrade Version is " + cai.VersionNumber + " saved ipd.Vers is " + ipd.Vers);
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "DOWNGRADE")
                                {
                                    //if (!mbeat)
                                    //vers = null;
                                    //if (string.IsNullOrEmpty(vers))
                                    if (string.IsNullOrEmpty(ipd.Vers))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Version was not able to be stored and cannot be verified.";
                                    }
                                    else if (string.IsNullOrEmpty(stru))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Version was not able to be stored and cannot be verified.";
                                    }
                                    else if (stru == ipd.Vers)
                                    {
                                        ipd.Result = "CHECK - Version was EQUAL to " + ipd.Vers + " before and " + stru + " after OTA. This may be a FAIL result depending on platform expected value.";
                                        InvColor(i, "yll");
                                    }
                                    else
                                    {
                                        ipd.Result = "PASS - Version was changed to " + stru + " from the starting value of " + ipd.Vers + ".";
                                        InvColor(i, "grn");
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 8:    //Downgrade Model/Serial Number check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "UPGRADE")
                                {
                                    if (cai.ModelNumber != ipd.Model)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Model Number was different after OTA was applied. " + ipd.Model +
                                                     " was the initial Model Number and " + cai.ModelNumber + " was the final Model Number.";
                                    }

                                    if (cai.SerialNumber != ipd.Serial)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Serial Number was different after OTA was applied. " + ipd.Serial +
                                                     " was the initial Serial Number and " + cai.SerialNumber + " was the final Serial Number.";
                                    }

                                    if (cai.SerialNumber != ipd.Serial && cai.ModelNumber != ipd.Model)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - BOTH Model Number and Serial Number were different after OTA was applied."
                                            + ipd.Model + " was the initial Model Number and " + cai.ModelNumber + " was the final Model Number."
                                            + ipd.Serial + " was the initial Serial Number and " + cai.SerialNumber + " was the final Serial Number.";
                                    }
                                    else
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - BOTH Model Number of " + cai.ModelNumber + " and Serial Number of " + cai.SerialNumber + " were the same after OTA was applied.";
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 9:    //Downgrade Version Number check
                            if (iter == LASTITER)   //Dont run on last iteration / overwrite previous results
                                break;
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                string[] str = cai.VersionNumber.Replace(" ", "").Split('|');
                                //Console.WriteLine("Downgrade Version is " + cai.VersionNumber + " saved ipd.Vers is " + ipd.Vers);
                                int j;
                                string stru = str[0];
                                bool result = int.TryParse(str[0], out j);
                                if (!result)
                                    stru = vers;
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "UPGRADE")
                                {
                                    //if (!mbeat)
                                    //vers = null;
                                    if (string.IsNullOrEmpty(ipd.Vers))
                                        //if (string.IsNullOrEmpty(vers))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Version was not able to be stored and cannot be verified.";
                                    }
                                    else if (string.IsNullOrEmpty(stru))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Version was not able to be stored and cannot be verified.";
                                    }
                                    else if (stru == ipd.Vers)
                                    {
                                        ipd.Result = "CHECK - Version was EQUAL to " + ipd.Vers + " before and " + stru + " after OTA. This may be a FAIL result depending on platform expected value.";
                                        InvColor(i, "yll");
                                    }
                                    else
                                    {
                                        ipd.Result = "PASS - Version was changed to " + stru + " from the starting value of " + ipd.Vers + ".";
                                        InvColor(i, "grn");
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 10:    //Upgrade CCURI check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "DOWNGRADE")
                                {
                                    if (!tbeat)
                                        ccuri = null;
                                    if (string.IsNullOrEmpty(ccuri))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - CCURI was not able to be stored and cannot be verified.";
                                    }
                                    else if (ccuri == ipd.CCURI)
                                    {
                                        ipd.Result = "CHECK - CCURI was EQUAL to " + ipd.CCURI + " before and " + ccuri + " . This may be a FAIL result depending on platform expected value.";
                                        InvColor(i, "yll");
                                    }
                                    else
                                    {
                                        ipd.Result = "PASS - CCURI was changed to " + ccuri + " from the starting value of " + ipd.CCURI + ".";
                                        InvColor(i, "grn");
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 11:    //Downgrade CCURI check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && ipd.Next == "UPGRADE")
                                {
                                    if (!tbeat)
                                        ccuri = null;
                                    if (string.IsNullOrEmpty(ccuri))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - CCURI was not able to be stored and cannot be verified.";
                                    }
                                    else if (ccuri == ipd.CCURI)
                                    {
                                        ipd.Result = "CHECK - CCURI was EQUAL to " + ipd.CCURI + " before and " + ccuri + " . This may be a FAIL result depending on platform expected value.";
                                        InvColor(i, "yll");
                                    }
                                    else
                                    {
                                        ipd.Result = "PASS - CCURI was changed to " + ccuri + " from the starting value of " + ipd.CCURI + ".";
                                        InvColor(i, "grn");
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 12:    //Prov check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL"))
                                {
                                    if (string.IsNullOrEmpty(ipd.Prov))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Provision state could not be obtained";
                                    }
                                    if (prov != ipd.Prov)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Provision state was DIFFERENT than initial Provision = " + ipd.Prov + " and after OTA Provision = " + prov + ".";
                                    }
                                    else
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - Provision state was the same value of " + ipd.Prov + " before and " + prov + " after OTA was applied.";
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 13:    //Claim check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL"))
                                {
                                    if (string.IsNullOrEmpty(ipd.Clm))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Claim state could not be obtained";
                                    }
                                    if (clm != ipd.Clm)
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - Claim state was DIFFERENT than initial Claim = " + ipd.Clm + " and after OTA Claim = " + clm + ".";
                                    }
                                    else
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - Claim state was the same value before of " + ipd.Clm + " and " + clm + " after OTA was applied.";
                                    }
                                    changed = true;
                                }
                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 14:    //Progress message check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL"))
                                {
                                    if (prog > 0)
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - " + prog + " progress messages were detected during OTA download and application.";
                                    }
                                    else
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - " + prog + " progress messages were detected during OTA download and application.";
                                    }
                                    changed = true;
                                }
                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 15:    //Node OTA success check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL"))
                                {
                                    InvColor(i, "grn");
                                    ipd.Result = "PASS - OTA for node " + ipd.Node + " was successfully installed.";
                                    changed = true;
                                }
                            }

                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA for node " + ipd.Node + " was NOT successfully installed.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 16:    //RSSI Strong Check
                            if (!LBL_Auto.Text.Contains("FAIL"))   
                            {
                                if (results.Rows[i]["OTA Result"].ToString().Contains("PASS")) //Only check once (computer and product are in fixed locations)
                                    break;
                                if (string.IsNullOrEmpty(rssi))
                                {
                                    InvColor(i, "red");
                                    ipd.Result = "FAIL - RSSI value was not able to be obtained.";
                                    rssi = "0";
                                    changed = true;
                                }
                                int val = Int32.Parse(rssi);
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && val != 0 && val.CompareTo(-67) != -1) //Indicate -67 or better
                                {
                                    InvColor(i, "grn");
                                    ipd.Result = "PASS - OTA was successfully installed with a STRONG RSSI value of " + rssi + " and with Status result of PASS."; //OTA result label did not have FAIL so it is PASS
                                    changed = true;
                                }
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL") && val != 0 && val.CompareTo(-67) == -1) //Indicate worse than -67
                                {
                                    InvColor(i, "red");
                                    ipd.Result = "FAIL - OTA was successfully installed but the RSSI value of " + rssi + " was too great (signal too weak for the STRONG test)."; //OTA result label did not have FAIL so it is PASS
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted."; //OTA result label did not have PASS so it is FAIL
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 17:    //OTAs are possible after OTA check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                //Console.WriteLine("iter is " + iter + " and autottl is " + autottl);
                                //RTB_Diag.AppendText("iter is " + iter + " and autottl is " + autottl + Environment.NewLine); RTB_Diag.ScrollToCaret();

                                if (iter ==LASTITER && !results.Rows[i]["OTA Result"].ToString().Contains("FAIL")) //Check on last iteration
                                {
                                    
                                    if (autottl > 1)
                                    {
                                        InvColor(i, "grn");
                                        ipd.Result = "PASS - " + autottl + " OTAs have been sent, downloaded, and applied in successive order.";
                                    }
                                    else
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - " + autottl + " OTAs have been sent, downloaded, and applied in successive order.";
                                    }
                                    changed = true;
                                }
                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        case 18:    //ISPPartNumber check
                            if (!LBL_Auto.Text.Contains("FAIL"))
                            {
                                if (!results.Rows[i]["OTA Result"].ToString().Contains("FAIL"))
                                {
                                    //if (!mbeat)
                                        //ispp = null;
                                    if (string.IsNullOrEmpty(ispp))
                                    {
                                        InvColor(i, "red");
                                        ipd.Result = "FAIL - ISPPartNumber was not able to be stored and cannot be verified.";
                                    }
                                    else if (ispp == ipd.ISPP)
                                    {
                                        ipd.Result = "CHECK - ISPPartNumber was EQUAL to " + ipd.ISPP + " before and " + ispp + " . This may be a FAIL result depending on platform expected value.";
                                        InvColor(i, "yll");
                                    }
                                    else
                                    {
                                        ipd.Result = "PASS - ISPPartNumber was changed to " + ispp + " from the starting value of " + ipd.ISPP + ".";
                                        InvColor(i, "grn");
                                    }
                                    changed = true;
                                }

                            }
                            else
                            {
                                InvColor(i, "red");
                                ipd.Result = "FAIL - OTA failed to install. Unable to validate if test case was impacted.";
                                changed = true;
                            }

                            if (changed)
                                SetText("auto", "AutoGen Result", i);
                            break;

                        default:
                            break;
                    }
                }
            }
            /*catch
            {
                MessageBox.Show("Catastrophic TestCheck error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom TestCheck():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public bool PendCheck(int i)
        {
            try
            {

                string[] str = results.Rows[i]["Name"].ToString().Split(' ');
                string num = str[1];

                if (skipcyc && new[] { "131835", "131837", "131839", "131841" }.Contains(num))
                {
                    //results.Rows[i]["OTA Result"] = "Skipped by User.";
                    //DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                    return true;
                }

                if (skipttf && new[] { "131862", "132552", "131865", "131854" }.Contains(num))
                {
                    //results.Rows[i]["OTA Result"] = "Skipped by User.";
                    //DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                    return true;
                }

                if (skipgen && new[] { "131812", "154635", "131844", "131845", "131845", "131846", "131847"
                                     , "131849", "131850", "131851", "131852", "186300", "186529", "131863"
                                     ,"154667", "131821", "132549","132550", "131822",}.Contains(num))
                {
                    //results.Rows[i]["OTA Result"] = "Skipped by User.";
                    //DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                    return true;
                }
                return false;
            }
            /*catch {
                MessageBox.Show("Catastrophic PendCheck error.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom PendCheck():  Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }
        }
        public void FailLeft(int phase, bool type)
        {
            try
            {
                if (type)
                {
                    string i = phase.ToString();
                    for (int j = 0; j < NODECASEMAX; j++)
                    {
                        if (results.Rows[j]["OTA Result"].ToString().Contains("PENDING"))
                        {
                            if (new[] { "0", "1" }.Contains(i) && j < 4)    //Target range for remote test cases only
                            {
                                results.Rows[j]["OTA Result"] = "FAIL - Test case failed to execute for unknown reason. Re-run AutoGenerated Suite or attempt manual retest.";
                                DGV_Data.Rows[j].Cells[6].Style.BackColor = Color.Red;
                            }
                            else if (new[] { "4", "5" }.Contains(i) && j > 18)  //Target range for TTF cases only
                            {
                                results.Rows[j]["OTA Result"] = "FAIL - Test case failed to execute for unknown reason. Re-run AutoGenerated Suite or attempt manual retest.";
                                DGV_Data.Rows[j].Cells[6].Style.BackColor = Color.Red;
                            }
                            else     //Target range for generic test cases
                            {
                                results.Rows[j]["OTA Result"] = "FAIL - Test case failed to execute for unknown reason. Re-run AutoGenerated Suite or attempt manual retest.";
                                DGV_Data.Rows[j].Cells[6].Style.BackColor = Color.Red;
                            }
                        }
                        
                    }
                }
                else
                {
                    for (int i = 0; i < NODECASEMAX; i++)
                    {
                        if (results.Rows[i]["OTA Result"].ToString().Contains("PENDING"))
                        {
                            if (cancel_request)
                            {
                                results.Rows[i]["OTA Result"] = "Cancelled by User.";
                                DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                            }
                            else if (skipcyc || skipttf || skipgen)
                            {
                                if (!PendCheck(i))
                                {
                                    results.Rows[i]["OTA Result"] = "FAIL - Test case failed to execute for unknown reason. Re-run AutoGenerated Suite or attempt manual retest.";
                                    DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                results.Rows[i]["OTA Result"] = "FAIL - Test case failed to execute for unknown reason. Re-run AutoGenerated Suite or attempt manual retest.";
                                DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Red;
                            }

                        }
                    }
                }
                
            }
            /*catch
            {
                MessageBox.Show("Catastrophic Failleft error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom FailLeft():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
        public void ProcessCyc(IPData ipd)
        {
            try
            {
                if (ipd.LList == null && LBL_Auto.Text.Contains("PASS"))
                    return;

                LinkedListNode<string> node = ipd.LList.First;

                for (int i = 0; i < 4; i++)
                {
                    if (cancel_request)
                        return;

                    if (node == null)
                        break;

                    if (node.Value.Contains("PASS"))
                        SetCyc(ipd, i, "grn", node);
                    
                    else if (node.Value.Contains("kipped"))                    
                        SetCyc(ipd, i, "yll", node);

                    else
                        SetCyc(ipd, i, "red", node);

                    node = node.Next;
                }

                ipd.LList.Clear();
            }
            /*catch
            {

                MessageBox.Show("Catastrophic ProcessCyc error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom ProcessCyc():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        private void SetResult(byte res)
        {
            if (res == 0)
            {
                cycstart = true;
            }
            else
            {
                cycstart = false;
            }
        }
        public byte[] SetTestTarget(IPData ipd, int i)
        {
            try
            {
                byte[] paybytes;
                if (ipd.Next == "UPGRADE")
                {
                    InvLabel("ud", ipd.Next);
                    paybytes = Encoding.ASCII.GetBytes(ipd.Payload);
                    ipd.Next = "DOWNGRADE";
                }
                else
                {
                    InvLabel("ud", ipd.Next);
                    paybytes = Encoding.ASCII.GetBytes(ipd.Down);
                    ipd.Next = "UPGRADE";
                }


                if (i == 0 || i == 1)
                {
                    if (skipcyc)
                        return paybytes;
                    InvLabel("phase", "REMOTE");
                    InvColor(4, "remhi");
                }
                if (i == 2 || i == 3)
                {
                    if (skipgen)
                        return paybytes;
                    InvLabel("phase", "GENERIC");
                    InvColor(19, "genhi");
                }
                if (i == 4 || i == 5)
                {
                    if (skipttf)
                        return paybytes;
                    InvLabel("phase", "TTF");
                    InvColor(23, "ttfhi");
                }

                return paybytes;
            }

            /*catch
            {
                MessageBox.Show("Catastrophic SetTestTarget error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom SetTestTarget():  Message and Stacktrace were ");
                LogException(e, true);
                return null;
            }

        }
        public void ResetTarget(int i)
        {
            try
            {

                if (i == 0 || i == 1)
                    InvColor(4, "uremhi");

                if (i == 2 || i == 3)
                    InvColor(19, "ugenhi");

                if (i == 4 || i == 5)
                    InvColor(23, "uttfhi");
            }
            /*catch
            {
                MessageBox.Show("Catastrophic ResetTarget error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom ResetTarget():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void RunTask(ConnectedApplianceInfo cai, ManualResetEventSlim sig, string ipindex, Barrier barrier)
        {
            try
            {

                foreach (IPData ipd in iplist)
                {
                    if (cancel_request)
                    {
                        //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                        return;
                    }
                    if (ipd.IPAddress.Equals(cai.IPAddress) && Thread.CurrentThread.Name.ToString().Equals(ipindex))
                    {
                        ipd.IPIndex = Int32.Parse(ipindex);
                        ipd.Signal = sig;
                        ipd.TabIndex = iplist.IndexOf(ipd);
                        bool thread_waits = true;   // Indicates this is a thread that will require a reboot time for the product

                        //Force each thread to live only two hours (process somehow got stuck)
                        System.Timers.Timer timer = new System.Timers.Timer();
                        timer.Interval = TMAX;
                        timer.Elapsed += (sender, e) => ProgressThread(sender, e, ipd);
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

                        // See if sending over MQTT or Revelation
                        if (ipd.Delivery.Equals("MQTT"))
                        {
                            if (SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                                thread_waits = true;
                            else
                            {
                                //Otherwise IP changed, use MAC address to map to new IP
                                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio_m = WifiLocal.ConnectedAppliances;
                                ConnectedApplianceInfo cai_m = cio_m.FirstOrDefault(x => x.MacAddress == ipd.MAC);
                                if (cai_m != null)
                                {
                                    string[] n_ipad = cai_m.IPAddress.Split('.');
                                    byte[] n_ipbytes = new byte[4];
                                    for (int j = 0; j < 4; j++)
                                    {
                                        n_ipbytes[j] = byte.Parse(n_ipad[j]);
                                    }
                                    if (SendMQTT(n_ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai_m, ipd))
                                        thread_waits = true;
                                }
                                else
                                {
                                    MessageBox.Show("OTA target IP Address of " + cai.IPAddress + "was changed and unable to be remapped. Ending OTA attempts and " +
                                        "closing corresponding thread.", "Error: Unable to change IP Address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    ipd.Result = "FAIL - Bad IP Address. Attempted to map new IP Address from MAC address failed.";
                                    SetText("status", "Bad IP Address", ipd.TabIndex);
                                    thread_waits = false;
                                }
                            }
                        }

                        // Set wait signal to be unlocked by thread after job is complete (result seen from log)
                        if (thread_waits)
                        {
                            //Console.WriteLine("Thread Wait reached. Thread with lock ID " + ipd.Signal.WaitHandle.Handle + " and name " + Thread.CurrentThread.Name +
                             //  " and this IP Index (from thread order) " + ipd.IPIndex + " for this IP Address " + ipd.IPAddress + ".");
                            ipd.Signal.Wait();

                            if (ipd.Result.Contains("timeout"))
                            {
                                SetText("status", "Force Close", ipd.TabIndex);
                                for (int i = 0; i < NODECASEMAX; i++)
                                {
                                    if (results.Rows[i]["OTA Result"].ToString().Contains("PENDING"))
                                        results.Rows[i]["OTA Result"] = ipd.Result;

                                    DGV_Data.Rows[i].Cells[6].Style.BackColor = Color.Red;
                                }
                            }
                                
                        }

                        timer.Stop();
                        timer.Dispose();
                        // Check to see if thread should be cancelled
                        if (cancel_request)
                        {
                            //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                            return;
                        }
                        System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio_n = WifiLocal.ConnectedAppliances;
                        ConnectedApplianceInfo cai_n = cio_n.FirstOrDefault(x => x.IPAddress == ipd.IPAddress);
                        int REMOVEME = 0;
                        Wait(3 * RECONWAIT); //Wait X minutes for MQTT to come back on after reboout out of IAP
                        for (int i = 0; i < MQTTMAX; i++)
                        {
                            if (cai_n.IsMqttConnected)
                                break;
                            //CycleWifi();
                            Wait(RECONWAIT); //If not MQTT, give more time to reconnect
                            REMOVEME = i;

                        }

                        //Console.WriteLine("Thread " + Thread.CurrentThread.Name + " finished a task.");
                        ipd.Signal.Reset();
                        //Console.WriteLine("MQTT reconnect called " + REMOVEME + " times.");
                    }
                    else
                    {
                        continue;
                    }
                }
                //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " reached barrier.");

                //Thread task cancelled or completed, signal others it is done
                barrier.SignalAndWait();
                lock (lockObj)
                {
                    FinalResult();
                }
            }
            /*catch
            {
                MessageBox.Show("Catastrophic RunTask error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                barrier.SignalAndWait();
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom RunTask():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void RunAuto(ManualResetEventSlim sig, string ipindex)
        {
            try
            {
                //Not currently used 
                //uPLibrary.Networking.M2Mqtt.Utility.Trace.TraceLevel = uPLibrary.Networking.M2Mqtt.Utility.TraceLevel.Verbose;
                //uPLibrary.Networking.M2Mqtt.Utility.Trace.TraceListener = WriteDebugTrace;
                
                if (cancel_request)
                {
                    //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");

                    return;
                }

                int dex = Int32.Parse(ipindex);
                IPData ipd = iplist[dex];
                ipd.IPIndex = dex;
                ipd.Signal = sig;
                ipd.TabIndex = iplist.IndexOf(ipd);

                timeout = 0;
                //Map most up-to-date CAI
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.MacAddress == ipd.MAC);
                if (cai != null)
                {
                    ipd.IPAddress = cai.IPAddress;
                    glblip = ipd.IPAddress;
                }

                else
                {
                    MessageBox.Show("OTA target IP Address of " + cai.IPAddress + "was changed and unable to be remapped. Ending OTA attempts and " +
                                "closing corresponding thread.", "Error: Unable to change IP Address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ipd.Result = "FAIL - Bad IP Address. Attempted to map new IP Address from MAC address failed.";
                    SetText("status", "Bad IP Address", ipd.TabIndex);
                    SetText("auto", "Bad IP Address", ipd.TabIndex);
                    //Console.WriteLine("Thread " + Thread.CurrentThread.Name + " failed to connect to CAI.");
                    FailLeft(0, false);
                    return; //THIS MAY BE A BAD IDEA
                }
                //Console.WriteLine("Pre-run CAI is" + cai.VersionNumber);
                for (int i = 0; i < AUTOCNT; i++)
                {
                    glob_i = i;
                    bool thread_waits = true;   // Indicates this is a thread that will require a reboot time for the product
                    bool check = false;
                    //Force each thread to live only two hours (process somehow got stuck)
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = TMAX;
                    timer.Elapsed += (sender, e) => ProgressThread(sender, e, ipd);
                    timer.Start();

                    //Parse payload into byte array
                    byte[] paybytes = SetTestTarget(ipd, i);

                    //Prepare IP address for sending via MQTT
                    string[] ipad = ipd.IPAddress.Split('.');
                    byte[] ipbytes = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        ipbytes[j] = byte.Parse(ipad[j]);
                    }

                    //Console.WriteLine("Iteration " + i + " starting to run.");
                    //RTB_Diag.AppendText("Iteration " + i + " starting to run." + Environment.NewLine); RTB_Diag.ScrollToCaret();

                    // Switch statement for each test group type (REMOTE, GENERIC, TTF)
                    if (ipd.Delivery.Equals("MQTT"))
                    {
                        switch (i)
                        {
                            case (0):   //Test cases that involve sending cycles using Upgrade
                                CheckBeat("init", cai, ipd);
                                TestInit(ipbytes, cai, ipd, i); //First time through we have to update globals from null
                                if (skipcyc)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                thread_waits = CycRun(cai, ipd, ipbytes);
                                check = false;
                                break;
                            case (1):   //Send Downgrade back to SOP
                                if (skipcyc)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                if (SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                                    thread_waits = true;
                                else
                                {
                                    InvLabel("auto", "FAIL");
                                    ipd.Result = "FAIL - Unable to send OTA payload to product using MQTT. Unable to validate OTA test case results.";
                                    thread_waits = false;
                                    check = false;
                                }
                                check = false;
                                break;
                            case (2):   //Generic test cases using Upgrade
                                if (skipgen)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                if (indigo && skipcyc)
                                {
                                    if (ipd.Signal != null)
                                        ipd.Signal.Reset();
                                }
                                CheckBeat("init", cai, ipd);
                                TestInit(ipbytes, cai, ipd, i);
                                if (SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                                    thread_waits = true;
                                else
                                {
                                    InvLabel("auto", "FAIL");
                                    ipd.Result = "FAIL - Unable to send OTA payload to product using MQTT. Unable to validate OTA test case results.";
                                    thread_waits = false;
                                    check = false;
                                }
                                check = true;
                                break;
                            case (3):   //Generic test cases using Downgrade
                                if (skipgen)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                CheckBeat("init", cai, ipd);
                                TestInit(ipbytes, cai, ipd, i);
                                if (SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                                    thread_waits = true;
                                else
                                {
                                    InvLabel("auto", "FAIL");
                                    ipd.Result = "FAIL - Unable to send OTA payload to product using MQTT. Unable to validate OTA test case results.";
                                    thread_waits = false;
                                    check = false;
                                }
                                check = true;
                                break;
                            case (4):   //Run TTF using Upgrade
                                if (skipttf)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                if (skipgen && skipcyc && indigo)
                                {
                                    if (ipd.Signal != null)
                                        ipd.Signal.Reset();
                                }
                                CheckBeat("init", cai, ipd);    //rem this and next
                                TestInit(ipbytes, cai, ipd, i);
                                TTFRun(cai, ipd, ipbytes);
                                thread_waits = true;
                                check = false;
                                break;
                            case (5):   //Send Downgrade back to SOP
                                if (skipttf)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    continue;
                                }
                                if (SendMQTT(ipbytes, "iot-2/cmd/isp/fmt/json", paybytes, cai, ipd))
                                    thread_waits = true;
                                else
                                {
                                    InvLabel("auto", "FAIL");
                                    ipd.Result = "FAIL - Unable to send OTA payload to product using MQTT. Unable to validate OTA test case results.";
                                    thread_waits = false;
                                    check = false;
                                }
                                check = false;
                                break;
                            default:
                                break;
                        }
                                           
                    }

                    int tstart = (int)g_time.ElapsedMilliseconds;   //Start timers to detect version already installed
                    int tstop;

                    // Set wait signal to be unlocked by thread after job is complete (result seen from log)
                    if (thread_waits)
                    {
                        if (cancel_request)
                        {
                            //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");

                            return;
                        }
                        //Console.WriteLine("Thread Wait reached. Thread with lock ID " + ipd.Signal.WaitHandle.Handle + " and name " + Thread.CurrentThread.Name +
                           //" and this IP Index (from thread order) " + ipd.IPIndex + " for this IP Address " + ipd.IPAddress + ".");
                        //RTB_Diag.AppendText("Thread Wait reached. Thread with lock ID " + ipd.Signal.WaitHandle.Handle + " and name " + Thread.CurrentThread.Name +
                        //" and this IP Index (from thread order) " + ipd.IPIndex + " for this IP Address " + ipd.IPAddress + "." + Environment.NewLine); RTB_Diag.ScrollToCaret();
                        //if (ipd.Signal != null)
                        //ipd.Signal.Reset();

                        if (i == 3 && indigo)
                            ipd.Signal.Reset();

                        ipd.Signal.Wait();

                        tstop = (int)g_time.ElapsedMilliseconds;
                        //RTB_Diag.AppendText("Thread wait unlocked after X ms " + tstop + Environment.NewLine); RTB_Diag.ScrollToCaret();

                        if (tstop-tstart < RECONWAIT && ipd.Result.Contains("PASS"))    //Less than a minute OTA with pass means version installed so not valid test
                        {
                            ipd.Result = "FAIL - Version sent already installed caused waiting thread to release too quickly."
;                           SetText("status", "Force Close", ipd.TabIndex);
                            SetText("auto", "Force Close", ipd.TabIndex);
                            FailLeft(i, true);
                        }
                        if (ipd.Result.Contains("timeout"))
                        {
                            SetText("status", "Force Close", ipd.TabIndex);
                            SetText("auto", "Force Close", ipd.TabIndex);
                            FailLeft(i, true);
                        }
                    }

                    timer.Stop();
                    timer.Dispose();

                    // Check to see if thread should be cancelled
                    if (cancel_request)
                    {
                        //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                        return;
                    }

                    int recontot = 0;
                    string res = LBL_Auto.Text;

                    if (!res.Contains("FAIL"))
                    {
                        if (i == CYCGO)
                            ProcessCyc(ipd);

                        InvLabel("auto", "RECONN");
                        autottl++;
                    }

                    if (i == LASTITER)
                    {
                        if (!skipgen)
                        {
                            InvLabel("auto", "CHECK");
                            CheckBeat("check", cai, ipd);
                            TestCheck(cai, ipd, i);
                        }
                        ipd.Result = "";
                        continue;
                    }

                    InvLabel("ud", "PENDING");

                    MqttRecon(cai, "dis");  //Disconnect MQTT to prepare for reconnect
                    StartTimer(25 * RECONWAIT + 30000);  //Allow MQTTMAX minutes for product to reboot and reconnection attempts to end (happy path)
                    //Console.WriteLine("4 minute reconwait started");
                    Wait(3 * RECONWAIT); //Wait X minutes for product to finish fully rebooting out out of IAP
                    
                    MqttRecon(cai, "con"); //Start process to reconnect MQTT
                    Wait(RECONWAIT-6000); //Give time to reconnect
                    //Console.WriteLine("4 minute reconwait ended");
                    //bool mqtt_retry = false;
                    /*if (cai.IsMqttConnected && cai.IsTraceOn)
                    {
                        Console.WriteLine("MQTT recon skipped (was connected with traceon) for iteration " + i);
                        StopTimer();
                        mqtt_retry = true;
                    }
                    else
                    {
                        Console.WriteLine("Before MQTT recon showed that IsMqttConnected was " + cai.IsMqttConnected + " and that IsTraceOn was " +
                                            cai.IsTraceOn);
                    }*/

                    //See if IP changed and we need a new CAI
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio_n = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai_n;
                    cai_n = cio_n.FirstOrDefault(x => x.MacAddress == ipd.MAC);

                    if (cai_n != null)
                    {
                        cai = cai_n;
                        ipd.IPAddress = cai.IPAddress;    //Update if IP changed
                        glblip = ipd.IPAddress;

                        MqttRecon(cai, "dis");  //Disconnect from old cai to connect to new cai
                        Wait(3000);

                        MqttRecon(cai, "con"); //Establish MQTT with new CAI
                        Wait(RECONWAIT); //Give more time to reconnect and rescan list for new changes

                        //StopTimer();
                        for (int j = 0; j < MQTTMAX; j++)
                        {
                            if (cancel_request)
                                return;
                            if (cai.IsMqttConnected && cai.IsTraceOn)
                            {
                                if (cai.IPAddress != ipd.IPAddress)
                                {
                                    ipd.IPAddress = cai.IPAddress;
                                    glblip = ipd.IPAddress;

                                }
                                break;
                            }
                            else
                            {
                                //Console.WriteLine("Iter= " + j + " in MQTT recon showed that IsMqttConnected was " + cai.IsMqttConnected + " and that IsTraceOn was " +
                                   // cai.IsTraceOn);
                            }

                            MqttRecon(cai, "dis");  //Disconnect MQTT to prepare for reconnect
                            Wait(3000);

                            MqttRecon(cai, "con");    //Reconnect MQTT

                            //StartTimer(RECONWAIT);
                            Wait(RECONWAIT); //Give more time to reconnect and rescan list for new changes

                            cai = cio_n.FirstOrDefault(x => x.MacAddress == ipd.MAC); //Search again to see if CAI has changed
                            recontot = j;
                        }

                        //Console.WriteLine("MQTT reconnect called " + recontot + " times.");

                    }

                    StopTimer();

                    if (cai == null || recontot == MQTTMAX)
                    {
                        StopTimer();
                        ipd.Result = "FAIL - Unable to reconnect to product. Unable to check test case result(s).";

                        SetText("status", "Force Close", ipd.TabIndex);
                        SetText("auto", "Force Close", ipd.TabIndex);

                        //Console.WriteLine("Thread " + Thread.CurrentThread.Name + " finished a task.");

                        if (ipd.Signal != null)
                            ipd.Signal.Reset();

                        //Console.WriteLine("Thread " + Thread.CurrentThread.Name + " failed to connect to CAI.");

                        ipd.Result = "";
                        InvLabel("auto", "PENDING");

                        continue;   //THIS MAY BE A BAD IDEA
                    }

                    //Console.WriteLine("Thread " + Thread.CurrentThread.Name + " finished a task.");
                    
                    if (ipd.Signal != null)
                        ipd.Signal.Reset();

                    if (check && !res.Contains("FAIL"))
                    {
                        InvLabel("auto", "CHECK");
                        CheckBeat("check", cai, ipd);
                        TestCheck(cai, ipd, i);
                    }
                    if (res.Contains("FAIL"))   //Skip next OTA as it would send wrong version (EX. No upgrade so don't downgrade)
                        i += 1;
                    ipd.Result = "";
                    prog = 0;   //Reset progress message counter
                    InvLabel("auto", "PENDING");
                    ResetTarget(i);

                }

                ResetTarget(5);
                FinalResult();
            }
            /*catch (Exception e)
            {
                MessageBox.Show("Catastrophic RunAuto error. Exception was " + GetExceptionDetails(e), "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom RunAuto(): i was " + glob_i + " Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        /*public static string GetExceptionDetails(Exception exception)
        {
            return "Exception: " + exception.GetType()
                + "\r\nInnerException: " + exception.InnerException
                + "\r\nMessage: " + exception.Message
                + "\r\nStackTrace: " + exception.StackTrace;
        }*/
        public void ProcessIP()
        {
            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;


                foreach (IPData ipd in iplist)
                {
                    //Selects the appliance based on IP address
                    ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ipd.IPAddress);

                    if (cancel_request)
                    {
                        //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                        return;
                    }
                    if (cai.IsTraceOn)
                        continue;
                    // Enable Trace for IP address in list
                    if (!TraceConnect(cai))
                        return;
                }

                //Set barrier to wait for all threads to complete
                Barrier barrier = new Barrier(participantCount: LB_IPs.Items.Count);

                for (int i = 0; i < LB_IPs.Items.Count; i++)
                {
                    string ip = LB_IPs.Items[i].ToString();
                    string number = "";
                    ManualResetEventSlim sig = new ManualResetEventSlim();
                    if (autogen)
                    {
                        tbeat = false;
                        mbeat = false;
                        Thread th = new Thread(() => RunAuto(sig, number));
                        th.Name = i.ToString();
                        number = th.Name;
                        th.IsBackground = true;
                        th.Start();
                    }
                    else
                    {
                        ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.IPAddress == ip);
                        Thread th = new Thread(() => RunTask(cai, sig, number, barrier));
                        th.Name = i.ToString();
                        number = th.Name;
                        th.IsBackground = true;
                        th.Start();
                    }
                }

                return;
            }
            /*catch
            {
                MessageBox.Show("ProcessIP exception.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom ProcessIP():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        /*public static string GetExceptionDetails(this Exception exception)
        {
            var properties = exception.GetType()
                                    .GetProperties();
            var fields = properties
                             .Select(property => new {
                                 Name = property.Name,
                                 Value = property.GetValue(exception, null)
                             })
                             .Select(x => String.Format(
                                 "{0} = {1}",
                                 x.Name,
                                 x.Value != null ? x.Value.ToString() : String.Empty
                             ));
            return String.Join("\n", fields);
        }*/
        bool RevelationConnect(ConnectedApplianceInfo cai)
        {
            int traceattempt = 0;

            while (traceattempt < ATTEMPTMAX)
            {
                try
                {
                    if (cancel_request)
                    {
                        //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                        return false;
                    }

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
                        return false;
                    }

                }


                /*catch
                {
                    MessageBox.Show("Trace was not able to start.  Please verify the socket can be " +
                                    "opened and it is not in use (UITracer is not running). You may need to close" +
                                "Widebox and try again.", "Error: Unable to start Trace",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }*/
                catch (Exception e)
                {
                    /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                        + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                    LogException("Venom RevelationConnect():  Message and Stacktrace were ");
                    LogException(e, true);
                    return false;
                }
            }


            return false;
        }
        bool TraceConnect(ConnectedApplianceInfo cai)
        {
            try
            {
                if (!cai.IsTraceOn)
                {
                    //CycleWifi(cai);
                    Wait(2000);
                    //If an appliance with the specified IP address is found in the list
                    if (!RevelationConnect(cai))
                    {
                        if (cancel_request)
                        {
                            //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                            return false;
                        }
                        //First round of attempts failed, close data / open data and try again
                        //CycleWifi();

                        if (!RevelationConnect(cai))
                        {
                            MessageBox.Show("Revelation/Trace was not able to start even after retry. Please close Venom and WideBox and try again.",
                                            "Error: WideBox issue prevents running Plugin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                return true;
            }
            /*catch
            {
                MessageBox.Show("Catastrophic TraceConnect error.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom TraceConnect():  Message and Stacktrace were ");
                LogException(e, true);
                return false;
            }
        }
        public void MqttRecon(ConnectedApplianceInfo cai, string type)
        {
            try
            {
                if (type.Equals("dis"))
                {
                    // Close all WifiBasic connections
                    WifiLocal.CloseAll(true);
                    Wait(2000);
                }

                if (type.Equals("con"))
                {
                    string localIP = WifiLocal.Localhost.ToString();
                    CertManager.CertificateManager certMgr = new CertManager.CertificateManager();
                    //Instead of below, see if WifiLocal.SetWifi(System.Net.IPAddress.Parse(localIP), null); works (should take what was previously set)
                    // Restart Wifi Connection
                    if (certMgr.IsLocalValid)
                    {
                        if (symcert)
                            WifiLocal.SetWifi(System.Net.IPAddress.Parse(localIP), certMgr.GetCertificate(CertManager.CertificateManager.CertificateTypes.Symantec20172020));
                        else
                            WifiLocal.SetWifi(System.Net.IPAddress.Parse(localIP), certMgr.GetCertificate(CertManager.CertificateManager.CertificateTypes.Digicert20202022));

                        Wait(2000);
                    }

                    WifiLocal.ScanConnectedAppliances(true, localIP);
                    Wait(2000);

                    WifiLocal.ConnectTo(cai);
                    Wait(5000);

                    WifiLocal.EnableTrace(cai, true);
                    Wait(2000);

                    WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                    Wait(2000);
                }

            }
            /*atch
            {
                MessageBox.Show("Catastrophic MqttRecon error.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom MqttRecon():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        public void SizeCol()
        {
            DGV_Data.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Data.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Data.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Data.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        private void BTN_Remove_Click(object sender, EventArgs e)
        {
            try
            {
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
                    if (autogen)
                    {
                        autogen = false;
                        ResetForm(true);                        
                    }
                }

            }
            /*catch { }*/
            catch (Exception f)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom BTN_Remove():  Message and Stacktrace were ");
                LogException(f, true);
                return;
            }
        }
        private void BTN_Clr_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will clear all IPs and their results from all windows. Press Yes to Clear or No to Cancel.",
                                                        "Verify Full Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                ResetForm(true);
            }

        }
        private void BTN_MakeList_Click(object sender, EventArgs e)
        {
            try
            {
                if (plist != null)
                    plist.BringToFront();
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
                try
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
                                IPData newip = new IPData(cai.IPAddress, value[1]); //MQTT Forced as type at value[1] add Revelation logic if supported
                                iplist.Add(newip);
                                newip.Node = value[2];
                                newip.Type = value[3];
                                newip.MQTTPay = value[4];
                                newip.Name = value[5];
                                newip.MAC = cai.MacAddress; //value[6]

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
                            string badips = "";
                            foreach (string ips in ipskipped)
                                badips = badips + "  " + ips;
                            if (String.IsNullOrWhiteSpace(badips))
                                badips = "EMPTY LINES";
                            MessageBox.Show("Import skipped a total of " + skipped + " line(s) for IP Address(es) " +
                                 badips + " due to NOT being listed in Wifibasic OR the import file having EMPTY lines. Please verify the IP Address(es) or empty lines " +
                                "that were skipped are connected and listed in Wifibasic or expected, then retry importing.", "Error: WifiBasic IP Address(es) Not Found or File Has Empty Lines",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        SizeCol();
                    }
                }
                /*catch
                {
                    MessageBox.Show("Import failed due to unkown error. Verify the import file has not been corrupted or in an incorrect format" +
                                    "then retry importing. Clearing IP Address list.", "Error: Import Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    iplist.Clear();
                    LB_IPs.Items.Clear();
                }*/
                catch (Exception f)
                {
                    /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                        + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                    LogException("Venom BTN_Import():  Message and Stacktrace were ");
                    LogException(f, true);

                    iplist.Clear();
                    LB_IPs.Items.Clear();
                    return;
                }
            }
        }
        private void ResetGlobal()
        {
            autottl = 0;
            gstatus = 0;
            retcnt = 0;
            prog = 0;
            timeleft = 0;
            dlcnt = 0;
            tbeat = false;
            mbeat = false;
            ttf = false;
            cycstart = false;
            cyc = false;
            skipcyc = false;
            skipttf = false;
            skipgen = false;
            indigo = false;
            tourma = false;
            tfound = false;
            symcert = false;
            ccuri = "";
            vers = "";
            ispp = "";
            prov = "";
            clm = "";
            rssi = "";
            glblip = "";
            glob_i = 0;
    }
        public void ResetForm(bool operation)
        {
            Invoke((MethodInvoker)delegate {

                BTN_Payload.Text = "Run Test List";
                //BTN_Remove.Enabled = true;
                BTN_Clr.Enabled = true;
                TB_LogDir.Enabled = true;
                LB_IPs.Enabled = true;
                //BTN_Import.Enabled = true;
                //BTN_MakeList.Enabled = true;  REMOVE COMMENT WHEN SUPPORT AGAIN
                BTN_LogDir.Enabled = true;
                BTN_Auto.Enabled = true;
                LabelSet(false);
                LBL_Auto.Text = "PENDING";
                LBL_UD.Text = "PENDING";
                LBL_i.Text = "PENDING";
                LBL_Time.Text = "00:00:00";
                StopTimer();
                if (operation)
                {
                    iplist.Clear();
                    results.Clear();
                    LB_IPs.Items.Clear();
                    DGV_Data.Refresh();
                    rerun = false;
                    autogen = false;
                    ResetGlobal();
                }
            });

        }
        public void FinalResult()
        {
            try
            {
                thread_done++;
                if (thread_done == LB_IPs.Items.Count)
                {
                    thread_done = 0;
                    //Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " was last and reset form.");
                    ResetForm(false);
                    g_time.Stop();
                    long duration = g_time.ElapsedMilliseconds;
                    g_time.Reset();
                    TimeSpan t = TimeSpan.FromMilliseconds(duration);
                    string s_dur = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
                    int totalran;
                    if (autogen)
                    {
                        totalran = autottl;
                        FailLeft(0, false);
                    }
                    else
                        totalran = iplist.Count();
                    double average = 0.0;
                    if (totalran != 0)
                        average = (double)duration / totalran;
                    TimeSpan a = TimeSpan.FromMilliseconds(average);
                    string s_avg = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                a.Hours,
                                a.Minutes,
                                a.Seconds,
                                a.Milliseconds);
                    SetText(s_dur + '\t' + s_avg, "Final", 0);
                    MessageBox.Show(totalran + " OTA Update(s) ran with a total running time of " + s_dur +
                                    "  that resulted in an average run time per OTA Update of " + s_avg
                                    + ".", "Final Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cancel_request = true;
                }
                else
                    return;
            }

            /*catch
            {
                MessageBox.Show("Catastrophic FinalResult error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            catch (Exception e)
            {
                /*MessageBox.Show("Catastrophic ProcessPayload error. source was " + source + " raw was " 
                    + raw + " sb was " + sb + " and call was " + call, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                LogException("Venom FinalResult():  Message and Stacktrace were ");
                LogException(e, true);
                return;
            }
        }
        private void Venom_Load(object sender, EventArgs e)
        {
            ResetForm(true);
        }
        private void BTN_Auto_Click(object sender, EventArgs e)
        {
            try
            {
                if (auto != null)
                    auto.BringToFront();
                auto.Show();
            }
            catch
            {

                auto = new AutoGen(this, this.WideLocal, this.WifiLocal);
                auto.Show();
            }
        }
        public void StopTimer()
        {
            if (cancel_request)
                return;
            timeleft = 0;
            Invoke((MethodInvoker)delegate
            {
                LBL_Rmn.Visible = false;
                LBL_Time.Visible = false;
                LBL_Time.Text = "00:00:00";
                TMR_Tick.Stop();
            });
        }
        public void StartTimer(int val)
        {
            if (cancel_request)
                return;
            timeleft = val;
            Invoke((MethodInvoker)delegate
            {

                LBL_Rmn.Visible = true;
                LBL_Time.Visible = true;
                TimeSpan t = TimeSpan.FromMilliseconds(val);
                LBL_Time.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
                TMR_Tick.Enabled = true;
                TMR_Tick.Start();
            });
        }
        private void TMR_Tick_Tick(object sender, EventArgs e)
        {
            if (cancel_request)
            {
                if (LBL_Time.Enabled)
                Invoke((MethodInvoker)delegate
                {
                    LBL_Time.Enabled = false;
                    LBL_Time.Visible = false;

                });
                return;
            }
            timeleft -= 1000;

            if (timeleft < 0)
                timeleft = 0;

            Invoke((MethodInvoker)delegate
            {
                TimeSpan t = TimeSpan.FromMilliseconds(timeleft);
                LBL_Time.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    t.Hours,
                    t.Minutes,
                    t.Seconds);
            });
        }
        /*public static void WriteDebugTrace(string format, params object[] args) Lucas extra debug logging....doesn't work right now
        {
            File.AppendAllLines(@"c:\temp\wideboxdebugtrace.log", new List<string>() { string.Format(format, args) });
        }*/
    }
}
