// Ignore Spelling: Utils
//
using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace PK.PkUtils.Extensions;

/// <summary>
/// Static class containing methods extending the KeyedCollection generic
/// </summary>
[CLSCompliant(true)]
public static class KeyedCollectionExtensions
{
    /// <summary>
    /// Adding a new item to the KeyedCollection. Throws ArgumentException if the item is present already.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the KeyedCollection.</typeparam>
    /// <typeparam name="TValue">The type of the values in the KeyedCollection.</typeparam>
    /// <param name="keyedCollection">The KeyedCollection instance where the new item is added. 
    ///  Must not equal to null.</param>
    /// <param name="item">The new item to be added to the end of the KeyedCollection</param>
    public static void AddNew<TKey, TValue>(
        this KeyedCollection<TKey, TValue> keyedCollection,
        TValue item)
    {
        ArgumentNullException.ThrowIfNull(keyedCollection);

        if (keyedCollection.Contains(item))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The item '{0}' is already present in KeyedCollection", item);
            throw new ArgumentException(strErr, nameof(item));
        }
        keyedCollection.Add(item);
    }
}