using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VenomNamespace
{
    public class Enumeration
    {

        //The name of the enumeration set
        private string _name;
        public string Name
        {
            get {
                if (_name.IndexOf(':') > 0)
                {
                    _name = _name.Substring(0, _name.IndexOf(':'));
                }
                return _name;
            }
            set { _name = value; }
        }

        //An array of 255 objects containing the associated enumeration at each index
        private ArrayList _enums;
        public ArrayList Enums
        {
            get { return _enums; }
        }

        private List<bool> _isUsed;
        public List<bool> IsUsed
        {
            get { return _isUsed; }
        }

        private string _usedby;
        public string UsedBy
        {
            get { return _usedby; }
            set { _usedby = value; }
        }

        private string _lcs;
        public string LCS
        {
            get { return _lcs; }
        }

        /// <summary>
        /// An Enumeration object
        /// </summary>
        /// <param name="name">The name of the enumeration set</param>
        /// <param name="alist">An ArrayList containing the enumerations</param>
        public Enumeration(string name, ArrayList alist)
        {
            _name = name;
            _enums = alist;
            _usedby = "";
            _lcs = "";
        }

        /// <summary>
        /// An empty Enumeration object
        /// </summary>
        /// <param name="name">The name of the enumeration set</param>
        public Enumeration(string name)
        {
            _name = name;
            _enums = new ArrayList(255);
            _isUsed = new List<bool>();
            _usedby = "";
            _lcs = "";
            FillEnums();
        }

        /// <summary>
        /// An empty Enumeration object
        /// </summary>
        public Enumeration()
        {
            _name = "";
            _enums = new ArrayList(255);
            _isUsed = new List<bool>();
            _usedby = "";
            _lcs = "";
            FillEnums();
        }

        /// <summary>
        /// Create an empty enumeration array with 255 blank objects, to be filled later
        /// </summary>
        private void FillEnums()
        {
            for (int i = 0; i < 256; i++)
            {
                _enums.Add(null);
                _isUsed.Add(false);
            }
        }

        /// <summary>
        /// Insert an enumeration value at the selected index
        /// </summary>
        /// <param name="index">Index to insert value</param>
        /// <param name="value">Value to insert</param>
        public void insertEnum(int index, string value)
        {
            _enums[index] = value;
            //_isUsed[index] = value == null ? false : true;
        }

        /// <summary>
        /// Sets whether an enumeration is used or not
        /// </summary>
        /// <param name="index"></param>
        /// <param name="enabled"></param>
        public void enableEnum(int index, bool enabled)
        {
            if (_isUsed[index])
            {
            }
            else
            {
                _isUsed[index] = enabled;
            }
            
        }

        /// <summary>
        /// Add an enumeration to the next available position in the list
        /// </summary>
        /// <param name="value">Value to add</param>
        public void addEnum(string value)
        {
            _enums.Add(value);
        }

        public bool ContainsEnums()
        {
            bool notnull = false;
            foreach (string s in _enums)
            {
                if (s != null) { notnull = true; }
            }

            return notnull;
        }

        public string ToCamelCase(string input,bool me)
        {
            string op = input.Substring(input.IndexOf('.')+1).Replace("_", " ");
            op = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(op.ToLower());
            op = op.Replace("Type", "").Replace(" ","");
        if(me)
    {
            _lcs = op;
    }
            return op;
        }

        private string FindLCS()
        {
            string lcs = _enums[0].ToString();
            foreach (string s in _enums)
            {
                if (s!= null && !s.Contains(lcs))
                {
                    for (int i = 0; i < lcs.Length; i++)
                    {
                        if (lcs[i] != s[i])
                        {
                            if (!Char.IsUpper(s[i]))// if the first few letters of the actual enum match, back up and find the last capital letter
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (Char.IsUpper(s[j]))
                                    {                                       
                                        lcs = lcs.Substring(0, j);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                lcs = lcs.Substring(0, i);
                            }
                            break;
                        }
                    }
                }
            }
            if (lcs == _enums[0].ToString())
            {
                lcs = "";
            }

            return lcs;
        }

        public void TrimEnums()
        {
            try
            {
                string lcs = ToCamelCase(_name,true);
                if (!(lcs != "" && _enums[0].ToString().StartsWith(lcs) && _enums[0].ToString() != lcs))
                {
                    lcs = FindLCS();                    
                }
                for (int i = 0; i < _enums.Count; i++)
                {
                    if(_enums[i] != null && lcs != "")//if (_isUsed[i])
                    {
                        _enums[i] = _enums[i].ToString().Replace(lcs, "");
                    }
                }
                _lcs = lcs != "" ? lcs : _lcs;
            }
            catch { }
        }
    }
}
