// Ignore Spelling: Utils
//
namespace PK.PkUtils.Extensions;

/// <summary> Static class implementing extension methods on char. </summary>
public static class CharExtensions
{
    /// <summary> Query if <paramref name="ch"/> is alpha numeric or underscore character. </summary>
    /// 
    /// <param name="ch">  The examined character. </param>
    /// <returns>   True if alpha numeric or underscore, false if not. </returns>
    public static bool IsAlphaNumericOrUnderscore(this char ch)
    {
        return (char.IsLetterOrDigit(ch) || ch.Equals('_'));
    }
}