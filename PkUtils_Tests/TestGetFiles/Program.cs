#define TEST_EVENT

using System;
using PK.PkUtils.Utils;


namespace PK.TestGetFiles
{
    /// <summary> A program. </summary>
    internal class Program
    {
        #region Public Methods

        /// <summary> Main entry-point for this application. </summary>
        /// <param name="args"> Array of command-line argument strings. </param>
        public static void Main(string[] args)
        {
            string strPath = string.Empty;
            string searchPattern = string.Empty;
            bool bDumpExceptions = false;
            CommandLineInfoEx cmdInfo = new(args, true, string.Empty);
            SearchTester tester = new(str => Console.WriteLine(str));

            // Parse command-line arguments (options and switches).
            // Example of possible command line:  PK.TestGetFiles.exe -d c:\Tmp3 -p *.txt

            // 1. Figure-out given path to parse, specified by command-line option -d, like 
            //   TestGetFiles.exe -d c:\Tmp3
            // If none specified, program will use Environment.GetFolderPath(Environment.SpecialFolder.System),
            // which is usually evaluated to "C:\windows\system32"
            if (!cmdInfo.GetOption("d", out strPath))
                strPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

            // 2. Figure-out search pattern, specified by command-line option -p, like
            //   TestGetFiles.exe -p *.txt
            // If none is specified, program will use "*.dll" string
            if (!cmdInfo.GetOption("p", out searchPattern))
                searchPattern = "*.dll";

            // 3. Figure-out whether to dump directory access errors 
            // ( caught SystemException(s), like UnauthorizedAccessException or IOException )
            //    
            bDumpExceptions = cmdInfo.GetSwitch("DumpExceptions");

            // test now
            tester.PerformFilesSearch(strPath, searchPattern, bDumpExceptions);
            Console.WriteLine("Press Enter to proceed");
            Console.ReadLine();

            tester.PerformFoldersSearch(strPath, bDumpExceptions);
        }
        #endregion // Public Methods
    }
}