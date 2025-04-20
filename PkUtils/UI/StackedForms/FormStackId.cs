/***************************************************************************************************************
*
* FILE NAME:   .\UI\Stack\FormStackId.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class FormStackId, derived from FormStac.StackId
*
**************************************************************************************************************/


// Ignore Spelling: TDATA, Utils
//
using System;
using System.Collections.Generic;

namespace PK.PkUtils.UI.StackedForms;

/// <summary>
/// This generic extends the base class FormStack.StackId, for the case when I need more FormStack
/// granularity, to keep in the cache several forms of the same type, depending on additional data.
/// The data type is specified by generic TDATA argument. </summary>
///
/// <remarks>
/// The main part of the functionality consist of overriding these two methods
/// <code>
///   public override bool Equals(IStackId other)
///   public override int GetHashCode()
/// </code>
/// Obviously, I do NOT need to override the method
/// <code>
///   public override bool Equals(object obj)
/// </code>
/// since the implementation FormStack.StackId just delegates to the other overload. </remarks>
///
/// <typeparam name="TDATA"> The class or structure keeping the additional data, that are used in
///   FormStackId comparisons. </typeparam>
[CLSCompliant(true)]
public class FormStackId<TDATA> : FormStack.StackId, IEquatable<FormStackId<TDATA>>
{
    #region Fields
    /// <summary>
    /// The comparer that will be used to compare TDATA instances.
    /// </summary>
    protected readonly IEqualityComparer<TDATA> _comparer;
    private readonly TDATA _data;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor accepting form type and additional data that distinguish concrete form instances from each other.
    /// </summary>
    /// <param name="formType">The supported form type.</param>
    /// <param name="data">Additional data that distinguish concrete form instances from each other.</param>
    public FormStackId(Type formType, TDATA data)
      : this(formType, data, null)
    {
    }

    /// <summary>
    /// Overloaded constructor that supports passing a custom IEqualityComparer{T}. <br/>
    /// If that comparer argument is null, the default comparer will be used.<br/>
    /// For more information, see <a href="http://stackoverflow.com/questions/5857654/equalitycomparert-default-vs-t-equals">
    /// EqualityComparer{T}.Default vs. T.Equals.</a>
    /// </summary>
    /// <param name="formType">The supported form type.</param>
    /// <param name="data">Additional data that distinguish concrete form instances from each other.</param>
    /// <param name="comparer">Supplied comparer of TDATA instances.</param>
    public FormStackId(Type formType, TDATA data, IEqualityComparer<TDATA> comparer)
      : base(formType)
    {
        _data = data;
        _comparer = comparer ?? EqualityComparer<TDATA>.Default;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The extra data that were specified by the constructor.
    /// </summary>
    public TDATA Data
    {
        get { return _data; }
    }

    /// <summary>
    /// Currently used comparer ( either custom specified by the constructor, or a default one if none was specified).
    /// </summary>
    public IEqualityComparer<TDATA> DataComparer
    {
        get { return _comparer; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class.
    /// Returns the hash code of this FormType, combined with the hash of data object.
    /// </summary>
    /// <returns>The resulting hash.</returns>
    public override int GetHashCode()
    {
        int nTemp1 = base.GetHashCode();
        int nTemp2 = DataComparer.GetHashCode(Data);
        int nResult = nTemp1 ^ nTemp2;

        return nResult;
    }

    /// <summary>
    /// Overrides the implementation of the base class, in order to support additional data in this class
    /// </summary>
    /// <param name="other">The object to compare with the current FormStackId. </param>
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(IStackId other)
    {
        Type otherType;
        bool result = false;

        if (other is null)
        {
            /* result = false; already is */
        }
        else if (ReferenceEquals(this, other))
        {
            result = true;
        }
        else if (ReferenceEquals(typeof(FormStack.StackId), otherType = other.GetType()))
        {   // the other object type is just my base class StackId, so return false
            /* result = false; already is */
        }
        else if (ReferenceEquals(GetType(), otherType))
        {   // the other object type is directly of my type, call the method of IEquatable<FormStackId> 
            result = Equals(other as FormStackId<TDATA>);
        }
        else if (GetType().IsAssignableFrom(otherType))
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }

        return result;
    }
    #endregion // Methods

    #region IEquatable<FormStackId> Members

    /// <summary>
    /// Implementation of IEquatable{FormStackId}, 
    /// either called directly or from an overloaded Equals method 
    /// <code>
    ///  bool Equals(IStackId other)
    /// </code>
    /// </summary>
    /// <param name="other">The object to compare with the current FormStackId. </param>
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public virtual bool Equals(FormStackId<TDATA> other)
    {
        bool result = false;

        if (other is null)
        {
            /* result = false; already is */
        }
        else if (ReferenceEquals(this, other))
        {
            result = true;
        }
        else if (ReferenceEquals(GetType(), other.GetType()))
        { // the other object is exactly of my type; let's do it
            result = FormType.Equals(other.FormType) && DataComparer.Equals(Data, other.Data);
        }
        else
        { // delegate to the overload
            result = Equals(other as IStackId);
        }
        return result;
    }
    #endregion // IEquatable<FormStackId> Members
}
