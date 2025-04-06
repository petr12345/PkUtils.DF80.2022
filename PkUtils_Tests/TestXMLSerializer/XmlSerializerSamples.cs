///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSDN license agreement notice
// 
// This software is a Derivative Work based upon a June 2003 MSDN Magazine article
// "Advanced Type Mappings", by Aaron Skonnard
// http://msdn.microsoft.com/en-us/magazine/cc164135.aspx
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Define the symbol OLD_MSDN_MAGAZINE_CODE, if you want to execute the original code
// http://msdn.microsoft.com/en-us/magazine/cc164135.aspx
// Otherwise, following code code involves the functionality of XMLSerializerAdapter.
//
// #define OLD_MSDN_MAGAZINE_CODE

using System;
using System.IO;
using System.Text;
using System.Globalization;

#if !OLD_MSDN_MAGAZINE_CODE
using PK.PkUtils.XmlSerialization;
using PK.PkUtils.Utils;
#endif

/// <summary>
/// Performs serialization tests
/// </summary>
internal class XmlSerializerSamples
{
    #region Fields
    protected internal static readonly string _strPaymentFileFullName;
    protected internal static readonly string _strEmployeeFileFullName;
    protected const string __strPaymentFileFullNameName = "payment.xml";
    protected const string __strEmployeeFileFullNameName = "employee.xml";
    #endregion // Fields

    #region Constructor(s)
    static XmlSerializerSamples()
    {
        FileInfo fi_payment = new(__strPaymentFileFullNameName);
        FileInfo fi_employee = new(__strEmployeeFileFullNameName);

        _strPaymentFileFullName = fi_payment.FullName;
        _strEmployeeFileFullName = fi_employee.FullName;
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Tests the serialization
    /// </summary>
    private static void Serialize()
    {
        // Serialize PaymentMethod
        PaymentMethod p = new()
        {
            Item = 7.5,
            ItemElementName = ItemChoiceType.hourly
        };

#if OLD_MSDN_MAGAZINE_CODE
    XmlSerializer s = new XmlSerializer(typeof(PaymentMethod));
    FileStream fs = new FileStream(_strPaymentFileFullName, FileMode.Create);
    s.Serialize(fs, p);
    fs.Close();
#else
        XMLSerializerAdapter<PaymentMethod> pSerz = new();
        pSerz.WriteXmlFile(_strPaymentFileFullName, Encoding.UTF8, p);
#endif // OLD_MSDN_MAGAZINE_CODE
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "The PaymentMethod is stored in '{0}'", _strPaymentFileFullName));

        // Serialize Employee
        Employee e = new()
        {
            id = "333-33-3333",
            name = "Bob Smith",
            Text = new string[] { "here is some mixed text...", "here is some more..." }
        };

#if OLD_MSDN_MAGAZINE_CODE
    XmlSerializer s2 = new XmlSerializer(typeof(Employee));
    FileStream fs2 = new FileStream(_strEmployeeFileFullName, FileMode.Create);
    s2.Serialize(fs2, e);
    fs2.Close();
#else
        XMLSerializerAdapter<Employee> eSerz = new();
        eSerz.WriteXmlFile(_strEmployeeFileFullName, Encoding.UTF8, e);
#endif // OLD_MSDN_MAGAZINE_CODE
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "The Employee is stored in '{0}'", _strEmployeeFileFullName));
    }

    /// <summary>
    /// Tests deserialization
    /// </summary>
    private static void Deserialize()
    {
#if OLD_MSDN_MAGAZINE_CODE
    XmlSerializer s = new XmlSerializer(typeof(PaymentMethod));
    FileStream fs = new FileStream(_strPaymentFileFullName, FileMode.Open);
    PaymentMethod p = (PaymentMethod)s.Deserialize(fs);
    fs.Close();
#else
        XMLSerializerAdapter<PaymentMethod> pSerz = new();
        PaymentMethod p = pSerz.ReadFile(_strPaymentFileFullName);
#endif // OLD_MSDN_MAGAZINE_CODE

        Console.WriteLine("{0}: {1}", p.ItemElementName, p.Item);

#if OLD_MSDN_MAGAZINE_CODE
    XmlSerializer s2 = new XmlSerializer(typeof(Employee));
    FileStream fs2 = new FileStream(_strEmployeeFileFullName, FileMode.Open);
    Employee e = (Employee)s2.Deserialize(fs2);
    fs2.Close();
#else
        XMLSerializerAdapter<Employee> eSerz = new();
        Employee e = eSerz.ReadFile(_strEmployeeFileFullName);
#endif // OLD_MSDN_MAGAZINE_CODE
        Console.WriteLine("id: {0}, name: {1}", e.id, e.name);
        foreach (string t in e.Text)
        {
            Console.WriteLine(t);
        }
    }

    /// <summary>
    /// If you run the application with no argument or the other argument than -d,
    /// the code will serialize some stuff to files "payment.xml" and "employee.xml".
    /// 
    /// If you run the application with the argument -d, 
    /// the code will deserialize the previously serialized files "payment.xml" and "employee.xml".
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        CommandLineInfoEx cmdinfo = new(args);

        if (cmdinfo.GetSwitch("d"))
        {
            Deserialize();
        }
        else
        {
            Serialize();
        }
    }
    #endregion // Methods
}
