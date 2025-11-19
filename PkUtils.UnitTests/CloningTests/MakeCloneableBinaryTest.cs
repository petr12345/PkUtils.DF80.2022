// Ignore Spelling: Utils, Cloneable
// 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Interfaces;

#pragma warning disable IDE0079 // Remove unnecessary suppressions
#pragma warning disable CA1859  // Change type of variable 'cache' from ..
#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.UnitTests.CloningTests;

/// <summary>
/// This is a test class for CommandLineInfoEx and is intended
/// to contain all CommandLineInfoEx Unit Tests
///</summary>
[TestClass()]
public class MakeCloneableBinaryTest
{
    #region Auxiliary_types

    [Serializable]
    public class Animal : MakeCloneableBinary<Animal>, IDeepCloneable<Animal>
    {
        private readonly int _legs;
        public int Legs => _legs;

        public Animal(int legs)
        {
            _legs = (legs >= 0) ? legs : throw new ArgumentOutOfRangeException(
                nameof(legs), legs, $"Count of {nameof(legs)} can't be negative");
        }
        public Animal(Animal a) : this(a.Legs)
        { }
    }

    [Serializable]
    public class Baboon : Animal, IDeepCloneable<Baboon>
    {
        private readonly string _name;
        public string Name => _name;

        public Baboon(string name) : base(4)
        { _name = name; }

        public Baboon(Baboon b) : this(b.Name)
        { }

        Baboon IDeepCloneable<Baboon>.DeepClone()
        {
            return (this.DeepClone()! as Baboon)!;
        }
    }
    #endregion // Auxiliary_types

    #region Tests

    [TestMethod()]
    public void MakeCloneableBinary_BaseClassCopy()
    {
        Animal an01 = new(6);
        Animal an02 = an01.DeepClone();

        Assert.IsFalse(object.ReferenceEquals(an01, an02));
        Assert.AreEqual(an01.Legs, an02.Legs);
    }

    [TestMethod()]
    public void MakeCloneableBinary_DerivedClassCopy()
    {
        Animal an01 = new Baboon("Joe");
        Animal an02 = an01.DeepClone();

        Assert.IsFalse(object.ReferenceEquals(an01, an02));
        Assert.AreEqual(an01.Legs, an02.Legs);
        Assert.AreEqual("Joe", (an02 as Baboon)!.Name);
    }

    #endregion // Tests
}
#pragma warning restore IDE0290 // Use primary constructor
#pragma warning restore CA1859  // Change type of variable 'cache' from ..
#pragma warning restore IDE0079 // Remove unnecessary suppressions