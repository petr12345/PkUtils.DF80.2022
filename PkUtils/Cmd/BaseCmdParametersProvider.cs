// Ignore Spelling: Utils, Comparers
// 

using PK.PkUtils.Extensions;

namespace PK.PkUtils.Cmd;


/// <summary> A base command parameters provider, implementing <see cref="ICmdParametersProvider"/>.
/// 
/// This is an abstract class, with abstract method <see cref="GetStringParameter"/>.
/// The only thing derived class needs to do is to implement that method.
/// </summary>
public abstract class BaseCmdParametersProvider : ICmdParametersProvider
{
    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived class. </summary>
    protected BaseCmdParametersProvider()
    { }
    #endregion // Constructor(s)

    #region ICmdParametersProvider members

    /// <inheritdoc/>
    public T GetParameter<T>(string name)
    {
        return GetStringParameter(name).ToType<T>();
    }

    /// <inheritdoc/>
    public T GetParameterOrDefault<T>(string name, T defaultValue)
    {
        return TryGetParameter(name, out T value) ? value : defaultValue;
    }

    /// <inheritdoc/>
    public bool TryGetParameter<T>(string name, out T value)
    {
        string stringValue = GetStringParameter(name);
        bool result;

        if (stringValue == null)
        {
            value = default;
            result = false;
        }
        else
        {
            result = stringValue.TryToType(out value);
        }

        return result;
    }
    #endregion // ICmdParametersProvider members

    #region Methods

    /// <summary> Gets parameter value as string. </summary>
    ///
    /// <param name="name"> The parameter name. Can't be null or empty. </param>
    ///
    /// <returns>   The string parameter value. </returns>
    protected abstract string GetStringParameter(string name);
    #endregion // Methods
}