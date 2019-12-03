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
    public partial class Rev : WideInterface
    {
        public Venom parent;

        public static int ATTEMPTMAX = 5;
        public static int TMAX = 120 * 60000; //OTA max thread time is 2 hours
        public static int RECONWAIT = 1 * 60000;

        public static object lockObj = new object();

        public bool cancel_request = false;

        private BindingSource sbind = new BindingSource();

        public Rev(Venom ParentForm, WideBox wideLocal, WhirlpoolWifi wifiLocal)
            : base(wideLocal, wifiLocal)
        {
            InitializeComponent();
            parent = ParentForm;

            // Generate tables
            sbind.DataSource = parent.results;
            DGV_Data.AutoGenerateColumns = true;
            DGV_Data.DataSource = parent.results;
            DGV_Data.DataSource = sbind;

            // Do not allow columns to be sorted
            foreach (DataGridViewColumn column in DGV_Data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }      

        private void BTN_Start_Click(object sender, EventArgs e)
        {
            string TVers = TB_Version.Text;
            int TAmt = Int32.Parse(TB_Max.Text);
            if (BTN_Start.Text == "Start")
            {

                //Write info to log
                if (!File.Exists(parent.TB_LogDir.Text + "\\" + "OTALog" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv"))
                {
                    {
                        // Verify directory exists, if not, throw exception
                        parent.curfilename = parent.TB_LogDir.Text + "\\" + "OTALog_" + DateTime.Now.ToString("MMddyyhhmmss") + ".csv";
                        try
                        {
                            using (StreamWriter sw = File.CreateText(parent.curfilename))
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
                BTN_Start.Text = "Stop Running";
                TB_Payload.Enabled = false;
                TB_Max.Enabled = false;
                TB_Version.Enabled = false;
                cancel_request = false;
                parent.g_time.Start();

                ProcessIP(TVers, TAmt);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("This will dispose of all running threads and end the OTA list execution. Are you sure you want to exit?",
                                                        "Verify Exiting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    cancel_request = true;
                    TB_Payload.Enabled = true;
                    TB_Max.Enabled = true;
                    TB_Version.Enabled = true;
                    parent.g_time.Stop();
                    parent.g_time.Reset();
                    BTN_Start.Text = "Start";

                    try
                    {
                        int stop = parent.iplist.Count();
                        for (int i_base = 0; i_base < stop; i_base++)
                        {

                            if (parent.iplist[i_base].Result.Contains("PASS") || parent.iplist[i_base].Result.Contains("FAIL"))
                            {
                                if (parent.iplist[i_base].Signal == null)
                                    continue;
                                if (!parent.iplist[i_base].Signal.IsSet)
                                    parent.iplist[i_base].Signal.Set();
                            }
                            else
                            {
                                parent.iplist[i_base].Result = "Cancelled by User.";
                                parent.results.Rows[i_base]["OTA Result"] = parent.iplist[i_base].Result;
                                if (parent.iplist[i_base].Signal == null)
                                    continue;
                                if (!parent.iplist[i_base].Signal.IsSet)
                                    parent.iplist[i_base].Signal.Set();
                            }
                        }

                        Invoke((MethodInvoker)delegate
                        {
                            DGV_Data.Refresh();
                        });

                        foreach (Thread thread in parent.waits)
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
                else //Dialog was Yes
                    return;

            }
        }
        public void ProcessIP(string TVers, int TAmt)
        {
            int number;
            int ipc;
            try
            {

                //Initialize list
                for (int i = 0; i < TAmt; i++)
                {
                    IPData newip = new IPData("0.0.0.0", "");
                    parent.iplist.Add(newip);
                }
                //Parse payload into byte array
                byte[] paybytes = Encoding.ASCII.GetBytes(TB_Payload.Text);

                ipc = parent.iplist.Count();
                //Set barrier to wait for all threads to complete
                Barrier barrier = new Barrier(participantCount: ipc);
                foreach (IPData ipd in parent.iplist)
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio = WifiLocal.ConnectedAppliances;
                    //ConnectedApplianceInfo cai = cio.FirstOrDefault(x => x.VersionNumber.Split('|').Select(y => new { value = y[0] }).ToString() != TVers);
                    /*cio.FirstOrDefault(x =>
                    {
                        var pair = x.VersionNumber.Split(new char[] { '|' });
                        string version = pair[0];
                    });*/
                    string[] parts;
                    number = cio.Count;
                    ConnectedApplianceInfo cai = null;
                    for (int i = 0; i < number; i++)
                    {
                        parts = cio[i].VersionNumber.Replace(" ", "").Split('|');
                        if (parts[0] != TVers)
                        {
                            cai = cio[i];
                            break;
                        }

                    }
                    number = 0;

                    if (cai != null)
                    {
                        if (cai.IsTraceOn && cai.IsRevelationConnected)
                            continue;
                        if (!RevelationConnect(cai, ipd))
                            return;

                        if (parent.iplist[number].IPAddress == cai.IPAddress)
                        {
                            number++;
                            continue;
                        }

                        else
                        {
                            ipd.IPAddress = cai.IPAddress;
                            ipd.Payload = TB_Payload.Text;
                            ipd.Node = "HMI";
                            ipd.Type = "Upgrade";
                            ipd.Delivery = "Revelation";
                            ipd.MQTTPay = "NA";
                            ipd.Name = "User Input";
                            ipd.MAC = cai.MacAddress;
                            ipd.Result = "PENDING";

                            // Update window for added IP
                            DataRow dr = parent.results.NewRow();

                            dr["IP Address"] = ipd.IPAddress;
                            dr["OTA Payload"] = ipd.Payload;
                            dr["Delivery Method"] = ipd.Delivery;
                            dr["OTA Type"] = ipd.Type;
                            dr["Node"] = ipd.Node;
                            dr["Name"] = ipd.Name;
                            dr["OTA Result"] = ipd.Result;
                            parent.results.Rows.Add(dr);

                            string name = "";

                            ManualResetEventSlim sig = new ManualResetEventSlim();
                            Thread th = new Thread(() => RunTask(cai, sig, name, barrier, ipd, ref paybytes));
                            th.Name = number.ToString();
                            name = th.Name;
                            th.IsBackground = true;
                            th.Start();
                        }

                        number++;
                    }
                }

            }
            catch
            {
                MessageBox.Show("Catastrophic ProcessIP error.", "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static void ProgressThread(object sender, ElapsedEventArgs e, IPData ipd)
        {
            try
            {
                Console.WriteLine("End Thread was called for signal " + ipd.Signal.WaitHandle.Handle +
                              " and thread was released (set).");
                if (!ipd.Result.Contains("PASS") || !ipd.Result.Contains("FAIL"))
                    ipd.Result = "FAIL - Thread timeout maximum reached. Unknown issue caused thread to be stuck. Moving to next in list.";
                ipd.Signal.Set();
            }
            catch
            {
                MessageBox.Show("Catastrophic ProgressThread error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void RunTask(ConnectedApplianceInfo cai, ManualResetEventSlim sig, string ipindex, Barrier barrier, IPData ipd, ref byte[] paybytes)
        {
            try
            {
                if (cancel_request)
                {
                    Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                    return;
                }
                if (ipd.IPAddress.Equals(cai.IPAddress) && Thread.CurrentThread.Name.ToString().Equals(ipindex))
                {
                    ipd.IPIndex = Int32.Parse(ipindex);
                    ipd.Signal = sig;
                    ipd.TabIndex = parent.iplist.IndexOf(ipd);
                    bool thread_waits = true;   // Indicates this is a thread that will require a reboot time for the product

                    //Force each thread to live only two hours (process somehow got stuck)
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = TMAX;
                    timer.Elapsed += (sender, e) => ProgressThread(sender, e, ipd);
                    timer.Start();


                    // See if sending over MQTT or Revelation
                    if (ipd.Delivery.Equals("Revelation"))
                    {
                        if (SendRevelation(cai, ref paybytes))
                            thread_waits = true;

                        else
                        {
                            //Otherwise IP changed, use MAC address to map to new IP
                            System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio_m = WifiLocal.ConnectedAppliances;
                            ConnectedApplianceInfo cai_m = cio_m.FirstOrDefault(x => x.MacAddress == ipd.MAC);
                            if (cai_m != null)
                            {                                
                                if (SendRevelation(cai_m, ref paybytes))
                                    thread_waits = true;
                            }
                            else
                            {
                                MessageBox.Show("OTA target IP Address of " + cai.IPAddress + "was changed and unable to be remapped. Ending OTA attempts and " +
                                    "closing corresponding thread.", "Error: Unable to change IP Address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                ipd.Result = "FAIL - Bad IP Address. Attempted to map new IP Address from MAC address failed.";
                                parent.SetText("status", "Bad IP Address", ipd.TabIndex);
                                thread_waits = false;
                            }
                        }

                    }

                    // Set wait signal to be unlocked by thread after job is complete (result seen from log)
                    if (thread_waits)
                    {
                        Console.WriteLine("Thread Wait reached. Thread with lock ID " + ipd.Signal.WaitHandle.Handle + " and name " + Thread.CurrentThread.Name +
                           " and this IP Index (from thread order) " + ipd.IPIndex + " for this IP Address " + ipd.IPAddress + ".");
                        ipd.Signal.Wait();

                        if (ipd.Result.Contains("timeout"))
                            parent.SetText("status", "Force Close", ipd.TabIndex);
                    }

                    timer.Stop();
                    timer.Dispose();
                    // Check to see if thread should be cancelled
                    if (cancel_request)
                    {
                        Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
                        return;
                    }
                    System.Collections.ObjectModel.ReadOnlyCollection<ConnectedApplianceInfo> cio_n = WifiLocal.ConnectedAppliances;
                    ConnectedApplianceInfo cai_n = cio_n.FirstOrDefault(x => x.IPAddress == ipd.IPAddress);
                    parent.Wait(2 * RECONWAIT); //Wait two minutes for MQTT to come back on after reboout out of IAP
                    

                    Console.WriteLine("Thread " + Thread.CurrentThread.Name + " finished a task.");
                    ipd.Signal.Reset();
                }
                /*else
                {
                    continue;
                }*/


                Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " reached barrier.");

                //Thread task cancelled or completed, signal others it is done
                barrier.SignalAndWait();
                lock (lockObj)
                {
                    parent.FinalResult();
                }
            }
            catch
            {
                MessageBox.Show("Catastrophic RunTask error.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                barrier.SignalAndWait();
                return;
            }

        }
        bool RevelationConnect(ConnectedApplianceInfo cai, IPData ipd)
        {
            int traceattempt = 0;

            while (traceattempt < ATTEMPTMAX)
            {
                try
                {
                    if (cancel_request)
                    {
                        Console.WriteLine("Thread with name " + Thread.CurrentThread.Name + " and ID " + Thread.CurrentThread.ManagedThreadId + " closed.");
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
                                if (traceattempt > (ATTEMPTMAX / 2))
                                    parent.CycleWifi();
                                WifiLocal.ConnectTo(cai);
                                parent.Wait(2000);
                            }
                            if (cai.IsRevelationConnected)
                            {
                                //If Revelation is enabled, enable Trace
                                WifiLocal.EnableTrace(cai, true);
                                parent.Wait(2000);
                            }
                        }
                        else
                        {
                            //If the Trace is enabled and Revelation is also connected return
                            if (cai.IsRevelationConnected)
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


                catch
                {
                    MessageBox.Show("Trace was not able to start.  Please verify the socket can be " +
                                    "opened and it is not in use (UITracer is not running). You may need to close" +
                                "Widebox and try again.", "Error: Unable to start Trace",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }


            return false;
        }
        public bool SendRevelation(ConnectedApplianceInfo cai, ref byte[] paybytes)
        {
            bool revconnect = false;
                try
                {
                    if (cai != null && !cai.IsRevelationConnected)
                    {
                        // Connect revelation
                        WifiLocal.ConnectTo(cai);
                        parent.Wait(2000);
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
                        parent.Wait(2000);
                        revconnect = true;
                    }

                    // Close revelation
                    if (revconnect)
                    {
                        WifiLocal.CloseRevelation(System.Net.IPAddress.Parse(cai.IPAddress));
                        //WifiLocal.Close(cai);
                        parent.Wait(2000);
                    return true;
                    }
                    else
                        return false;
                }
                catch {
                    MessageBox.Show("Catastrophic SendRevelation error.", "Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
        }
    }
}
