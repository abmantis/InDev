using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbIOT
{
    public partial class RelationalContainer : UserControl
    {
        public Control parent;
        public RelationalBuilder2 topParent;
        public Control selectedChild;
        public int Level;
        public string type;
        public string datatype = "01";
        public int entityCount = 0;
        public int arrayCount;
        public string Value;

        public RelationalContainer(KeyValue openKey, KeyValue closeKey, Control parent, RelationalBuilder2 topparent, int lev, string val)
        {
            InitializeComponent();
            selectedChild = null;
            this.parent = parent;
            this.topParent = topparent;
            this.Level = lev;
            arrayCount = 0;
            RelationalItem2 ri = new RelationalItem2(openKey, this, topparent, val);
            this.Controls.Add(ri);
            RelationalItem2 ri2 = new RelationalItem2(closeKey, this, topparent, val);
            this.Controls.Add(ri2);
           // this.topParent.ClearAllSelected();
            //ri.SetSelected(true);
            RefreshSize();
            Value = val;
        }

        public void RefreshSize()
        {
            int height = 0;
            foreach (Control c in this.Controls)
            {
                c.Top = height;
                height += c.Height;
            }

            this.Height = height;
            if (this.parent != this.topParent)
            {
                ((RelationalContainer)parent).RefreshSize();
            }
            else
            {
                this.topParent.RefreshArrayCount();
            }
        }

        public void AddKey(KeyValue kv, string val)
        {
            RelationalItem2 ri = new RelationalItem2(kv, this, topParent, val);
            if (kv.Instance.Contains("Upper"))
            {
                foreach (KeyValue k in this.topParent.keyValues)
                {
                    if (k.Instance.Contains("Lower") && k.AttributeName == kv.AttributeName)
                    {
                        ri.SetAltCavityKV(k, true);
                    }
                }
            }
            if (kv.Instance.Contains("Lower"))
            {
                foreach (KeyValue k in this.topParent.keyValues)
                {
                    if (k.Instance.Contains("Upper") && k.AttributeName == kv.AttributeName)
                    {
                        ri.SetAltCavityKV(k, false);
                    }
                }
            }
            this.Controls.Add(ri);
            int index = selectedChild == null ? this.Controls.Count - 2 : this.Controls.GetChildIndex(selectedChild)+1;
            this.Controls.SetChildIndex(ri, index);
            this.topParent.ClearAllSelected();
            ri.SetSelected(true);
            RefreshSize();
        }

        public void AddContainer(RelationalContainer rc)
        {
            this.Controls.Add(rc);
            int index = selectedChild == null ? this.Controls.Count - 2 : this.Controls.GetChildIndex(selectedChild) + 1;
            this.Controls.SetChildIndex(rc, index);
            RefreshSize();
        }

        public void UpdateCount(bool updateValues)
        {
            string dt = this.topParent.GetGEBLCB() ? "02" : this.datatype;
            switch (this.type)
            {
                case "Envelope":
                    this.entityCount = 0;
                    this.topParent.numContainers = 1;
                    break;
                case "Array":
                    topParent.arrayCount++;
                    this.arrayCount = topParent.arrayCount;
                    this.entityCount = 0;
                    this.SetCount(dt + this.topParent.envLSB + (topParent.GetGEBLCB() ? entityCount.ToString("X2") + this.arrayCount.ToString("X2") : this.arrayCount.ToString("X2") + entityCount.ToString("X2")),updateValues);
                    this.entityCount++;
                    this.topParent.numContainers++;
                    break;
                case "Entity": 
                    this.SetCount(dt + this.topParent.envLSB + ((RelationalContainer)this.parent).arrayCount.ToString("X2") + ((RelationalContainer)parent).entityCount.ToString("X2"),updateValues);
                    ((RelationalContainer)parent).entityCount++;
                    this.topParent.numContainers++;
                    break;
                default:
                    break;
            }
            try
            {
                foreach (Control c in this.Controls)
                {
                    if (c.GetType().Name == "RelationalContainer")
                    {
                        ((RelationalContainer)c).UpdateCount(updateValues);
                    }
                }
            }
            catch
            {
            }
        }

        public void SetCount(string count, bool updateValues)
        {
            if (!updateValues)
            {
                count = this.Value;
            }
            ((RelationalItem2)this.Controls[0]).SetValue(count);
            ((RelationalItem2)this.Controls[this.Controls.Count-1]).SetValue(count);
            ((RelationalItem2)this.Controls[0]).SetCBOVisible(!topParent.GetGEBLCB());
            Value = count;
        }

        public void ClearAllSelected()
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType().Name == "RelationalContainer")
                {
                    ((RelationalContainer)c).ClearAllSelected();
                }
                else
                {
                    ((RelationalItem2)c).SetSelected(false);
                }
            }
            selectedChild = null;
        }

        public void MoveChild(Control c,int dir)
        {
            try
            {
                if (c.GetType().Name == "RelationalItem2")
                {
                    if (((RelationalItem2)c).OpenBracket || ((RelationalItem2)c).CloseBracket)
                    {
                        if (this.parent != this.topParent)
                        {
                            ((RelationalContainer)parent).MoveChild(this, dir);
                        }
                        else
                        {
                            //this.parent.Controls.SetChildIndex(this, this.parent.Controls.GetChildIndex(this) - dir);
                            //RefreshSize();
                        }
                    }
                    else
                    {
                        c.Parent.Controls.SetChildIndex(c, c.Parent.Controls.GetChildIndex(c) - dir);
                        RefreshSize();
                    }
                }
                else
                {
                    c.Parent.Controls.SetChildIndex(c, c.Parent.Controls.GetChildIndex(c) - dir);
                    RefreshSize();
                }
            }
            catch { }
        }

        public void RemoveChild(Control c)
        {
            if (c.GetType().Name == "RelationalItem2")
            {
                if (((RelationalItem2)c).OpenBracket || ((RelationalItem2)c).CloseBracket)
                {
                    if (this.parent != this.topParent)
                    {
                        ((RelationalContainer)parent).RemoveChild(this);
                    }
                    else
                    {
                        this.Parent.Controls.Remove(this);
                        this.topParent.RefreshArrayCount();
                    }
                }
                else
                {
                    this.Controls.Remove(c);
                    RefreshSize();
                }
            }
            else
            {
                this.Controls.Remove(c);
                RefreshSize();
            }
            
        }

        public string KVPMessage()
        {
            string mes = "";
            foreach (Control c in this.Controls)
            {
                if (c.GetType().Name == "RelationalContainer")
                {
                    mes += ((RelationalContainer)c).KVPMessage();
                }
                else
                {
                    mes += ((RelationalItem2)c).KVPMessage();
                }
            }

            return mes;
        }

        public string JSONMessage()
        {
            string mes = "";
            foreach (Control c in this.Controls)
            {
                if (c.GetType().Name == "RelationalContainer")
                {
                    mes += ((RelationalContainer)c).JSONMessage();
                }
                else
                {
                    mes += ((RelationalItem2)c).JSONMessage();
                }
            }

            return mes;
        }

        public void SwapCavities()
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType().Name == "RelationalContainer")
                {
                    ((RelationalContainer)c).SwapCavities();
                }
                else
                {
                    ((RelationalItem2)c).SwapCavities();
                }
            }
        }
    }
}
