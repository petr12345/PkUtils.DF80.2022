using System;
using System.Threading;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;
using PK.PkUtils.WinApi;
using PK.TestCloning.ByContract;

namespace PK.TestCloning
{
    public class Program : Singleton<Program>, IDisposableEx
    {
        #region Fields
        private static CancellationTokenSource _tokenSource;
        private static CancellationToken _token;
        private bool _isDisposed;
        #endregion // Fields

        #region Constructor(s)

        protected Program()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }
        #endregion // Constructor(s)

        #region Methods

        public static void Main(string[] args)
        {
            Instance.InitializeAndRunTests();
        }

        protected void DoCloningTests()
        {
            TestCloningByContract.Test1();
            TestCloningByContract.Test2();

            TestCloningBinary.Test1();
            TestCloningBinary.Test2();

            TestCloningBinary2nd.TestCloningSimpleDictionary();
            TestCloningBinary2nd.TestCloningImageDict();
        }


        protected void InitializeAndRunTests()
        {
            Kernel32.SetConsoleCtrlHandler(new Kernel32.HandlerRoutine(ConsoleExitHanlder), true);

            DoCloningTests();
            Console.WriteLine("Pres Ctrl + C to quit");

            _token.WaitHandle.WaitOne();
        }

        private static bool ConsoleExitHanlder(Kernel32.CtrlTypes ctrlType)
        {
            bool result = false;

            switch (ctrlType)
            {
                case Kernel32.CtrlTypes.CTRL_CLOSE_EVENT:
                case Kernel32.CtrlTypes.CTRL_LOGOFF_EVENT:
                case Kernel32.CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    Instance.Dispose();
                    result = true;
                    break;
            }

            return result;
        }
        #endregion // Methods

        #region IDisposableEx Members

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // Uncomment following to verify the dispose is actually called
                    // Debug.Assert(false);
                }
                // Now free unmanaged resources (unmanaged objects) and override a finalizer below.

                // assign property backing field
                _isDisposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Program() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed { get { return _isDisposed; } }
        #endregion // IDisposableEx Members
    }
}
