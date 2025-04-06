// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;

namespace PK.PkUtils.Interfaces;

/// <summary> Interface for size limited cache. </summary>
///
/// <typeparam name="TKey"> Type of the key. </typeparam>
/// <typeparam name="TValue">   Type of the value. </typeparam>
public interface ISizeLimitedCache<TKey, TValue> : IDisposableEx
{
    /// <summary> Gets the current size of cache. </summary>
    int CurrentSize { get; }

    /// <summary> Gets or sets the number of maximums items in cache. That value must be positive. </summary>
    int MaxSize { get; set; }

    /// <summary> Gets a value indicating whether we should dispose evicted values. </summary>
    bool DisposesEvictedValues { get; }

    /// <summary> Gets or sets the element with the specified key. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="key"/> is null. </exception>
    /// <exception cref="KeyNotFoundException"> Thrown by getter when a supplied <paramref name="key"/> was not found. </exception>
    /// 
    /// <param name="key"> The key of the element to get or set. </param>
    /// <returns> The value associated with the specified key. </returns>
    TValue this[TKey key] { get; set; }

    /// <summary> Gets the value associated with the specified key. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="key"/> is null. </exception>
    ///
    /// <param name="key"> The key whose value to get. </param>
    /// <param name="value"> [out] The retrieved value, if any. </param>
    /// <returns> True if the cache contains an element with the specified key; otherwise, false. </returns>
    bool TryGetValue(TKey key, out TValue value);

    /// <summary> Adds an element with the provided key and value to the cache. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="key"/> is null. </exception>
    ///
    /// <param name="key"> The key to add. </param>
    /// <param name="value"> The value to add. </param>
    void AddOrModify(TKey key, TValue value);

    /// <summary> Removes the element with the specified key from the cache. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="key"/> is null. </exception>
    /// 
    /// <param name="key"> The key of the element to remove. </param>
    /// <returns>
    /// True if the element is successfully removed; otherwise, if the key was not found in the cache, returns false.
    /// </returns>
    bool Remove(TKey key);

    /// <summary> Clears the cache completely. </summary>
    void Clear();
}

/// <summary> A static class implementing extensions for ISizeLimitedCache. </summary>
public static class SizeLimitedCacheExtensions
{
    /// <summary> An ISizeLimitedCache{TKey, TValue} extension method that query if cache contains specified key. </summary>
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <typeparam name="TValue"> Type of the value. </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown when any of supplied arguments is null. </exception>
    /// 
    /// <param name="cache"> The cache to act on. Can't be null. </param>
    /// <param name="key"> The key to be located. </param>
    /// <returns> True if the cache contains an element with the key; otherwise, false. </returns>
    public static bool ContainsKey<TKey, TValue>(this ISizeLimitedCache<TKey, TValue> cache, TKey key)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(key);

        return cache.TryGetValue(key, out _);
    }

    /// <summary>
    /// An ISizeLimitedCache{TKey, TValue} extension method to get a cached value by <paramref name="key"/>.
    /// If the is not present in cache, null is returned for a reference type of <typeparamref name="TKey"/>
    /// For value types, a default value is returned ( default(TValue)).
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when any of supplied arguments is null. </exception>
    ///
    /// <typeparam name="TKey">     Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="cache">   The cache where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. </param>
    ///
    /// <returns> A TValue. </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this ISizeLimitedCache<TKey, TValue> cache,
        TKey key)
    {
        return ValueOrDefault<TKey, TValue>(cache, key, default);
    }

    /// <summary>
    /// An ISizeLimitedCache{TKey, TValue} extension method to get a cached value by <paramref name="key"/>.
    /// If the is not present in cache, a <paramref name="default"/> is returned.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when either <paramref name="cache"/> or <paramref name="key"/> is null. </exception>
    ///
    /// <typeparam name="TKey">     Type of the key. </typeparam>
    /// <typeparam name="TValue">   Type of the value. </typeparam>
    /// <param name="cache">   The cache where the value is returned from. Can't be null. </param>
    /// <param name="key">          The key for which the value is retrieved. Can't be null. </param>
    /// <param name="default">      The default value to be returned if <paramref name="key"/> is not present. </param>
    ///
    /// <returns> A TValue. </returns>
    public static TValue ValueOrDefault<TKey, TValue>(
        this ISizeLimitedCache<TKey, TValue> cache,
        TKey key,
        TValue @default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(key);

        if (cache.TryGetValue(key, out TValue value))
            return value;
        else
            return @default;
    }

}
