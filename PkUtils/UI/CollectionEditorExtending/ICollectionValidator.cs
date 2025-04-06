// Ignore Spelling: Utils, validator
//
using System.Collections.Generic;

namespace PK.PkUtils.UI.CollectionEditorExtending;

/// <summary> Interface for collection validator. </summary>
/// <typeparam name="T"> Type of items in the collection. </typeparam>
public interface ICollectionValidator<in T>
{
    /// <summary> Validates the given items collection. Throws exception if collection is invalid</summary>
    /// <param name="items"> The items. </param>
    void Validate(IEnumerable<T> items);
}
