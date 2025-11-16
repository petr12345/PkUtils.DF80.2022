// Ignore Spelling: Utils
// 
using System;
using System.Threading;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Threading;

#region SlimLockReaderGuard_class

/// <summary> The class SlimLockReaderGuard is a wrapper around <see cref="System.Threading.ReaderWriterLockSlim"/>.
/// The primary purpose of the wrapper is to wrap reader-lock acquiring into "something IDisposable-derived".
/// This way, the traditional try-finally code, like following
/// <code>
/// <![CDATA[
/// public void Read()
/// {
///   try
///   {
///     _readerWriterLock.EnterReadLock();
///     // read thread-safe stuff ...
///   }
///   finally
///   {
///     _readerWriterLock.ExitReadLock();
///   }
/// }
/// ]]>
/// </code>
/// could be replaced by more simple code
/// <code>
/// <![CDATA[
/// public void Read()
/// {
///   using (new SlimLockReaderGuard(_readerWriterLock))
///   {
///      // read thread-safe stuff ...
///   }
/// }
/// ]]>
/// </code>
/// </summary>
public class SlimLockReaderGuard : IDisposableEx
{
    #region Fields

    /// <summary> The reader writer lock, initialized by constructor. </summary>
    private ReaderWriterLockSlim _readerWriterLock;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Public constructor. </summary>
    /// <param name="readerWriterLock"> The reader writer lock. </param>
    public SlimLockReaderGuard(ReaderWriterLockSlim readerWriterLock)
    {
        ArgumentNullException.ThrowIfNull(readerWriterLock);

        _readerWriterLock = readerWriterLock;
        _readerWriterLock.EnterReadLock();
    }
    #endregion // Constructor(s)

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to override
    /// this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary> Returns true in case the object has been disposed and no longer should be used. </summary>
    /// <value> true if this object is disposed, false if not. </value>
    public bool IsDisposed => _readerWriterLock == null;
    #endregion // IDisposableEx Members

    #region Methods

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you should
    /// not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed)
        {
            _readerWriterLock.ExitReadLock();
            _readerWriterLock = null;
        }
    }
    #endregion // Methods
}
#endregion // SlimLockReaderGuard_class

#region SlimLockWriterGuard_class

/// <summary> The class SlimLockWriterGuard is a wrapper around <see cref="System.Threading.ReaderWriterLockSlim"/>.
/// The primary purpose of the wrapper is to wrap writer-lock acquiring into "something IDisposable-derived".
/// This way, the traditional try-finally code, like following
/// <code>
/// <![CDATA[
/// public void Read()
/// {
///   try
///   {
///     _readerWriterLock.EnterWriteLock();
///     // write thread-safe stuff ...
///   }
///   finally
///   {
///     _readerWriterLock.ExitWriteLock();
///   }
/// }
/// ]]>
/// </code>
/// could be replaced by more simple code
/// <code>
/// <![CDATA[
/// public void Read()
/// {
///   using (new SlimLockWriterGuard(_readerWriterLock))
///   {
///     // write thread-safe stuff ...
///   }
/// }
/// ]]>
/// </code>
/// </summary>
public class SlimLockWriterGuard : IDisposableEx
{
    #region Fields
    /// <summary> The reader writer lock, initialized by constructor. </summary>
    private ReaderWriterLockSlim _readerWriterLock;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Public constructor. </summary>
    /// <param name="readerWriterLock"> The reader writer lock. </param>
    public SlimLockWriterGuard(ReaderWriterLockSlim readerWriterLock)
    {
        ArgumentNullException.ThrowIfNull(readerWriterLock);

        _readerWriterLock = readerWriterLock;
        _readerWriterLock.EnterWriteLock();
    }
    #endregion // Constructor(s)

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to override
    /// this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary> Returns true in case the object has been disposed and no longer should be used. </summary>
    /// <value> true if this object is disposed, false if not. </value>
    public bool IsDisposed => _readerWriterLock == null;
    #endregion // IDisposableEx Members

