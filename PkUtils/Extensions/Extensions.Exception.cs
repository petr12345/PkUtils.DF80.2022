// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PK.PkUtils.Extensions;

/// <summary>
/// Implements extension methods for Exception
/// </summary>
public static class ExceptionExtension
{
    #region Fields

    private const string _seeInnerDetails = "See the inner exception for details.";
    #endregion // Fields

    #region Methods

    /// <summary> Enumerates all inner exceptions in <paramref name="ex"/>. </summary>
    ///
    /// <param name="ex">  The exception this is all about. Can be null. </param>
    /// <param name="includeThisOne"> (Optional) True to include, false to exclude this <paramref name="ex"/>. </param>
    ///
    /// <returns>   An enumerator that allows foreach to be used to process all inner exceptions. </returns>
    public static IEnumerable<Exception> AllInnerExceptions(this Exception ex, bool includeThisOne = true)
    {
        // iterates only over items required by the caller
        Exception exInner;

        if (includeThisOne && (ex != null))
        {
            yield return ex;
        }

        for (Exception current = ex; current != null;)
        {
            if (null != (exInner = current.InnerException))
                yield return (current = exInner);
            else
                yield break;
        }
    }

    /// <summary>
    /// Retrieves the most inner exception for a given exception.
    /// In case there is no inner exception, returns the original argument.
    /// </summary>
    /// <param name="ex">The exception this is all about.</param>
    /// <returns>The most inner exception that can be found 'inside' <paramref name="ex"/></returns>
    public static Exception MostInnerException(this Exception ex)
    {
        return ex.AllInnerExceptions(true).LastOrDefault();
    }

    /// <summary> Get the text (message) of the exception and inner exceptions. </summary>
    ///
    /// <param name="ex">                   The exception this is all about. </param>
    /// <param name="includeStackTrace"> If true, the output text will contain stack trace of this exception and
    /// all involved inner exceptions. </param>
    ///
    /// <returns>   The text representation of the exception and inner exceptions. </returns>
    public static string ExceptionDetails(this Exception ex, bool includeStackTrace)
    {
        string strType, strMsg;
        StringBuilder sbErr = new();

        for (Exception exTmp = ex; exTmp != null;)
        {
            strType = exTmp.GetType().ToString();
            strMsg = exTmp.Message;

            if ((exTmp.InnerException != null) &&
                (0 <= strMsg.IndexOf(_seeInnerDetails, StringComparison.OrdinalIgnoreCase)))
            {
                strMsg = strMsg.Replace(_seeInnerDetails, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            if (includeStackTrace)
            {
                sbErr.AppendFormat(
                  CultureInfo.InvariantCulture,
                  "Exception Type: {0}{2}Exception Message: {1}{2}StackTrace: {3}",
                  strType,
                  strMsg,
                  Environment.NewLine,
                  exTmp.StackTrace);
            }
            else
            {
                sbErr.AppendFormat(
                  CultureInfo.InvariantCulture,
                  "Exception Type {0}{2}Exception Message: {1}{2}",
                  strType,
                  strMsg,
                  Environment.NewLine);
            }
            if (null != (exTmp = exTmp.InnerException))
            {
                sbErr.AppendLine();
                sbErr.Append("====Inner exception:");
            }
        }

        return sbErr.ToString();
    }

    /// <summary> Get the text (message) of the exception and inner exceptions. </summary>
    /// 
    /// <param name="ex">The exception this is all about.</param>
    /// <returns>The text representation of the exception and inner exceptions.</returns>
    /// <remarks>Gets the overloaded method, with the second argument includeStackTrace = true</remarks>
    public static string ExceptionDetails(this Exception ex)
    {
        return ExceptionDetails(ex, true);
    }
    #endregion // Methods
}
