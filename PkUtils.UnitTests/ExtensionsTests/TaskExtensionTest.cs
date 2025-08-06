﻿// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;
using PK.PkUtils.MathEx;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{

    /// <summary>
    /// This is a test class for TaskExtensions
    /// </summary>
    [TestClass()]
    public class TaskExtensionTest
    {
        #region Tests

        /// <summary>
        /// A test for TaskExtensions.ExecuteSynchronously
        /// </summary>
        [TestMethod()]
        public void ExecuteSynchronously_Test_01()
        {
            Task<int> t = null!;

            Assert.ThrowsExactly<ArgumentNullException>(() => TaskEx.ExecuteSynchronously(t));
        }

        /// <summary>
        /// A test for TaskExtensions.ExecuteSynchronously
        /// </summary>
        [TestMethod()]
        public void ExecuteSynchronously_Test_02()
        {
            static int FnGetInt()
            {
                throw new InvalidOperationException("April Fools' Day");
            }

            Task<int> testedTask = Task.Run(() => FnGetInt());
            Assert.ThrowsExactly<InvalidOperationException>(() => TaskEx.ExecuteSynchronously(testedTask));
        }

        /// <summary>
        /// A test for TaskExtensions.ExecuteSynchronously
        /// </summary>
        [TestMethod()]
        public void ExecuteSynchronously_Test_03()
        {
            const int primeRange = 999999;
            Task<int> primeTask = Task.Run(
                () => Primes.GeneratePrimesInRange(primeRange).Last());
            int result = TaskEx.ExecuteSynchronously(primeTask);

            Trace.WriteLine(result.ToString());
        }
        #endregion // Tests
    }
}
