using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace APIDebugger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WideBoxInterface.WideBoxInterfaceForm main = new WideBoxInterface.WideBoxInterfaceForm();
            SymbIOT.SymbIOT debug = new SymbIOT.SymbIOT(main.WideLocal, main.WifiLocal);
            main.Run(new WideInterface[] { debug });
        }
    }
}
