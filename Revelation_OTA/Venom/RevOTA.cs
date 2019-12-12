using System;
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

namespace VenomNamespace
{
    public partial class Revelation_OTA : WideInterface
    {
        public const byte API_NUMBER = 0;
        public static int ATTEMPTMAX = 3;
        public static int RECONWAIT = 1 * 60000; //MQTT max reconnect timer
        public static int TWAIT = 1 * 60000; //Time interval to check for update
        public static int AMT = 10;
        public int timeleft = TWAIT;

        //Global timer
        public Stopwatch g_time = new Stopwatch();
        
        // Entities used to store log and window data
        public DataTable results;
        public BindingSource sbind = new BindingSource();

        // Entities used to store the list of targeted IPs and their progress
        public List<IPData> iplist;
        public List<Thread> waits;

        public int totalran = 0;
        public string TVers = "";
        public string curfilename;
        public bool cancel_request = false;
        public byte[] paybytes = null;
        public enum OPCODES { }

        //to access wideLocal use base.WideLocal or simple WideLocal
        public Revelation_OTA(WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            AutoSave = true;


            // Build log and window table
            results = new DataTable();
            results.Columns.Add("IP Address");
            results.Columns.Add("MAC");
            results.Columns.Add("OTA Payload");
            results.Columns.Add("Model");
            results.Columns.Add("Serial");
            results.Columns.Add("Version");
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
            waits = new List<Thread> ();


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
                /*case "iot-2/evt/isp/fmt/json":
                    // Process OTA-related messages
                    string sb = System.Text.Encoding.ASCII.GetString(data.Message);
                    lock (writeobj)
                    {
                        ProcessPayload(sb, data.Source.ToString(), "MQTT Message", "NA");
                    }
                    break;*/

                /*case "iot-2/evt/cc_Kvp/fmt/binary":
                    string savedExtractedMessage = string.Concat(Array.ConvertAll(data.Message, b => b.ToString("X2")));

                    if (savedExtractedMessage.Equals("000A035D4983AC0109000502"))
                        lock (writeobj)
                        {
                            ProcessPayload("Programming", data.Source.ToString(), "MQTT Message");
                        }
                    break;*/

               // case "iot-2/evt/cc_SetKvpResult/fmt/binary":
                    //SetResult(data.Message[data.Message.Length - 1]);
                   // break;

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
        public void WriteFile(int listindex, string final)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(curfilename))
                {
                    if (final == null)
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + "," + iplist[listindex].IPAddress + "," +
                        iplist[listindex].MAC + "," +
                        iplist[listindex].Serial + "," +
                        iplist[listindex].Model + "," +
                        iplist[listindex].Version + "," +
                        iplist[listindex].Result + "," +
                        iplist[listindex].Payload);
                        return;
                    }
                    else
                    {
                        string[] results = final.Split('\t');
                        {
                            sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss") + ", " +
                                totalran + " OTA Update(s) ran with a total running time of " + results[0] +
                                "  that resulted in an average run time per OTA Update of " + results[1]
                                + ".");
                        }
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("The file could not be written.", "Error: File Write",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public bool IsEmpty()
        {
            if (String.IsNullOrEmpty(TB_Version.Text))
            {
                MessageBox.Show("No Target Version input. Please input a Target Version and try again.", "Error: Target Version Empty",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if (String.IsNullOrEmpty(TB_Payload.Text))
            {
                MessageBox.Show("No Payload input. Please input a Payload and try again.", "Error: Payload Empty",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            //Parse payload into byte array
            paybytes = Encoding.ASCII.GetBytes(TB_Payload.Text);
            //Set version
            TVers = TB_Version.Text;
            return false;
        }
        public void StartLog()
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
                            sw.WriteLine("Time,IP,MAC,Model,Serial Number,SW Version,Result,Payload");
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
        public void CancelRequest()
        {
            try
            {
                int stop = iplist.Count();
                for (int i_base = 0; i_base < stop; i_base++)
                {

                    if (iplist[i_base].Result.Contains("PASS") || iplist[i_base].Result.Contains("FAIL"))
                        continue;
                    else
                    {
                        if (results.Rows.Count < 0)
                            break;
                        iplist[i_base].Result = "FAIL - Cancelled by User.";
                        results.Rows[i_base]["OTA Result"] = iplist[i_base].Result;
                        iplist[i_base].Done = true;
                        WriteFile(i_base, null);
                    }
                }

                DGV_Data.Refresh();

                foreach (Thread thread in waits)
                {
                    //Wake sleeping wait threads that will no longer be used
                    if (thread.IsAlive)
                        thread.Interrupt();
                }
            }
            catch
            {
                MessageBox.Show("Catastrophic thread closure error. Closing all threads and parent environment (Widebox).", "Error: Threads Failed to Close",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Environment.Exit(1);
            }
        }
        public void SetForm(bool reset)
        {
            if (!reset)
            {
                BTN_Start.Text = "Stop Running";
                TB_Payload.Enabled = false;
                TB_Version.Enabled = false;
                BTN_Clr.Enabled = false;
                cancel_request = false;
                g_time.Restart();
                ResetForm(false, true);
            }
            else
            {
                cancel_request = true;
                TB_Payload.Enabled = true;
                TB_Version.Enabled = true;
                BTN_Clr.Enabled = true;
                LBL_Time.Text = "00:00:00";
                BTN_Start.Text = "Start";
            }
            
        }
        private void BTN_Start_Click(object sender, EventArgs e)
        {
            
            if (BTN_Start.Text == "Start")
            {
                if (IsEmpty())
                    return;

                StartLog();

                SetForm(false);

                ProcessCAI();
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("This will dispose of all running threads and may change log results to FAIL for in progress" +
                                                            " OTA installs. Are you sure you want to exit?",
                                                            "Verify Exiting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    SetForm(true);

                    CancelRequest();
                }
                else //Dialog was Yes
                    return;

            }
        }
        public bool RunCAITask(ConnectedApplianceInfo cai)
        {
            try
            {
                string[] parts;
                IPData ipd;
                if (cai != null)
                {
                    parts = cai.VersionNumber.Replace(" ", "").Split('|');
                    if (parts[0] == TVers)
                        return false;
                    ipd = AddResult(cai);
                    if (!RevelationConnect(cai))
                    {
                        ipd.Result = "FAIL - Revelation unable to connect.";
                        ipd.Done = true;
                        results.Rows[ipd.TabIndex]["OTA Result"] = iplist[ipd.TabIndex].Result;
                        WriteFile(ipd.TabIndex, null);
                        return false;
                    }
                    else
                    {
                        if (SendRevelation(cai))
                        {
                            ipd.Result = "Revelation OTA payload sent.";
                            ipd.Sent = true;
                            results.Rows[ipd.TabIndex]["OTA Result"] = iplist[ipd.TabIndex].Result;
                        }
                        else
                        {
                            ipd.Result = "FAIL - Revelation unable to connect.";
                            ipd.Done = true;
                            results.Rows[ipd.TabIndex]["OTA Result"] = iplist[ipd.TabIndex].Result;
                            WriteFile(ipd.TabIndex, null);
                            return false;
                        }
                    }
                    DGV_Data.Refresh();
                    return true;
                }
                else
                {
                    ipd = AddResult(cai);
                    ipd.Result = "FAIL - " + cai.IPAddress + " was unable to connect from WifiBasic.";
                    ipd.Done = true;
                    results.Rows[ipd.TabIndex]["OTA Result"] = iplist[ipd.TabIndex].Result;
                    DGV_Data.Refresh();
                    WriteFile(ipd.TabIndex, null);
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Catastrophic CAITask error.", "Error: RunCAITask",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
        public void StartTimer()
        {
            if (cancel_request)
                return;
            TimeSpan t = TimeSpan.FromMilliseconds(TWAIT);
            LBL_Time.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    t.Hours,
                    t.Minutes,
                    t.Seconds);
            timeleft = TWAIT;
            TMR_Tick.Enabled = true;
            TMR_Tick.Start();
        }
        public void Build()
        {
            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = null;
                int number = Math.Min(cio.Count, (AMT - iplist.Count));
                //Start a batch
                int j = 0;
                for (int i = 0; i < number; i++)
                {
                    if (cancel_request)
                        break;
                    cai = cio.ElementAtOrDefault(j);
                    if (cai != null)
                    {
                        IPData ipd = iplist.FirstOrDefault(x => x.MAC == cai.MacAddress);
                        if (ipd != null && ipd.Done)
                        {
                            Remove(cai, ipd);
                            i--;
                            j++;
                            continue;
                        }
                        if (ipd != null && ipd.Sent)
                        {
                            i--;
                            j++;
                            continue;
                        }
                        if (RunCAITask(cai))
                        {
                            j++;
                            totalran++;
                        }
                        else
                        {
                            j++;
                            i--;
                        }
                    }
                    else
                    {
                        j++;
                        continue;
                    }
                }
            }

            catch
            {
                MessageBox.Show("Catastrophic Remove error.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool Remove(ConnectedApplianceInfo cai, IPData ipd)
        {
            bool changed = false;
            try
            {
                string[] vers = cai.VersionNumber.Replace(" ", "").Split('|');
                if (vers[0] == TVers)
                {
                    ipd.Version = vers[0];
                    ipd.Done = true;
                    ipd.Result = "PASS - Product Version changed to final version.";
                    WriteFile(ipd.TabIndex, null);
                    DataRow dr = results.Rows[ipd.TabIndex];
                    results.Rows.Remove(dr);
                    changed = true;
                    DGV_Data.Refresh();
                }
                return changed;
            }
            catch
            {
                MessageBox.Show("Catastrophic Remove error.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public void Check()
        {
            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                ConnectedApplianceInfo cai = null;
                int j = 0;
                int number = iplist.Count;
                for (int i = 0; i < number; i++)
                {
                    if (cancel_request)
                        break;
                    cai = cio.ElementAtOrDefault(j);
                    if (cai != null)
                    {
                        IPData ipd = iplist.FirstOrDefault(x => x.MAC == cai.MacAddress);
                        if (ipd != null && !ipd.Done)
                        {
                            j++;
                            if (Remove(cai, ipd))
                                iplist.RemoveAt(ipd.TabIndex);
                        }
                        else
                        {
                            j++;
                            i--;
                            continue;
                        }
                    }
                    else
                    {
                        j++;
                        continue;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Catastrophic Check error.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProcessCAI()
        {
            try
            {
                
                while (!cancel_request)
                {
                    Build();

                    StartTimer();
                    Wait(TWAIT);
                    TMR_Tick.Stop();
                    LBL_Time.Text = "RUNNING";

                    Scan();

                    Check();
                }

                DGV_Data.Refresh();
                FinalResult();
            }
            catch
            {
                MessageBox.Show("Catastrophic ProcessCAI error.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                        Console.WriteLine("Thread with ID " + Thread.CurrentThread.ManagedThreadId + " interrupted.");
                    }
                });
                waits.Add(thread);
                thread.Start();
                while (thread.IsAlive)
                {
                    Application.DoEvents();
                }
            }

            catch
            {
                MessageBox.Show("Catastrophic Wait error.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }       
        public IPData AddResult(ConnectedApplianceInfo cai)
        {
            try
            {

                IPData ipd = new IPData(cai.IPAddress, TB_Payload.Text); 
                iplist.Add(ipd);
                ipd.TabIndex = iplist.IndexOf(ipd);
                ipd.IPAddress = cai.IPAddress;
                ipd.Payload = TB_Payload.Text;
                ipd.Model = cai.ModelNumber;
                ipd.Serial = cai.SerialNumber.Replace(" ","");                
                ipd.MAC = cai.MacAddress;
                ipd.Result = "PENDING";

                string[] vers = cai.VersionNumber.Replace(" ", "").Split('|');
                ipd.Version = vers[0];

                // Update window for added IP
                DataRow dr = results.NewRow();

                dr["IP Address"] = ipd.IPAddress;
                dr["OTA Payload"] = ipd.Payload;
                dr["Model"] = ipd.Model;
                dr["Serial"] = ipd.Serial;
                dr["MAC"] = ipd.MAC;
                dr["Version"] = ipd.Version;
                dr["OTA Result"] = ipd.Result;
                results.Rows.Add(dr);

                return ipd;
            }
            catch
            {
                IPData ipd = new IPData("0.0.0.0", "");
                return ipd;
            }

        }
        bool RevelationConnect(ConnectedApplianceInfo cai)
        {
            int revatt = 0;

            while (revatt < ATTEMPTMAX)
            {
                try
                {
                    if (cancel_request)
                    {
                        return false;
                    }

                    revatt++;
                    if (cai != null)
                    {
                        //if it's not and Revelation is also not enabled, enable Revelation
                        if (!cai.IsRevelationConnected)
                        {
                            WifiLocal.ConnectTo(cai);
                            Wait(2000);
                        }
                        if (!cai.IsRevelationConnected)
                        {
                            //If Revelation is enabled, enable Trace
                            continue;
                        }

                        //If the Trace is enabled and Revelation is also connected return
                        if (cai.IsRevelationConnected)
                            return true;
                    }
                    else
                    {
                        MessageBox.Show("Revelation was not able to be connected due to error in WifiBasic." +
                                "If persists, you may need to close Widebox and try again.", "Error: Unable to start",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }

                catch
                {
                    MessageBox.Show("Revelation was not able to be connected due to error in WifiBasic." +
                                "If persists, you may need to close Widebox and try again.", "Error: Unable to start",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }


            return true;
        }
        public bool SendRevelation(ConnectedApplianceInfo cai)
        {
            bool revconnect = false;
            try
            {
                if (cai != null && !cai.IsRevelationConnected)
                {
                    // Connect revelation
                    WifiLocal.ConnectTo(cai);
                    Wait(2000);
                }

                // Send Revelation message
                if (cai != null && cai.IsRevelationConnected)
                {
                    WifiLocal.SendRevelationMessage(cai, new RevelationPacket()
                    {
                        API = 0xF1,
                        Opcode = 00,
                        Payload = paybytes,
                    });
                    Wait(2000);
                    revconnect = true;
                }

                // Close revelation
                if (revconnect)
                {
                    WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                    //WifiLocal.Close(cai);
                    Wait(2000);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                MessageBox.Show("Catastrophic SendRevelation error.", "Error",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void BTN_Clr_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will clear all IPs and their results from all windows. Press Yes to Clear or No to Cancel.",
                                                        "Verify Full Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                ResetForm(true, true);
            }
            
        }
        private void ResetForm(bool freset, bool resclear)
        {
            try
            {
                if (freset)
                {
                    BTN_Start.Text = "Start";
                    BTN_Clr.Enabled = true;
                    TB_LogDir.Enabled = true;
                    BTN_LogDir.Enabled = true;
                    TB_Payload.Enabled = true;
                    TB_Version.Enabled = true;
                    LBL_Time.Text = "00:00:00";
                }

                if (resclear)
                {
                    if (BTN_Start.Text == "Start")
                    {
                        TB_Payload.Text = "";
                        TB_Version.Text = "";
                    }
                    results.Clear();
                    DGV_Data.Refresh();
                    iplist.Clear();
                    waits.Clear();
                }
            }
            catch
            {
                MessageBox.Show("Catastrophic ResetForm error.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void FinalResult()
        {
            try
            {
                if (cancel_request)
                    return;
                ResetForm(true, false);
                g_time.Stop();
                long duration = g_time.ElapsedMilliseconds;
                double average = 0.0;
                g_time.Reset();
                TimeSpan t = TimeSpan.FromMilliseconds(duration);
                string s_dur = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);
                if (totalran != 0)
                    average = (double)duration / totalran;

                TimeSpan a = TimeSpan.FromMilliseconds(average);
                string s_avg = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            a.Hours,
                            a.Minutes,
                            a.Seconds,
                            a.Milliseconds);
                WriteFile(0, s_dur + '\t' + s_avg);
                /*MessageBox.Show(totalran + " OTA Update(s) ran with a total running time of " + s_dur +
                                "  that resulted in an average run time per OTA Update of " + s_avg
                                + ".", "Final Result", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
                          
            }

            catch
            {
                MessageBox.Show("Catastrophic FinalResult error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void Scan()
        {
            if (cancel_request)
                return;
            string localIP = WifiLocal.Localhost.ToString();
            try
            {   
                WifiLocal.ScanConnectedAppliances(true, localIP);
                Wait(2000);
            }

            catch
            {
                MessageBox.Show("Catastrophic Scan error.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        private void Venom_Load(object sender, EventArgs e)
        {
            ResetForm(true, true);
        }
        private void TMR_Tick_Tick(object sender, EventArgs e)
        {
            timeleft -= 1000;

            if (timeleft < 0)
                timeleft = 0;

            TimeSpan t = TimeSpan.FromMilliseconds(timeleft);
            LBL_Time.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);
        }

    }
}
