// Ignore Spelling: PkUtils, Utils, Impl
// 
using PK.PkUtils.MathEx;

namespace PK.PkUtils.NUnitTests.MathExTests;

/// <summary> This is a unit test class for class <see cref="Primes"/> </summary>
[TestFixture()]
[CLSCompliant(false)]
public class PrimesTests
{
    #region Tests
    #region TestsGeneratePrimesInRange_SingleArgument

    [Test, Description("Tests exception handling for non-positive inputs")]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    public void GeneratePrimesInRange_InvalidInput_ThrowsArgumentOutOfRangeException(int maxValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Primes.GeneratePrimesInRange(maxValue));
    }

    [Test, Description("Tests generating primes within a valid positive range")]
    [TestCase(1, new int[] { })]
    [TestCase(2, new int[] { 2 })]
    [TestCase(10, new int[] { 2, 3, 5, 7 })]
    [TestCase(20, new int[] { 2, 3, 5, 7, 11, 13, 17, 19 })]
    public void GeneratePrimesInRange_ValidInput_ReturnsCorrectPrimes(int maxValue, IEnumerable<int> expectedPrimes)
    {
        // Act
        IEnumerable<int> result = Primes.GeneratePrimesInRange(maxValue);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPrimes));
    }

    [Test, Description("Tests if the last prime in the range matches the expected large prime")]
    [TestCase(100, 97)]
    [TestCase(1000, 997)]
    //    [TestCase(5000, 4999)]
    //    [TestCase(10000, 9973)]
    public void GeneratePrimesInRange_LastPrimeMatchesExpected(int maxValue, int expectedLastPrime)
    {
        // Act
        IEnumerable<int> result = Primes.GeneratePrimesInRange(maxValue);

        // Assert
        Assert.That(result.Last(), Is.EqualTo(expectedLastPrime));
    }
    #endregion // TestsGeneratePrimesInRange_SingleArgument

    #region TestsGeneratePrimesInRange_TwoArguments

    [Test, Description("Tests exception handling for non-positive minValue or maxValue")]
    [TestCase(0, 10)]
    [TestCase(2, 0)]
    [TestCase(-1, 10)]
    [TestCase(2, -1)]
    public void GeneratePrimesInRange_InvalidInput_ThrowsArgumentOutOfRangeException(int minValue, int maxValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Primes.GeneratePrimesInRange(minValue, maxValue));
    }

    [Test, Description("Tests exception handling for minValue greater than maxValue")]
    [TestCase(10, 5)]
    [TestCase(20, 10)]
    [TestCase(100, 50)]
    public void GeneratePrimesInRange_MinValueGreaterThanMaxValue_ThrowsArgumentException(int minValue, int maxValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Primes.GeneratePrimesInRange(minValue, maxValue));
    }

    [Test, Description("Tests generating primes within a valid range of two arguments")]
    [TestCase(1, 10, new int[] { 2, 3, 5, 7 })]
    [TestCase(2, 20, new int[] { 2, 3, 5, 7, 11, 13, 17, 19 })]
    [TestCase(5, 20, new int[] { 5, 7, 11, 13, 17, 19 })]
    public void GeneratePrimesInRange_ValidInput_ReturnsCorrectPrimes(int minValue, int maxValue, IEnumerable<int> expectedPrimes)
    {
        // Act
        IEnumerable<int> result = Primes.GeneratePrimesInRange(minValue, maxValue);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPrimes));
    }

    [Test, Description("Tests if the last prime in the range matches the expected large prime")]
    [TestCase(66, 100, 97)]
    [TestCase(50, 1000, 997)]
    public void GeneratePrimesInRange_LastPrimeMatchesExpected(int minValue, int maxValue, int expectedLastPrime)
    {
        // Act
        IEnumerable<int> result = Primes.GeneratePrimesInRange(minValue, maxValue);

        // Assert
        Assert.That(result.Last(), Is.EqualTo(expectedLastPrime));
    }
    #endregion // TestsGeneratePrimesInRange_TwoArguments
    #endregion // Tests
}
