// Ignore Spelling: Nullable
//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.FormattableString;

namespace PK.PkUtils.Extensions;

/// <summary> A static class implementing Enum-related methods and extensions. </summary>
public static class EnumExtensions
{
    #region Public Methods
    #region Getting_values

    /// <summary> Gets the individual flags of this enum value, assuming that T is an enum with flags. </summary>
    /// <typeparam name="T"> Generic type parameter - the type of the enum. </typeparam>
    /// <param name="value"> The input enum value. </param>
    /// <returns> Collection of individual, nonzero flags that are present in <paramref name="value"/>. </returns>
    public static IEnumerable<T> GetFlags<T>(T value) where T : Enum
    {
        CheckIsFlagsEnumType<T>(nameof(value));
        ulong valueBits = ToUInt64Bits(value);

        foreach (T flag in Enum.GetValues(typeof(T)))
        {
            ulong flagBits = ToUInt64Bits(flag);
            if (IsSingleBit(flagBits) && (valueBits & flagBits) == flagBits)
                yield return flag;
        }
    }

    #endregion // Getting_values
    #region Checking_values

    /// <summary>
    /// Checks if value <paramref name="value"/> is a defined value in <typeparamref name="T"/>.
    /// Throws <see cref="ArgumentException"/> if not.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="value"/> has illegal value. </exception>
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
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="value"> The value to be checked. </param>
    /// <returns> True if valid flag combination, false if not. </returns>
    public static bool IsValidFlagsCombination<T>(this T value) where T : Enum
    {
        CheckIsFlagsEnumType<T>(nameof(value));
        ulong definedBits = 0;
        foreach (T definedValue in Enum.GetValues(typeof(T)))
        {
            definedBits |= ToUInt64Bits(definedValue);
        }

        ulong valueBits = ToUInt64Bits(value);
        return (valueBits & ~definedBits) == 0;
    }

    /// <summary>
    /// Checks if value <paramref name="value"/> is a flag or combination of flags of <typeparamref name="T"/>.
    /// Throws <see cref="ArgumentException"/> if not.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="value"/> has illegal value. </exception>
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
    /// <exception cref="ArgumentException"> Thrown when <paramref name="value"/> has illegal value. </exception>
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
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="value"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="value"/> is not null, but can't be converted to <typeparamref name="T"/>. </exception>
    /// <typeparam name="T"> The type of the target enum. </typeparam>
    /// <param name="value"> The string value to be parsed. </param>
    /// <param name="ignoreCase"> (Optional) True to ignore case. </param>
    /// <remarks> By default, ignoreCase = false, because an enum can contain values which differ only in case. </remarks>
    /// <returns> A resulting T value. </returns>
    public static T Parse<T>(string value, bool ignoreCase = false) where T : Enum
    {
        T result = (T)Enum.Parse(typeof(T), value, ignoreCase);
        return result.CheckIsValidEnum();
    }

    /// <summary> Parses <paramref name="value"/> as an enum of type <typeparamref name="T"/>. If value can't be parsed, returns default(T). </summary>
    public static T ToEnum<T>(
        string value, T fallBackValue = default, bool ignoreCase = false) where T : struct, Enum
    {
        T? result = ToNullableEnum<T>(value, ignoreCase);
        return result ?? fallBackValue;
    }

    /// <summary> Parses <paramref name="value"/> as an enum of type <typeparamref name="T"/>. If value can't be parsed, returns null. </summary>
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
                return Enum.IsDefined(result) ? result : null;
            }
        }
        return null;
    }

    #endregion // Conversions
    #endregion // Public Methods
    #region Private Methods

    private static bool IsSingleBit(ulong value)
    {
        return value != 0 && (value & (value - 1)) == 0;
    }

    // Convert an enum via IConvertible; direct casts from a boxed enum to its underlying primitive throw InvalidCastException.
    // Signed values are masked to their declared underlying width so the highest-bit flag remains a single bit.
    private static ulong ToUInt64Bits<T>(T value) where T : Enum
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(T));
        switch (Type.GetTypeCode(underlyingType))
        {
            case TypeCode.SByte:
                return unchecked((ulong)Convert.ToInt64(value)) & 0xFFUL;
            case TypeCode.Int16:
                return unchecked((ulong)Convert.ToInt64(value)) & 0xFFFFUL;
            case TypeCode.Int32:
                return unchecked((ulong)Convert.ToInt64(value)) & 0xFFFFFFFFUL;
            case TypeCode.Int64:
                return unchecked((ulong)Convert.ToInt64(value));
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return Convert.ToUInt64(value);
            default:
                throw new InvalidOperationException($"Unsupported enum underlying type '{underlyingType}'.");
        }
    }

    private static void CheckIsFlagsEnumType<T>(string argName) where T : Enum
    {
        if (!HasFlagsAttribute(typeof(T)))
        {
            string errorMessage = Invariant($"The type {typeof(T)} does not have the Flags attribute.");
            throw new ArgumentException(errorMessage, argName);
        }
    }

    private static bool HasFlagsAttribute(Type t)
    {
        return t.IsDefined(typeof(FlagsAttribute), inherit: false);
    }

    #endregion // Private Methods
}
