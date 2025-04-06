/***************************************************************************************************************
*
* FILE NAME:   .\DataStructures\Repository.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of generic Repository class
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Globalization;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary> Possible generic implementation of IRepository interface. </summary>
///
/// <typeparam name="T"> The type of the class that is held by or attached to this Repository instance. </typeparam>
[CLSCompliant(true)]
public class Repository<T> : IRepository<T>, IEquatable<Repository<T>>
{
    #region Fields

    /// <summary>
    /// The actual data that are either held (owned) or just attached
    /// </summary>
    private T _data;

    /// <summary>
    /// True if the data are attached, false otherwise
    /// </summary>
    private bool _isAttached;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public Repository()
    {
        ValidateMe();
    }

    /// <summary>
    /// Construct a new repository with the data owned by that repository.
    /// </summary>
    /// <param name="data">The data that will be owned by this Repository object</param>
    public Repository(T data)
    {
        Keep(data);
        ValidateMe();
    }

    /// <summary>
    /// Construct a new repository with the data either owned by that repository or attached to it.
    /// </summary>
    /// <exception cref="ArgumentNullException"> 
    /// Thrown when T is a reference type, and <paramref name="data"/> is null.
    /// </exception>
    /// 
    /// <param name="data">The data that will be owned by or attached to this Repository object</param>
    /// <param name="owned">If true, the Repository becomes owner of  the <paramref name="data"/>; 
    /// otherwise the <paramref name="data"/> are just attached.</param>
    public Repository(T data, bool owned)
    {
        if (owned)
            Keep(data);
        else
            Attach(data);
        ValidateMe();
    }
    #endregion // Constructor(s)

    #region Static Operators

    /// <summary> Converts given data to new data repository. </summary>
    /// <param name="data"> The data to process. </param>
    /// <returns> A new repository instance constructed with <paramref name="data"/> argument. </returns>
    public static explicit operator Repository<T>(T data)
    {
        return new Repository<T>(data);
    }

    /// <summary> Converts a repository to data T. </summary>
    /// <param name="repository"> The converted repository. May be null. </param>
    /// <returns> For a non-null <paramref name="repository"/> returns repository.Data; otherwise default T value. </returns>
    public static explicit operator T(Repository<T> repository)
    {
        T result;

        if (repository == null)
            result = default;
        else
            result = repository.Data;

        return result;
    }


    /// <summary> The equality operator.</summary>
    ///
    /// <param name="first"> The first operand. </param>
    /// <param name="second"> The second operand. </param>
    ///
    /// <returns> Returns true if both operands have the same value.</returns>
    public static bool operator ==(Repository<T> first, Repository<T> second)
    {
        if (ReferenceEquals(first, second)) return true;
        if ((first is null) || (second is null)) return false;

        return first.Equals(second);
    }

    /// <summary> The non-equality operator.</summary>
    ///
    /// <param name="first"> The first operand. </param>
    /// <param name="second"> The second operand. </param>
    ///
    /// <returns> Returns true if both operands do not have the same value.</returns>
    public static bool operator !=(Repository<T> first, Repository<T> second)
    {
        return !(first == second);
    }
    #endregion // Static Operators

    #region Methods
    #region Public Methods

    /// <summary> Determines whether the specified object is equal to the current object. </summary>
    ///
    /// <param name="obj">  The object to compare with the current object. </param>
    ///
    /// <returns> true if the specified object  is equal to the current object; otherwise, false. </returns>
    public override bool Equals(object obj)
    {
        bool result = (obj is Repository<T> other) && this.Equals(other);

        return result;
    }

    /// <summary>	Serves as a hash function for a particular type. </summary>
    /// <returns>	A hash code for the current object. </returns>
    public override int GetHashCode()
    {
        int nResult;

        if (this.HasData)
            nResult = this.IsAttached.GetHashCode() ^ this.Data.GetHashCode();
        else
            nResult = base.GetHashCode();

        return nResult;
    }

    /// <summary>	Returns a string that represents the current object. </summary>
    /// <returns>	A string that represents the current object. </returns>
    public override string ToString()
    {
        string strRes;
        string dataInfo = this.DataToString();
        string strType = typeof(T).ToString();

        if (this.IsAttached || !HasNullData())
        {
            dataInfo = string.Format(CultureInfo.InvariantCulture, "'{0}', IsAttached:{1}", dataInfo, IsAttached);
        }
        else
        {
            // keep dataInfo as is
        }
        strRes = string.Format(CultureInfo.InvariantCulture, "Repository {0} ({1})", strType, dataInfo);

        return strRes;
    }

    /// <summary>
    /// Virtual method validating the object instance.
    /// </summary>
    [Conditional("DEBUG")]
    public virtual void AssertValid()
    {
        ValidateMe();
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Non-virtual method validating an instance. 
    /// The reason of existence of this method is to avoid calling virtual method from constructor.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(!(_data == null && _isAttached), "Can't be attached with data being null");
    }

    /// <summary> Query if this object has just null data; 
    ///           i.e. if T is a reference type, and nothing is attached or owned.
    /// </summary>
    /// <returns> True if null data, false otherwise. </returns>
    protected bool HasNullData()
    {
        return (Data == null);
    }

    /// <summary> Converts currently owned or attached data (if any) to string. </summary>
    /// 
    /// <returns>  A string representation of <see cref="Data"/>. </returns>
    protected string DataToString()
    {
        return Data.AsString();
    }

    /// <summary>
    /// Get rid of data owned by this instance (assuming it is the owner of the data).
    /// </summary>
    protected virtual void DisposeData()
    {
        if (this.IsAttached)
        {
            throw new InvalidOperationException("The data are just attached, but not owned");
        }
        if (HasData)
        {
            (_data as IDisposable)?.Dispose();
            _data = default;
        }
        ValidateMe();
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Forfeit();
        }
    }

    /// <summary> Member-wise equals comparison. An auxiliary method called from IEquatable.Equals. </summary>
    /// <param name="other"> The second compared instance. Can't be null. </param>
    /// <returns> true objects are found member-wise equal, false if not. </returns>
    protected virtual bool MemberWiseEquals(Repository<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        bool result = false;

        if (this.IsAttached == other.IsAttached)
        {
            if (typeof(T).IsValueType)
                result = this.Data.Equals(other.Data);
            else if (ReferenceEquals(this.Data, other.Data))
                result = true;
            else if (this.Data == null)
                result = false;
            else
                result = this.Data.Equals(other.Data);
        }

        return result;
    }
    #endregion // Protected Methods

    #endregion // Methods

    #region IEquatable<Repository<T>>

    /// <inheritdoc/>
    public bool Equals(Repository<T> other)
    {
        bool result;

        if (other is null)
            result = false;
        else if (ReferenceEquals(this, other))
            result = true;
        else
            result = this.MemberWiseEquals(other);

        return result;
    }
    #endregion // IEquatable<Repository<T>>

    #region IRepository<T> Members

    #region IDisposable Members

    /// <summary>
    /// Implementation of IDisposable.Dispose()
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary> Gets a value indicating whether this object has any data of type T (either attached or owned).</summary>
    /// <remarks> If data are just attached, can simply return true; otherwise must check for contents. </remarks>
    public bool HasData
    {
        get { return IsAttached || !HasNullData(); }
    }

    /// <summary> Gets a value indicating whether this object has attached data. </summary>
    ///
    /// <value> true if this object has attached data, false if not. </value>
    public bool IsAttached
    {
        get { return _isAttached; }
    }

    /// <summary> Get currently owned or attached data. Returns null if there are no such data.</summary>
    ///
    /// <value> The data. </value>
    public T Data
    {
        get { return _data; }
    }

    /// <summary> Keep the data ( that means became an owner of it ). </summary>
    /// <exception cref="ArgumentNullException">  Thrown when T is a reference type, and <paramref name="data"/> is null.
    /// </exception>
    /// 
    /// <param name="data"> The data that will be owned by this repository object. </param>
    public void Keep(T data)
    {
        if (typeof(T).IsClass)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
        }

        if (this.HasData)
        {
            Debug.Fail("This instance already contains other data");
            throw new InvalidOperationException("This instance already contains other data");
        }

        this._data = data;
        this._isAttached = false;
    }

    /// <summary>
    /// Attach the data ( that means do NOT became an owner of it ).
    /// Throws InvalidOperationException if there are already any data owned or attached.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException">  Thrown when T is a reference type, and <paramref name="data"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException"> Thrown if there are already any data owned or attached.
    /// </exception>
    /// 
    /// <remarks> If the data are just attached, the methods <see cref="Forfeit"/> and <see cref="Dispose()"/>(
    /// which just calls Forfeit ) will call <see cref="Detach"/>.  <br/>
    /// </remarks>
    /// 
    /// <param name="data"> The data being attached to this repository object. </param>
    /// <seealso cref="Keep"/>
    public void Attach(T data)
    {
        if (typeof(T).IsClass)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
        }

        if (this.HasData)
        {
            Debug.Fail("This instance already contains other data");
            throw new InvalidOperationException("This instance already contains other data");
        }

        this._data = data;
        this._isAttached = true;
    }

    /// <summary>
    /// Detach the data previously attached and return these data. <br/>
    /// Should not make any change and return null in case there are no data attached 
    /// ( which means either there are no data at all, or the instance is actually the owner of the data ).
    /// </summary>
    /// <returns> Previously attached data or default(T). </returns>
    public T Detach()
    {
        T result = default;

        if (this.IsAttached)
        {
            result = this._data;
            this._data = default;
            this._isAttached = false;
        }
        return result;
    }

    /// <summary>
    /// Forfeit the data ( get rid of the data ). <br/>
    /// In case the data were attached, calls <see cref="Detach"/>. <br/>
    /// In case the data were not attached but this object was the owner of the data, calls <see cref="DisposeData"/>. <br/>
    /// </summary>
    /// <seealso cref="Detach"/>
    /// <seealso cref="DisposeData"/>
    public void Forfeit()
    {
        if (HasData)
        {
            if (IsAttached)
            {
                Detach();
            }
            else
            {
                DisposeData();
            }
        }
        AssertValid();
    }
    #endregion // IRepository<T> Members
}
