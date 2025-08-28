// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Cmd;

/// <summary> Interface for command-line parameters provider. </summary>
public interface ICmdParametersProvider
{
    /// <summary> Gets a parameter value of given <paramref name="name"/>. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="name"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="name"/> is empty. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="name"> The parameter name. Can't be null or empty. </param>
    ///
    /// <returns> The parameter value. </returns>
    T GetParameter<T>(string name);

    /// <summary> Gets parameter value or default value <paramref name="defaultValue"/>. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="name"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="name"/> is empty. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="name"> The parameter name. Can't be null or empty. </param>
    /// <param name="defaultValue"> The default value. </param>
    ///
    /// <returns> The retrieved parameter value or default <paramref name="defaultValue"/>. </returns>
    T GetParameterOrDefault<T>(string name, T defaultValue);

    /// <summary> Attempts to get parameter value T from the given parameter name. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="name"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="name"/> is empty. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="name"> The parameter name. Can't be null or empty. </param>
    /// <param name="value">    [out] The retrieved value. </param>
    ///
    /// <returns> True if it succeeds, false if it fails. </returns>
    bool TryGetParameter<T>(string name, out T value);
}