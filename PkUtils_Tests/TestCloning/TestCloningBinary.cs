
using System;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Interfaces;

namespace PK.TestCloning
{
    #region Classes_tested_by_non_generic_copying

    [Serializable]
    internal class Animal
    {
        public string Name { get; set; }

        public Animal()
          : this(string.Empty)
        {
        }
        public Animal(string strName)
        {
            Name = strName;
        }
    }

    [Serializable]
    internal class Cat : Animal
    {
        public Dog Dog { get; set; }

        public Cat()
          : this(string.Empty)
        { }
        public Cat(string strName)
          : base(strName)
        { }
    }

    [Serializable]
    internal class Dog : Animal
    {
        public Cat Cat { get; set; }

        public Dog()
          : this(string.Empty)
        { }
        public Dog(string strName)
          : base(strName)
        { }
    }
    #endregion // Classes_tested_by_non_generic_copying

    #region Classes_tested_by_generic_interface_support

    [Serializable]
    internal class CatEx : MakeCloneableBinary<CatEx>, IDeepCloneable<CatEx>
    {
        public string Name { get; set; }
        public DogEx DogEx { get; set; }

        public CatEx()
          : this(string.Empty)
        {
        }
        public CatEx(string strName)
        {
            Name = strName;
        }
    }

    [Serializable]
    internal class DogEx
    {
        public string Name { get; set; }
        public CatEx CatEx { get; set; }

        public DogEx()
          : this(string.Empty)
        {
        }
        public DogEx(string strName)
        {
            Name = strName;
        }
    }
    #endregion // Classes_tested_by_generic_interface_support

    /// <summary>
    /// Static class performing tests by deep copying the whole graph
    /// </summary>
    public static class TestCloningBinary
    {
        /// <summary>
        /// Simple test of non-generic copying, by call of CloneHelperBinary extension method
        /// </summary>
        public static void Test1()
        {
            // test 1
            Console.WriteLine("----TestCloningBinary - TEST 1");
            Cat miaow = new("Šklíba");
            Dog bowwow = new("Azor");
            miaow.Dog = bowwow;
            bowwow.Cat = miaow; // nice cyclic graph

            // The code line below has the the same sense as: 
            // Cat clone = CloneHelperBinary.DeepClone(miaow);
            //   or
            // Cat clone = CloneHelperBinary.DeepClone<Cat>(miaow);

            Cat clone = miaow.DeepClone();
            Console.WriteLine("Check Cat '{0}' clone is not original: {1}", miaow.Name, miaow != clone); //true - new object
            Console.WriteLine("Check Cat '{0}' copied graph: {1}", clone.Name, clone.Dog.Cat == clone); // true; copied graph
            Console.WriteLine("---- end of test");
        }

        /// <summary>
        /// Test copying the object through generic interface IDeepCloneable<typeparamref name="T"/>,
        /// which is implemented by inheriting from generics MakeCloneableBinary
        /// </summary>
        public static void Test2()
        {
            // test 2
            Console.WriteLine("----TestCloningBinary - TEST 2");
            CatEx miaowEx = new("Tlapka");
            DogEx bowwowEx = new("Lesan");
            miaowEx.DogEx = bowwowEx;
            bowwowEx.CatEx = miaowEx; // nice cyclic graph

            CatEx cloneEx = miaowEx.DeepClone();
            Console.WriteLine("Check CatEx '{0}' clone is not original: {1}", miaowEx.Name, miaowEx != cloneEx); //true - new object
            Console.WriteLine("Check CatEx '{0}' copied graph: {1}", cloneEx.Name, cloneEx.DogEx.CatEx == cloneEx); // true; copied graph

            Console.WriteLine("---- end of test");
        }
    }
}
