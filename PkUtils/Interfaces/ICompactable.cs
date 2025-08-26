// Ignore Spelling: Compactable, Utils
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary> ICompactable defines a functionality for 'compacting' objects instances.  <br/>
/// Object which supports ICompactable is able to free expendable system resources, while
/// preserving the same state visible to external users, and after that being able to perform the
/// same functionality as before. </summary>
///
/// <remarks> The (remote) analogy of ICompactable is <see cref="System.IDisposable"/>.  <br/>
/// Note that unlike IDisposable, the call of ICompactable.Compact  must preserve the object
/// state.. </remarks>
[CLSCompliant(true)]
public interface ICompactable
{
    /// <summary> Compacts this object, freeing up non-necessary resources. </summary>
    void Compact();
}
