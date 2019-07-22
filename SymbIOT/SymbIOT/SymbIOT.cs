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
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SymbIOT
{
    public partial class SymbIOT : WideInterface
    {
        public const byte API_NUMBER = 144;//TODO: change to API value
        /// <summary>
        /// Opcodes parsed by this form
        /// </summary>
        public enum OPCODES
        {
            //Include your opcodes in here
            //SET_VALUE = 1
        }

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
        public Cycle curCycle;
        public string curString;
        public string tempKey;
        public string timeKey;
        public string timeOpKey;
        public string delayKey;
        public string opKey;
        public string cpltKey;
        public string sabKey;
        public string stepKey;
        public string probeAmtKey;
        public string doneKey;
        public string powKey;
        public string v1msgKey;
        public string v2msgKey;
        public string savedExtractedMessage;
        public bool relational;
        public bool fromAppliance;
        public int delaymax;
        public int delaymin;
        public List<ComboBox> settings;
        public List<Label> setLabels;
        public List<string> rawlogs;
        public string[, ,] displayLabels;
        public Panel OuterPanel;
        public Panel InnerPanel;
        public XCategory xc;
        public string implementation;
        public string dm_name;
        public bool usesCookTimeOp;
        public int selectedTab;
        public static int tempUnits;
        public string rawpacket;
        private DataTable parsedlog;
        public string[] categoryList = { "Cooking", "Dish", "Laundry", "Refrigeration", "Small Appliance" };
        private KeyValue selectedMod;

        public Scripter scriptwindow;
        public DataTable listenList;
        public bool listening;
        public DateTime listenUntil;
        public System.Timers.Timer listenTimer;
        public bool capabilityLoaded;

        private bool isExporting;
        private string exportLog;

        private int logtype;
        const int NONE = 0;
        const int UITRACE = 1;
        const int WIDE = 2;
        const int WIN = 3;
        const int MQTT = 4;        

        //to access wideLocal use base.WideLocal or simple WideLocal
        public SymbIOT(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();

            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";           

            rawlogs = new List<string>();
            keyValues = new List<KeyValue>();
            enumerations = new List<Enumeration>();
            allenums = new List<Enumeration>();
            cycles = new List<List<Cycle>>();
            cyclesU = new List<Cycle>();
            cyclesL = new List<Cycle>();
            cyclesM = new List<Cycle>();
            cyclesW = new List<Cycle>();
            cyclesD = new List<Cycle>();            
            settings = new List<ComboBox>();
            setLabels = new List<Label>();
            curCycle = null;
            curString = "";
            kvapi = 144;
            savedExtractedMessage = "";
            tempUnits = 1;
            relational = false;
            fromAppliance = false;
            usesCookTimeOp = true;
            isExporting = false;
            exportLog = "";
            capabilityLoaded = false;

            Scripter scriptwindow = new Scripter(this, keyValues);
            listenList = new DataTable();
            listenList.Columns.Add("Key");
            listenList.Columns.Add("Name");
            listenList.Columns.Add("Value");
            listenList.Columns.Add("HexValue");
            listenList.Columns.Add("Result");
            listenList.Columns.Add("Pass");
            listenList.Columns.Add("Timestamp");
            listening = false;
            listenUntil = DateTime.Now;
            listenList.RowChanged += new DataRowChangeEventHandler(listenList_RowChanged);
            listenTimer = new System.Timers.Timer();
            listenTimer.AutoReset = false;
            listenTimer.Elapsed += new System.Timers.ElapsedEventHandler(listenTimer_Elapsed);         
            

            //SwitchCycles(TAB_Platform_Cooking.SelectedIndex);

            implementation = "";
            dm_name = "";
            //Add your constructor operations here

            //AutoSave: Load and Save your CheckBoxes, TextBoxes and ComboBoxes last conditions
            //Default is true, this line can be removed
            AutoSave = false;
            //To facilitate the debug of your form you can trace your exception or print messages using LogException function
            //  LogException("My Error Message");
            //  or
            //  try { //your code } catch (Exception ex) { LogException(ex,false); }
            //Avoid to do a to much operations during the construction time to have a faster open time and avoid opening the form errors.
            //Errors during the construction time are hard to debug because your object are not instantiated yet.
        }

        void SetupLabels()
        {
            if (dm_name == "Laundry")
            {
                displayLabels = new string[2, 5, 3]; //tab#, label#, key,value,rawvalue
                displayLabels[0, 0, 0] = "03010002;03050001";
                displayLabels[1, 0, 0] = "03010002;03050001";

                displayLabels[0, 1, 0] = "03070001";
                displayLabels[1, 1, 0] = "03070001";

                displayLabels[0, 2, 0] = "05050006";
                displayLabels[1, 2, 0] = "07040001";

                displayLabels[0, 3, 0] = "03010001";
                displayLabels[1, 3, 0] = "03010001;07040005";

                displayLabels[0, 4, 0] = "03040001";
                displayLabels[1, 4, 0] = "03040001";

                OuterPanel = PAN_Laundry_Outer;
                InnerPanel = PAN_Laundry_Inner;
            }
            else
            {
                displayLabels = new string[3, 12, 3]; //tab#, label#, key,value,rawvalue
                for (int i = 0; i < displayLabels.GetLength(0); i++)
                {
                    string timestatus = "0301";
                    string displaystatus = "0302";
                    string opstatus = "0303";
                    string timeset = "030D";
                    string opset = "030F";
                    string cycleset = "0310";
                    string custcycleset = "0317";
                    switch (i)
                    {
                        case 0://Upper
                            timestatus = "0301";
                            displaystatus = "0302";
                            opstatus = "0303";
                            timeset = "030D";
                            opset = "030F";
                            cycleset = "0310";
                            custcycleset = "0317";
                            break;
                        case 1://Lower
                            timestatus = "0307";
                            displaystatus = "0308";
                            opstatus = "0309";
                            timeset = "0312";
                            opset = "0314";
                            cycleset = "0315";
                            custcycleset = "0318";
                            break;
                        case 2://MWO
                            timestatus = "0401";
                            displaystatus = "0402";
                            opstatus = "0403";
                            timeset = "0406";
                            opset = "0408";
                            cycleset = "0409";
                            custcycleset = "040B";
                            break;
                        default:
                            break;
                    }
                    displayLabels[i, 0, 0] = timestatus + "0001;" + timeset + "0001";
                    displayLabels[i, 1, 0] = timestatus + "0002";
                    displayLabels[i, 2, 0] = timestatus + "0003;" + timeset + "0003";
                    displayLabels[i, 3, 0] = i == 2 ? displaystatus + "0001;" + cycleset + "0003" : displaystatus + "0001";
                    displayLabels[i, 4, 0] = opset + "0001";
                    displayLabels[i, 5, 0] = i == 2 ? cycleset + "0001;" + cycleset + "0004" : cycleset + "0001";

                    displayLabels[i, 7, 0] = "08010002;08060001";
                    displayLabels[i, 8, 0] = opstatus + "0001";
                    displayLabels[i, 9, 0] = opstatus + "0003";
                    displayLabels[i, 10, 0] = custcycleset + "0002";
                    displayLabels[i, 11, 0] = opset + "0002";
                    for (int j = 0; j < displayLabels.GetLength(1); j++)
                    {
                        displayLabels[i, j, 2] = "0";
                    }
                }
                OuterPanel = PAN_Outer;
                InnerPanel = PAN_Inner;
            }
        }

        void listenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            listening = false;
            listenTimer.Enabled = false;
            scriptwindow.Invoke(scriptwindow.scriptDelegate);            
        }

        void listenList_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (listening)
            {
            }
        }        

        /// <summary>
        /// Parse message from WideBoxInterface
        /// </summary>
        /// <param name="data">Simple Whirlpool packet data with extended info (TimeStamp and IsValid)</param>
        public override void parseSimpleWhirlpoolMessage(ExtendedSimpleWhirlpoolPacket data)
        {
            if (RB_Reveal.Checked)
            {
                RevealPacket reveal_pkt = new RevealPacket();
                int m2mnode;
                try
                {
                     m2mnode = TB_Node.Text == "" ? 15 : int.Parse(TB_Node.Text);
                }
                catch
                {
                    m2mnode = 15;
                }
                if (reveal_pkt.ParseSimpleWhirlpoolMessage(data))
                {
                    switch (reveal_pkt.API)
                    {
                        case API_NUMBER:   //parse opcodes to this API (already without the feedback bit, i.e. 0x25: reveal_pkt.OpCode = 5 , reveal_pkt.IsFeedback = true )
                            
                                switch (reveal_pkt.OpCode)
                                {
                                    case 6:
                                        if (reveal_pkt.IsFeedback && ((int)reveal_pkt.Source == m2mnode || m2mnode == 15))
                                        {
                                            savedExtractedMessage += string.Concat(Array.ConvertAll(reveal_pkt.PayLoad, b => b.ToString("X2"))).Substring(2);
                                            if (reveal_pkt.PayLoad[0] > 127)
                                            {
                                                rawpacket = reveal_pkt.ToSimpleWhirlpoolPacket().ToString();
                                                relational = savedExtractedMessage.ToUpper().StartsWith("FE") ? true : false;
                                                InterpretMQTT(savedExtractedMessage, false, true);                                                
                                            }
                                        }
                                        break;
                                    case 5:
                                        if (reveal_pkt.IsFeedback)
                                        {
                                            SetResult(reveal_pkt.PayLoad[2]);
                                        }
                                        else
                                        {
                                            int startbyte = 2;
                                            if (reveal_pkt.PayLoad[0] == 0 || reveal_pkt.PayLoad[0] == 128)
                                            {
                                                startbyte = 6;
                                            }
                                            savedExtractedMessage += string.Concat(Array.ConvertAll(reveal_pkt.PayLoad, b => b.ToString("X2"))).Substring(startbyte);
                                            if (reveal_pkt.PayLoad[0] > 127)
                                            {
                                                rawpacket = reveal_pkt.ToSimpleWhirlpoolPacket().ToString();
                                                InterpretMQTT(savedExtractedMessage, false, false);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            
                            break;
                        case 141:
                            if (reveal_pkt.OpCode > 1 && reveal_pkt.OpCode < 6 && reveal_pkt.PayLoad.Length > 0)
                            {
                                setLEDs(reveal_pkt.OpCode,reveal_pkt.PayLoad[0]);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void setLEDs(byte opCode, byte payload)
        {
            if (opCode < 4)
            {
                switch (payload)
                {
                    case 0:
                        LED_Router.SetColor(Color.Red);
                        break;
                    case 1:
                        LED_Router.SetColor(Color.Yellow);
                        break;
                    case 2:
                        LED_Router.SetColor(Color.LightGreen);
                        break;
                    default:
                        LED_Router.SetColor(Color.DarkGray);
                        break;
                }
            }
            else
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
            if (RB_MQTT.Checked)
            {
                //RevealPacket reveal_pkt = new RevealPacket();

                switch (data.Topic)
                {
                    case "iot-2/evt/cc_Kvp/fmt/binary":
                        savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2")));//.Substring(14);
                        rawpacket = savedExtractedMessage;
                        relational = savedExtractedMessage.ToUpper().Substring(14).StartsWith("FE") ? true : false;
                        InterpretMQTT(savedExtractedMessage,true,true);
                        break;
                    case "iot-2/evt/cc_SetKvpResult/fmt/binary":
                        SetResult(data.Message[data.Message.Length - 1]);
                        break;
                    case "iot-2/cmd/cc_SetKvp/fmt/binary":
                        savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2"))).Substring(10);
                        rawpacket = savedExtractedMessage;
                        InterpretMQTT(savedExtractedMessage, false, false);
                        break;
                    case "iot-2/evt/subscribe/fmt/json":
                        string pay = System.Text.Encoding.ASCII.GetString(data.Message);
                        setLEDs((byte)4,(pay.Contains("1") ? (byte)3 : (byte)2));
                        setLEDs((byte)3, (byte)2);
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

        private void BTN_LoadDM_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data Model/DDM Definitions (*.xlsx, *.json)|*.xlsx; *.json";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                keyValues.Clear();
                enumerations.Clear();
                settings.Clear();
                setLabels.Clear();
                cyclesL.Clear();
                cyclesM.Clear();
                cyclesU.Clear();
                PAN_Settings.Controls.Clear();                
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
                    TB_Def.Text = ofd.FileName;
                    //BTN_LoadCap.Enabled = true;
                    //BTN_App.Enabled = true;
                    BTN_XCat.Enabled = true;
                    xc = new XCategory(keyValues, this);
                    BTN_Relational.Enabled = true;
                    BTN_LoadLog.Enabled = true;
                    BTN_Script.Enabled = true;
                    BTN_Capability.Enabled = true;

                    if (Array.IndexOf(categoryList, dm_name) < 0)
                    {
                        TabSelect dm = new TabSelect("Please select Data Model category", categoryList);
                        dm.ShowDialog();
                        dm_name = dm.SelectedTab;
                    }

                    TB_Cap.Text = dm_name + " - " + implementation;
                    switch (dm_name)
                    {
                        case "Laundry":
                            TAB_Platform_Laundry.Visible = true;
                            TAB_Platform_Cooking.Visible = false;
                            cycles.Add(cyclesW);
                            cycles.Add(cyclesD);
                            break;
                        default:
                            TAB_Platform_Cooking.Visible = true;
                            TAB_Platform_Laundry.Visible = false;
                            cycles.Add(cyclesU);
                            cycles.Add(cyclesL);
                            cycles.Add(cyclesM);
                            break;
                    }

                    SetupLabels();
                    CyclesFromDM(keyValues);
                    SettingsLabels();
                    SwitchCycles(0);
                }

            }
        }

        /// <summary>
        /// Parse key value data from an Excel file.  Currently used for Jenn-Air Minerva
        /// </summary>
        /// <param name="filename">Filename to parse</param>
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
                if (Array.IndexOf(excelSheets, "Definitions") >=0)
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
                                    if (e.Name == curEntity +"."+ fixedname)
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
                                    curEnum = new Enumeration(curEntity+"."+data2.Rows[j][2].ToString());
                                    cloneEnum = new Enumeration(curEntity+"."+data2.Rows[j][2].ToString());
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
            ddm = ddm.Replace("\"","");
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
                    level ++;
                    if(level > 1)
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
                            string [] enums = attdict["EnumValues"].Substring(1,attdict["EnumValues"].Length-2).Split(';');
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
            for(int i = 0; i < keyValues.Count; i++)
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

        private void BTN_LoadCap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Capability Data (*.xlsx)|*.xlsx";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=No'", ofd.FileName);
                string query;// = string.Format("SELECT * FROM [{0}$]", "Keys");
                DataTable data = new DataTable();
                DataTable dt = new DataTable();
                string[] excelSheets = null;
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
                    CBO_Cycles.Items.Clear();
                    foreach (List<Cycle> l in cycles)
                    {
                        l.Clear();
                    }
                    settings.Clear();
                    setLabels.Clear();
                    PAN_Settings.Controls.Clear();

                    foreach (string s in excelSheets)
                    {
                        if (!s.Contains("#"))
                        {
                            query = string.Format("SELECT * FROM [{0}$]", s);

                            using (OleDbConnection con = new OleDbConnection(connectionString))
                            {
                                con.Open();
                                //dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                                adapter.Fill(data);
                                con.Close();
                            }
                            string mqttString = "";
                            int binaryCol = -1;
                            for (int j = 0; j < data.Columns.Count; j++)
                            {
                                if (data.Rows[1][j].ToString() == "Binary") { binaryCol = j; }
                            }
                            if (binaryCol > 0)
                            {
                                for (int k = 2; k < data.Rows.Count; k++)
                                {
                                    mqttString += data.Rows[k][binaryCol].ToString();
                                }
                                relational = true;
                                InterpretMQTT(mqttString,false,true);
                            }
                            data.Clear();
                        }
                    }
                    TB_Cap.Text = ofd.FileName;
                    TAB_Platform_Cooking.Enabled = true;
                    BTN_Relational.Enabled = true;


                    /*foreach (Cycle c in cycles)
                    {
                        CBO_Cycles.Items.Add(c.Name);
                    }*/
                }
                catch { }
            }


        }

        public void InterpretMQTT(string input, bool hasLength, bool isfb)
        {
            string payload;// = input.Substring(input.IndexOf("[", input.IndexOf("DATA")) + 1);
            payload = input.ToUpper();
            savedExtractedMessage = "";
            string extractedMessage = payload.ToUpper();
            //bool isfb = true;
            int msgstart = 10;
            /*if (input.Contains("SETKVP"))
            {
                isfb = false;
                msgstart = 6;
            }*/

            int curapi = 144;

            if (hasLength)
            {
                extractedMessage = "";
                while (payload.Length > 0)
                {
                    try
                    {
                        int messagelength = int.Parse(payload.Substring(0, 4), NumberStyles.AllowHexSpecifier);
                        extractedMessage += payload.Substring(4, messagelength * 2).Substring(msgstart);
                        payload = payload.Substring(4 + messagelength * 2);
                    }
                    catch
                    {
                        try
                        {
                            int messagelength = int.Parse(payload.Substring(0, 4), NumberStyles.AllowHexSpecifier);
                            extractedMessage += payload.Substring(4).Substring(msgstart);
                            payload = "";
                        }
                        catch
                        {
                            payload = "";
                        }
                    }
                }
            }

            if (!isfb)
            {
                extractedMessage = input.ToUpper();
            }

            int keylength = 8;
            string zeroes = "00000000";
            bool badlength = false;
            bool isSettings = false;
            //loop through the message and extract the KVP data based on the loaded KVP definitions
            while (extractedMessage.Length > 0)
            {
                try
                {
                    //extract the first key ID from the message based on the expected length of the key
                    string currentKey = extractedMessage.Substring(0, keylength);
                    bool keyfound = false;
                    //compare the extracted key to all the keys in the list of loaded values
                    foreach (KeyValue kv in keyValues)
                    {
                        string paddedkey = (zeroes + kv.KeyID).Substring((zeroes + kv.KeyID).Length - keylength);
                        if (paddedkey == currentKey && (kv.API == curapi || kv.API == 0))
                        {
                            keyfound = true;
                            string value = "";
                            string retvalue;
                            int messagelength = 0;
                            //If key is found, use length and type data associated with this key to extract the correct number of
                            //payload bytes and format them appropriately
                            if (kv.Length > 0)
                            {
                                value = extractedMessage.Substring(keylength, kv.Length);
                                try
                                {
                                    retvalue = kv.Process(value);
                                    /*if (kv.Length < 8)
                                    {
                                        value = int.Parse(extractedMessage.Substring(keylength, kv.Length), NumberStyles.AllowHexSpecifier);
                                        if (kv.isSigned)
                                        {
                                            string un_data = extractedMessage.Substring(keylength, kv.Length);
                                            if (un_data.Length == 2 && int.Parse(un_data, NumberStyles.AllowHexSpecifier) > 127)
                                            {
                                                un_data = un_data.PadLeft(4, 'F');
                                            }
                                            value = unchecked((short)Convert.ToUInt16(un_data, 16));
                                        }
                                        retvalue = value.ToString();
                                    }
                                    else
                                    {
                                        //Default assumption is that a 4-byte value is a bit-shifted integer
                                        /*value = int.Parse(extractedMessage.Substring(keylength, kv.Length), NumberStyles.AllowHexSpecifier);
                                        double dec = (double)value / 65536;
                                        retvalue = dec != 0 ? dec.ToString("#.####") : "0";
                                        retvalue = long.Parse(extractedMessage.Substring(keylength, kv.Length), NumberStyles.AllowHexSpecifier).ToString();
                                    }*/
                                }
                                catch (ArgumentOutOfRangeException) //if the end of the message is reached prematurely
                                {
                                    retvalue = "Key length mismatch";
                                    badlength = true;
                                }
                                catch //if value cannot be parsed as an int, show raw hex
                                {
                                    try
                                    {
                                        retvalue = float.Parse(value, NumberStyles.AllowHexSpecifier).ToString();
                                    }
                                    catch
                                    {
                                        retvalue = value;
                                    }

                                }
                                messagelength = kv.Length;
                            }
                            else //if a string value, convert each hex digit to a char and string them together until null character is reached
                            {
                                messagelength = 0;
                                string _string = "";
                                for (int i = keylength; i < extractedMessage.Length; i += 2)
                                {
                                    if (extractedMessage.Substring(i, 2) == "00")
                                    {
                                        messagelength = i - (keylength - 2);
                                        break;
                                    }
                                    else
                                    {
                                        messagelength += 2;
                                        _string += Convert.ToChar(int.Parse(extractedMessage.Substring(i, 2), NumberStyles.AllowHexSpecifier));
                                    }
                                }
                                retvalue = _string;//extractedMessage.Substring(4, messagelength);
                                curString = kv.KeyID.StartsWith("FE") ? _string : curString;
                            }
                            if (isfb)
                            {
                                kv.RcvValue = retvalue;
                                kv.RawValue = value;
                            }
                            else
                            {
                                kv.SetValue = retvalue;
                                kv.SetRawValue = value;
                            }
                            string direction = isfb ? "\t<--" : "-->";
                            string rv = CB_Raw.Checked ? " (0x" + value + ")" : "";
                            string timestamp = CB_Timestamp.Checked ? DateTime.Now.ToString("HH:mm:ss.fff") + " " : "";
                            if (!isExporting)
                            {
                                LB_Log.Items.Add(timestamp + direction + "(0x" + kv.KeyID + ")" + kv.Instance + "_" + kv.AttributeName + ": " + retvalue + rv);
                                rawlogs.Add(rawpacket);
                                if (CB_Scroll.Checked)
                                {
                                    LB_Log.TopIndex = LB_Log.Items.Count - 1;
                                }

                                if (listening)
                                {
                                    //if (DateTime.Now < listenUntil)
                                    {
                                        foreach (DataRow dr in listenList.Rows)
                                        {
                                            if (dr["Key"].ToString() == kv.KeyID /*&& dr["Result"].ToString() == ""*/)
                                            {
                                                if (dr["Value"].ToString() == retvalue || dr["Value"].ToString() == "" || dr["HexValue"].ToString() == kv.KeyID + value)
                                                {
                                                    dr["Result"] += "\tReceived result: (0x" + kv.KeyID + ")" + kv.Instance + "_" + kv.AttributeName + ": " + retvalue + "\r\n";
                                                    dr["Pass"] += ";1";
                                                    dr["Timestamp"] += ";" + DateTime.Now.ToString("MMddyy hh:mm:ss");
                                                }
                                                else
                                                {
                                                    dr["Result"] += "\tReceived result: (0x" + kv.KeyID + ")" + kv.Instance + "_" + kv.AttributeName + ": " + retvalue + "\r\n";
                                                    dr["Pass"] += ";0";
                                                    dr["Timestamp"] += ";" + DateTime.Now.ToString("MMddyy hh:mm:ss");
                                                }
                                            }
                                        }
                                    }
                                    /*else
                                    {
                                        listening = false;
                                        scriptwindow.UpdateLog();
                                    }*/
                                }
                                if (isfb)
                                {
                                    if (kv.KeyID == "03100001")
                                    {
                                    }
                                    //if (kv.KeyID.StartsWith("01"))
                                    {
                                        xc.RefreshAll();
                                    }
                                    if (relational)
                                    {
                                        /*if (kv.KeyID == "010C0004")
                                        {
                                            SettingsData(retvalue);
                                        }
                                        else
                                        {
                                            if (kv.KeyID == "FE010001")
                                            {
                                                if (retvalue == "FD010002")
                                                {
                                                    isSettings = true;
                                                }
                                            }
                                            else
                                            {
                                                SetCycle(kv, value, retvalue);
                                            }
                                        }*/
                                    }
                                    else
                                    {
                                        try
                                        {
                                            foreach (ComboBox c in settings)
                                            {
                                                if ((KeyValue)c.Tag == kv)
                                                {
                                                    fromAppliance = true;
                                                    for (int i = 0; i < c.Items.Count; i++)
                                                    {
                                                        if (c.Items[i].ToString().StartsWith(value))
                                                        {
                                                            c.SelectedIndex = i;
                                                        }
                                                    }
                                                    if (fromAppliance)
                                                    {
                                                        Settings_SelectedIndexChanged(c, null);
                                                    }
                                                }
                                            }
                                        }
                                        catch { }
                                        try
                                        {
                                            for (int j = 0; j < displayLabels.GetLength(0); j++)
                                            {
                                                for (int k = 0; k < displayLabels.GetLength(1); k++)
                                                {
                                                    try
                                                    {
                                                        if (displayLabels[j, k, 0].Contains(kv.KeyID))
                                                        {
                                                            displayLabels[j, k, 1] = retvalue;
                                                            displayLabels[j, k, 2] = value;
                                                            RefreshDisplay();
                                                        }
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                        catch { }
                                        if (kv.KeyID == "01080001")
                                        {
                                            if (retvalue == "1")
                                            {
                                                TB_Remote.Text = "REMOTE ON";
                                                TB_Remote.BackColor = Color.LightGreen;
                                            }
                                            else
                                            {
                                                TB_Remote.Text = "REMOTE OFF";
                                                TB_Remote.BackColor = Color.Red;
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                exportLog += timestamp + direction + "(0x" + kv.KeyID + ")" + kv.Instance + "_" + kv.AttributeName + ": " + retvalue + rv + "\r\n";
                            }
                            //if key value has an associated enum, replace enum with value
                            /*if (kv.EnumName != null)
                             {
                                 foreach (Enumeration enu in enumerations)
                                 {
                                     if (enu.Name == kv.EnumName.Name)
                                     {
                                         try
                                         {
                                             retvalue = enu.Enums[value].ToString();
                                         }
                                         catch
                                         {
                                             retvalue = value.ToString();
                                         }
                                     }
                                 }
                             }*/
                            extractedMessage = badlength ? "" : extractedMessage.Substring(keylength + messagelength);
                            /*string outputString = "(0x" + paddedkey + ")" + kv.Class + "_" + kv.DisplayName + ": " + retvalue;
                            extractedMessage = badlength ? "" : extractedMessage.Substring(keylength + messagelength);
                            if (isfb)
                            {
                                LB_ToServer.Items.Add(outputString);
                                LB_ToServer.SelectedIndex = LB_ToServer.Items.Count - 1;
                                if (islogging)
                                {
                                    sw.WriteLine("\t\t\t\t" + outputString);
                                }
                            }
                            else
                            {
                                LB_FromServer.Items.Add(outputString);
                                LB_FromServer.SelectedIndex = LB_FromServer.Items.Count - 1;
                                if (islogging)
                                {
                                    sw.WriteLine(outputString);
                                }
                            }

                            //savedExtractedMessage = extractedMessage;
                            //extractedMessage = "";*/
                        }
                    }
                    if (!keyfound)
                    {
                        LB_Log.Items.Add("!! Unreadable Message - Key Not Found In Definitions (0x" + currentKey + ")");
                        rawlogs.Add(rawpacket);
                        if (CB_Scroll.Checked)
                        {
                            LB_Log.TopIndex = LB_Log.Items.Count - 1;
                        }
                        if (isExporting)
                        {
                            exportLog += "!! Unreadable Message - Key Not Found In Definitions (0x" + currentKey + ")" + "\r\n";
                        }
                        extractedMessage = "";
                    }
                }
                catch
                {
                    //savedExtractedMessage = extractedMessage;
                    extractedMessage = "";
                }

            }
            if (isSettings)
            {
                TB_Cap.Text = "Loaded from Appliance";
                SettingsLabels();
                TAB_Platform_Cooking.Enabled = true;
                BTN_Relational.Enabled = true;
                SwitchCycles(TAB_Platform_Cooking.SelectedIndex);
            }
        }

        public void RefreshDisplay()
        {
            foreach (Control c in OuterPanel.Controls)
            {
                if (c.Tag.ToString() != "PAN_Inner")
                {
                    c.Visible = false;
                }
            }
            foreach (Control c in InnerPanel.Controls)
            {
                c.Visible = false;
            }

            int j = selectedTab;

            for (int k = 0; k < displayLabels.GetLength(1); k++)
            {
                foreach (Control c in OuterPanel.Controls)
                {
                    try
                    {
                        if (c.Tag.ToString() == (k.ToString()) && displayLabels[j, k, 1] != null)
                        {
                            c.Text = displayLabels[j, k, 1];

                            if (int.Parse(displayLabels[j, k, 2], NumberStyles.AllowHexSpecifier) == 0 || (k == 4 && int.Parse(displayLabels[j, k, 2], NumberStyles.AllowHexSpecifier) > 3 && dm_name == "Cooking"))
                            {
                                c.Visible = false;
                            }
                            else
                            {
                                c.Visible = true;
                            }
                        }
                    }
                    catch { }
                }
                foreach (Control c in InnerPanel.Controls)
                {
                    try
                    {
                        if (c.Tag.ToString() == (k.ToString()) && displayLabels[j, k, 1] != null)
                        {
                            c.Text = displayLabels[j, k, 1];
                            if (int.Parse(displayLabels[j, k, 2], NumberStyles.AllowHexSpecifier) == 0)
                            {
                                c.Visible = false;
                            }
                            else
                            {
                                c.Visible = true;
                            }
                        }
                    }
                    catch { }
                }
            }

        }

        public void CyclesFromDM(List<KeyValue> k)
        {
            foreach (KeyValue kv in k)
            {
                if (kv.IsUsed)
                {
                    if (kv.EnumName != null)
                    {
                        for (int i = 0; i < kv.EnumName.Enums.Count; i++)
                        {
                            if (kv.EnumName.IsUsed[i])
                            {
                                SetCycle(kv, i.ToString("X2"), kv.EnumName.Enums[i].ToString());
                            }
                        }
                    }
                    else
                    {
                        if(kv.KeyID.StartsWith("02"))
                        {
                            SetCycle(kv, null, null);
                        }
                    }
                }
            }
        }

        public void SetCycle(KeyValue kv, string rawvalue, string retvalue)
        {
            if (kv.KeyID.StartsWith("FE"))
            {
                curCycle = null;
            }
            else
            {
                if (dm_name == "Laundry")
                {
                    switch (kv.KeyID)
                    {
                        case "05050006":
                            string cwname = retvalue;
                            Cycle cw = new Cycle(kv.KeyID, int.Parse(rawvalue, NumberStyles.AllowHexSpecifier), cwname);
                            cyclesW.Add(cw);
                            break;
                        case "07040001":
                            string cdname = retvalue;
                            Cycle cd = new Cycle(kv.KeyID, int.Parse(rawvalue, NumberStyles.AllowHexSpecifier), cdname);
                            cyclesD.Add(cd);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (!kv.KeyID.StartsWith("09") && !(curString.Contains("Setting") || curString.Contains("Conversion") || curString.Contains("0x")) && rawvalue != "00")
                    {
                        bool found = false;
                        List<Cycle> l = cyclesU;
                        int idx = 0;
                        int attid = int.Parse(kv.KeyID.Substring(4, 4), NumberStyles.AllowHexSpecifier);
                        switch (kv.KeyID.Substring(0, 4))
                        {
                            case "0310":
                                if (attid > 11 || attid == 3)
                                {
                                    l = cyclesU;
                                    idx = 0;
                                }
                                else
                                {
                                    l = null;
                                }
                                break;
                            case "0315":
                                if (attid > 11 || attid == 3)
                                {
                                    l = cyclesL;
                                    idx = 1;
                                }
                                else
                                {
                                    l = null;
                                }
                                break;
                            case "0B02":
                                l = cyclesM;
                                idx = 2;
                                break;
                            default:
                                l = null;
                                break;
                        }

                        if (l != null)
                        {
                            string cname = kv.AttributeName + "_" + retvalue;

                            foreach (Cycle c in l)
                            {
                                if (cname == c.Name)
                                {
                                    found = true;
                                    curCycle = c;
                                }
                            }
                            if (!found)
                            {
                                Cycle c = new Cycle(kv.KeyID, int.Parse(rawvalue, NumberStyles.AllowHexSpecifier), cname);
                                l.Add(c);
                                //CBO_Cycles.Items.Add(c.Name);
                                curCycle = c;
                                displayLabels[idx, 6, 0] += kv.KeyID + ";";
                            }
                        }
                    }
                }
                if (kv.KeyID.StartsWith("09") && !(curString.Contains("Setting") || curString.Contains("0x")))
                {
                    switch (kv.KeyID)
                    {
                        case "09020002":
                            curCycle.MaxTemp = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020003":
                            curCycle.MinTemp = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09010001":
                            curCycle.MaxTime = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09010002":
                            curCycle.MinTime = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020006":
                            curCycle.MinAmt = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020005":
                            curCycle.MaxAmt = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020007":
                            curCycle.StepAmt = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020004":
                            curCycle.DefAmt = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09020001":
                            curCycle.DefTemp = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "0902000C":
                            curCycle.ValAmt = retvalue;
                            break;
                        case "09010003":
                            delaymax = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        case "09010004":
                            delaymin = int.Parse(rawvalue, NumberStyles.AllowHexSpecifier);
                            break;
                        default:
                            break;
                    }
                }
                if (/*curString.Contains("Setting") && !kv.KeyID.StartsWith("01")*/ kv.KeyID.StartsWith("02") && kv.isSet && kv.Length == 2)
                {
                    if (setLabels.Count == 0 || setLabels[setLabels.Count - 1].Text != kv.AttributeName && kv.isSet)
                    {
                        Label lab = new Label();
                        lab.Text = kv.AttributeName;
                        setLabels.Add(lab);
                        ComboBox c = new ComboBox();
                        c.Tag = kv;
                        settings.Add(c);
                    }
                }
            }
        }

        public void SettingsData(string retvalue)
        {
            usesCookTimeOp = false;
            string[] setpairs = retvalue.Split(';');
            foreach (string s in setpairs)
            {
                if (s.StartsWith("0x"))
                {
                    string[] keynums = s.Split(':');
                    string kv = keynums[0].Substring(2);
                    usesCookTimeOp = kv == timeOpKey ? true : usesCookTimeOp;
                    string[] ens = keynums[1].Split(',');
                    KeyValue kvp = null;
                    foreach (KeyValue k in keyValues)
                    {
                        if (k.KeyID == kv)
                        {
                            kvp = k;
                        }
                    }
                    if (kvp != null && !ens.Contains("?à "))
                    {
                        for (int i = 0; i < kvp.EnumName.Enums.Count; i++)
                        {
                            if (ens.Contains(i.ToString()))
                            {
                                if (kvp.EnumName.Enums[i] == null)
                                {
                                    kvp.EnumName.insertEnum(i, i.ToString());
                                }
                                else
                                {
                                    SetCycle(kvp, i.ToString("X2"), kvp.EnumName.Enums[i].ToString());
                                }
                            }
                            else
                            {
                                kvp.EnumName.enableEnum(i, false);
                            }
                        }
                    }
                }
            }
        }

        public void SetMods(int tab)
        {
            CBO_Laundry_Mod.Items.Clear();
            CBO_Laundry_Mod.SelectedIndex = -1;
            CBO_Laundry_Mod.SelectedItem = "";
            CBO_Laundry_Mod.Text = "";
            selectedMod = null;
            CBO_Laundry_Val.SelectedIndex = -1;
            CBO_Laundry_Val.Text = "";
            foreach (KeyValue k in keyValues)
            {
                switch (tab)
                {
                    case 0: //Washer
                        if (k.KeyID.StartsWith("05") && long.Parse(k.KeyID, NumberStyles.AllowHexSpecifier) > 0x05050000 && k.IsUsed && /*(k.LengthString == "boolean" || k.EnumName != null) &&*/ k.KeyID != "05050006")
                        {
                            CBO_Laundry_Mod.Items.Add(k.AttributeName);
                        }
                        break;
                    case 1: //Dryer
                        if (k.KeyID.StartsWith("07") && long.Parse(k.KeyID, NumberStyles.AllowHexSpecifier) > 0x07040001 && k.IsUsed /*&& (k.LengthString == "boolean" || k.EnumName != null)*/)
                        {
                            CBO_Laundry_Mod.Items.Add(k.AttributeName);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void SettingsLabels()
        {

            for (int i = 0; i < settings.Count; i++)
            {
                setLabels[i].Width = PAN_Settings.Width / 2 - 10;
                setLabels[i].Location = new Point(PAN_Settings.Width / 2 - setLabels[i].Width, (3 + setLabels[i].Height) * i);
                PAN_Settings.Controls.Add(setLabels[i]);

                KeyValue kv = (KeyValue)settings[i].Tag;
                if (kv.EnumName == null)
                {
                    if (kv.AttributeName.Contains("Percent"))
                    {
                        for (int j = 0; j <= 100; j += 10)
                        {
                            settings[i].Items.Add(j.ToString("X2") + "=" + j + "%");
                        }
                    }
                    else
                    {
                        settings[i].Items.Add("00=Off");
                        settings[i].Items.Add("01=On");
                    }
                }
                else
                {
                    for (int j = 0; j < kv.EnumName.Enums.Count; j++)
                    {
                        if (kv.EnumName.IsUsed[j])
                        {
                            settings[i].Items.Add(j.ToString("X2") + "=" + kv.EnumName.Enums[j]);
                        }
                    }
                }
                settings[i].Width = PAN_Settings.Width / 2 - 30;
                settings[i].Location = new Point(PAN_Settings.Width / 2 + 3, (3 + setLabels[i].Height) * i);
                PAN_Settings.Controls.Add(settings[i]);
                settings[i].SelectedIndexChanged += new EventHandler(Settings_SelectedIndexChanged);
            }
        }

        public void Settings_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            KeyValue k = (KeyValue)c.Tag;
            if (!fromAppliance)
            {                
                string mes = k.KeyID + c.SelectedItem.ToString().Substring(0, 2);
                SendMessage(MakeBytes(mes,true),true);
            }
            else
            {
                fromAppliance = false;
            }

            if (k.AttributeName == "TempUnits")
            {
                SymbIOT.tempUnits = c.SelectedIndex;
                SwitchCycles(selectedTab);
            }
        }

        public int Cx10toF(int cx10)
        {
            double a = cx10 / 10;
            double b = a * 9 / 5 + 32;
            return (int)b;
        }

        public string FtoCx10(string f)
        {
            double b = 0;
            if (SymbIOT.tempUnits == 1)
            {
                double fs = double.Parse(f);
                double a = fs - 32;
                b = (a * 5 / 9) * 10;
            }
            else
            {
                b = int.Parse(f) * 10;
            }
            return ((int)b).ToString("X4");
        }

        public void SwitchCycles(int c)
        {
            selectedTab = c;
            if (dm_name == "Laundry")
            {
                switch (c)
                {
                    case 0: //Washer
                        break;
                    case 1: //Dryer
                        break;
                    default:
                        break;
                }
                TAB_Platform_Laundry.SelectedTab.Controls.Add(PAN_Laundry_Tab_Controls);
                CBO_Laundry_Cycles.Items.Clear();
                foreach (Cycle cy in cycles[c])
                {
                    CBO_Laundry_Cycles.Items.Add(cy.Name);
                }
                CBO_Laundry_Cycles.Text = "";
                CBO_Laundry_Cycles.SelectedIndex = -1;
                SetMods(c);
            }
            else
            {
                switch (c)
                {
                    case 0: //Upper Oven
                        tempKey = "03100001";
                        timeKey = "030D0001";
                        timeOpKey = "030D0002";
                        delayKey = "030D0003";
                        opKey = "030F0002";
                        cpltKey = "030F0001";
                        sabKey = "030F0003";
                        stepKey = "03170002";
                        probeAmtKey = "03100004";
                        doneKey = "";
                        v1msgKey = "030E0003";
                        v2msgKey = "030E0004";
                        LBL_SetTemp.Text = "Temp";
                        LBL_SetTempF.Text = SymbIOT.tempUnits == 1 ? "F" : "C";
                        LBL_SetDelay.Text = "Delay";
                        LBL_SetDelayMin.Text = "min";
                        LBL_SetProbe.Text = "Probe";
                        LBL_SetProbeF.Text = SymbIOT.tempUnits == 1 ? "F" : "C";
                        CBO_Doneness.Visible = false;
                        CBO_Amt.Visible = false;
                        break;
                    case 1: //Lower Oven
                        tempKey = "03150001";
                        timeKey = "03120001";
                        timeOpKey = "03120002";
                        delayKey = "03120003";
                        opKey = "03140002";
                        cpltKey = "03140001";
                        sabKey = "03140003";
                        stepKey = "03180002";
                        probeAmtKey = "03150004";
                        doneKey = "";
                        v1msgKey = "03130003";
                        v2msgKey = "03130004";
                        LBL_SetTemp.Text = "Temp";
                        LBL_SetTempF.Text = SymbIOT.tempUnits == 1 ? "F" : "C";
                        LBL_SetDelay.Text = "Delay";
                        LBL_SetDelayMin.Text = "min";
                        LBL_SetProbe.Text = "Probe";
                        LBL_SetProbeF.Text = SymbIOT.tempUnits == 1 ? "F" : "C";
                        CBO_Doneness.Visible = false;
                        CBO_Amt.Visible = false;
                        break;
                    case 2: //Microwave
                        timeKey = "04060001";
                        timeOpKey = "04060002";
                        opKey = "04080002";
                        cpltKey = "04080001";
                        tempKey = "04090001";
                        probeAmtKey = "04090004";
                        doneKey = "04090002";
                        powKey = "04090003";
                        stepKey = "040B0002";
                        v1msgKey = "04070003";
                        v2msgKey = "04070004";
                        LBL_SetTemp.Text = "Power";
                        LBL_SetTempF.Text = "%";
                        LBL_SetDelay.Text = "Done";
                        LBL_SetDelayMin.Text = "";
                        LBL_SetProbe.Text = "Amt";
                        LBL_SetProbeF.Text = "";
                        CBO_Doneness.Visible = true;
                        //CBO_Amt.Visible = true;
                        break;
                    default:
                        tempKey = "03100001";
                        timeKey = "030D0001";
                        timeOpKey = "030D0002";
                        delayKey = "030D0003";
                        opKey = "030F0002";
                        break;
                }

                TAB_Platform_Cooking.SelectedTab.Controls.Add(PAN_TabControls);

                CBO_Cycles.Items.Clear();
                foreach (Cycle cy in cycles[c])
                {
                    CBO_Cycles.Items.Add(cy.Name);
                }
                CBO_Cycles.Text = "";
                CBO_Cycles.SelectedIndex = -1;

                CBO_End.Items.Clear();
                CBO_End.SelectedItem = null;
                CBO_End.Text = "";
                CBO_Doneness.Items.Clear();
                CBO_Doneness.SelectedItem = null;
                CBO_Doneness.Text = "";
                foreach (KeyValue k in keyValues)
                {
                    if (k.KeyID == cpltKey)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            try
                            {
                                CBO_End.Items.Add(k.EnumName.Enums[i]);
                            }
                            catch
                            {
                            }
                        }
                        //break;
                    }
                    if (k.KeyID == doneKey)
                    {
                        for (int j = 0; j <= k.EnumName.Enums.Count; j++)
                        {
                            try
                            {
                                if (k.EnumName.Enums[j] != null)
                                {
                                    CBO_Doneness.Items.Add(k.EnumName.Enums[j]);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        private void CBO_Cycles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = CBO_Cycles.SelectedIndex;
            BTN_Send.Enabled = true;
            BTN_Display.Enabled = true;
            BTN_Mod.Enabled = true;
            Cycle c = cycles[TAB_Platform_Cooking.SelectedIndex][idx];

            //SwitchCycles(c);
            

            if (TAB_Platform_Cooking.SelectedIndex != 2 || CBO_Cycles.SelectedItem.ToString().StartsWith("Convect"))
            {
                LBL_SetTemp.Text = "Temp";
                LBL_SetTempF.Text = SymbIOT.tempUnits == 1 ? "F" : "C";
                TB_Temp.Text = c.DefTemp > 0 ? Cx10toF(c.DefTemp).ToString() : TB_Temp.Text;
            }
            else
            {
                TB_Temp.Text = "";
                LBL_SetTemp.Text = "Power";
                LBL_SetTempF.Text = "%";
            }

            if (!(c.MinTime == 0 && c.MaxTime == 0))
            {
                TB_Time.Enabled = true;
                TB_Delay.Enabled = true;
                CBO_End.Enabled = true;
            }
            /*else
            {
                TB_Time.Text = "";
                TB_Time.Enabled = false;
                TB_Delay.Enabled = false;
                CBO_End.Enabled = false;
                TB_Delay.Text = "";
            }*/

            if (c.DefAmt != 0)
            {
                CBO_Amt.Items.Clear();
                if (c.ValAmt == "")
                {
                    for (int i = c.MinAmt; i <= c.MaxAmt; i += c.StepAmt)
                    {
                        CBO_Amt.Items.Add(i.ToString());
                    }
                }
                else
                {
                    string[] vals = c.ValAmt.Split(',');
                    for (int i = 0; i < vals.Length; i++)
                    {
                        CBO_Amt.Items.Add(vals[i]);
                    }
                }
                CBO_Amt.SelectedItem = c.DefAmt.ToString();
                CBO_Amt.Enabled = true;
            }
            else
            {
                CBO_Amt.Items.Clear();
                CBO_Amt.Enabled = false;
            }
        }

        private bool SendMessage(byte[] bytes, bool set)
        {
            bool sent = false;
            if (bytes != null)
            {
                if (RB_MQTT.Checked)
                {
                    if (TB_IP.Text != "")
                    {
                        string[] ipad = TB_IP.Text.Split('.');
                        byte[] ipbytes = new byte[4];
                        for (int j = 0; j < 4; j++)
                        {
                            ipbytes[j] = byte.Parse(ipad[j]);
                        }
                        System.Net.IPAddress ip = new System.Net.IPAddress(ipbytes);
                        try
                        {
                            string topic = set ? "iot-2/cmd/cc_SetKvp/fmt/binary" : "iot-2/cmd/cc_GetKvp/fmt/binary";
                            WifiLocal.SendMqttMessage(ip, topic, bytes);
                            TB_Sent.Text = string.Join(".", Array.ConvertAll(bytes, b => b.ToString("X2")));
                            sent = true;
                        }
                        catch
                        {
                            MessageBox.Show("Cannot reach specified IP address");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid destination IP address.");
                    }
                }
                else
                {
                    if (TB_Node.Text != "")
                    {
                        SendWideMessage(byte.Parse(TB_Node.Text), 4, bytes);
                        TB_Sent.Text = string.Join(".", Array.ConvertAll(bytes, b => b.ToString("X2")));
                        sent = true;
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid destination node.");
                        TB_Cap.Text = TB_Cap.Text.Contains("Loading") ? "" : TB_Cap.Text;
                        TB_Node.Focus();
                    }
                }
                TB_Result.Text = "";
                TB_Result.BackColor = Color.White;
            }
            return sent;
        }

        private byte[] MakeBytes(string mes, bool isSet)
        {
            /*if (TB_Node.Text != "" || TB_IP.Text != "")
            {
                InterpretMQTT(mes, false, false);
            }*/
            string header = "";
            if(RB_MQTT.Checked)
            {
                header = isSet ? "FF3333" : "FF";
            }
            else
            {
                header = isSet ? "9005803333" : "900680";
            }
            mes = header + mes;
            if (RB_MQTT.Checked)
            {
                string meslen = (mes.Length / 2).ToString("X4");
                mes = meslen + mes;
            }
            byte[] bytes = new byte[mes.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(mes.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
            }           

            return bytes;
        }

        private byte[] SetStartDisplay(bool start, bool modify)
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
                mes = /*CBO_Amt.Enabled && CBO_Amt.SelectedItem != null*/ TB_Probe.Text != "" && TAB_Platform_Cooking.SelectedIndex == 2 ? mes + probeAmtKey + int.Parse(TB_Probe.Text.ToString()).ToString("X4") : mes;

                mes = CBO_End.SelectedItem != null ? mes + cpltKey + endfunc.ToString("X2") : mes;

                mes = TB_Delay.Enabled && TB_Delay.Text != "" && TAB_Platform_Cooking.SelectedIndex != 2 ? mes + delayKey + (int.Parse(TB_Delay.Text) * 60).ToString("X8") : mes;
                mes = CBO_Doneness.Visible && CBO_Doneness.SelectedItem != null ? mes + doneKey + CBO_Doneness.SelectedIndex.ToString("X2") : mes;

                mes += opKey + startdisp;
                mes = CBO_Cycles.SelectedItem.ToString().Contains("Sabbath") ? mes += sabKey + "01" : mes;

                byte[] bytes = MakeBytes(mes,true);

                return bytes;
            }
            catch
            {
                MessageBox.Show("Input contains invalid arguments");
                return null;
            }


        }

        private byte[] SetCancel()
        {
            //string header = RB_MQTT.Checked ? "FF3333" : "9005803333";
            string mes = opKey + "01";

            byte[] bytes = MakeBytes(mes,true);

            return bytes;
        }

        private void BTN_Send_Click(object sender, EventArgs e)
        {
            if (CBO_Cycles.SelectedIndex >= 0)
            {
                byte[] bytes = SetStartDisplay(true, false);
                SendMessage(bytes,true);
            }
            else
            {
                MessageBox.Show("Please select valid cycle.");
            }
        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            byte[] bytes = SetCancel();
            SendMessage(bytes,true);
        }

        private void BTN_Display_Click(object sender, EventArgs e)
        {
            if (CBO_Cycles.SelectedIndex >= 0)
            {
                byte[] bytes = SetStartDisplay(false, false);
                SendMessage(bytes,true);
            }
            else
            {
                MessageBox.Show("Please select valid cycle.");
            }
        }

        private void BTN_Mod_Click(object sender, EventArgs e)
        {
            if (CBO_Cycles.SelectedIndex >= 0)
            {
                byte[] bytes = SetStartDisplay(true, true);
                SendMessage(bytes,true);
            }
            else
            {
                MessageBox.Show("Please select valid cycle.");
            }
        }

        private string BytesToLua(byte[] bytes)
        {
            string output = "";
            foreach (byte b in bytes)
            {
                output += ",'" + b.ToString("X2") + "'";
            }
            return output.Substring(1);
        }

        private string SetWINOutput(byte[] bytes, bool pass, int i, bool cancel)
        {
            if (bytes != null)
            {
                string pf = pass ? "00" : "02";
                string sendandwait = "a" + i + " = wide:SendAndWaitMessage(" + "3000" + "," + "_dest" + ",4,{" + BytesToLua(bytes) + "},{'90','25','33','33','" + pf + "'},4,_dest,-1,{'FF','FF','FF','FF','FF'})";
                sendandwait += "\nif a" + i + " then";
                sendandwait += "\n\tprint(\"PASS\")";
                sendandwait += "\nelse";
                sendandwait += "\n\tprint(\"FAILED\")";
                sendandwait += "\nend\n";

                if (cancel)
                {
                    sendandwait += "\nwait(_time_before_cancel_ms)\n";
                    sendandwait += "\nprint(\"==> CANCEL\")";
                    sendandwait += "\nwide:SendMessage(_dest,4,{" + BytesToLua(MakeBytes(opKey + "01",true)) + "})\n";
                    sendandwait += "\nwait(_time_between_cmd_ms)\n";
                }

                return sendandwait;
            }
            else
            {
                return "";
            }
        }

        private void BTN_LUA_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = (cycles[TAB_Platform_Cooking.SelectedIndex].Count + 1).ToString() + " scripts will be generated.\nSelect destination folder for generated scripts";
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                string fnhead = RB_MQTT.Checked ? "MQTT" : "WIN";
                bool pass = true;
                string dest = RB_MQTT.Checked ? "_dest = " + TB_IP.Text : "_dest = " + TB_Node.Text;
                StreamWriter swall = new StreamWriter(fbd.SelectedPath + "\\" + fnhead + "_AllCycles.lua");
                swall.WriteLine("-- " + fnhead + " KVP test script for All Cycles --\n");
                swall.WriteLine(dest);
                swall.WriteLine("_time_before_cancel_ms = 15000\n_time_between_cmd_ms = 2000");
                swall.WriteLine("\n\n-----------------\n\n");
                for (int i = 0; i < CBO_Cycles.Items.Count; i++)
                {
                    CBO_Cycles.SelectedIndex = i;
                    StreamWriter sw = new StreamWriter(fbd.SelectedPath + "\\" + fnhead + "_" + CBO_Cycles.SelectedItem.ToString() + ".lua");
                    sw.WriteLine("-- " + fnhead + " KVP test script for " + CBO_Cycles.SelectedItem.ToString() + " --\n");
                    sw.WriteLine(dest);
                    sw.WriteLine("_time_before_cancel_ms = 15000\n_time_between_cmd_ms = 2000");
                    sw.WriteLine("\n\n-----------------\n\n");

                    byte[] bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTemp) + 5).ToString();
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    //pass = cycles[i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MinTemp) - 5).ToString();
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    //pass = cycles[i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    swall.WriteLine("print(\"Starting timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = (((int)((cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime) / 60)) + 1).ToString();
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    swall.WriteLine("print(\"Starting timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    //pass = cycles[i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    TB_Delay.Text = "1";
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Delaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    swall.WriteLine("print(\"Delaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    TB_Delay.Text = (((int)((delaymax) / 60)) + 1).ToString(); ;
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Delaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    swall.WriteLine("print(\"Delaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    //pass = cycles[i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Time.Text = "";
                    TB_Delay.Text = "";
                    CBO_End.SelectedItem = null;

                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTemp) + 5).ToString();
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    //pass = cycles[i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MinTemp) - 5).ToString();
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Displaying untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    //pass = cycles[i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    swall.WriteLine("print(\"Displaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = (((int)((cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime) / 60)) + 1).ToString();
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    swall.WriteLine("print(\"Displaying timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Time.Text + " minutes\")");
                    //pass = cycles[i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    TB_Delay.Text = "1";
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying delayed timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    swall.WriteLine("print(\"Displaying delayed timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, true));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, true));

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].DefTemp)).ToString();
                    TB_Time.Text = "1";
                    TB_Delay.Text = (((int)((delaymax) / 60)) + 1).ToString(); ;
                    CBO_End.SelectedIndex = CBO_End.Items.Count - 1;
                    bytes = SetStartDisplay(false, false);
                    sw.WriteLine("print(\"Displaying delayed timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    swall.WriteLine("print(\"Displaying delayed timed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F for " + TB_Delay.Text + " minutes\")");
                    //pass = cycles[i].MaxTime == 0 ? false : true;
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Time.Text = "";
                    TB_Delay.Text = "";
                    CBO_End.SelectedItem = null;

                    bytes = SetStartDisplay(true, false);
                    sw.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Starting untimed " + cycles[TAB_Platform_Cooking.SelectedIndex][i].Name + " for " + TB_Temp.Text + "F\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MinTime == 0 ? true : false;
                    sw.WriteLine(SetWINOutput(bytes, pass, i, false));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, false));
                    sw.WriteLine("\nwait(_time_before_cancel_ms)\n");
                    swall.WriteLine("\nwait(_time_before_cancel_ms)\n");

                    TB_Temp.Text = Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTemp).ToString();
                    sw.WriteLine("print(\"Modifying temperature to " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Modifying temperature to " + TB_Temp.Text + "F\")");
                    bytes = SetStartDisplay(true, true);
                    sw.WriteLine(SetWINOutput(bytes, pass, i, false));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, false));
                    sw.WriteLine("\nwait(_time_before_cancel_ms)\n");
                    swall.WriteLine("\nwait(_time_before_cancel_ms)\n");

                    TB_Temp.Text = (Cx10toF(cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTemp) + 5).ToString();
                    sw.WriteLine("print(\"Modifying temperature to " + TB_Temp.Text + "F\")");
                    swall.WriteLine("print(\"Modifying temperature to " + TB_Temp.Text + "F\")");
                    bytes = SetStartDisplay(true, true);
                    sw.WriteLine(SetWINOutput(bytes, false, i, false));
                    swall.WriteLine(SetWINOutput(bytes, false, i, false));
                    sw.WriteLine("\nwait(_time_before_cancel_ms)\n");
                    swall.WriteLine("\nwait(_time_before_cancel_ms)\n");

                    TB_Temp.Text = "";
                    TB_Time.Text = "1";
                    sw.WriteLine("print(\"Adding 1-minute cook timer\")");
                    swall.WriteLine("print(\"Adding 1-minute cook timer\")");
                    pass = cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime == 0 ? false : true;
                    bytes = SetStartDisplay(true, true);
                    sw.WriteLine(SetWINOutput(bytes, pass, i, false));
                    swall.WriteLine(SetWINOutput(bytes, pass, i, false));
                    sw.WriteLine("\nwait(_time_before_cancel_ms)\n");
                    swall.WriteLine("\nwait(_time_before_cancel_ms)\n");

                    TB_Time.Text = (((int)((cycles[TAB_Platform_Cooking.SelectedIndex][i].MaxTime) / 60)) + 1).ToString();
                    sw.WriteLine("print(\"Modifying cook timer to " + TB_Time.Text + " minutes\")");
                    swall.WriteLine("print(\"Modifying cook timer to " + TB_Time.Text + " minutes\")");
                    bytes = SetStartDisplay(true, true);
                    sw.WriteLine(SetWINOutput(bytes, false, i, true));
                    swall.WriteLine(SetWINOutput(bytes, false, i, true));

                    TB_Time.Text = "";
                    TB_Delay.Text = "";
                    CBO_End.SelectedItem = null;

                    sw.Close();
                }
                swall.Close();
            }
        }

        private void BTN_App_Click(object sender, EventArgs e)
        {
            CBO_Cycles.Items.Clear();
            foreach (List<Cycle> l in cycles)
            {
                l.Clear();
            }
            settings.Clear();
            setLabels.Clear();
            PAN_Settings.Controls.Clear();
            byte[] bytes = MakeBytes("0109000100",true);
            TB_Cap.Text = "Loading from Appliance...";
            SendMessage(bytes,true);

        }

        private void BTN_KTSet_Click(object sender, EventArgs e)
        {
            Button s = (Button)sender;
            string msg = "";
            if (s.Tag.ToString() == "01")
            {
                msg = "0805000201";
            }
            else
            {
                int hours = TB_KTH.Text == "" ? 0 : int.Parse(TB_KTH.Text.ToString());
                int mins = TB_KTM.Text == "" ? 0 : int.Parse(TB_KTM.Text.ToString());
                int secs = TB_KTS.Text == "" ? 0 : int.Parse(TB_KTS.Text.ToString());
                msg = "080500020208050001" + (hours * 1200 + mins * 60 + secs).ToString("X8");
            }
            SendMessage(MakeBytes(msg,true),true);
        }

        private void TAB_Platforms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dm_name == "Laundry")
            {                
                SwitchCycles(TAB_Platform_Laundry.SelectedIndex);
            }
            else
            {
                SwitchCycles(TAB_Platform_Cooking.SelectedIndex);
            }            
            RefreshDisplay();
            //CBO_Cycles.SelectedItem = null;
        }

        private void BTN_Time_Click(object sender, EventArgs e)
        {
            string mes = "01070001";
            char[] values = DT_Date.Text.ToCharArray();
            foreach (char c in values)
            {
                mes += Convert.ToInt32(c).ToString("X2");
            }
            mes += "00";
            SendMessage(MakeBytes(mes,true),true);
        }

        private void SetResult(byte res)
        {
            if (res == 0)
            {
                TB_Result.Text = "ACCEPTED-00";
                TB_Result.BackColor = Color.LightGreen;
            }
            else
            {
                TB_Result.Text = "REJECTED-" + res.ToString("X2");
                TB_Result.BackColor = Color.Red;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*KeyValue cplt = new KeyValue();
            foreach (KeyValue kv in keyValues)
            {
                if (kv.KeyID == cpltKey)
                {
                    cplt = kv;
                    break;
                }
            }
            RelationalBuilder rb = new RelationalBuilder(this, cycles[TAB_Platforms.SelectedIndex], tempKey, timeKey, probeAmtKey, doneKey, powKey, cplt);
            rb.Show();*/
            RelationalBuilder2 rb = new RelationalBuilder2(this, keyValues);
            rb.Show();
        }

        public bool SendExternal(string mes, bool isRelational, bool isSet)
        {
            bool sent = false;
            if (mes.Length > 100)
            {
                if (RB_MQTT.Checked)
                {
                    sent = SendMessage(MakeBytes(mes,isSet), isSet);
                }
                else
                {
                    MessageBox.Show("Message length is too long for Reveal message.  Send as LUA or MQTT instead.");
                }
            }
            else
            {
                sent = SendMessage(MakeBytes(mes,isSet), isSet);
            }

            return sent;
        }

        private void BTN_Step_Click(object sender, EventArgs e)
        {
            if (TB_Step.Text != "")
            {
                SendMessage(MakeBytes(stepKey + int.Parse(TB_Step.Text).ToString("X2"),true),true);
            }
        }

        private void BTN_XCat_Click(object sender, EventArgs e)
        {
            try
            {
                xc.Show();
            }
            catch
            {
                xc = new XCategory(keyValues, this);
                xc.Show();
            }
        }

        public bool IsReveal()
        {
            return RB_Reveal.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/a/whirlpool.com/document/d/1-V4JON0WFjwJRzOBuWWTnSpf2mVe4n6-XqhB9QS3cVk/edit?usp=sharing");
        }

        private void BTN_ClearLog_Click(object sender, EventArgs e)
        {
            LB_Log.Items.Clear();
            rawlogs.Clear();
            rawpacket = "";
        }

        private void BTN_SaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "SymbIOTLog" + DateTime.Now.ToString("yyMMddhhmmss") + ".txt";
            sfd.Filter = "Text Files (*.txt)|*.txt";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK && sfd.FileName != "")
            {
                Stream str = sfd.OpenFile();
                StreamWriter sw = new StreamWriter(str);
                sw.WriteLine("Log generated by SymbIOT on " + DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
                sw.WriteLine("Using Data Model Definition '" + TB_Def.Text.Substring(TB_Def.Text.LastIndexOf("\\") + 1) + "'");
                sw.WriteLine("---------------------------------------");
                foreach (string s in LB_Log.Items)
                {
                    sw.WriteLine(s);
                }
                str.Flush();
                sw.Close();
                MessageBox.Show("Log file saved.");
            }
        }

        public string GetNodeID()
        {
            return TB_Node.Text != "" ? TB_Node.Text : "FF";
        }

        private void MI_CopyLog_Click(object sender, EventArgs e)
        {
            string clip = "";
            if (LB_Log.SelectedIndex >= 0)
            {                
                foreach (string s in LB_Log.SelectedItems)
                {
                    clip += s.Trim() + "\n";
                }
                Clipboard.SetText(clip);
            }
            MessageBox.Show("\"" + clip + "\" " + "copied to clipboard");
        }

        private void MI_CopyRaw_Click(object sender, EventArgs e)
        {
            if (LB_Log.SelectedIndex >= 0)
            {
                Clipboard.SetText(rawlogs[LB_Log.SelectedIndex].Replace(',','.'));
            }
            MessageBox.Show("\"" + rawlogs[LB_Log.SelectedIndex].Replace(',', '.') + "\" " + "copied to clipboard");
        }

        private void LB_Log_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LB_Log.SelectedItems.Count > 1)
            {
                MI_CopyRaw.Enabled = false;
                MI_CopyLog.Text = "Copy Log Lines";
            }
            else
            {
                MI_CopyRaw.Enabled = true;
                MI_CopyLog.Text = "Copy Log Line";
            }
        }

        private void SymbIOT_Resize(object sender, EventArgs e)
        {
            LB_Log.Width = this.Width - 97;
            LB_Log.Height = this.Height - 476;
        }

        private void BTN_LoadLog_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                parsedlog = new DataTable();
                parsedlog.Columns.Add("Log");
                parsedlog.Columns.Add("Cmd");
                parsedlog.Columns.Add("WIN");
                parsedlog.Columns.Add("Parsed");
                ParseLog(ofd.FileName);
                //savedWIDEMessage = "";
                savedExtractedMessage = "";
            }
        }

        /// <summary>
        /// Attempt to parse out the selected log file into the Log window
        /// </summary>
        /// <param name="filename">Filename to parse</param>
        private void ParseLog(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            bool typefound = false;
            //Loop through each line of the log looking for strings that identify whether it's a known format of Reveal log or
            //Trace log
            while (!typefound && !sr.EndOfStream)
            {
                string teststring = sr.ReadLine().ToUpper();
                //teststring = teststring.Replace(',', '.');
                if ((teststring.IndexOf("API[0X30]") >= 0 || teststring.IndexOf("API[0XE6]") >= 0 || teststring.IndexOf("API[0X03]") >= 0 || teststring.IndexOf("API[0XF6]") >= 0))
                {
                    logtype = UITRACE;
                    //LB_Log.Items.Clear();
                    typefound = true;
                    ParseUITracer(teststring, sr);
                }
                /*else if(Regex.IsMatch(teststring,@"1002\w\w\w\w\w\w(8f|93)26"))
                {
                    logtype = WIN;
                    LB_Log.Items.Clear();
                    typefound = true;
                    ParseUITracerSniffer(teststring, sr);
                }*/
                else if (teststring.IndexOf("ED.") >= 0 || teststring.IndexOf("ED,") > 0)
                {
                    logtype = WIDE;
                    //LB_Log.Items.Clear();
                    typefound = true;
                    ParseWIDE(teststring, sr);
                }
                else if (teststring.IndexOf(" 10.02") >= 0 || teststring.IndexOf("SWM:") >= 0 || ((teststring.IndexOf("FBK") >= 0 || teststring.IndexOf("CMD") >= 0) && teststring.IndexOf("/") < 0))
                {
                    logtype = WIN;
                    //LB_Log.Items.Clear();
                    typefound = true;
                    ParseWIDE(teststring, sr);
                }
                else if (teststring.IndexOf("IOT-2/") >= 0)
                {
                    logtype = MQTT;
                    //LB_Log.Items.Clear();
                    typefound = true;
                    ParseMQTT(teststring, sr);
                }
            }

            sr.Close();
            //Display if no lines in the log could be parsed into a recognized format
            if (parsedlog.Rows.Count == 0)
            {
                MessageBox.Show("Not a recognized log format or no KVP data found in log.");
            }
            else
            {
                string typemsg = "";
                switch (logtype)
                {
                    case UITRACE:
                        typemsg = "UITracer ";
                        break;
                    case WIDE:
                        typemsg = "WIDE ";
                        break;
                    case WIN:
                        typemsg = "WIN ";
                        break;
                    case MQTT:
                        typemsg = "MQTT ";
                        break;
                    default:
                        break;
                }

                typemsg += "log recognized.  " + parsedlog.Rows.Count + " KVP log lines extracted.  Press OK to translate and save to file.";
                MessageBox.Show(typemsg);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "SymbIOTLog" + DateTime.Now.ToString("yyMMddhhmmss") + ".txt";
                sfd.Filter = "Text Files (*.txt)|*.txt";
                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK && sfd.FileName != "")
                {
                    isExporting = true;
                    Stream str = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(str);
                    sw.WriteLine("Log generated by SymbIOT on " + DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
                    sw.WriteLine("Using Data Model Definition '" + TB_Def.Text.Substring(TB_Def.Text.LastIndexOf("\\") + 1) + "'");
                    sw.WriteLine("---------------------------------------");
                    foreach (DataRow dr in parsedlog.Rows)
                    {
                        bool haslength = dr["WIN"].ToString() == "0" && dr["Cmd"].ToString() == "0";
                        bool isfb = dr["Cmd"].ToString() == "0";
                        InterpretMQTT(dr[0].ToString(), haslength, isfb);
                    }
                    sw.Write(exportLog);
                    str.Flush();
                    sw.Close();
                    MessageBox.Show("Log file saved.");
                    exportLog = "";
                    isExporting = false;
                }
            }
        }

        //Parser called if log is identified as a MQTT log
        private void ParseMQTT(string firststring, StreamReader stream)
        {
            //int startpay = 0;
            string teststring = firststring;
            bool lastline = false;

            while (!stream.EndOfStream || lastline)
            {
                try
                {
                    if (teststring.IndexOf("KVP/FMT/BINARY") >= 0 && !teststring.Contains("GET") && !teststring.Contains("/SM_"))
                    {
                        //startpay = teststring.IndexOf('[', teststring.IndexOf("tp[") + 3) + 1;
                        DataRow dat = parsedlog.NewRow();
                        try
                        {
                            dat[0] = teststring.Substring(teststring.IndexOf("TP["));
                            while (!dat[0].ToString().EndsWith("]"))//check if payload wraps to the next line
                            {
                                string nextline = stream.ReadLine().ToUpper();
                                dat[0] += nextline.Substring(nextline.LastIndexOf("-") + 2);
                            }
                            dat[1] = teststring.Contains("CC_SET") ? 1 : 0;
                            dat[2] = 0;
                            dat[0] = dat[0].ToString().Substring(dat[0].ToString().LastIndexOf('[') + 1);
                            dat[0] = dat[0].ToString().Substring(0, dat[0].ToString().Length - 1);
                            dat[0] = dat[1].ToString() == "1" ? dat[0].ToString().Substring(10) : dat[0].ToString();
                        }
                        catch //WBI MQTT log
                        {
                            try
                            {
                                string[] tstrings = teststring.Split(',');
                                //string wifiCommanderfmt = "TP[" + tstrings[3] + "],DATA[" + tstrings[4].Replace(".", "") + "]";
                                dat[0] = tstrings[4].Replace(".", "");
                                dat[1] = teststring.Contains("CC_SET") ? 1 : 0;
                                dat[2] = 0;
                                dat[0] = dat[1].ToString() == "1" ? dat[0].ToString().Substring(10) : dat[0].ToString();
                            }
                            catch { }
                        }
                        parsedlog.Rows.Add(dat);
                        //LB_Log.Items.Add(dat[0].ToString());
                    }
                }
                catch
                {
                }
                if (!lastline)
                {
                    teststring = stream.ReadLine().ToUpper();
                }
                if (stream.EndOfStream && !lastline)
                {
                    lastline = true;
                }
                else
                {
                    lastline = false;
                }
            }
            stream.Close();
        }

        //Parser called if log is identified as a WIDE/Reveal log
        private void ParseWIDE(string firststring, StreamReader stream)
        {
            int startpay = 0;
            string teststring = firststring;
            string curmes = "";
            string savedmes = "";
            bool lastline = false;
            bool rlog = false;

            while (!stream.EndOfStream || lastline)
            {
                if (teststring.Contains("FBK") || teststring.Contains("CMD"))
                {
                    rlog = true;
                    string[] rlogparts = teststring.Split(',');
                    try
                    {
                        teststring = "10.02." + (int.Parse(rlogparts[1], NumberStyles.AllowHexSpecifier)).ToString("X2").Substring(1) + (int.Parse(rlogparts[2], NumberStyles.AllowHexSpecifier)).ToString("X2").Substring(1) +
                            ".04." + ((int)((rlogparts[8].Length + 1) / 3) + 2).ToString("X2") + "." +
                            (int.Parse(rlogparts[6], NumberStyles.AllowHexSpecifier)).ToString("X2") + "." + (int.Parse(rlogparts[7], NumberStyles.AllowHexSpecifier) + (rlogparts[3] == "FBK" ? 32 : 0)).ToString("X2") +
                            (rlogparts[8].Length > 0 ? "." : "") + rlogparts[8];
                    }
                    catch
                    {
                    }
                    //teststring += ".00";
                }
                if (teststring.Contains("SWM:"))
                {
                    string[] swmparts = teststring.Split(':');
                    string[] swmbits = swmparts[swmparts.Length - 1].Split('.');
                    try
                    {
                        teststring = "10.02." + swmbits[0].Substring(1) + swmbits[1].Substring(1) + "." + "04." + (swmbits.Length - 3).ToString("X2");
                        for (int i = 3; i < swmbits.Length; i++)
                        {
                            teststring += "." + swmbits[i];
                        }
                        //teststring += ".00";
                    }
                    catch { }
                }
                try
                {
                    string apiopcode = teststring.Substring(15, 5);
                    //Extract only the KVP messages and discard the rest
                    if ((teststring.IndexOf("ED.") >= 0 || teststring.IndexOf("10.02") >= 0) &&
                        (teststring.IndexOf("8F.26") >= 0 || (teststring.IndexOf("8F.05") >= 0 && teststring.IndexOf("01.21.0") <= 0) ||                        
                        (apiopcode == "90.26" || (apiopcode == "90.05") && teststring.IndexOf("01.21.0") <= 0)))
                    {                        
                        startpay = logtype == WIDE ? Math.Max(teststring.IndexOf("ED.") - 3, 0) : teststring.IndexOf("10.02");
                        curmes = teststring.Substring(21).Replace(".","");
                        curmes = (apiopcode.EndsWith("05") && savedmes == "") ? curmes.Substring(6) : curmes.Substring(2);
                        savedmes += curmes;
                        if (int.Parse(teststring.Substring(21).Replace(".", "").Substring(0, 2), NumberStyles.AllowHexSpecifier) < 128)
                        {
                            curmes = "";
                        }
                        else
                        { 
                            DataRow dat = parsedlog.NewRow();
                            dat[0] = savedmes;
                            dat[1] = apiopcode.EndsWith("05") ? 1 : 0;
                            dat[2] = 1;
                            parsedlog.Rows.Add(dat);
                            savedmes = "";
                            //LB_Log.Items.Add(teststring.Substring(startpay));
                        }
                    }
                }
                catch
                {
                }
                if (!lastline)
                {
                    teststring = stream.ReadLine();
                    if (!rlog)
                    {
                        teststring = teststring.Replace(',', '.');
                    }
                }
                if (stream.EndOfStream && !lastline)
                {
                    lastline = true;
                }
                else
                {
                    lastline = false;
                }
            }
            stream.Close();
        }

        //Parser called if log is identified as a UITracer Trace log
        private void ParseUITracer(string firststring, StreamReader stream)
        {
            int startpay = 0;
            string teststring = firststring;
            bool lastline = false;

            while (!stream.EndOfStream || lastline)
            {
                try
                {
                    startpay = teststring.LastIndexOf("[");
                    //Extract only KVP messages and discard the rest
                    if ((teststring.IndexOf("API[0X30]") >= 0 || teststring.IndexOf("API[0XE6]") >= 0 || teststring.IndexOf("API[0X03]") >= 0) &&
                        (teststring.IndexOf("8F26", startpay, 15) >= 0 || teststring.IndexOf("8F05", startpay, 15) >= 0 || teststring.IndexOf("8F06", startpay, 15) >= 0 ||                        
                        teststring.IndexOf("9026", startpay, 15) >= 0 || teststring.IndexOf("9005", startpay, 15) >= 0 || teststring.IndexOf("9006", startpay, 15) >= 0) ||
                        (teststring.Contains("API[0XF6]") && (teststring.Contains("OPC[0X83]") || teststring.Contains("OPC[0X04]"))))
                    {
                        if (!(teststring.IndexOf("API[0X03]") >= 0 && (teststring.IndexOf("8F05") >= 0 || teststring.IndexOf("9005") >= 0) ||
                            ((teststring.IndexOf("[C:01]") >= 0 || teststring.IndexOf("[C:04]") >= 0) && (teststring.IndexOf("8F06") >= 0 || teststring.IndexOf("9006") >= 0))) ||
                            (teststring.Contains("API[0XF6]") && (teststring.Contains("OPC[0X83]") || teststring.Contains("OPC[0X04]"))))
                        {
                            startpay = teststring.IndexOf("[") - 3;
                            string line = teststring.Substring(startpay);
                            DataRow dat = parsedlog.NewRow();
                            startpay = teststring.IndexOf("[") - 3;
                            dat[0] = line;
                            parsedlog.Rows.Add(dat);
                            //LB_Log.Items.Add(line);
                            if (parsedlog.Rows.Count > 1)
                            {
                                //remove duplicates
                                if (parsedlog.Rows[parsedlog.Rows.Count - 1][0].ToString() == parsedlog.Rows[parsedlog.Rows.Count - 2][0].ToString())
                                {
                                    parsedlog.Rows.RemoveAt(parsedlog.Rows.Count - 1);
                                    //LB_Log.Items.RemoveAt(LB_Log.Items.Count - 1);
                                }
                            }

                        }
                    }
                }
                catch
                {
                }
                if (!lastline)
                {
                    teststring = stream.ReadLine().ToUpper();
                }
                if (stream.EndOfStream && !lastline)
                {
                    lastline = true;
                }
                else
                {
                    lastline = false;
                }
            }
            stream.Close();
        }

        //Parser called if log is identified as a UITracer log in Sniffer mode
        private void ParseUITracerSniffer(string firststring, StreamReader stream)
        {
            string regex = @"1002\w\w\w\w\w\w(8f|93|90)26";
            int startpay = 0;
            string teststring = firststring;
            bool lastline = false;

            while (!stream.EndOfStream || lastline)
            {
                try
                {
                    if (Regex.IsMatch(teststring, regex))
                    {
                        DataRow dat = parsedlog.NewRow();
                        startpay = teststring.IndexOf("1002");
                        dat[0] = teststring.Substring(startpay);
                        for (int i = 2; i < dat[0].ToString().Length - 1; i += 3)
                        {
                            dat[0] = dat[0].ToString().Insert(i, ".").ToUpper();
                        }
                        dat[0] += ".00";
                        parsedlog.Rows.Add(dat);
                        //LB_Log.Items.Add(dat[0].ToString());
                    }
                }
                catch
                {
                }
                if (!lastline)
                {
                    teststring = stream.ReadLine();
                }
                if (stream.EndOfStream && !lastline)
                {
                    lastline = true;
                }
                else
                {
                    lastline = false;
                }
            }
            stream.Close();
        }

        private void BTN_Script_Click(object sender, EventArgs e)
        {
            try
            {
                scriptwindow.Show();
            }
            catch
            {
                scriptwindow = new Scripter(this, keyValues);
                scriptwindow.Show();
            }
        }

        private void CBO_Laundry_Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            CBO_Laundry_Val.Items.Clear();
            TB_Laundry_Val.Visible = false;
            LBL_Laundry_Val.Visible = false;
            CBO_Laundry_Val.Visible = true;
            foreach (KeyValue k in keyValues)
            {
                if (CBO_Laundry_Mod.SelectedItem.ToString() == k.AttributeName && (k.KeyID.StartsWith("0505") || k.KeyID.StartsWith("0704")))
                {
                    selectedMod = k;
                    if (k.EnumName != null)
                    {
                        for (int i = 0; i < k.EnumName.Enums.Count; i++)
                        {
                            if (k.EnumName.IsUsed[i])
                            {
                                CBO_Laundry_Val.Items.Add(i.ToString("X2") + "=" + k.EnumName.Enums[i]);
                            }
                        }
                    }
                    else if (k.LengthString == "boolean")
                    {
                        CBO_Laundry_Val.Items.Add("00=Off");
                        CBO_Laundry_Val.Items.Add("01=On");
                    }
                    else
                    {
                        CBO_Laundry_Val.Visible = false;
                        TB_Laundry_Val.Visible = true;
                        LBL_Laundry_Val.Visible = k.Format == "time";
                    }
                }
            }
        }

        private void BTN_Laundry_SetCycle_Click(object sender, EventArgs e)
        {
            string output = cycles[TAB_Platform_Laundry.SelectedIndex][CBO_Laundry_Cycles.SelectedIndex].KeyID + cycles[TAB_Platform_Laundry.SelectedIndex][CBO_Laundry_Cycles.SelectedIndex].Enum.ToString("X2");
            SendExternal(output, false, true);
        }

        private void BTN_Laundry_SetMod_Click(object sender, EventArgs e)
        {
            try
            {
                string output;
                if (CBO_Laundry_Val.Visible)
                {
                    output = selectedMod.KeyID + CBO_Laundry_Val.SelectedItem.ToString().Substring(0, 2);
                }
                else
                {
                    output = LBL_Laundry_Val.Visible ? selectedMod.OutValue((int.Parse(TB_Laundry_Val.Text)*60).ToString()) : selectedMod.OutValue(TB_Laundry_Val.Text);
                }
                SendExternal(output, false, true);
            }
            catch
            {
                MessageBox.Show("Please select a valid modifier and value.");
            }
        }

        private void BTN_Laundry_Operations_Click(object sender, EventArgs e)
        {
            SendExternal("03070001"+((Button)sender).Tag.ToString(),false,true);
        }

        private void BTN_Laundry_SetDelay_Click(object sender, EventArgs e)
        {
            try
            {
                SendExternal("03050001" + (int.Parse(TB_Laundry_Delay.Text) * 60).ToString("X8"), false, true);
            }
            catch
            {
                MessageBox.Show("Please set a valid delay time.");
            }
        }

        private void BTN_Capability_Click(object sender, EventArgs e)
        {
            Capability cap = new Capability(this, cycles, keyValues);
            cap.ShowDialog();
        }

    }
    
}
