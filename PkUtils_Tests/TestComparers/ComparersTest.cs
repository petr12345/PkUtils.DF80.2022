using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using PK.PkUtils.Comparers;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.TestComparers
{
    /// <summary>
    /// The class actually doing the test
    /// </summary>
    public static class ComparersTest
    {
        #region Typedefs
        public enum TestMode
        {
            TestFuncEqualityComparer = 0,
            TestKeyEqualityComparer = 1,
        }
        #endregion // Typedefs

        #region Methods

        #region Private Methods

        private static string Dump2Text(object obj)
        {
            return ObjectDumper.Dump2Text(obj, ", ", " ");
        }

        private static string ComparerDectiption(ComparersTest.TestMode testMode)
        {
            string strRes = string.Empty;
            switch (testMode)
            {
                case TestMode.TestFuncEqualityComparer:
                    strRes = "FunctionalEqualityComparer";
                    break;
                case TestMode.TestKeyEqualityComparer:
                    strRes = "KeyEqualityComparer";
                    break;
            }
            return strRes;
        }
        #endregion // Private Methods

        #region Public Methods

        public static void TestCaseInsensitiveDistinct(ComparersTest.TestMode testMode, IDumper dumper)
        {
            IEnumerable<string> foo = ["abc", "de", "Abc", "DE", "aBc", "abC",];
            IEnumerable<string> distinct1 = null;
            IEnumerable<string> distinct2 = null;

            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "---- Testing case-insensitive distinct by {0}...",
              ComparerDectiption(testMode)));
            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "Input: {0}", Dump2Text(foo)));

            if (testMode == TestMode.TestFuncEqualityComparer)
            {
                Func<string, string, bool> fComparer = (x, y) => x.ToLower() == y.ToLower();
                Func<string, int> fHash = (x) => x.ToLower().GetHashCode();

                distinct1 = foo.Distinct<string>(new FunctionalEqualityComparer<string>(fComparer)).ToList();
                distinct2 = foo.Distinct<string>(new FunctionalEqualityComparer<string>(fComparer, fHash)).ToList();
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "Output 1: {0}", Dump2Text(distinct1)));
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "Output 2: {0}", Dump2Text(distinct2)));
                if (distinct1.Count() > distinct2.Count())
                {
                    dumper.DumpLine("The first output is invalid, since the first comparer does not have proper GetHashCode");
                }
            }
            else if (testMode == TestMode.TestKeyEqualityComparer)
            {
                // first approach uses directly the KeyEqualityComparer
                distinct1 = foo.Distinct<string>(new KeyEqualityComparer<string, string>(x => x.ToLower()));
                // second approach uses the extension method of UsageKeyEqualityComparer
                distinct2 = foo.Distinct<string, string>(x => x.ToLower());
                // Func<string, string> keyExtractor = x => x.ToLower();
                // var distinct3 = foo.Distinct<string, string>(keyExtractor);
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "Output 1 (directly using KeyEqualityComparer): {0}", Dump2Text(distinct1)));
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "Output 2 (involving UsageKeyEqualityComparer): {0}", Dump2Text(distinct2)));
                dumper.DumpLine(distinct1.SequenceEqual<string>(distinct2) ? "Test succeeded ..." : "Test failed !!");
            }
            else
            {
                Debug.Assert(false);
            }

            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "---- end of test"));
        }

        public static void DoTest(ComparersTest.TestMode testMode, IDumper dumper)
        {
            dumper.CheckArgNotNull(nameof(dumper));
            TestCaseInsensitiveDistinct(testMode, dumper);
        }
        #endregion // Public Methods
        #endregion // Methods
    }
}
