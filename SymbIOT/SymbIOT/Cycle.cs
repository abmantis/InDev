using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbIOT
{
    public class Cycle
    {
        private string _keyID;
        public string KeyID
        {
            get { return _keyID; }
        }

        private KeyValue _keyvalue;
        public KeyValue KeyValue
        {
            get { return _keyvalue; }
            set { _keyvalue = value; }
        }

        private KeyValue _operationkey;
        public KeyValue OperationKey
        {
            get { return _operationkey; }
            set { _operationkey = value; }
        }

        private KeyValue _statekey;
        public KeyValue StateKey
        {
            get { return _statekey; }
            set { _statekey = value; }
        }

        private KeyValue _cycletimeremainingkey;
        public KeyValue CycleTimeRemainingKey
        {
            get { return _cycletimeremainingkey; }
            set { _cycletimeremainingkey = value; }
        }

        private KeyValue _displaytempkey;
        public KeyValue DisplayTempKey
        {
            get { return _displaytempkey; }
            set { _displaytempkey = value; }
        }

        private int _enum;
        public int Enum
        {
            get { return _enum; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private int _minTemp;
        public int MinTemp
        {
            get { return _minTemp; }
            set { _minTemp = value; }
        }

        private int _maxTemp;
        public int MaxTemp
        {
            get { return _maxTemp; }
            set { _maxTemp = value; }
        }

        private int _defTemp;
        public int DefTemp
        {
            get { return _defTemp; }
            set { _defTemp = value; }
        }

        private int _minTime;
        public int MinTime
        {
            get { return _minTime; }
            set { _minTime = value; }
        }

        private int _maxTime;
        public int MaxTime
        {
            get { return _maxTime; }
            set { _maxTime = value; }
        }

        private int _minAmt;
        public int MinAmt
        {
            get { return _minAmt; }
            set { _minAmt = value; }
        }

        private int _maxAmt;
        public int MaxAmt
        {
            get { return _maxAmt; }
            set { _maxAmt = value; }
        }

        private int _stepAmt;
        public int StepAmt
        {
            get { return _stepAmt; }
            set { _stepAmt = value; }
        }

        private string _valAmt;
        public string ValAmt
        {
            get { return _valAmt; }
            set { _valAmt = value; }
        }

        private int _defAmt;
        public int DefAmt
        {
            get { return _defAmt; }
            set { _defAmt = value; }
        }

        private bool _canSet;
        public bool CanSet
        {
            get { return _canSet; }
            set { _canSet = value; }
        }

        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private List<KeyValueProperty> requiredkeys;
        public List<KeyValueProperty> RequiredKeys
        {
            get { return requiredkeys; }
        }

        private List<KeyValueProperty> optionalkeys;
        public List<KeyValueProperty> OptionalKeys
        {
            get { return optionalkeys; }
        }

        private int _ccid;
        public int CCID
        {
            get { return _ccid; }
        }


        public Cycle(string id, int en, string name)
        {
            _keyID = id;
            _enum = en;
            _name = name;
            _minTemp = 0;
            _maxTemp = 0;
            _defTemp = 0;
            _minTime = 0;
            _maxTime = 0;
            _minAmt = 0;
            _maxAmt = 0;
            _stepAmt = 0;
            _defAmt = 0;
            _valAmt = "";
        }

        public Cycle(KeyValue k, int en)
        {
            _keyID = k.KeyID;
            _keyvalue = k;
            if (k.EnumName != null)
            {
                _enum = en;
                _name = k.EnumName.Enums[en].ToString();
            }
            else
            {
                _ccid = en;
                _name = en.ToString();
            }
            _minTemp = 0;
            _maxTemp = 0;
            _defTemp = 0;
            _minTime = 0;
            _maxTime = 0;
            _minAmt = 0;
            _maxAmt = 0;
            _stepAmt = 0;
            _defAmt = 0;
            _valAmt = "";
            requiredkeys = new List<KeyValueProperty>();
            optionalkeys = new List<KeyValueProperty>();
        }

        public void AddOptionKey(KeyValueProperty key, bool isRequired)
        {
            if (isRequired)
            {
                requiredkeys.Add(key);
            }
            else
            {
                optionalkeys.Add(key);
            }
        }
    }
}
