// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;


/// <summary>   This is a test class for <see cref="TaskExtensions"/> </summary>
[TestFixture()]
public class TaskExtensionTest
{
    #region Tests

    /// <summary>
    /// A test for TaskExtensions.ExecuteSynchronously
    /// </summary>
    [Test()]
    public void ExecuteSynchronously_Test_01()
    {
        Task<int> t = null!;

        Assert.Throws<ArgumentNullException>(() => TaskEx.ExecuteSynchronously(t));
    }

    /// <summary>
    /// A test for TaskExtensions.ExecuteSynchronously
    /// </summary>
    [Test()]
    public void ExecuteSynchronously_Test_02()
    {
        static int FnGetInt()
        {
            throw new InvalidOperationException("April Fools' Day");
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            try
            {
                Task<int> testedTask = Task.Run(() => FnGetInt());
                int result = TaskEx.ExecuteSynchronously(testedTask);
            }
            catch (SystemException)
            {
                throw;
            }
        });
    }

#if NOTDEF
    // For some reason, this fails with 
    // System.IO.FileLoadException : 
    // Could not load file or assembly 'System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)


    /// <summary>  A test for TaskExtensions.ExecuteSynchronously. </summary>
    [Test()]
    public void ExecuteSynchronously_Test_03()
    {
        const int primeRange = 999999;
        Task<int> primeTask = Task.Run(
            () => Primes.GeneratePrimesInRange(primeRange).Last());
        int result = TaskEx.ExecuteSynchronously(primeTask);

        Trace.WriteLine(result.ToString());
    }
#endif
    #endregion // Tests
}
