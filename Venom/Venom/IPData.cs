using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private string _model;
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }
        private string _serial;
        public string Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }
        private string _down;
        public string Down
        {
            get { return _down; }
            set { _down = value; }
        }
        private string _next;
        public string Next
        {
            get { return _next; }
            set { _next = value; }
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
            _model = "";
            _serial = "";
            _down = "";
            _next = "UPGRADE";
        }

    }
}
