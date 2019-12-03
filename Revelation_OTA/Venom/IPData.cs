
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

        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }
        private string _serial;
        public string Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }
        private string _model;
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string _ccuri;
        public string CCURI
        {
            get { return _ccuri; }
            set { _ccuri = value; }
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

        private int _wait;
        public int Wait
        {
            get { return _wait; }
            set { _wait = value; }
        }
        private bool _written;
        public bool Written
        {
            get { return _written; }
            set { _written = value; }
        }
        public IPData(string ip, string payload)
        {
            _ipaddress = ip;
            _payload = payload;
            _model = "";
            _serial = "";
            _version = "";
            _mac = "";
            _result = "";
            _ipindex = 0;
            _tabindex = 0;
            _signal = null;
            _wait = 0;
            _written = false;
            _ccuri = "";            
        }

    }
}
