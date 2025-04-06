/***************************************************************************************************************
*
* FILE NAME:   .\Dump\DumperTextWriterAndTraceListener.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class DumperTextWriterAndTraceListener
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;

namespace PK.PkUtils.Dump;

/// <summary>
/// The class encapsulating TextWriterTraceListener and its DumperTextWriter.
/// </summary>
[CLSCompliant(true)]
public class DumperTextWriterAndTraceListener : IDisposableEx
{
    #region Fields
    private TextWriterTraceListener _listener;
    private DumperTextWriter _dumpWriter = new(null);

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public DumperTextWriterAndTraceListener()
      : this(null)
    {
    }

    /// <summary>
    /// Constructor getting the IDumper-supporting output .
    /// <para>
    /// If provided IDumper output is not null, 
    /// it will create the trace listener TextWriterTraceListener with empty name,
    /// and set the output of DumperTextWriter _dumpWriter to IDumper output. 
    /// </para>
    /// <para>
    /// If the provided IDumper output is null, nothing is created, 
    /// but you can later explicitly call SetDumpOutput(IDumper output)
    /// and CreateTraceListener.
    /// </para>
    /// </summary>
    /// <param name="output"> Output object, having <see cref="PK.PkUtils.Interfaces.IDumper"/> functionality.</param>
    public DumperTextWriterAndTraceListener(IDumper output)
      : this(output, null)
    {
    }

    /// <summary>
    /// Constructor getting the IDumper-supporting output .
    /// <para>
    /// If provided IDumper output is not null, 
    /// it will create the trace listener TextWriterTraceListener with provided name,
    /// and set the output of DumperTextWriter _dumpWriter to IDumper output.
    /// </para>
    /// <para>
    /// If the provided IDumper output is null, nothing is created, 
    /// and the argument <paramref name="name "/> is not used at all.
    /// Note that you can later explicitly call SetDumpOutput(IDumper output)
    /// and CreateTraceListener.
    /// </para>
    /// </summary>
    /// <param name="output"> Output object, having <see cref="PK.PkUtils.Interfaces.IDumper"/> functionality.</param>
    /// <param name="name">The name of created <see cref="TextWriterTraceListener"/></param>
    public DumperTextWriterAndTraceListener(IDumper output, string name)
    {
        if (null != output)
        {
            SetDumpOutput(output);
            CreateTraceListener(name);
        }
    }
    #endregion Constructors

    #region Properties

    /// <summary>
    /// Is the trace listener created
    /// </summary>
    public bool HasTraceListener
    {
        get { return (null != MyListener); }
    }

    /// <summary>
    /// Returns true if TraceListener has been created and added to Trace.Listeners list.
    /// See also property MyListener and methods StartTraceListening(), StopTraceListening().
    /// </summary>
    public bool IsTraceListenerListening
    {
        get { return HasTraceListener && Trace.Listeners.Contains(MyListener); }
    }

    /// <summary>
    /// Returns true if the internal queue buffer is empty; false otherwise.
    /// </summary>
    public bool IsWriterQueueEmpty
    {
        get { return _dumpWriter.IsWriterQueueEmpty; }
    }

    /// <summary>
    /// Return the current DumperTextWriter ( if there is any )
    /// </summary>
    protected DumperTextWriter DumpWriter
    {
        get { return _dumpWriter; }
    }

    /// <summary>
    /// Get the current output ( if there is any ) of dump writer
    /// </summary>
    protected IDumper DumpOutput
    {
        get { return IsDisposed ? null : DumpWriter.Output; }
    }

    /// <summary>
    /// Give me the listener that I have created (if any)
    /// </summary>
    protected TraceListener MyListener
    {
        get { return _listener; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Sets the IDumper output of dump writer. The argument may be null. </summary>
    ///
    /// <param name="output"> Provided IDumper -supporting output  object, that will be assigned to
    ///   <see cref="DumperTextWriter"/> </param>
    public void SetDumpOutput(IDumper output)
    {
        this.CheckNotDisposed();
        DumpWriter.SetOutput(output);
    }

    /// <summary>
    /// Creates the trace listener, if has not been created already.
    /// </summary>
    public void CreateTraceListener()
    {
        CreateTraceListener(null);
    }

    /// <summary>
    /// Creates the trace listener using a provided name, 
    /// if the listener has not been created already.
    /// </summary>
    /// <param name="name">The name given to created listener.</param>
    public void CreateTraceListener(string name)
    {
        if (!HasTraceListener)
        {
            _listener = new TextWriterTraceListener(DumpWriter, name);
        }
    }

    /// <summary>
    /// Destroy the trace listener
    /// </summary>
    public void DestroyTraceListener()
    {
        if (HasTraceListener)
        {
            StopTraceListening();
            Disposer.SafeDispose(ref _listener);
        }
    }

    /// <summary>
    /// Start top trace listening, will you?
    /// </summary>
    /// <remarks> Trace listener has to be created already </remarks>
    public void StartTraceListening()
    {
        Debug.Assert(HasTraceListener);
        if (!IsTraceListenerListening)
        {
            Trace.Listeners.Add(MyListener);
        }
    }

    /// <summary>
    /// Stop trace listening, will you?
    /// </summary>
    /// <remarks> Trace listener has to be created already </remarks>
    public void StopTraceListening()
    {
        Debug.Assert(HasTraceListener);
        if (IsTraceListenerListening)
        {
            Trace.Listeners.Remove(MyListener);
        }
    }

    /// <summary>
    /// Flush the buffered contents of DumperTextWriter to its specified output
    /// </summary>
    public virtual void Flush()
    {
        this.CheckNotDisposed();
        if (!IsWriterQueueEmpty)
        {
            DumpWriter.Flush();
        }
    }

    /// <summary>
    /// For the current DumperTextWriter, which is returned by property <see cref="DumpWriter"/>, 
    /// calls Flush ( if bFlush is true) and then RequestStop,
    /// which involves stopping the writing thread.
    /// </summary>
    /// <param name="bFlush"> If true,  DumpWriter.Flush will be called.</param>
    public virtual void RequestStopDumper(bool bFlush)
    {
        if (!this.IsDisposed)
        {
            if (bFlush)
            {
                DumpWriter.Flush();
            }
            DumpWriter.RequestStop(DumperTextWriter._defaultWaitForJoinMs);
        }
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DestroyTraceListener();
            Disposer.SafeDispose(ref _dumpWriter);
        }
    }
    #endregion // Methods

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return (null == DumpWriter); }
    }
    #endregion // IDisposableEx Members
}
