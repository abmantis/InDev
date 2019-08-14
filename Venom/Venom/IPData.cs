using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VenomNamespace

{
    public class IPData
    {
        private string _ipaddress;
        public string IPAddress
        {
            get { return _ipaddress; }
            set { _ipaddress = value; }
        }

        private string _payload;
        public string Payload
        {
            get { return _payload; }
            set { _payload = value; }
        }

        private string _delivery;
        public string Delivery
        {
            get { return _delivery; }
            set { _delivery = value; }
        }
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private string _mac;
        public string MAC
        {
            get { return _mac; }
            set { _mac = value; }
        }
        
        private ManualResetEventSlim _signal;
        public ManualResetEventSlim Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }
        private int _ipindex;
        public int IPIndex
        {
            get { return _ipindex; }
            set { _ipindex = value; }
        }

        private int _tabindex;
        public int TabIndex
        {
            get { return _tabindex; }
            set { _tabindex = value; }
        }
        public IPData(string ip, string payload)
        {
            _ipaddress = ip;
            _payload = payload;
            _delivery = "MQTT";
            _type = "";
            _mac = "";
            _result = "";
            _ipindex = 0;
        }

    }
}
