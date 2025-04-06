/***************************************************************************************************************
*
* FILE NAME:   .\Program\BitmapUtils.cs
* 
* PROJECT:     Demo project regarding Using Dataflow in a Windows Forms Application,
*              based on MSDN project https://msdn.microsoft.com/en-us/library/hh228605(v=vs.110).aspx
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of Program class
*
**************************************************************************************************************/

using System;
using System.Windows.Forms;

using TestCompositeImages.UI;

namespace TestCompositeImages.Program
{
    /// <summary> A program containing main entry-point. </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
