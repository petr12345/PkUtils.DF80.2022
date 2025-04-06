using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using TestDataDef;

namespace TestBinding
{
    public class Program : Singleton<Program>
    {
        public List<Person> Data_p_People { get; set; }
        public List<Salary> Data_p_Salaries { get; set; }
        public List<Role> Data_p_Roles { get; set; }
        public List<Package> Data_p_Packages { get; set; }

        public string Data_p_String { get; set; }

        private Program()
        {
            Data_p_People = TestData.People();
            Data_p_Salaries = TestData.Salaries();
            Data_p_Roles = TestData.Roles();
            Data_p_Packages = TestData.Packages();
            Data_p_String = TestData.Sentence().Reverse().Aggregate((workingSent, next) => workingSent + " " + next);
        }

        protected void Execute()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyForm());
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Instance.Execute();
        }
    }
}
