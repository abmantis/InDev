
using System.Threading;
using WideBoxLib;
using WirelessLib;

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

        private string _mqttpay;
        public string MQTTPay
        {
            get { return _mqttpay; }
            set { _mqttpay = value; }
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

        private string _node;
        public string Node
        {
            get { return _node; }
            set { _node = value; }
        }

        private string _name;
        public string Name
        { 
            get { return _name; }
            set { _name = value; }
        }
        private string _waittype;
        public string WaitType
        {
            get { return _waittype; }
            set { _waittype = value; }
        }
        private string _typeres;
        public string Typeres
        {
            get { return _typeres; }
            set { _typeres = value; }
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

        private byte[] _bipadd;
        public byte[] ByteIP
        {
            get { return _bipadd; }
            set { _bipadd = value; }
        }
        private byte[] _bpay;
        public byte[] BytePay
        {
            get { return _bpay; }
            set { _bpay = value; }
        }
        private ConnectedApplianceInfo _cai;
        public ConnectedApplianceInfo CAI
        {
            get { return _cai; }
            set { _cai = value; }
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
            _mqttpay = "";
            _wait = 0;
            _node = "";
            _name = "";
            _waittype = "";
            _typeres = "";
            _written = false;
            _bipadd = new byte[] { };
            _bpay = new byte[] { };
            _cai = null;
        }

    }
}
