/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\IPeekAbleEnumerator.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interfaces IPeekAbleEnumerator, IPeekAbleEnumerator<T>,
*              IPeekAbleIEnumerable, IPeekAbleIEnumerable<T>
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections;
using System.Collections.Generic;


namespace PK.PkUtils.Interfaces;

#region IPeekAbleEnumerator interface

/// <summary> Non-generic peek-able enumerator interface, extending non-generic IEnumerator. </summary>
[CLSCompliant(true)]
public interface IPeekAbleEnumerator : IEnumerator
{
    /// <summary> Gets a value indicating whether we can peek. 
    /// If there is any next value that we could get either by MoveNext + Current, or directly by Peek,
    /// returns true; otherwise returns false;
    /// </summary>
    /// <value> true if we can peek, false if not. </value>
    bool CanPeek { get; }

    /// <summary> Peeks the next value, if there is any. 
    /// If there is no such value available, throws <see cref="InvalidOperationException"/>.
    /// </summary>
    ///
    /// <value> The peeked next value. </value>
    object Peek { get; }
}
#endregion // IPeekAbleEnumerator interface

#region IPeekAbleEnumerator<T> interface

/// <summary> Generic peek-able enumerator interface, extending generic IEnumerator{T}. </summary>
///
/// <typeparam name="T"> Generic type parameter; the type of objects to enumerate.
/// <para>
/// This type parameter is covariant, hence assignment compatibility is preserved.
/// That means, one can assign object that is instantiated with a more derived type argument
/// to an object instantiated with a less derived type argument like following.
/// <code>
/// <![CDATA[
/// MyRectangle[] arrInput = { new MyRectangle(5), new MyRectangle(6), new MyRectangle(7) };
/// CachedEnumerable<MyRectangle> enData = new CachedEnumerable<MyRectangle>(arrInput);
/// IPeekAbleEnumerator<MyRectangle> enRects = enData.GetPeekAbleEnumerator();
/// 
/// // makes sense, as MyRectangle derives from MyShape, but covariance is needed to let compiler allow that
/// IPeekAbleEnumerator<MyShape> enShapes = enRects;
/// ]]>
/// </code>
/// </para>
/// 
/// For more information about covariance and contravariance, see
/// <see href="https://msdn.microsoft.com/en-us/library/dd799517(v=vs.110).aspx">
/// Covariance and Contravariance in Generics</see>
/// </typeparam>
[CLSCompliant(true)]
public interface IPeekAbleEnumerator<out T> : IEnumerator<T>, IPeekAbleEnumerator
{
    /// <summary> Peeks the next value, if there is any. 
    /// If there is no such value available, throws <see cref="InvalidOperationException"/>.
    /// </summary>
    ///
    /// <value> The peeked next value. </value>
    new T Peek { get; }
}
#endregion // IPeekAbleEnumerator<T> interface

#region IPeekAbleEnumerable interface

/// <summary> Non-generic peek-able enumerable interface, extending non-generic IEnumerable. </summary>
[CLSCompliant(true)]
public interface IPeekAbleEnumerable : IEnumerable
{
    /// <summary> Returns a peek-able enumerator that could iterate through a non-generic sequence. </summary>
    /// <returns> The peek-able enumerator. </returns>
    IPeekAbleEnumerator GetPeekAbleEnumerator();
}
#endregion // IPeekAbleEnumerable interface

#region IPeekAbleEnumerable<T> interface

/// <summary> Generic peek-able enumerable interface, extending generic IEnumerable{T}. </summary>
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
public interface IPeekAbleEnumerable<out T> : IPeekAbleEnumerable, IEnumerable<T>
{
    /// <summary> Returns a peek-able enumerator that could iterate through a generic sequence. </summary>
    /// <returns> The peek-able enumerator. </returns>
    new IPeekAbleEnumerator<T> GetPeekAbleEnumerator();
}
#endregion // IPeekAbleEnumerable<T> interface
