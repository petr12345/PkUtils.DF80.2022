// Ignore Spelling: CCA
//

using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// Represents a legacy version of <see cref="IComplexErrorResult{TError}"/> interface with no content;
/// error details are untyped (<see cref="object"/>).
/// </summary>
[CLSCompliant(true)]
public interface IComplexResult : IComplexErrorResult<object>
{
}

/// <summary>
/// Represents a legacy version of <see cref="IComplexErrorResult{T, TError}"/>
/// with customizable content and untyped error details.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
[CLSCompliant(true)]
public interface IComplexResult<out T> : IComplexResult, IComplexErrorResult<T, object>
{
}
