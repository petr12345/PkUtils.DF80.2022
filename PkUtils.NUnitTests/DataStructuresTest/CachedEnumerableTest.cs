// Ignore Spelling: Utils
//
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary>
/// This is a test class for CachedEnumerable generic
///</summary>
[TestFixture()]
public class CachedEnumerableTest
{
    #region Tests

    #region Tests_Constructor

    /// <summary> A test for CachedEnumerable constructor. </summary>
    [Test]
    public void CachedEnumerable_Constructor_01()
    {
        var enumerab = new CachedEnumerable<int>(null);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
    }
    #endregion // Tests_Constructor

    #region Tests_Parsing

    /// <summary> A test for CachedEnumerable parsing, which should throw InvalidOperationException. </summary>
    [Test]
    public void CachedEnumerable_Parsing_01()
    {
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(null);

        Assert.Throws<InvalidOperationException>(() => enumerab.Any());
    }

    /// <summary> A test for CachedEnumerable parsing, which should throw ObjectDisposedException. </summary>
    [Test]
    public void CachedEnumerable_Parsing_02()
    {
        string[] arrInput = Enumerable.Range(0, 3).Select(i => i.ToString()).ToArray();
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(arrInput);

        Assert.That(enumerab.Any(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));

        enumerab.Dispose();
        Assert.Throws<ObjectDisposedException>(() => enumerab.Any());
    }

    /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
    [Test]
    public void CachedEnumerable_Parsing_03()
    {
        string[] arrInput = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(arrInput);

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        Assert.That(enumerab.Any(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
    }

    /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
    [Test]
    public void CachedEnumerable_Parsing_04()
    {
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(Enumerable.Empty<string>());

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        Assert.That(enumerab.Any(), Is.False);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParsedOk));
    }

    /// <summary> A test for CachedEnumerable parsing, which should succeed. </summary>
    [Test]
    public void CachedEnumerable_Parsing_05()
    {
        int[] arrInput = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);

        IEnumerable<int> output1 = enumerab.Take(arrInput.Length - 2);
        IEnumerable<int> output2 = enumerab.Take(arrInput.Length);
        IEnumerable<int> output3 = enumerab.Take(arrInput.Length + 2);

