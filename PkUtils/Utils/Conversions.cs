/***************************************************************************************************************
*
* FILE NAME:   .\Utils\Conversions.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains the static class Conversions
*   
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Cloneable
//

using System;
using System.Globalization;

namespace PK.PkUtils.Utils;

/// <summary>
/// Wrapper class around various conversions implemented
/// </summary>
public static class Conversions
{
    #region Public Methods
    /// <summary>
    /// Get human-readable string representation of integer.
    /// For instance, for the input value like 1234567, returns "1 234 567".
    /// </summary>
    /// <param name="provider">An IFormatProvider that supplies culture-specific 
    /// formatting information.</param>
    /// <param name="n">The input integer value.</param>
    /// <returns> "Human-readable" string representation of integer.</returns>
    public static string IntegerToReadable(int n, IFormatProvider provider)
    {
        string strRet;

        if (n < 0)
        {
            strRet = "-" + IntegerToReadable(-n, provider);
        }
        else if (n > 0)
        {
            strRet = n.ToString("#,#", provider);
            strRet = strRet.Replace(',', ' ');
        }
        else
        { // I must handle the case zero separately.
          // In that case, the above code for some weird reason returns just an empty string.
            strRet = n.ToString(provider);
        }
        return strRet;
    }

    /// <summary>
    /// Get human-readable string representation of integer.
    /// Delegates the functionality to overloaded method,
    /// with value of the IFormatProvider argument as CultureInfo.InvariantCulture.
    /// </summary>
    /// <param name="n">The input integer value.</param>
    /// <returns> "Human-readable" string representation of integer.</returns>
    public static string IntegerToReadable(int n)
    {
        return IntegerToReadable(n, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Get human-readable string representation of long.
    /// For instance, for the input value like 1234567, returns "1 234 567".
    /// </summary>
    /// <param name="provider">An IFormatProvider that supplies culture-specific 
    /// formatting information.</param>
    /// <param name="n">The input long value.</param>
    /// <returns> "Human-readable" string representation of long.</returns>
    public static string LongToReadable(long n, IFormatProvider provider)
    {
        string strRet;

        if (n < 0)
        {
            strRet = "-" + LongToReadable(-n, provider);
        }
        else if (n > 0)
        {
            strRet = n.ToString("#,#", provider);
            strRet = strRet.Replace(',', ' ');
        }
        else
        { // I must handle the case zero separately.
          // In that case, the above code for some weird reason returns just an empty string.
            strRet = n.ToString(provider);
        }
        return strRet;
    }

    /// <summary>
    /// Get human-readable string representation of long.
    /// Delegates the functionality to overloaded method,
    /// with value of the first argument as CultureInfo.InvariantCulture.
    /// </summary>
    /// <param name="n">The input long value.</param>
    /// <returns> "Human-readable" string representation of long.</returns>
    public static string LongToReadable(long n)
    {
        return LongToReadable(n, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns the string representing current time of the day, given the DateTime.
    /// </summary>
    /// <param name="now">Represents an instant in time.</param>
    /// <returns>A string like "14:04:09"</returns>
    public static string DayTimeString(DateTime now)
    {
        TimeSpan ts = now.TimeOfDay;
        string strResult = string.Format(CultureInfo.InvariantCulture,
          "{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        return strResult;
    }
    #endregion // Public Methods
}
