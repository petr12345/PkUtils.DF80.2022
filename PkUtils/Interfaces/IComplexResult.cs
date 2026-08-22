// Ignore Spelling: CCA
//

namespace PK.PkUtils.Interfaces;

/// <summary>
/// Represents a legacy version of <see cref="IComplexErrorResult{TError}"/> interface with no content;
/// error details are untyped (<see cref="object"/>).
/// </summary>
public interface IComplexResult : IComplexErrorResult<object>
{
}

/// <summary>
/// Represents a legacy version of <see cref="IComplexErrorResult{T, TError}"/>
/// with customizable content and untyped error details.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public interface IComplexResult<out T> : IComplexResult, IComplexErrorResult<T, object>
{
}
