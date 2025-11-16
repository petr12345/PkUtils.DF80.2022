// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// Any class whose instances are countable should support ICountable interface. <br/>
/// This way, the class supporting ICountable keeps track of count of all "living" instances of the
/// same type. The object is considered living, if it has not been disposed explicitly by Dispose
/// or by the Finalizer. 
/// </summary>
[CLSCompliant(true)]
public interface ICountable
{
    /// <summary>
    /// The current count of objects of the same type in 'living state'.
    /// Counts only objects of exactly the same type.
    /// </summary>
    int CountExact { get; }

    /// <summary>
    /// The current count of this type descendants in 'living state'.
    /// Counts only descendants, NOT objects of exactly the same type.
    /// </summary>
    int CountDescendants { get; }

    /// <summary>
    /// Get count of all instances of given type and instances of derived types.
    /// </summary>
    /// <returns></returns>
    int CountIncludingDescendants { get; }

    /// <summary>
    /// The object order, which has been initialized at the moment of this instance creation
    /// ( the actual value of order index that the implementation maintains for this specific type).
    /// </summary>
    int Order { get; }
}
