using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;

namespace PK.Testmemcmp
{
    /// <summary>
    /// The class actually doing the test
    /// </summary>
    public static class ComparisonTest
    {
        #region Typedefs

        public enum TestMode
        {
            Test_memcmp = 0,
            Test_SequenceEquals = 1,
        }
        #endregion // Typedefs

        #region Fields
        public const int LOOP_COUNT = 100000;
        #endregion // Fields

        #region Methods

        public static void DoTest(TestMode mode, IDumper dumper, int parallelThreadsCount, int arrayLength)
        {
            dumper.CheckArgNotNull(nameof(dumper));
            if (parallelThreadsCount <= 0) throw new ArgumentException("value must be positive", nameof(parallelThreadsCount));
            if (arrayLength <= 0) throw new ArgumentException("value must be positive", nameof(arrayLength));

            if (!Enum.IsDefined(typeof(TestMode), mode))
            {
                string strErr = string.Format(CultureInfo.InvariantCulture, "Invalid value of mode = '{0}'", mode);
                throw new ArgumentException(strErr, nameof(mode));
            }
            else
            {
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture,
                  "---- Testing '{0}', nArrayLength = {1}, nParallelThreadsCount = {2} ----",
                  mode, Conversions.IntegerToReadable(arrayLength), parallelThreadsCount));
            }

            ThreadStart ts = null;

            switch (mode)
            {
                case TestMode.Test_memcmp:
                    {
                        ts = new ThreadStart(delegate
                        {
                            byte[] arrA = GenerateArray(arrayLength);
                            byte[] arrB = arrA.DeepClone();

                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                bool bEqual = MemUtils.memcmp(arrA, arrB);
                                if (!bEqual)
                                {
                                    dumper.DumpLine(string.Format(CultureInfo.InvariantCulture,
                          "Comparison has FAILED returning FALSE, nArrayLength =  '{0}'", arrayLength));
                                    break;
                                }
                            }
                        });
                    }
                    break;

                case TestMode.Test_SequenceEquals:
                    {
                        ts = new ThreadStart(delegate
                        {
                            byte[] arrA = GenerateArray(arrayLength);
                            byte[] arrB = arrA.DeepClone();

                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                // Note: here I could not use Array.Equals(arrA, arrB);  - simply returns false 
                                // For more info see for instance http://www.pcreview.co.uk/forums/array-equality-t1235114.html
                                bool bEqual = arrA.SequenceEqual(arrB);
                                if (!bEqual)
                                {
                                    dumper.DumpLine(string.Format(CultureInfo.InvariantCulture,
                          "Comparison has FAILED returning FALSE, nArrayLength =  '{0}'", arrayLength));
                                    break;
                                }
                            }
                        });
                    }
                    break;
            }

            Thread[] threads = new Thread[parallelThreadsCount];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(ts);
            }
            Stopwatch stopWatch = new();
            stopWatch.Start();
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }
            stopWatch.Stop();
            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "time passed: {0}(ms)",
              Conversions.LongToReadable(stopWatch.ElapsedMilliseconds)));
        }

        private static byte[] GenerateArray(int nLength)
        {
            if (nLength <= 0) throw new ArgumentException("value must be positive");
            byte[] arr = new byte[nLength];

            for (int ii = 0; ii < nLength; ii++)
            {
                arr[ii] = (byte)(((uint)ii) % 256);
            }

            return arr;
        }

        #endregion // Methods
    }
}
