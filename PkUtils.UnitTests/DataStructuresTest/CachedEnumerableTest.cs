// Ignore Spelling: Utils

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UnitTests.DataStructuresTest
{
    /// <summary>
    /// This is a test class for CachedEnumerable generic
    ///</summary>
    [TestClass()]
    public class CachedEnumerableTest
    {
        #region Tests

        #region Tests_Constructor

        /// <summary> A test for CachedEnumerable constructor. </summary>
        [TestMethod()]
        public void CachedEnumerable_Constructor_01()
        {
            var enumerab = new CachedEnumerable<int>(null);
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
        }
        /// <summary> A test for CachedEnumerable parsing, which should throw InvalidOperationException. </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_01()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                CachedEnumerable<int> enumerab = new(null);
                bool bTemp = enumerab.Any();
            });
        }

        /// <summary> A test for CachedEnumerable parsing, which should throw ObjectDisposedException. </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_02()
        {
            string[] arrInput = Enumerable.Range(0, 3).Select(i => i.ToString()).ToArray();
            CachedEnumerable<string> enumerab = new(arrInput);

            Assert.AreEqual(true, enumerab.Any());
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);

            enumerab.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() =>
            {
                bool bAny = enumerab.Any();
            });
        }

        /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_03()
        {
            string[] arrInput = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            CachedEnumerable<string> enumerab = new(arrInput);

            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            Assert.AreEqual(true, enumerab.Any());
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
        }

        /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_04()
        {
            CachedEnumerable<string> enumerab = new(Enumerable.Empty<string>());

            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            Assert.IsFalse(enumerab.Any());
            Assert.AreEqual(ParseStatus.ParsedOk, enumerab.Status);
        }

        /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_05()
        {
            int[] arrInput = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            CachedEnumerable<int> enumerab = new(arrInput);

            IEnumerable<int> output1 = enumerab.Take(arrInput.Length - 2);
            IEnumerable<int> output2 = enumerab.Take(arrInput.Length);
            IEnumerable<int> output3 = enumerab.Take(arrInput.Length + 2);

            Assert.AreEqual(arrInput.Length - 2, output1.Count());
            Assert.AreEqual(arrInput.Length - 2, enumerab.CachedItemsCount);
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);

            Assert.AreEqual(arrInput.Length, output2.Count());
            Assert.AreEqual(arrInput.Length, enumerab.CachedItemsCount);
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);

            Assert.AreEqual(arrInput.Length, output3.Count());
            Assert.AreEqual(arrInput.Length, enumerab.CachedItemsCount);
            Assert.AreEqual(ParseStatus.ParsedOk, enumerab.Status);

            enumerab.ResetCache();
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
        }

        /// <summary>
        /// A test for CachedEnumerable parsing, which should throw ObjectDisposedException,
        /// since CachedEnumerable object has been disposed.
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_06()
        {
            int[] arrInput = Enumerable.Range(0, 10).ToArray();
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            enumerab.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() =>
            {
                int nVal = en.Current;
            });
        }

        /// <summary>
        /// A test for CachedEnumerable parsing, which should throw InvalidOperationException,
        /// since CachedEnumerable have not started parsing yet
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_07()
        {
            int[] arrInput = Enumerable.Range(0, 10).ToArray();
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                int nVal = en.Current;
            });
        }

        /// <summary>
        /// A test for CachedEnumerable parsing, which should throw InvalidOperationException,
        /// since the second enumerator did not call MoveNext
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parsing_08()
        {
            int[] arrInput = Enumerable.Range(0, 10).ToArray();
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> enA = enumerab.GetPeekAbleEnumerator();
            IPeekAbleEnumerator<int> enB = enumerab.GetPeekAbleEnumerator();

            int dummyA = enA.Peek;
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                int dummyB = enB.Current;
            });
        }
        #endregion // Tests_Parsing

        #region Tests_ResetCache

        /// <summary> A test for CachedEnumerable parsing involving Cache Reset, which should succeed. </summary>
        [TestMethod()]
        public void CachedEnumerable_Reset_01()
        {
            int[] arrInput = { 2, 4, 6, 8, 10 };
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> en;

            // Attempt parsing and ResetCache three times. That amount has nothing to do with size of arrInput
            for (int ii = 0; ii < 3; ii++)
            {
                Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
                en = enumerab.GetPeekAbleEnumerator();
                Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);

                for (int jj = 0; jj < arrInput.Length; jj++)
                {
                    Assert.IsTrue(en.CanPeek);
                    Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
                    Assert.AreEqual(arrInput[jj], en.Peek);
                    Assert.IsTrue(en.MoveNext());
                }
                enumerab.ResetCache();
            }
        }

        /// <summary>
        /// A test for Cache Reset, which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Reset_02()
        {
            int[] arrInput = { 2, 4, 6, 8, 10 };
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> en_CurrentGeneration = null!;

            for (int ii = 0; ii < 2; ii++)
            {
                // i/ enumerators initialization
                Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);

                IPeekAbleEnumerator<int> en_PrevGeneration = en_CurrentGeneration;
                en_CurrentGeneration = enumerab.GetPeekAbleEnumerator();

                // ii/ if there was previous enumerator, test that Current and Peek throws ArgumentException
                if (en_PrevGeneration != null)
                {
                    int dDummyVal;
                    bool bCaughtOnCurrent = false;
                    bool bCaughtOnPeek = false;

                    try
                    {
                        dDummyVal = en_PrevGeneration.Current;
                    }
                    catch (ArgumentException)
                    {
                        bCaughtOnCurrent = true;
                    }
                    Assert.IsTrue(bCaughtOnCurrent);

                    try
                    {
                        dDummyVal = en_PrevGeneration.Peek;
                    }
                    catch (Exception)
                    {
                        bCaughtOnPeek = true;
                    }
                    Assert.IsTrue(bCaughtOnPeek);
                }

                // iv/ test functionality of current enumerator
                Assert.AreEqual(true, en_CurrentGeneration.CanPeek);
                Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
                Assert.AreEqual(arrInput[0], en_CurrentGeneration.Peek);

                // iv/ reset cache, invalidating all enumerators created so far
                enumerab.ResetCache();
            }
        }
        /// <summary>
        /// A test for CachedEnumerable.FillBuffer, which should fail
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_FillBuffer_01()
        {
            string[] arrInput = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            CachedEnumerable<string> enumerab = new(arrInput);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                enumerab.FillBuffer(-2);
            });
        }

        /// <summary>
        /// A test for CachedEnumerable.FillBuffer, which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_FillBuffer_02()
        {
            string[] arrInput = Enumerable.Range(-1, 10).Select(i => i.ToString()).ToArray();
            CachedEnumerable<string> enumerab = new(arrInput);

            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            for (int ii = 1; ii < arrInput.Length; ii++)
            {
                Assert.AreEqual(ii, enumerab.FillBuffer(ii));
                for (int jj = 1; jj <= ii; jj++)
                {
                    Assert.AreEqual(ii, enumerab.FillBuffer(jj));
                }
            }

            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
        }
        #endregion // Tests_FillBuffer

        #region Tests_ResumeParsing

        /// <summary>
        /// A test for CachedEnumerable.ResumeParsing(), which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_ResumeParsing_01()
        {
            int[] arrInput = { 0, 1, 2, 3 };
            CachedEnumerable<int> enumerab = new(arrInput);

            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
        }

        /// <summary>
        /// A test for CachedEnumerable.ResumeParsing(), which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_ResumeParsing_02()
        {
            string[] arrInput = Enumerable.Range(0, 9).Select(i => i.ToString()).ToArray();
            CachedEnumerable<string> enumerab = new(arrInput);
            IPeekAbleEnumerator<string> en = enumerab.GetPeekAbleEnumerator();

            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);

            Assert.AreEqual(true, en.CanPeek);
            Assert.AreEqual("0", en.Peek);
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);

            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
        }

        /// <summary> Auxiliary Class, used in CachedEnumerable_ResumeParsing_03. </summary>
        internal class AuxIntCachedEnumerable<T> : CachedEnumerable<T>
        {
            public AuxIntCachedEnumerable(IEnumerable<T> dataSource)
              : this(dataSource, int.MaxValue)
            {
            }

            public AuxIntCachedEnumerable(IEnumerable<T> dataSource, int maxBufferSize)
              : base(dataSource)
            {
                this.MaxBufferSize = maxBufferSize;
            }

            public int MaxBufferSize { get; set; }

            protected override bool PrematureParseEndCriteria()
            {
                bool bRes;

                if (this.CachedItemsCount >= MaxBufferSize)
                    bRes = true;
                else
                    bRes = base.PrematureParseEndCriteria();

                return bRes;
            }
        }

        /// <summary>
        /// A test for CachedEnumerable.ResumeParsing(), which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_ResumeParsing_03()
        {
            int[] arrInput = { 0, 1, 2, 3, 4 };
            const int nFirstSmallMaxSize = 3;
            AuxIntCachedEnumerable<int> enumerab = new(arrInput, nFirstSmallMaxSize);

            // i/ play with status ParseStatus.ParseNotInitialized
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);
            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.ParseNotInitialized, enumerab.Status);

            // ii/ play with status ParseStatus.Parsing
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            Assert.AreEqual(true, en.CanPeek);
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);

            // iii/ play with status ParseStatus.ParsePrematureEnd, when max buffer size is still small
            enumerab.FillBuffer(999);
            Assert.AreEqual(nFirstSmallMaxSize, enumerab.CachedItemsCount);
            Assert.AreEqual(ParseStatus.ParsePrematureEnd, enumerab.Status);
            Assert.AreEqual(false, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.ParsePrematureEnd, enumerab.Status);

            // iv/ play with status ParseStatus.ParsePrematureEnd, when max buffer size is enlarged
            enumerab.MaxBufferSize = int.MaxValue;
            Assert.AreEqual(true, enumerab.ResumeParsing());
            Assert.AreEqual(ParseStatus.Parsing, enumerab.Status);
            Assert.AreEqual(arrInput.Length, enumerab.FillBuffer());
            Assert.AreEqual(ParseStatus.ParsedOk, enumerab.Status);
        }
        #endregion // Tests_ResumeParsing

        #region Tests_PeekAbleEnumerator

        /// <summary>  A test for GetPeekAbleEnumerator(), which should succeed. </summary>
        [TestMethod()]
        public void CachedEnumerable_PeekAbleEnumerator_01()
        {
            int[] arrInput = { 1, 2, 3 };
            CachedEnumerable<int> enumerab = new(arrInput);
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();

            for (int ii = 0; ii < arrInput.Length;)
            {
                Assert.AreEqual(true, en.CanPeek);
                Assert.AreEqual(++ii, en.Peek);
                Assert.AreEqual(true, en.MoveNext());
                Assert.AreEqual(ii, en.Current);
            }

            Assert.AreEqual(false, en.CanPeek);
            Assert.AreEqual(false, en.MoveNext());
        }
        /// <summary>
        /// A single-thread test for CachedEnumerable usage, which should throw ObjectDisposedException,
        /// since CachedEnumerable object has been disposed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Disposing_01()
        {
            CachedEnumerable<int> enumerab = new(Enumerable.Range(0, 10));
            enumerab.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() =>
            {
                IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            });
        }

        /// <summary>
        /// A single-thread test for CachedEnumerable usage, which should throw ObjectDisposedException,
        /// since CachedEnumerable object has been disposed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Disposing_02()
        {
            CachedEnumerable<int> enumerab = new(Enumerable.Range(0, 10));
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            enumerab.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() =>
            {
                int nVal = en.Current;
            });
        }

        /// <summary>
        /// A single-thread test for CachedEnumerable Disposing, called several times, which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Disposing_03()
        {
            CachedEnumerable<int> enumerab = new(Enumerable.Range(0, 10));
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
            enumerab.Dispose();
            enumerab.Dispose();
        }
        #endregion // Tests_Diposing

        #region Tests_Parallel

        /// <summary>
        /// A multiple-thread test for CachedEnumerable Disposing, which should succeed.
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parallel_Disposing_01()
        {
            const int nMaxThreads = 12;
            var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };
            CachedEnumerable<int> enumerab = new(Enumerable.Range(0, 10));

            Parallel.For(0, nMaxThreads, options, i =>
            {
                enumerab.Dispose();
            });
        }

        /// <summary>
        /// A multiple-thread test for CachedEnumerable Disposing, which should succeed.
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parallel_Disposing_02()
        {
            const int nMaxThreads = 12;
            var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };
            CachedEnumerable<int> enumerab = new(Enumerable.Range(0, 10));

            try
            {
                Parallel.For(0, nMaxThreads, options, i =>
                {
                    if (i == 7)
                    {
                        if (enumerab.CachedItemsCount == 0)
                            enumerab.Dispose();
                    }
                    else
                    {
                        while (true)
                        {
                            enumerab.ResetCache();
                            System.Threading.Thread.Sleep(1);
                        }
                    }
                });
            }
            catch (AggregateException caught)
            {
                foreach (var ex in caught.InnerExceptions)
                {
                    Assert.AreEqual(typeof(ObjectDisposedException), ex.GetType());
                }
            }
        }

        /// <summary>
        /// A multiple-thread test for GetPeekAbleEnumerator(), which should succeed
        /// </summary>
        [TestMethod()]
        public void CachedEnumerable_Parallel_PeekAbleEnumerator_01()
        {
            const int nMaxThreads = 12;
            var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };

            const int nInputLength = 128;
            var arrInput = Enumerable.Range(1, nInputLength);
            CachedEnumerable<int> enumerab = new(arrInput);

            Parallel.For(0, nMaxThreads, options, i =>
            {
                IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();

                for (int ii = 0; ii < nInputLength;)
                {
                    Assert.AreEqual(true, en.CanPeek);
                    Assert.AreEqual(++ii, en.Peek);
                    Assert.AreEqual(true, en.MoveNext());
                    Assert.AreEqual(ii, en.Current);
                }

                Assert.AreEqual(false, en.CanPeek);
                Assert.AreEqual(false, en.MoveNext());
            });
        }
        #endregion // Tests_Parallel

        #region Tests_Covariance

        /// <summary> Auxiliary class shape. </summary>
        internal class MyShape
        {
            protected int width;
            protected int height;

            public MyShape(int aWidth, int aHeight)
            {
                width = aWidth;
                height = aHeight;
            }
            public int Width
            {
                get { return width; }
            }
            public int Height
            {
                get { return height; }
            }
        }

        /// <summary> Auxiliary class rectangle. </summary>
        internal class MyRectangle : MyShape
        {
            public MyRectangle(int dx) : base(dx, dx)
            {
            }
            public int GetArea()
            {
                return (width * height);
            }
        }

        /// <summary> A test for covariance of IPeekAbleEnumerator ( compilation of assignment ). </summary>
        [TestMethod()]
        public void CachedEnumerable_InterfaceCovariance_01()
        {
            MyRectangle[] arrInput = { new(5), new(6), new(7) };
            CachedEnumerable<MyRectangle> enData = new(arrInput);

            // enumerator with more derived type
            IPeekAbleEnumerator<MyRectangle> enRects = enData.GetPeekAbleEnumerator();
            // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
            IPeekAbleEnumerator<MyShape> enShapes = enRects;
        }

        /// <summary> A test for covariance of IPeekAbleEnumerable ( compilation of assignment ). </summary>
        [TestMethod()]
        public void CachedEnumerable_InterfaceCovariance_02()
        {
            MyRectangle[] arrInput = { new(5), new(6), new(7) };

            // enumerable with more derived type
            IPeekAbleEnumerable<MyRectangle> enDataRectangles = new CachedEnumerable<MyRectangle>(arrInput);
            // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
            IPeekAbleEnumerable<MyShape> enDatShapes = enDataRectangles;
        }

        /// <summary> A test for covariance of ICachedEnumerable ( compilation of assignment ). </summary>
        [TestMethod()]
        public void CachedEnumerable_InterfaceCovariance_03()
        {
            MyRectangle[] arrInput = { new(5), new(6), new(7) };

            // enumerable with more derived type
            ICachedEnumerable<MyRectangle> enDataRectangles = new CachedEnumerable<MyRectangle>(arrInput);
            // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
            ICachedEnumerable<MyShape> enDatShapes = enDataRectangles;
        }
        #endregion // Tests_Covariance
        #endregion // Tests
    }
}
