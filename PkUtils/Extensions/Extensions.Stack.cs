// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;


namespace PK.PkUtils.Extensions;

/// <summary> 
/// Static class containing methods extending Stack generic. 
/// </summary>
[CLSCompliant(true)]
public static class StackExtensions
{
    #region Public Methods

    /// <summary> A Stack&lt;T&gt; extension method that pushes a range of objects at the top 
    ///  of the Stack&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T"> The type of items in the stack. </typeparam>
    /// <param name="stack"> The stack to act on. </param>
    /// <param name="items"> The objects to push onto the stack. </param>
    public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(stack);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
            stack.Push(item);
    }
    #endregion // Public Methods
}