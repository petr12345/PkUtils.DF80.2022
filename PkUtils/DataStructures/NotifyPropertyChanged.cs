// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
///    set { SetField(ref _name, value); }
///  }
/// ]]>
/// </code>
/// </summary>
public abstract class NotifyPropertyChanged : INotifyPropertyChanged
{
    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived classes.
    /// Enables property name verification in DEBUG builds.
    /// </summary>
    protected NotifyPropertyChanged()
    {
#if DEBUG
        VerifyPropertyNames = true;
#endif
    }

    /// <summary> Specialized constructor for use only by derived classes. 
    /// Initializes property name verification. 
    /// </summary>
    /// <param name="verifyPropertyNames">
    /// <see langword="true"/> to validate property names when raising the
    /// <see cref="PropertyChanged"/> event; otherwise <see langword="false"/>.
    /// </param>
    protected NotifyPropertyChanged(bool verifyPropertyNames)
    {
        VerifyPropertyNames = verifyPropertyNames;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Occurs when a property value changes. </summary>
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets a value indicating whether property names are validated
    /// when raising the <see cref="PropertyChanged"/> event.
    /// </summary>
    protected bool VerifyPropertyNames { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary> Raises the <see cref="PropertyChanged"/> event for the specified property. </summary>
    /// <param name="propertyName"> Name of the property that changed. </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (VerifyPropertyNames)
        {
            VerifyPropertyName(propertyName);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary> Sets a new value of a property <paramref name="propertyName"/>. </summary>
    /// <remarks>
    /// Delegates its functionality to the overload having the IEqualityComparer argument.
    /// </remarks>
    /// <typeparam name="T"> Generic type parameter which is the type of modified property. </typeparam>
    /// <param name="field"> [in,out] The backup field implementing the property. </param>
    /// <param name="value"> The new value. </param>
    /// <param name="propertyName"> Name of the property. If omitted, the caller member name is used.</param>
    /// <returns>
    /// true if it modified the property to a new value, false if the <paramref name="value"/> equals to
    /// the old value.
    /// </returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        return SetField<T>(ref field, value, EqualityComparer<T>.Default, propertyName);
    }

    /// <summary> Sets a new value of a property <paramref name="propertyName"/>. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="comparer"/> argument is null. 
    /// </exception>
    /// <typeparam name="T"> Generic type parameter which is the type of modified property. </typeparam>
    /// <param name="field"> [in,out] The backup field implementing the property. </param>
    /// <param name="value"> The new value. </param>
    /// <param name="propertyName"> Name of the property. If omitted, the caller member name is used.</param>
    /// <param name="comparer"> The comparer used for comparison old and new property value. </param>
    /// <returns>
    /// true if it modified the property to a new value, false if the <paramref name="value"/> equals to
    /// the old value.
    /// </returns>
    protected bool SetField<T>(
        ref T field,
        T value,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        bool result;

        if (result = !comparer.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
        return result;
    }

    /// <summary> Sets a new value of a property and raises the <see cref="PropertyChanged"/> event
    /// for the property and any specified related properties.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter representing the type of the modified property. </typeparam>
    /// <param name="field"> Reference to the backing field implementing the property. </param>
    /// <param name="value"> The new value. </param>
    /// <param name="propertyName"> Name of the property. If omitted, the caller member name is used. </param>
    /// <param name="relatedProperties">
    /// Names of additional properties whose values are affected by this
    /// property change and for which <see cref="PropertyChanged"/> should
    /// also be raised.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the property value was modified;
    /// otherwise <see langword="false"/>.
    /// </returns>
    protected bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string propertyName = null,
        params string[] relatedProperties)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            if (relatedProperties != null)
            {
                foreach (string relatedProperty in relatedProperties)
                {
                    OnPropertyChanged(relatedProperty);
                }
            }

            return true;
        }

        return false;
    }

    /// <summary> Verifies if a real, public property of the given name exists. </summary>
    /// <param name="propertyName"> The property name. </param>
    /// <remarks>
    /// Be aware The PropertyChanged event can indicate all properties on the object have changed by using either 
    /// null or String.Empty as the property name in the PropertyChangedEventArgs. For more info see
    /// For more info see
    /// <see href="https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.propertychanged(v=vs.110).aspx">
    /// INotifyPropertyChanged.PropertyChanged Event </see>.
    /// </remarks>
    protected void VerifyPropertyName(string propertyName)
    {
        if (!string.IsNullOrEmpty(propertyName))
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new InvalidOperationException($"Invalid property name: '{propertyName}'");
            }
        }
    }
    #endregion // Methods
}
