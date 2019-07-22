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
    public partial class LED : UserControl
    {
        public LED()
        {
            InitializeComponent();            
        }

        public void SetColor(Color c)
        {
            but.BackColor = c;
        }
    }
}