    #region Methods

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you should
    /// not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed)
        {
            _readerWriterLock.ExitWriteLock();
            _readerWriterLock = null;
        }
    }
    #endregion // Methods
}
#endregion // SlimLockWriterGuard_class

#region SlimLockUpgradeableGuard_class

/// <summary> The class SlimLockUpgradeableGuard is a wrapper around <see cref="System.Threading.ReaderWriterLockSlim"/>.
/// The primary purpose of the wrapper is to wrap upgradeable-lock acquiring into "something IDisposable-derived".
/// This way, the traditional tedious try-finally code, like following
/// <code>
/// <![CDATA[
/// public void ReadThenWrite()
/// {
///   try
///   {
///     _readerWriterLock.EnterUpgradeableReadLock();
///     // read thread-safe stuff ... and get data for condition
///     if (condition)
///     {
///       try
///       {
///         _readerWriterLock.EnterWriteLock();
///          // write thread-safe stuff ...
///       }
///       finally
///       {
///         _readerWriterLock.ExitWriteLock();
///       }
///     }
///   }
///   finally
///   {
///     _readerWriterLock.ExitUpgradeableReadLock();
///   }
/// }
/// ]]>
/// </code>
/// could be replaced by more simple code
/// <code>
/// <![CDATA[
/// public void ReadThenWrite()
/// {
///   using (var upgradeableGuard = new SlimLockUpgradeableGuard(_readerWriterLock))
///   {
///     // read thread-safe stuff ... and get data for condition
///     if (condition)
///     {
///       using (upgradeableGuard.UpgradeToWriterLock())
///       {
///          // write thread-safe stuff ...
///       }
///     }
///   }
/// }
/// ]]>
/// </code>
/// </summary>
public class SlimLockUpgradeableGuard : IDisposableEx
{
    #region Typedefs

    private class UpgradedGuard : IDisposable
    {
        private readonly SlimLockUpgradeableGuard _parentGuard;
        private readonly SlimLockWriterGuard _writerLock;
        public UpgradedGuard(SlimLockUpgradeableGuard parentGuard)
        {
            _parentGuard = parentGuard;
            _writerLock = new SlimLockWriterGuard(_parentGuard._readerWriterLock);
        }
        public void Dispose()
        {
            _writerLock.Dispose();
            _parentGuard._upgradedLock = null;
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> The reader writer lock, initialized by constructor. </summary>
    private ReaderWriterLockSlim _readerWriterLock;
    /// <summary> The object representing the upgraded lock ( read lock upgraded to write lock ). </summary>
    private UpgradedGuard _upgradedLock;

    #endregion // Fields

    #region Constructor(s)

    /// <summary> Public constructor. </summary>
    /// <param name="readerWriterLock"> The reader writer lock, initialized by constructor. </param>
    public SlimLockUpgradeableGuard(ReaderWriterLockSlim readerWriterLock)
    {
        _readerWriterLock = readerWriterLock;
        _readerWriterLock.EnterUpgradeableReadLock();
    }
    #endregion // Constructor(s)

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to override
    /// this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary> Returns true in case the object has been disposed and no longer should be used. </summary>
    /// <value> true if this object is disposed, false if not. </value>
    public bool IsDisposed => _readerWriterLock == null;
    #endregion // IDisposableEx Members

    #region Methods

    /// <summary> Upgrade the read lock to writer lock, if not upgraded already. </summary>
    /// <returns> An IDisposable wrapper round lock upgrade. </returns>
    public IDisposable UpgradeToWriterLock()
    {
        _upgradedLock ??= new UpgradedGuard(this);
        return _upgradedLock;
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you should
    /// not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed)
        {
            _upgradedLock?.Dispose();
            _readerWriterLock.ExitUpgradeableReadLock();
            _readerWriterLock = null;
        }
    }
    #endregion // Methods
}
#endregion // SlimLockUpgradeableGuard_class
