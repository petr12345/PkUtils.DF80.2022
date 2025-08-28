/***************************************************************************************************************
*
* FILE NAME:   .\Extensions\Extensions.Collections.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains extension-methods class DictionaryExtensions
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Memberwise
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace PK.PkUtils.Extensions;

/// <summary> Static class implementing a dictionary extension methods. </summary>
public static class DictionaryExtensions
{
    #region Typedefs

    // Here I must declare my own delegate to mimic TryGetvalue for both Dictionary and IReadOblyDictionary.
    // Unfortunately they don't have common predecessor.
    private delegate bool FnTryGetValue<TKey, TValue>(TKey input, out TValue value);
    #endregion // Typedefs

    #region Extensions_common_for_both

    /// <summary>
    /// <br>
    /// An IDictionary and IReadOnlyDictionary extension method that converts a dictionary to a string.
    /// It's more convenient way than a default ToString(), which returns just type of the dictionary,
    /// i.e. something like "System.Collections.Generic.Dictionary`2[System.Int32,System.String]". </br>
    /// 
    /// <br>
    /// Assuming you have dictionary created like
    /// <code>
    /// <![CDATA[
    /// var myDict = new Dictionary<int, string> { {4, "a"}, {5, "b"} };
    /// ]]>
    /// </code>
    /// your output will be "{4=a,5=b}".
    /// </br>
    /// </summary>
    ///
    /// <typeparam name="TKey">     Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="leftBrace">    The left separating brace. </param>
    /// <param name="rightBrace">   The right separating brace. </param>
    ///
    /// <returns> Dictionary as a string. </returns>
    /// <exception cref="System.ArgumentNullException">Thrown when input argument <paramref name="dictionary"/> is null.</exception>        
    public static string ToStringEx<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
        string leftBrace = "{",
        string rightBrace = "}")
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        IEnumerable<string> items = from pair in dictionary select (pair.Key + "=" + pair.Value);

        return leftBrace + string.Join(",", items) + rightBrace;
    }

    /// <summary>
    /// <br>
    /// An IDictionary and IReadOnlyDictionary extension method that converts a dictionary to a string. It's more convenient way than a
    /// default ToString(), which returns just type of the dictionary,
    /// i.e. something like "System.Collections.Generic.Dictionary`2[System.Int32,System.String]". </br>
    /// 
    /// <br>
    /// Assuming you have dictionary created like
    /// <code>
    /// <![CDATA[
    /// var myDict = new Dictionary<int, string> { {4, "a"}, {5, "b"} };
    /// ]]>
    /// </code>
    /// your output will be "{4=a,5=b}".
    /// </br></summary>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="dictionary"> The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="fnKey2String"> Function converting the key to string. Can't be null.</param>
    /// <param name="fnValue2String"> Function converting the value to string. Can't be null.</param>
    /// 
    /// <param name="keyValueSeparator"> (Optional) Separator of the key and value in the output. </param>
    /// <param name="pairsSeparator"> (Optional) Separator of individual peirs in the output. </param>
    /// <param name="leftBrace"> (Optional) The left separating brace. </param>
    /// <param name="rightBrace"> (Optional) The right separating brace. </param>
    ///
    /// <returns> Dictionary as a string.</returns>
    public static string ToStringEx<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
        Func<TKey, string> fnKey2String,
        Func<TValue, string> fnValue2String,
        string keyValueSeparator = "=",
        string pairsSeparator = ",",
        string leftBrace = "{",
        string rightBrace = "}")
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(fnValue2String);

        IEnumerable<string> items = from pair in dictionary select (fnKey2String(pair.Key) + keyValueSeparator + fnValue2String(pair.Value));
        string result = leftBrace + string.Join(pairsSeparator, items) + rightBrace;

        return result;
    }
    #endregion // Extensions_common_for_both

    #region IDictionary_extensions

    /*  --------------------------------------------------------------  */
    /*
     * Already present in public static class CollectionExtensions in System.Collections.Generic
    */


    /// <summary>
    /// An extension method to get a dictionary Item by Key. If the Key does not exist, null is returned for
    /// reference types. For Value types, default value is returned (default(TValue)).
    /// </summary>
    ///
    /// <remarks>
    /// <br>
    /// The purpose of this extension is code simplification, assuming that default(TValue) is distinctive enough
    /// to supply non-existing value. With this method you need just one statement (method call) to get the item
    /// for the key, without need to declare the auxiliary variable, using that as out argument and accessing that. </br>
    /// <br>
    /// Note that method name intentionally differs from ValueOrDefault(IReadOnlyDictionary{TKey, TValue}, TKey).
    /// Otherwise, we could get compilation errors "The call is ambiguous between the following methods...",
    /// in case the ValueOrDefault is called with Dictionary instance as "this" argument. 
    /// This is because Dictionary is derived from both IReadOnlyDictionary and IReadOnlyDictionary.</br>
    /// </remarks>
    ///
    /// <typeparam name="TKey">     The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue">   The type of the values in the dictionary. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. </param>
    ///
    /// <returns>
    /// Existing value in the dictionary if the <paramref name="key"/> exists, or a default(TValue).
    /// </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        return ValueOrDefault<TKey, TValue>(dictionary, key, default);
    }

    /// <summary> Returns the value for the key, or the default value if not found. </summary>
    ///
    /// <typeparam name="TKey">     The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue">   The type of the values in the dictionary. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. </param>
    /// <param name="default">    The default value to be retrieved, if <paramref name="key"/> is not present. </param>
    ///
    /// <returns>
    /// Existing value in the dictionary if the <paramref name="key"/> exists, or a <paramref name="default"/>.
    /// </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue @default)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (dictionary.TryGetValue(key, out TValue value))
            return value;
        else
            return @default;
    }

    /*  --------------------------------------------------------------  */

    /// <summary> Returns existing value for given key if the value for given key exists;
    /// otherwise creates a new value and adds it to the dictionary.</summary>
    ///
    /// <typeparam name="TKey"> The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
    /// <param name="dictionary"> The modified dictionary. Must not equal to null. </param>
    /// <param name="key"> The key for which the existing or new value is retrieved. </param>
    ///
    /// <returns> Already existing value in the dictionary, or the new value added by this call.</returns>
    public static TValue GetValueOrNew<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (!dictionary.TryGetValue(key, out TValue value))
        {
            dictionary.Add(key, value = new TValue());
        }

        return value;
    }

    /// <summary>
    /// Adds a new value to a dictionary, or throws ArgumentException if the key is present already.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="dictionary"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when the <paramref name="key"/> is present already. </exception>
    ///
    /// <typeparam name="TKey">     The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue">   The type of the values in the dictionary. </typeparam>
    /// <param name="dictionary">   The modified dictionary. Must not equal to null. </param>
    /// <param name="key">          The key being added. </param>
    /// <param name="value">        The value being added. </param>
    public static void AddNew<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (dictionary.ContainsKey(key))
        {
            string errorMessage = string.Format(CultureInfo.InvariantCulture,
                "The key '{0}' is already present in dictionary", key);
            throw new ArgumentException(errorMessage, nameof(key));
        }
        dictionary.Add(key, value);
    }

    /// <summary>
    /// An IDictionary extension method that adds a range of key-value pairs <paramref name="source"/>
    /// to the target dictionary <paramref name="target"/>. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when any of input arguments is null. </exception>
    ///
    /// <typeparam name="TKey">     Type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue">   Type of the values in the dictionary. </typeparam>
    /// 
    /// <param name="target">   The target dictionary where new pairs are added. Can't be null. </param>
    /// <param name="source">   The source of pairs to be added to the dictionary. Can't be null. </param>
    public static void AddRange<TKey, TValue>(
        this IDictionary<TKey, TValue> target,
        IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        foreach (var pair in source)
        {
            target.Add(pair);
        }
    }

    /// <summary>
    /// Removing an existing key from the dictionary. Throws ArgumentException if the key is not present there.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary the key is removed from. Must not equal to null.</param>
    /// <param name="key">The key being removed.</param>
    public static void RemoveExisting<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (!dictionary.Remove(key))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The key '{0}' is not present in the dictionary", key);
            throw new ArgumentException(strErr, nameof(key));
        }
    }

    /// <summary> Attempts to remove and return the value that has the specified key from the. </summary>
    ///
    /// <typeparam name="TKey">     The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue">   The type of the values in the dictionary. </typeparam>
    /// <param name="dictionary">   The dictionary where the key is searched for. Must not equal to null. </param>
    /// <param name="key">          The key of the element to remove and return. </param>
    /// <param name="value">      [out] When this method returns, contains the object removed, or the default value
    /// of the TValue type if key does not exist. </param>
    ///
    /// <returns> true if the object was removed successfully; otherwise, false. </returns>
    public static bool TryRemove<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        bool result;

        if (result = dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
        }
        else
        {
            value = default;
        }

        return result;
    }

    /// <summary> An IDictionary extension method that tests for equality.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="dictionary"/> is null. </exception>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="dictionary"> The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="other"> The second dictionary. It may be null</param>
    /// <param name="valueComparer"> (Optional) The value comparer. If null, default will be used.</param>
    ///
    /// <returns> True if dictionaries are considered equal, false if not.</returns>
    public static bool MemberwiseEqual<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> other,
        IEqualityComparer<TValue> valueComparer = null)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        bool result;

        if (object.ReferenceEquals(dictionary, other))
            result = true;
        else if (other == null)
            result = false;
        else if (dictionary.Count != other.Count)
            result = false;
        else
        {
            // Rather elaborate, but IEnumerable<KeyValuePair<TKey, TValue>> has no TryGetValue method,
            // and IDictionary and IReadOnlyDictionary have no closer common predecessor
            // 
            bool FnTryGetValue(TKey key, out TValue value) => other.TryGetValue(key, out value);
            IEqualityComparer<TValue> finalComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            result = SequencesMemberwiseEqual<TKey, TValue>(dictionary, FnTryGetValue, finalComparer);
        }

        return result;
    }


    /// <summary> An IDictionary extension method that computes its hash code considering all keys and values.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="dictionary"/> is null. </exception>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="dictionary"> The dictionary where the value is returned from. Can't be null. </param>
    ///
    /// <returns> Resulting hash.</returns>
    public static int DictionaryHashCode<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        // Note: We compute hash code "manually" for each KeyValue pair, because of efficiency.
        // The reason is, the KeyValuePair structure does not override GetHashCode(), so involving that would involve
        // structure default implementation, which is based on reflection, hence quite slow.
        // For more info see for instance 
        // https://stackoverflow.com/questions/38250596/why-does-keyvaluepair-not-override-equals-and-gethashcode
        // 
        return dictionary.Aggregate(0, (aggregate, next) => aggregate ^ MakeHash(ref next));
    }
    #endregion // IDictionary_extensions

    #region IReadOnlyDictionary_extensions

    /*
     * Already present in public static class CollectionExtensions in System.Collections.Generic

    /// <summary>
    /// An extension method to get a dictionary Item by Key. If the Key does not exist, null is returned for
    /// reference types. For Value types, default value is returned (default(TValue)).
    /// </summary>
    ///
    /// <typeparam name="TKey">     Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. </param>
    ///
    /// <returns> A TValue. </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        return ValueOrDefault<TKey, TValue>(dictionary, key, default(TValue));
    }
    */

    /*

    /// <summary>
    /// An extension method to get a dictionary Item by Key. If the Key does not exist, 
    /// <paramref name="default"/> is returned.
    /// </summary>
    ///
    /// <typeparam name="TKey">     Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. </param>
    /// <param name="default">      The default value to be returned if <paramref name="key"/> is not present. </param>
    ///
    /// <returns> A TValue. </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue @default)
    {
        dictionary.CheckArgNotNull(nameof(dictionary));

        TValue value;

        if (dictionary.TryGetValue(key, out value))
            return value;
        else
            return @default;
    }
    */

    /// <summary>
    /// An extension method that converts a IReadOnlyDictionary to a dictionary.
    /// </summary>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="dictionary">   The dictionary where the value is returned from. Can't be null. </param>
    ///
    /// <returns> Dictionary as a Dictionary&lt;TKey,TValue&gt; </returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        IEqualityComparer<TKey> comparer = (dictionary as Dictionary<TKey, TValue>)?.Comparer;
        comparer ??= EqualityComparer<TKey>.Default;
        return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
    }

    /// <summary> An IReadOnlyDictionary extension method that tests for equality.</summary>
    /// <remarks>
    /// The method needs a different name from MemberwiseEqual on not read-only dictionaries, 
    /// to avoid ambiguities when calling
    /// Error	CS0121	The call is ambiguous between the following methods or properties: ...
    /// </remarks>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="dictionary"/> is null. </exception>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="dictionary"> The dictionary where the value is returned from. Can't be null. </param>
    /// <param name="other"> The second dictionary. </param>
    /// <param name="valueComparer"> (Optional) The value comparer. If null, default will be used.</param>
    ///
    /// <returns> True if dictionaries are considered equal, false if not.</returns>
    public static bool MemberwiseEqualReadOnly<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        IReadOnlyDictionary<TKey, TValue> other,
        IEqualityComparer<TValue> valueComparer = null)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        bool result;

        if (object.ReferenceEquals(dictionary, other))
            result = true;
        else if (other == null)
            result = false;
        else if (dictionary.Count != other.Count)
            result = false;
        else
        {
            // Rather elaborate, but IEnumerable<KeyValuePair<TKey, TValue>> has no TryGetValue method,
            // and IDictionary and IReadOnlyDictionary have no closer common predecessor
            // 
            bool FnTryGetValue(TKey key, out TValue value) => other.TryGetValue(key, out value);
            IEqualityComparer<TValue> finalComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            result = SequencesMemberwiseEqual<TKey, TValue>(dictionary, FnTryGetValue, finalComparer);
        }

        return result;
    }

    /// <summary> An IReadOnlyDictionary extension method that computes its hash code considering all keys and values.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="dictionary"/> is null. </exception>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="dictionary"> The dictionary where the value is returned from. Can't be null. </param>
    ///
    /// <returns> Resulting hash.</returns>
    public static int DictionaryHashCode<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        return dictionary.Aggregate(0, (aggregate, next) => aggregate ^ MakeHash(ref next));
    }
    #endregion // IReadOnlyDictionary_extensions

    #region Private_methods

    /// <summary> Makes a hash from KeyValuePair.</summary>
    ///
    /// <remarks> Intentionally, this is not an extension method, because so far,
    ///  the structure is passed into the extension method in C# always slowly BY VALUE.
    ///  For more info, see for instance
    ///  https://github.com/dotnet/csharplang/issues/186
    ///  https://stackoverflow.com/questions/4656222/extension-methods-on-a-struct
    /// </remarks>
    ///
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <param name="pair"> [in,out] The pair the has is computed from. Its key cannot be null, but the value could be null. </param>
    ///
    /// <returns> Resulting hash.</returns>
    private static int MakeHash<TKey, TValue>(ref KeyValuePair<TKey, TValue> pair)
    {
        return (pair.Key.GetHashCode() ^ ((pair.Value == null) ? 0 : pair.Value.GetHashCode()));
    }

    private static bool SequencesMemberwiseEqual<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
        FnTryGetValue<TKey, TValue> fnTryGetOtherValue,
        IEqualityComparer<TValue> valueComparer)
    {
        Debug.Assert(dictionary != null);
        Debug.Assert(fnTryGetOtherValue != null);
        Debug.Assert(valueComparer != null);

        foreach (var kvp in dictionary)
        {
            if (!fnTryGetOtherValue(kvp.Key, out TValue value2))
                return false;
            if (!valueComparer.Equals(kvp.Value, value2))
                return false;
        }

        return true;
    }

    #endregion // Private_methods
}