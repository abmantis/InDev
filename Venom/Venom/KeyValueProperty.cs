using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VenomNamespace
{
    public class KeyValueProperty
    {
        private string _min;
        public string Min
        {
            get { return _min; }
            set { _min = value; }
        }

        private string _max;
        public string Max
        {
            get { return _max; }
            set { _max = value; }
        }

        private string _default;
        public string Default
        {
            get { return _default; }
            set { _default = value; }
        }

        private List<string> _list;
        public List<string> List
        {
            get { return _list; }
        }

        private string _step;
        public string Step
        {
            get { return _step; }
            set { _step = value; }
        }

        private string _stepC;
        public string StepC
        {
            get { return _stepC; }
            set { _stepC = value; }
        }

        private string _stepF;
        public string StepF
        {
            get { return _stepF; }
            set { _stepF = value; }
        }

        private List<int> _validenums;
        public List<int> ValidEnums
        {
            get { return _validenums; }
        }

        private KeyValue _keyvalue;
        public KeyValue KeyValue
        {
            get { return _keyvalue; }
        }

        private bool _required;
        public bool Required
        {
            get { return _required; }
        }

        public KeyValueProperty(KeyValue k, bool required)
        {
            _keyvalue = k;
            _validenums = new List<int>();
            _isenum = false;
            _default = "";
            _min = "0";
            _max = "1";
            _step = "1";
            _stepF = "1";
            _stepC = "1";
            _required = required;
            _list = new List<string>();
        }

        private bool _isenum;
        public bool IsEnum
        {
            get { return _isenum; }
            set { _isenum = value; }
        }

        public void AddValidEnum(int index)
        {
            _validenums.Add(index);
        }

        public void AddToList(string val)
        {
            _list.Add(val);
        }
    }
}
