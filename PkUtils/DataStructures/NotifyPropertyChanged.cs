/***************************************************************************************************************
*
* FILE NAME:   .\DataStructures\NotifyPropertyChanged.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of NotifyPropertyChanged class
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace PK.PkUtils.DataStructures;

/// <summary>
/// A base class implementing INotifyPropertyChanged. A derived class supporting that interface could implement
/// its properties in a single line of code, like
/// <code>
/// <![CDATA[
/// 
///  private string _name;
///  public string Name
///  {
///    get { return _name; }
///    set { SetField(ref _name, value, nameof(Name)); }
///  }
/// ]]>
/// </code>
/// </summary>
[CLSCompliant(true)]
[Serializable]
public abstract class NotifyPropertyChanged : INotifyPropertyChanged
{
    #region Protected Interface
    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived classes. </summary>
    protected NotifyPropertyChanged()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Occurs when a property value changes. </summary>
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion // Properties

    #region Methods

    /// <summary> Raises the <see cref="PropertyChanged"/> event, delegating the functionality to method
    ///  <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/>. </summary>
    /// <param name="propertyName"> Name of the property. </param>
    protected virtual void RaisePropertyChanged(string propertyName)
    {
        /* VerifyPropertyName(propertyName);  is called from inside OnPropertyChanged */
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary> Raises the <see cref="PropertyChanged"/> event. </summary>
    /// <param name="args"> Event information to send to registered event handlers. </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        // test name again just for case of call without involving RaisePropertyChanged
        VerifyPropertyName(args.PropertyName);
        PropertyChangedEventHandler handler = PropertyChanged;
        handler?.Invoke(this, args);
    }

    /// <summary> Sets a new value of a property <paramref name="propertyName"/>. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="comparer"/> argument is null. 
    /// </exception>
    /// <typeparam name="T"> Generic type parameter which is the type of modified property. </typeparam>
    /// <param name="field"> [in,out] The backup field implementing the property. </param>
    /// <param name="value"> The new value. </param>
    /// <param name="propertyName"> Name of the property. </param>
    /// <param name="comparer"> The comparer used for comparison old and new property value. </param>
    /// <returns>
    /// true if it modified the property to a new value, false if the <paramref name="value"/> equals to
    /// the old value.
    /// </returns>
    protected virtual bool SetField<T>(
        ref T field,
        T value,
        string propertyName,
        IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        bool bRes;

        if (bRes = !comparer.Equals(field, value))
        {
            field = value;
            RaisePropertyChanged(propertyName);
        }
        return bRes;
    }

    /// <summary> Sets a new value of a property <paramref name="propertyName"/>. </summary>
    /// <remarks>
    /// Delegates its functionality to the virtual overload having the IEqualityComparer argument.
    /// </remarks>
    /// <typeparam name="T"> Generic type parameter which is the type of modified property. </typeparam>
    /// <param name="field"> [in,out] The backup field implementing the property. </param>
    /// <param name="value"> The new value. </param>
    /// <param name="propertyName"> Name of the property. </param>
    /// <returns>
    /// true if it modified the property to a new value, false if the <paramref name="value"/> equals to
    /// the old value.
    /// </returns>
    protected bool SetField<T>(ref T field, T value, string propertyName)
    {
        return SetField<T>(ref field, value, propertyName, EqualityComparer<T>.Default);
    }

    /// <summary> Verifies if a real, public property of the given name exists. </summary>
    /// <param name="propertyName"> The property name. </param>
    /// <remarks>
    /// be aware The PropertyChanged event can indicate all properties on the object have changed by using either 
    /// null or String.Empty as the property name in the PropertyChangedEventArgs. For more info see
    /// For more info see
    /// <see href="https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.propertychanged(v=vs.110).aspx">
    /// INotifyPropertyChanged.PropertyChanged Event </see>.
    /// </remarks>
    [Conditional("DEBUG")]
    protected void VerifyPropertyName(string propertyName)
    {
        if (!string.IsNullOrEmpty(propertyName))
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string strErr = string.Format(CultureInfo.InvariantCulture, "Invalid property name: '{0}'", propertyName);
                Debug.Fail(strErr);
            }
        }
    }
    #endregion // Methods
    #endregion // Protected Interface
}
