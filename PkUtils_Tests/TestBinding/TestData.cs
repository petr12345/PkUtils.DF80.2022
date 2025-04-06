using System.Collections.Generic;

namespace TestDataDef
{
    #region Typedefs

    public class Person
    {
        private int _id;
        private int _idRole;
        private string _lastName;
        private string _firstName;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public int IDRole
        {
            get { return _idRole; }
            set { _idRole = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
    }

    public class Role
    {
        private int _idRole;
        private string _roleDescription;

        public int ID
        {
            get { return _idRole; }
            set { _idRole = value; }
        }
        public string RoleDescription
        {
            get { return _roleDescription; }
            set { _roleDescription = value; }
        }
    }

    public class Salary
    {
        private int _idPerson;
        private int _year;
        private double _salary;

        public int IDPerson
        {
            get { return _idPerson; }
            set { _idPerson = value; }
        }
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        public double SalaryYear
        {
            get { return _salary; }
            set { _salary = value; }
        }
    }

    public class Package
    {
        public string Company { get; set; }
        public double Weight { get; set; }
        public long TrackingNumber { get; set; }
    }
    #endregion //Typedefs

    public static class TestData
    {
        #region TEST_DATA_RETRIEVAL

        public static int[] Arr1357()
        {
            return _arr1357;
        }

        public static int[] Arr2468()
        {
            return _arr2468;
        }

        public static string[] Sentence()
        {
            return _sentence.Split(' ');
        }

        public static object[] ObjSequence()
        {
            return _sequence;
        }

        public static List<Person> People()
        {
            List<Person> people = [
            new Person { ID = 1, IDRole = 1, LastName = "Anderson", FirstName = "Brad"},
            new Person { ID = 2, IDRole = 2, LastName = "Gray", FirstName = "Tom"},
            new Person { ID = 3, IDRole = 2, LastName = "Grant", FirstName = "Mary"},
            new Person { ID = 4, IDRole = 3, LastName = "Cops", FirstName = "Gary"},
            new Person { ID = 5, IDRole = 2, LastName = "XYZ", FirstName = "Mary"},
            new Person { ID = 6, IDRole = 3, LastName = "Gray-ed", FirstName = "Tom"}];

            return people;
        }

        public static List<Salary> Salaries()
        {
            List<Salary> salaries = [
            new Salary { IDPerson = 1, Year = 2004, SalaryYear = 10000.00 },
            new Salary { IDPerson = 1, Year = 2005, SalaryYear = 15000.00 },
            new Salary { IDPerson = 2, Year = 2004, SalaryYear =  5000.00 },
            new Salary { IDPerson = 2, Year = 2005, SalaryYear = 10000.00 },
            new Salary { IDPerson = 2, Year = 2006, SalaryYear = 12000.00 }];

            return salaries;
        }

        public static List<Salary> SalariesAll()
        {
            List<Salary> salaries = [
            new Salary { IDPerson = 1, Year = 2004, SalaryYear = 10000.00 },
            new Salary { IDPerson = 1, Year = 2005, SalaryYear = 15000.00 },
            new Salary { IDPerson = 2, Year = 2004, SalaryYear =  5000.00 },
            new Salary { IDPerson = 2, Year = 2005, SalaryYear = 10000.00 },
            new Salary { IDPerson = 2, Year = 2006, SalaryYear = 12000.00 },
            new Salary { IDPerson = 3, Year = 2005, SalaryYear = 75000.00 },
            new Salary { IDPerson = 3, Year = 2006, SalaryYear = 75000.00 },
            new Salary { IDPerson = 4, Year = 2004, SalaryYear = 11000.00 },
            new Salary { IDPerson = 4, Year = 2005, SalaryYear = 12000.00 }];

            return salaries;
        }

        public static List<Role> Roles()
        {
            List<Role> roles = [
            new Role { ID = 1, RoleDescription = "Manager" },
            new Role { ID = 2, RoleDescription = "Developer" }];
            return roles;
        }

        public static List<Package> Packages()
        {
            List<Package> packages = [
                new Package { Company = "Coho Vineyard", Weight = 25.2, TrackingNumber = 89453312L },
                new Package { Company = "Lucerne Publishing", Weight = 18.7, TrackingNumber = 89112755L },
                new Package { Company = "Wingtip Toys", Weight = 6.0, TrackingNumber = 299456122L },
                new Package { Company = "Contoso Pharmaceuticals", Weight = 9.3, TrackingNumber = 670053128L },
                new Package { Company = "Wide World Importers", Weight = 33.8, TrackingNumber = 4665518773L } ];
            return packages;
        }
        #endregion // TEST_DATA_RETRIEVAL

        #region Fields
        private static readonly int[] _arr1357 = { 1, 3, 5, 7 };
        private static readonly int[] _arr2468 = { 2, 4, 6, 8 };
        private static readonly string _sentence = "The quick brown fox jumps over the lazy dog";
        private static readonly object[] _sequence = { 1, "Hello", 3.14, "World" };
        #endregion // Fields
    }
}