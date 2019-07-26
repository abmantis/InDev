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

        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public IPData(string ip, string payload)
        {
            _ipaddress = ip;
            _payload = payload;
            _result = "RUNNING";
        }

    }
}
