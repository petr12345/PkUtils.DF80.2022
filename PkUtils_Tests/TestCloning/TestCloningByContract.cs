
using System;
using System.Runtime.Serialization;
using PK.PkUtils.Cloning.ByContract;
using PK.PkUtils.Interfaces;

namespace PK.TestCloning.ByContract
{
    #region Classes_tested_by_non_generic_copying

    [DataContract(IsReference = true)]
    internal class Foo
    {
        [DataMember]
        public Bar Bar { get; set; }
    }

    [DataContract(IsReference = true)]
    internal class Bar
    {
        [DataMember]
        public Foo Foo { get; set; }
    }
    #endregion // Classes_tested_by_non_generic_copying

    #region Classes_tested_by_generic_interface_support

    [DataContract(IsReference = true)]
    internal class FooEx : MakeCloneableByContact<FooEx>, IDeepCloneable<FooEx>
    {
        [DataMember]
        public BarEx BarEx { get; set; }
    }

    [DataContract(IsReference = true)]
    internal class BarEx
    {
        [DataMember]
        public FooEx FooEx { get; set; }
    }
    #endregion // Classes_tested_by_generic_interface_support

    /// <summary>
    /// Static class performing tests by deep copying the whole graph
    /// </summary>
    public static class TestCloningByContract
    {
        /// <summary>
        /// Simple test of non-generic copying, by calling of CloneHelperByContract extension method
        /// </summary>
        public static void Test1()
        {
            // test 1
            Console.WriteLine("----TestCloningByContract - TEST 1");
            Foo foo = new();
            Bar bar = new();
            foo.Bar = bar;
            bar.Foo = foo; // nice cyclic graph

            // The code line below has the same sense as: 
            // Foo clone = CloneHelperByContract.DeepClone(foo);
            //  or
            // Foo clone = CloneHelperByContract.DeepClone<Foo>(foo);

            Foo clone = foo.DeepClone();
            Console.WriteLine("Check Foo clone is not original: {0}", foo != clone); //true - new object
            Console.WriteLine("Check Foo copied graph: {0}", clone.Bar.Foo == clone); // true; copied graph
            Console.WriteLine("---- end of test");
        }

        /// <summary>
        /// Test copying the object through generic interface IDeepCloneable<typeparamref name="T"/>,
        /// which is implemented by inheriting from generics MakeCloneableByContact
        /// </summary>
        public static void Test2()
        {
            // test 2
            Console.WriteLine("----TestCloningByContract - TEST 2");
            FooEx fooEx = new();
            BarEx barEx = new();
            fooEx.BarEx = barEx;
            barEx.FooEx = fooEx; // nice cyclic graph

            FooEx cloneEx = fooEx.DeepClone();
            Console.WriteLine("Check FooEx clone is not original: {0}", fooEx != cloneEx); //true - new object
            Console.WriteLine("Check FooEx copied graph: {0}", cloneEx.BarEx.FooEx == cloneEx); // true; copied graph

            Console.WriteLine("---- end of test");
        }
    }
}
