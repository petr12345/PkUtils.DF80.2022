// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// IDisposableEx extends IDisposable interface.
/// </summary>
[CLSCompliant(true)]
public interface IDisposableEx : IDisposable
{
    /// <summary>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    /// 
    /// <remarks>
    /// As with IDisposable.Dispose() method, do NOT make this property virtual. ( If one could override 
    /// the property in derived class, it might have unintended consequences in Dispose of base class).
    /// Should the class that inherits from IDisposableEx-implementing class override the virtual method
    /// Dispose(bool disposing), it should do in a following fashion:
    /// <code>
    /// public class DisposableClass : IDisposableEx
    /// {
    ///   private bool _Disposed;
    /// 
    ///   public bool IsDisposed
    ///   {
    ///     get { return _Disposed; }
    ///   }
    /// 
    ///   public void Dispose()
    ///   {
    ///     Dispose(true);
    ///   }
    /// 
    ///   protected virtual void Dispose(bool disposing)
    ///   {
    ///     if (disposing)
    ///     {
    ///       // free other managed objects that implement IDisposable
    ///     }
    ///     // release any unmanaged objects
    ///     _Disposed = true;
    ///   }
    /// }
    /// 
    /// public class SubDisposableClass : DisposableClass
    /// {
    ///   protected override void Dispose(bool disposing)
    ///   {
    ///     if (!IsDisposed)
    ///     {
    ///       if (disposing)
    ///       {
    ///         // free other managed objects that implement IDisposable only
    ///       }
    ///       // release any unmanaged objects; set object references to null
    ///       // ...
    ///       
    ///       // now call base class to finish the job
    ///       base.Dispose(disposing);
    ///       
    ///       // everything went ok?
    ///       Debug.Assert(this.IsDisposed, "should be disposed now");
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso href="http://lostechies.com/chrispatterson/2012/11/29/idisposable-done-right/">
    /// IDisposable, Done Right</seealso>
    bool IsDisposed { get; }
}

/// <summary>
/// Defining a notification delegates, called when the object is being disposed the first time.
/// </summary>
/// <remarks> 
/// The property <see cref="IDisposableEx.IsDisposed"/> should NOT be true at this moment yet.
/// Subsequent calls of Dispose should not cause that call being repeated.
/// </remarks>
/// <param name="sender">The source of the event.</param>
/// <param name="args">An object that contains the event data.</param>
[CLSCompliant(true)]
public delegate void DisposedEventHandler(IDisposableEx sender, EventArgs args);

/// <summary>
/// Extends the IDisposableEx interface
/// </summary>
[CLSCompliant(true)]
public interface IDisposableEx2 : IDisposableEx
{
    /// <summary>
    /// The event fired when the object is being disposed the first time.
    /// Subsequent calls of Dispose should not cause that event being raised again.
    /// </summary>
    /// <remarks>
    /// The property <see cref="IDisposableEx.IsDisposed"/> should NOT be true at this moment yet.
    /// </remarks>
    event DisposedEventHandler Disposed;
}
