// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Threading;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Utils;

/// <summary>
/// A base usage counter, internally using <see cref="Interlocked"/>methods.
/// Derived class may override ( re-implement ) <see cref="OnFirstAddReference"/> and <see cref="OnLastRelease"/>.
/// </summary>
/// <remarks> 
/// Implementing IUsageCounter, this class allows you to protect
/// against reentrancy issues. Note that after the first usage is acquired,
/// any subsequent usages on this thread or any other thread still may succeed as well.
/// The using code should check <see cref="IsUsed"/> property in any case. 
/// </remarks>
[CLSCompliant(true)]
public class UsageCounter : IUsageCounter
{
    #region Fields
    /// <summary> Current amount of locks. </summary>
    private volatile int _references;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> The default argument-less constructor. </summary>
    public UsageCounter()
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the references count. </summary>
    protected int References { get => _references; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Public method validating the object instance. 
    /// Current implementation just delegates the call to non-virtual method <see cref="ValidateMe"/>.
    /// </summary>
    [Conditional("DEBUG")]
    public virtual void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Non-virtual method validating an instance of this type. Checks that usage counter is not unbalanced.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(References >= 0);
    }

    /// <summary>
    /// The method invoked when the time usage has been acquired.
    /// Derived class may need to override ( re-implement ) that method.
    /// </summary>
    protected virtual void OnFirstAddReference()
    { }

    /// <summary>
    /// The method invoked when the last usage reference been released.
    /// Derived class may need to override ( re-implement ) that method.
    /// </summary>
    protected virtual void OnLastRelease()
    { }
    #endregion // Methods

    #region IUsageCounter Members

    /// <summary>
    /// Has been locked at all (once or more times)?
    /// </summary>
    public bool IsUsed
    {
        get
        {
            AssertValid();
            return (References > 0);
        }
    }

    /// <summary> Add reference please. </summary>
    /// <returns> The amount of total references acquired. </returns>
    public virtual int AddReference()
    {
        int nResult;

        AssertValid();
        if (1 == (nResult = Interlocked.Increment(ref _references)))
        {
            OnFirstAddReference();
        }
        return nResult;
    }

    /// <summary> Unlock that please. </summary>
    /// <returns> The amount of remaining references. </returns>
    public virtual int Release()
    {
        int nResult;

        if (0 == (nResult = Interlocked.Decrement(ref _references)))
        {
            OnLastRelease();
        }
        AssertValid();
        return nResult;
    }
    #endregion // IUsageCounter Members
}
