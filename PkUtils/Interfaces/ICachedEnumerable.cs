/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\ICachedEnumerable.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of generic interface ICachedEnumerable<T>
*
**************************************************************************************************************/


// Ignore Spelling: Utils
//
using System;
using System.ComponentModel;

namespace PK.PkUtils.Interfaces;

/// <summary> Values that represent current status of <see cref="ICachedEnumerable{T}"/>. </summary>
[CLSCompliant(true)]
public enum ParseStatus
{
    /// <summary> An enum constant representing the 'parse not initialized' case. </summary>
    ParseNotInitialized,

    /// <summary> An enum constant representing the 'currently parsing' case. </summary>
    Parsing,

    /// <summary> An enum constant representing the 'parsing ended with Ok' case.
    /// The internal buffer is completely filled ( till someone calls ResetCache ).
    /// </summary>
    ParsedOk,

    /// <summary> An enum constant representing the 'parsing stopped prematurely' case. </summary>
    ParsePrematureEnd,
};

/// <summary> A generic interface which extends <see cref="IPeekAbleEnumerable{T}"/> interface.
/// It is assumed that implementing class atop on generic IEnumerator builds a cache of items
/// parsed so far. Changes of status of cache are communicated through INotifyPropertyChanged.
/// </summary>
/// 
/// <typeparam name="T"> Generic type parameter; the type of objects to enumerate.
/// <para>
/// This type parameter is covariant, hence assignment compatibility is preserved.
/// That means, one can assign object that is instantiated with a more derived type argument
/// to an object instantiated with a less derived type argument like following.
/// <code>
/// <![CDATA[
/// MyRectangle[] arrInput = { new MyRectangle(5), new MyRectangle(6), new MyRectangle(7) };
/// 
/// // enumerable with more derived type
/// IPeekAbleEnumerable<MyRectangle> enDataRectangles = new CachedEnumerable<MyRectangle>(arrInput);
/// // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
/// IPeekAbleEnumerable<MyShape> enDatShapes = enDataRectangles;
/// ]]>
/// </code>
/// </para>
/// 
/// For more information about covariance and contravariance, see
/// <see href="https://msdn.microsoft.com/en-us/library/dd799517(v=vs.110).aspx">
/// Covariance and Contravariance in Generics</see>
/// </typeparam>
[CLSCompliant(true)]
public interface ICachedEnumerable<out T> : IPeekAbleEnumerable<T>, INotifyPropertyChanged
{
    /// <summary> Gets the current parse status. </summary>
    ParseStatus Status { get; }

    /// <summary> Gets the number of currently cached items. </summary>
    int CachedItemsCount { get; }

    /// <summary> Resets the cache of found items. </summary>
    void ResetCache();

    /// <summary> Attempts to fill the internal buffer to make it increase size to <paramref name="newLength"/>.
    /// If the buffer already has such size or even bigger, nothing is changed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="newLength"/> is negative. 
    /// </exception>
    ///
    /// <param name="newLength"> Maximal new length of the internal buffer.</param>
    /// <returns> An new length of the buffer. </returns>
    int FillBuffer(int newLength = int.MaxValue);

    /// <summary>
    /// Resumes the parsing for case the current <see cref="Status"/> is ParseStatus.ParsePrematureEnd.<br/>
    /// </summary>
    /// <remarks>
    /// It is assumed that parsing could be resumed if since the time the cache ended-up in premature end
    /// state, some internal data has changed, and now parsing could continue again. </remarks>
    /// <returns> true if it succeeds, false if it fails. </returns>
    bool ResumeParsing();
}
