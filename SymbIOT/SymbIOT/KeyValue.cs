using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SymbIOT
{
    public class KeyValue : IComparable<KeyValue>
    {
        //The container object name of the key value.  Most key values are named ObjectName_ElementName.  This is ObjectName.
        private string _class;
        public string Class
        {
            get { return _class; }
        }

        private string _namespace;
        public string Namespace
        {
            get { return _namespace; }
        }

        private string _entity;
        public string Entity
        {
            get { return _entity; }
        }

        private string _instance;
        public string Instance
        {
            get { return _instance; }
        }

        //The element name of the key value.  Most key values are named ObjectName_ElementName.  This is ElementName.
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
        }

        public string AttributeName
        {
            get { return _displayName.Substring(_displayName.LastIndexOf('_') + 1); }
        }

        public string CapabilityName
        {
            get { return _instance + AttributeName; }
        }

        //Boolean to determin whether to interpret the value as a signed or unsigned integer
        private bool _signed;
        public bool isSigned
        {
            get { return _signed; }
        }

        private bool _isset;
        public bool isSet
        {
            get { return _isset; }
            set { _isset = value; }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private string _equation;
        public string Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }
        //Length of the expected payload in bytes.  Takes the string representation of the data type and converts it to
        //a byte length and sign value
        private string _length;
        public int Length
        {
            get
            {
                int templen;
                if (_length == "uint8" || _length == "boolean")
                {
                    templen = 2;
                }
                else if (_length == "int8")
                {
                    templen = 2;
                    _signed = true;
                }
                else if (_length == "uint16")
                {
                    templen = 4;
                }
                else if (_length == "int16")
                {
                    templen = 4;
                    _signed = true;
                }
                else if (_length == "uint32")
                {
                    templen = 8;
                }
                else if (_length == "int32")
                {
                    templen = 8;
                    _signed = true;
                }
                else
                {
                    templen = 0;
                }

                return templen;

            }
            set { _length = value.ToString(); }
        }

        //The key value ID as a hexadecimal string
        private string _keyID;
        public string KeyID
        {
            get { return _keyID; }
        }

        //The name of the enumeration set associated with this key value
        private Enumeration _enumName;
        public Enumeration EnumName
        {
            get
            {  return _enumName; }
            set
            {
                _enumName = value;
                _format = _enumName == null ? "int" : "enumeration";
            }
        }

        private int _api;
        public int API
        {
            get { return _api; }
            set { _api = value; }
        }

        private int _node;
        public int Node
        {
            get { return _node; }
            set { _node = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
        }

        private string _payload;
        public string Payload
        {
            get { return _payload; }
        }

        private bool _canSet;
        public bool CanSet
        {
            get { return _canSet; }
            set { _canSet = value; }
        }

        private string _value;
        public string RcvValue
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _setvalue;
        public string SetValue
        {
            get { return _setvalue; }
            set { _setvalue = value; }
        }

        private string _rawvalue;
        public string RawValue
        {
            get { return _rawvalue; }
            set { _rawvalue = value; }
        }

        private string _setrawvalue;
        public string SetRawValue
        {
            get { return _setrawvalue; }
            set { _setrawvalue = value; }
        }

        private bool _isUsed;
        public bool IsUsed
        {
            get { return _isUsed; }
            set { _isUsed = value; }
        }

        private bool _isState;
        public bool IsState
        {
            get { return _isState; }
            set { _isState = value; }
        }

        public string LengthString
        {
            get { return _length; }
        }

        public int CompareTo(KeyValue other)
        {
            return _keyID.CompareTo(other.KeyID);
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public KeyValue()
        {
            _enumName = null;
            _node = -1;
        }

        /// <summary>
        /// Constructor for a KeyValue object
        /// </summary>
        /// <param name="cls">The name of the object class</param>
        /// <param name="disname">The display name of the key value</param>
        /// <param name="len">The data type of the key value as a string (e.g. "int8", "uint8", "string", etc)</param>
        /// <param name="id">The key id as a hexadecimal string</param>
        public KeyValue(string cls, string ns, string ent, string ins, string disname, string len, string id, int api, string desc, string pay)
        {
            _signed = false;
            _class = cls;
            _namespace = ns;
            _entity = ent;
            _instance = ins;
            _displayName = disname;
            _length = len;
            _keyID = id;
            _description = desc;
            _payload = pay;
            _enumName = null;
            _isset = false;
            _format = this.Length == 0 ? "ASCII" : "int";
            _format = this._displayName.Contains("Time") && !(this._displayName.Contains("Real_Time") || this._displayName.Contains("RealTime")) && this.Length > 2 ? "time" : _format;
            _format = this._displayName.Contains("Relational") && this.Length == 8 && this.KeyID.StartsWith("F") ? "hex" : _format;
            _equation = this._displayName.Contains("Temp") && this.Length == 4 ? "(X/10)*(9/5) + 32" : "";
            _format = this._displayName.Contains("Temp") && this.Length == 4 ? "equation" : _format;
            _api = api;
            _node = -1;
            _canSet = false;
            _value = "";
            _rawvalue = "";
            _isUsed = true;
        }

        //Take the raw hex value and return the translated output value
        public string Process(string value)
        {
            string retvalue = "";
            switch (_format)
            {
                case "int":
                    retvalue = returnInt(value);
                    break;
                case "enumeration":
                    retvalue = _enumName == null ? value : this.EnumName.Enums[int.Parse(value, NumberStyles.AllowHexSpecifier)].ToString();
                    break;
                case "hex":
                    retvalue = value;
                    break;
                case "equation":
                    retvalue = Evaluate(int.Parse(returnInt(value)), _equation);
                    break;
                case "time":
                    long parsed = long.Parse(returnInt(value));
                    retvalue = ((int)(parsed / 3600)).ToString() + ":" + ((int)(parsed / 60) % 60).ToString().PadLeft(2, '0') + ":" + (parsed % 60).ToString().PadLeft(2, '0');
                    break;
                default:
                    retvalue = value;
                    break;

            }

            return retvalue;
        }

        //Take the translated output value and convert it to hex for Set
        public string OutValue(string input)
        {
            string hexlength = "X" + this.Length.ToString();
            string retvalue = this._keyID;
            try
            {
                switch (_format)
                {
                    case "ASCII":
                        retvalue += StringtoHex(input);
                        break;
                    case "enumeration":
                        retvalue += this._enumName.Enums.IndexOf(input).ToString(hexlength);
                        break;
                    case "equation":
                        retvalue += FtoCx10(input);
                        break;
                    case "time":
                        retvalue += ConvertTime(input);
                        break;
                    case "hex":
                        retvalue += (new String('0',this.Length) + input).Substring(this.Length);
                        break;
                    default:
                        retvalue += int.Parse(input).ToString(hexlength);
                        break;

                }
            }
            catch
            {
                retvalue += 0.ToString(hexlength);
            }

            return retvalue;
        }

        public string ConvertTime(string f)
        {
            string[] hms = f.Split(':');
            int time = 0;
            for (int i = hms.Length - 1; i >= 0; i--)
            {
                hms[i] = hms[i] == "" ? "00" : hms[i];
                time += (int)(int.Parse(hms[i])*Math.Pow(60, hms.Length - 1 - i));
            }

            return time.ToString("X" + this.Length.ToString());
        }

        public string FtoCx10(string f)
        {
            double b = 0;
            if (SymbIOT.tempUnits == 1)
            {
                double fs = double.Parse(f);
                double a = fs - 32;
                b = (a * 5 / 9) * 10;
            }
            else
            {
                b = int.Parse(f) * 10;
            }
            return ((int)b).ToString("X4");
        }

        public string StringtoHex(string s)
        {
            string mes = "";
            char[] values = s.ToCharArray();
            foreach (char c in values)
            {
                mes += Convert.ToInt32(c).ToString("X2");
            }
            mes += "00";
            return mes;
        }

        public static string Evaluate(int rawvalue, string expression)
        {
            if (expression.StartsWith("+") || expression.StartsWith("-") || expression.StartsWith("*") || expression.StartsWith("/") || expression == "")
            {
                expression = "X" + expression;
            }
            if (SymbIOT.tempUnits == 0)
            {
                expression = "X/10";
            }
            expression = expression.Replace("X", rawvalue.ToString());
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("expression", string.Empty.GetType(), expression);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return Math.Round(double.Parse((string)row["expression"])).ToString();
        }

        private string returnInt(string value)
        {
            if (this.Length < 8)
            {
                int val = int.Parse(value, NumberStyles.AllowHexSpecifier);
                if (this.isSigned)
                {
                    string un_data = value;
                    if (un_data.Length == 2 && int.Parse(un_data, NumberStyles.AllowHexSpecifier) > 127)
                    {
                        un_data = un_data.PadLeft(4, 'F');
                    }
                    val = unchecked((short)Convert.ToUInt16(un_data, 16));
                }
                return val.ToString();
            }
            else
            {
                //Default assumption is that a 4-byte value is a bit-shifted integer
                /*value = int.Parse(extractedMessage.Substring(keylength, kv.Length), NumberStyles.AllowHexSpecifier);
                double dec = (double)value / 65536;
                retvalue = dec != 0 ? dec.ToString("#.####") : "0";*/
                return long.Parse(value, NumberStyles.AllowHexSpecifier).ToString();
            }
        }
    }
}