        Assert.That(output1.Count(), Is.EqualTo(arrInput.Length - 2));
        Assert.That(enumerab.CachedItemsCount, Is.EqualTo(arrInput.Length - 2));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));

        Assert.That(output2.Count(), Is.EqualTo(arrInput.Length));
        Assert.That(enumerab.CachedItemsCount, Is.EqualTo(arrInput.Length));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));

        Assert.That(output3.Count(), Is.EqualTo(arrInput.Length));
        Assert.That(enumerab.CachedItemsCount, Is.EqualTo(arrInput.Length));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParsedOk));

        enumerab.ResetCache();
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
    }

    /// <summary>
    /// A test for CachedEnumerable parsing, which should throw ObjectDisposedException,
    /// since CachedEnumerable object has been disposed.
    /// </summary>
    [Test]
    public void CachedEnumerable_Parsing_06()
    {
        int nVal;
        int[] arrInput = Enumerable.Range(0, 10).ToArray();
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
        enumerab.Dispose();

        Assert.Throws<ObjectDisposedException>(() => nVal = en.Current);
    }

    /// <summary>
    /// A test for CachedEnumerable parsing, which should throw InvalidOperationException,
    /// since CachedEnumerable have not started parsing yet
    /// </summary>
    [Test]
    public void CachedEnumerable_Parsing_07()
    {
        int nVal;
        int[] arrInput = Enumerable.Range(0, 10).ToArray();
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();

        Assert.Throws<InvalidOperationException>(() => nVal = en.Current);
    }

    /// <summary>
    /// A test for CachedEnumerable parsing, which should throw InvalidOperationException,
    /// since the second enumerator did not call MoveNext
    /// </summary>
    [Test]
    public void CachedEnumerable_Parsing_08()
    {
        int dummyA, dummyB;
        int[] arrInput = Enumerable.Range(0, 10).ToArray();
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> enA = enumerab.GetPeekAbleEnumerator();
        IPeekAbleEnumerator<int> enB = enumerab.GetPeekAbleEnumerator();

        dummyA = enA.Peek;
        Assert.Throws<InvalidOperationException>(() => dummyB = enB.Current);
    }
    #endregion // Tests_Parsing

    #region Tests_ResetCache

    /// <summary> A test for CachedEnumerable parsing involving Cache Reset, which should succeed. </summary>
    [Test]
    public void CachedEnumerable_Reset_01()
    {
        int[] arrInput = { 2, 4, 6, 8, 10 };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> en;

        // Attempt parsing and ResetCache three times. That amount has nothing to do with size of arrInput
        for (int ii = 0; ii < 3; ii++)
        {
            Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
            en = enumerab.GetPeekAbleEnumerator();
            Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));

            for (int jj = 0; jj < arrInput.Length; jj++)
            {
                Assert.That(en.CanPeek, Is.True);
                Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
                Assert.That(en.Peek, Is.EqualTo(arrInput[jj]));
                Assert.That(en.MoveNext(), Is.True);
            }
            enumerab.ResetCache();
        }
    }

    /// <summary>
    /// A test for Cache Reset, which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_Reset_02()
    {
        int[] arrInput = { 2, 4, 6, 8, 10 };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> en_CurrentGeneration = null!;

        for (int ii = 0; ii < 2; ii++)
        {
            // i/ enumerators initialization
            Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));

            IPeekAbleEnumerator<int> en_PrevGeneration = en_CurrentGeneration;
            en_CurrentGeneration = enumerab.GetPeekAbleEnumerator();

            // ii/ if there was a previous enumerator, test that Current and Peek throws ArgumentException
            if (en_PrevGeneration != null)
            {
                Assert.Throws<ArgumentException>(() => { int dDummyVal = en_PrevGeneration.Current; });
                Assert.Throws<ArgumentException>(() => { int dDummyVal = en_PrevGeneration.Peek; });
            }

            // iii/ test functionality of current enumerator
            Assert.That(en_CurrentGeneration.CanPeek, Is.True);
            Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
            Assert.That(en_CurrentGeneration.Peek, Is.EqualTo(arrInput[0]));

            // iv/ reset cache, invalidating all enumerators created so far
            enumerab.ResetCache();
        }
    }
    #endregion // Tests_ResetCache

    #region Tests_FillBuffer

    /// <summary>
    /// A test for CachedEnumerable.FillBuffer, which should fail
    /// </summary>
    [Test]
    public void CachedEnumerable_FillBuffer_01()
    {
        string[] arrInput = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(arrInput);

        Assert.Throws<ArgumentOutOfRangeException>(() => enumerab.FillBuffer(-2));
    }

    /// <summary>
    /// A test for CachedEnumerable.FillBuffer, which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_FillBuffer_02()
    {
        string[] arrInput = Enumerable.Range(-1, 10).Select(i => i.ToString()).ToArray();
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(arrInput);

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        for (int ii = 1; ii < arrInput.Length; ii++)
        {
            Assert.That(enumerab.FillBuffer(ii), Is.EqualTo(ii));
            for (int jj = 1; jj <= ii; jj++)
            {
                Assert.That(enumerab.FillBuffer(jj), Is.EqualTo(ii));
            }
        }

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
    }
    #endregion // Tests_FillBuffer

    #region Tests_ResumeParsing

    /// <summary>
    /// A test for CachedEnumerable.ResumeParsing(), which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_ResumeParsing_01()
    {
        int[] arrInput = { 0, 1, 2, 3 };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
    }

    /// <summary>
    /// A test for CachedEnumerable.ResumeParsing(), which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_ResumeParsing_02()
    {
        string[] arrInput = Enumerable.Range(0, 9).Select(i => i.ToString()).ToArray();
        CachedEnumerable<string> enumerab = new CachedEnumerable<string>(arrInput);
        IPeekAbleEnumerator<string> en = enumerab.GetPeekAbleEnumerator();

        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));

        Assert.That(en.CanPeek, Is.True);
        Assert.That(en.Peek, Is.EqualTo("0"));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));

        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
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
    [Test]
    public void CachedEnumerable_ResumeParsing_03()
    {
        int[] arrInput = { 0, 1, 2, 3, 4 };
        const int nFirstSmallMaxSize = 3;
        AuxIntCachedEnumerable<int> enumerab = new AuxIntCachedEnumerable<int>(arrInput, nFirstSmallMaxSize);

        // i/ play with status ParseStatus.ParseNotInitialized
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));
        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParseNotInitialized));

        // ii/ play with status ParseStatus.Parsing
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
        Assert.That(en.CanPeek, Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));

        // iii/ play with status ParseStatus.ParsePrematureEnd, when max buffer size is still small
        enumerab.FillBuffer(999);
        Assert.That(enumerab.CachedItemsCount, Is.EqualTo(nFirstSmallMaxSize));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParsePrematureEnd));
        Assert.That(enumerab.ResumeParsing(), Is.False);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParsePrematureEnd));

        // iv/ play with status ParseStatus.ParsePrematureEnd, when max buffer size is enlarged
        enumerab.MaxBufferSize = int.MaxValue;
        Assert.That(enumerab.ResumeParsing(), Is.True);
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.Parsing));
        Assert.That(enumerab.FillBuffer(), Is.EqualTo(arrInput.Length));
        Assert.That(enumerab.Status, Is.EqualTo(ParseStatus.ParsedOk));
    }
    #endregion // Tests_ResumeParsing

    #region Tests_PeekAbleEnumerator

    /// <summary>  A test for GetPeekAbleEnumerator(), which should succeed. </summary>
    [Test]
    public void CachedEnumerable_PeekAbleEnumerator_01()
    {
        int[] arrInput = { 1, 2, 3 };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();

        for (int ii = 0; ii < arrInput.Length;)
        {
            Assert.That(en.CanPeek, Is.True);
            Assert.That(++ii, Is.EqualTo(en.Peek));
            Assert.That(en.MoveNext(), Is.True);
            Assert.That(ii, Is.EqualTo(en.Current));
        }

        Assert.That(en.CanPeek, Is.False);
        Assert.That(en.MoveNext(), Is.False);
    }
    #endregion // Tests_PeekAbleEnumerator

    #region Tests_Diposing

    /// <summary>
    /// A single-thread test for CachedEnumerable usage, which should throw ObjectDisposedException,
    /// since CachedEnumerable object has been disposed
    /// </summary>
    [Test]
    public void CachedEnumerable_Disposing_01()
    {
        IPeekAbleEnumerator<int> en;
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(Enumerable.Range(0, 10));
        enumerab.Dispose();
        Assert.Throws<ObjectDisposedException>(() => en = enumerab.GetPeekAbleEnumerator());
    }

    /// <summary>
    /// A single-thread test for CachedEnumerable usage, which should throw ObjectDisposedException,
    /// since CachedEnumerable object has been disposed
    /// </summary>
    [Test]
    public void CachedEnumerable_Disposing_02()
    {
        int nVal;
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(Enumerable.Range(0, 10));
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
        enumerab.Dispose();
        Assert.Throws<ObjectDisposedException>(() => nVal = en.Current);
    }

    /// <summary>
    /// A single-thread test for CachedEnumerable Disposing, called several times, which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_Disposing_03()
    {
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(Enumerable.Range(0, 10));
        IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();
        enumerab.Dispose();
        enumerab.Dispose();
    }
    #endregion // Tests_Diposing

    #region Tests_Parallel

    /// <summary>
    /// A multiple-thread test for CachedEnumerable Disposing, which should succeed.
    /// </summary>
    [Test]
    public void CachedEnumerable_Parallel_Disposing_01()
    {
        const int nMaxThreads = 12;
        var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(Enumerable.Range(0, 10));

        Parallel.For(0, nMaxThreads, options, i =>
        {
            enumerab.Dispose();
        });
    }

    /// <summary>
    /// A multiple-thread test for CachedEnumerable Disposing, which should succeed.
    /// </summary>
    [Test]
    public void CachedEnumerable_Parallel_Disposing_02()
    {
        const int nMaxThreads = 12;
        var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(Enumerable.Range(0, 10));

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
                Assert.That(ex.GetType(), Is.EqualTo(typeof(ObjectDisposedException)));
            }
        }
    }

    /// <summary>
    /// A multiple-thread test for GetPeekAbleEnumerator(), which should succeed
    /// </summary>
    [Test]
    public void CachedEnumerable_Parallel_PeekAbleEnumerator_01()
    {
        const int nMaxThreads = 12;
        var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };

        const int nInputLength = 128;
        var arrInput = Enumerable.Range(1, nInputLength);
        CachedEnumerable<int> enumerab = new CachedEnumerable<int>(arrInput);

        Parallel.For(0, nMaxThreads, options, i =>
        {
            IPeekAbleEnumerator<int> en = enumerab.GetPeekAbleEnumerator();

            for (int ii = 0; ii < nInputLength;)
            {
                Assert.That(en.CanPeek, Is.True);
                Assert.That(++ii, Is.EqualTo(en.Peek));
                Assert.That(en.MoveNext(), Is.True);
                Assert.That(ii, Is.EqualTo(en.Current));
            }

            Assert.That(en.CanPeek, Is.False);
            Assert.That(en.MoveNext(), Is.False);
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
    [Test]
    public void CachedEnumerable_InterfaceCovariance_01()
    {
        MyRectangle[] arrInput = { new MyRectangle(5), new MyRectangle(6), new MyRectangle(7) };
        CachedEnumerable<MyRectangle> enData = new CachedEnumerable<MyRectangle>(arrInput);

        // enumerator with more derived type
        IPeekAbleEnumerator<MyRectangle> enRects = enData.GetPeekAbleEnumerator();
        // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
        IPeekAbleEnumerator<MyShape> enShapes = enRects;
    }

    /// <summary> A test for covariance of IPeekAbleEnumerable ( compilation of assignment ). </summary>
    [Test]
    public void CachedEnumerable_InterfaceCovariance_02()
    {
        MyRectangle[] arrInput = { new MyRectangle(5), new MyRectangle(6), new MyRectangle(7) };

        // enumerable with more derived type
        IPeekAbleEnumerable<MyRectangle> enDataRectangles = new CachedEnumerable<MyRectangle>(arrInput);
        // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
        IPeekAbleEnumerable<MyShape> enDatShapes = enDataRectangles;
    }

    /// <summary> A test for covariance of ICachedEnumerable ( compilation of assignment ). </summary>
    [Test]
    public void CachedEnumerable_InterfaceCovariance_03()
    {
        MyRectangle[] arrInput = { new MyRectangle(5), new MyRectangle(6), new MyRectangle(7) };

        // enumerable with more derived type
        ICachedEnumerable<MyRectangle> enDataRectangles = new CachedEnumerable<MyRectangle>(arrInput);
        // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
        ICachedEnumerable<MyShape> enDatShapes = enDataRectangles;
    }
    #endregion // Tests_Covariance
    #endregion // Tests
}
