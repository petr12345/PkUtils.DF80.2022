// Ignore Spelling: Utils, BSID
//
using PK.PkUtils.DataStructures;

#pragma warning disable VSSpell001

namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary> Unit Test of generic class Repository. </summary>
[TestFixture()]
public class RepositoryTests
{
    #region Tests_constructors

    [Test]
    public void Repository_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new Repository<string>(null!));
    }

    [Test]
    public void Repository_Constructor_02()
    {
        // Act
        Repository<string> rep = new();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rep.HasData, Is.False);
            Assert.That(rep.IsAttached, Is.False);
            Assert.That(rep.Data, Is.Null);
        });
    }

    [Test]
    public void Repository_Constructor_03()
    {
        // Arrange
        const string inputPalindrom = "jelenovipivonelej";

        // Act
        Repository<string> rep = new(inputPalindrom, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rep.HasData, Is.True);
            Assert.That(rep.IsAttached, Is.True);
            Assert.That(rep.Data, Is.EqualTo(inputPalindrom));
        });
    }

    [Test]
    public void Repository_Constructor_04()
    {
        // Arrange
        const string input = "不要倒鹿啤酒";

        // Act
        Repository<string> rep = new(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rep.HasData, Is.True);
            Assert.That(rep.IsAttached, Is.False);
            Assert.That(rep.Data, Is.EqualTo(input));
        });
    }

    [Test]
    public void Repository_Constructor_05()
    {
        // Act
        Repository<int> rep = new();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rep.HasData, Is.True);
            Assert.That(rep.IsAttached, Is.False);
            Assert.That(rep.Data, Is.Zero);
        });
    }

    #endregion // Tests_constructors

    #region Tests_Equals

    [Test]
    public void Repository_Equals_01()
    {
        // Act
        Repository<string> rep1 = new();
        Repository<string> rep2 = new();

        // Assert
        Assert.That(rep1, Is.EqualTo(rep2));
    }

    [Test]
    public void Repository_Equals_02()
    {
        // Arrange
        const string input = "荣耀于海盗政党";

        // Act
        Repository<string> rep1 = new(input);
        Repository<string> rep2 = new(input);
        Repository<string> rep3 = new(input, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rep1, Is.EqualTo(rep2));
            Assert.That(rep1, Is.Not.EqualTo(rep3));
            Assert.That(rep2, Is.Not.EqualTo(rep3));
        });
    }
    #endregion // Tests_Equals

    #region Tests_ToString

    [Test]
    public void Repository_ToString_01()
    {
        // Arrange
        const string inputPalindrom = "Báře jede jeřáb";

        // Act
        Repository<string> rep1 = new();
        Repository<string> rep2 = new(inputPalindrom);
        Repository<string> rep3 = new(inputPalindrom, false);

        // Act
        string s1 = rep1.ToString();
        string s2 = rep2.ToString();
        string s3 = rep3.ToString();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(!string.IsNullOrEmpty(s1), Is.True);
            Assert.That(!string.IsNullOrEmpty(s2), Is.True);
            Assert.That(!string.IsNullOrEmpty(s3), Is.True);
        });
    }
    #endregion // Tests_ToString
}
#pragma warning restore VSSpell001
