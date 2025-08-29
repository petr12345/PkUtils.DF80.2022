// Ignore Spelling: PkUtils, Utils, Cloneable
// 
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.NUnitTests.UtilsTests;

/// <summary>
/// This is a test class for CommandLineInfoEx and is intended
/// to contain all CommandLineInfoEx Unit Tests
///</summary>
[TestFixture]
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
            return (this.DeepClone() as Baboon)!;
        }
    }
    #endregion // Auxiliary_types

    #region Tests

    [Test]
    public void MakeCloneableBinary_BaseClassCopy()
    {
        Animal an01 = new Animal(6);
        Animal an02 = an01.DeepClone();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(an01, Is.Not.SameAs(an02));
            Assert.That(an01.Legs, Is.EqualTo(an02.Legs));
        }
    }

    [Test]
    public void MakeCloneableBinary_DerivedClassCopy()
    {
        Animal an01 = new Baboon("Joe");
        Animal an02 = an01.DeepClone();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(an01, Is.Not.SameAs(an02));
            Assert.That(an01.Legs, Is.EqualTo(an02.Legs));
            Assert.That((an02 as Baboon)?.Name, Is.EqualTo("Joe"));
        }
    }

    #endregion // Tests
}
