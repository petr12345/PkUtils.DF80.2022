// Ignore Spelling: Utils
// 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.FormattableString;

namespace PK.PkUtils.Extensions;

/// <summary> A static class implementing Enum-related methods and extensions. </summary>
[CLSCompliant(true)]
public static class EnumExtension
{
    #region Public Methods

    #region Getting_values

    /// <summary> Gets the individual flags of this enum value, assuming that T is an enum with flags. </summary>
    /// <typeparam name="T"> Generic type parameter - the type of the enum. </typeparam>
    /// <param name="value"> The input enum value. </param>
    /// <returns>  Collection o values of flags that are present in <paramref name="value"/>. </returns>
    public static IEnumerable<T> GetFlags<T>(T value) where T : Enum
    {
        CheckIsFlagsEnumType<T>(nameof(value));

        foreach (T flag in Enum.GetValues(value.GetType()))
        {
            if (value.HasFlag(flag))
                yield return flag;
        }
    }
    #endregion // Getting_values

    #region Checking_values

    /// <summary>
    /// Checks if value <paramref name="value"/> is a defined value in <typeparamref name="T"/>.
    /// Throws <see cref="ArgumentException"/> if not.
    /// </summary>
    ///
    /// <exception cref="ArgumentException">  Thrown when <paramref name="value"/> has illegal value. </exception>
    ///
    /// <typeparam name="T"> Generic type parameter - the type of the enum. </typeparam>
    /// <param name="value"> The value to be checked. </param>
    /// <param name="paramName"> Name of the parameter, used in ArgumentException. </param>
    /// <returns> The original value <paramref name="value"/>. </returns>
    public static T CheckIsDefinedValue<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))] string paramName = null) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            string errorMessage = Invariant($"Value '{value}' is not a member of {typeof(T).Name} enum.");
            throw new ArgumentException(errorMessage, paramName ?? nameof(value));
        }
        return value;
    }

    /// <summary> Query if 'value' is valid flag combination. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="value">    The value to be checked. </param>
    ///
    /// <returns>   True if valid flag combination, false if not. </returns>
    public static bool IsValidFlagsCombination<T>(this T value) where T : Enum
    {
        CheckIsFlagsEnumType<T>(nameof(value));

        IEnumerable<uint> flagsVals = GetFlags<T>(value).Select(x => Convert.ToUInt32(x, CultureInfo.InvariantCulture));
        uint flagsSuma = flagsVals.Aggregate((uint)0, (total, next) => total | next);
        bool result = (flagsSuma == Convert.ToUInt32(value, CultureInfo.InvariantCulture));

        return result;
    }

    /// <summary>
    /// Checks if value <paramref name="value"/> is a flag or combination of flags of <typeparamref name="T"/>.
    /// Throws <see cref="ArgumentException"/> if not.
    /// </summary>
    ///
    /// <exception cref="ArgumentException">  Thrown when <paramref name="value"/> has illegal value. </exception>
    ///
    /// <typeparam name="T"> Generic type parameter - the type of the enum. </typeparam>
    /// <param name="value"> The value to be checked. </param>
    /// <param name="paramName"> Name of the parameter, used in ArgumentException. </param>
    /// <returns> The original value <paramref name="value"/>. </returns>
    public static T CheckIsValidFlagsCombination<T>(this T value, string paramName = null) where T : Enum
    {
        if (!IsValidFlagsCombination(value))
        {
            string errorMessage = Invariant($"Value '{value}' is not valid combination of flags of {typeof(T).Name} enum.");
            throw new ArgumentException(errorMessage, paramName ?? nameof(value));
        }
        return value;
    }

    /// <summary>
    /// Checks if value <paramref name="value"/> is valid enum value.
    /// Throws <see cref="ArgumentException"/> if not.
    /// </summary>
    ///
    /// <exception cref="ArgumentException">  Thrown when <paramref name="value"/> has illegal value. </exception>
    ///
    /// <typeparam name="T"> Generic type parameter - the type of the enum. </typeparam>
    /// <param name="value"> The value to be checked. </param>
    /// <param name="paramName"> Name of the parameter, used in ArgumentException. </param>
    /// <returns> The original value <paramref name="value"/>. </returns>
    public static T CheckIsValidEnum<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))] string paramName = null) where T : Enum
    {
        if (HasFlagsAttribute(typeof(T)))
        {
            return CheckIsValidFlagsCombination<T>(value, paramName);
        }
        else
        {
            return CheckIsDefinedValue<T>(value, paramName);
        }
    }
    #endregion // Checking_values

    #region Conversions

    /// <summary>
    /// Parses <paramref name="value"/> as an enum of type <typeparamref name="T"/>. Throws exception if value
    /// can't be parsed to given enum.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when <paramref name="value"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="value"/> is not null, but can't be
    /// converted to <typeparamref name="T"/>. </exception>
    ///
    /// <typeparam name="T">    The type of the target enum. </typeparam>
    /// <param name="value">    The string value to be parsed. </param>
    /// <param name="ignoreCase">   (Optional) True to ignore case. </param>
    /// <remarks> By default, ignoreCase = false, because an enum can contain values which differ only in case. </remarks>
    ///
    /// <returns>   A resulting T value. </returns>
    public static T Parse<T>(string value, bool ignoreCase = false) where T : Enum
    {
        // Parse throws ArgumentNullException, ArgumentException, OverflowException
        // https://msdn.microsoft.com/en-us/library/essfb559(v=vs.110).aspx
        // 
        T result = (T)Enum.Parse(typeof(T), value, ignoreCase);

        // Still must handle the case when the caller used as input string a numeric literal, like "1000"
        return result.CheckIsValidEnum();
    }

    /// <summary> Parses <paramref name="value"/> as an enum of type <typeparamref name="T"/>. 
    ///            If value can't be parsed to given enum, returns default(T).
    /// </summary>
    ///
    /// <typeparam name="T"> The type of the target enum. </typeparam>
    /// <param name="value"> The string value to be parsed. </param>
    /// <param name="fallBackValue"> The fall-back value. </param>
    /// <param name="ignoreCase"> (Optional) True to ignore case. </param>
    /// <remarks> By default, ignoreCase = false, because an enum can contain values which differ only in case. </remarks>
    ///
    /// <returns>   A resulting T value. </returns>
    public static T ToEnum<T>(
        string value, T fallBackValue = default, bool ignoreCase = false) where T : struct, Enum
    {
        T? result = ToNullableEnum<T>(value, ignoreCase);
        return result ?? fallBackValue;
    }

    /// <summary> Parses <paramref name="value"/> as an enum of type <typeparamref name="T"/>. 
    ///            If value can't be parsed to given enum, returns null.
    /// </summary>
    ///
    /// <typeparam name="T"> The type of the target enum. </typeparam>
    /// <param name="value">  The string value to be parsed. </param>
    /// <param name="ignoreCase"> (Optional) True to ignore case. </param>
    /// <remarks> By default, ignoreCase = false, because an enum can contain values which differ only in case. </remarks>
    ///
    /// <returns> A Nullable{T} which contains either resulting T value or null.</returns>
    public static Nullable<T> ToNullableEnum<T>(string value, bool ignoreCase = false) where T : struct, Enum
    {
        if (Enum.TryParse(value, ignoreCase, out T result))
        {
            if (HasFlagsAttribute(typeof(T)))
            {
                if (IsValidFlagsCombination<T>(result))
                {
                    return result;
                }
            }
            else
            {
                return Enum.IsDefined(typeof(T), result) ? result : null;
            }
        }

        return null;
    }
    #endregion // Conversions
    #endregion // Public Methods

    #region Private Methods

    private static void CheckIsFlagsEnumType<T>(string argName) where T : Enum
    {
        if (!HasFlagsAttribute(typeof(T)))
        {
            string errorMessage = Invariant($"The type {typeof(T)} is does not have flags attribute.");
            throw new ArgumentException(errorMessage, argName);
        }
    }

    private static bool HasFlagsAttribute(Type t)
    {
        return (t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
    }
    #endregion // Private Methods
}
