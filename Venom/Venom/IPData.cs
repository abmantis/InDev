using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VenomNamespace

{

    class IPData
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
        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private int _mqtt;
        public int MQTT
        {
            get { return _mqtt; }
            set { _mqtt = value; }
        }

        public IPData(string ip, string payload, string delivery)
        {
            _ipaddress = ip;
            _payload = payload;
            _delivery = delivery;
            _result = "";
            _mqtt = 0;
        }

    }
}
