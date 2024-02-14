using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace TransactionHelpers.Interface;

/// <summary>
/// The interface for all results.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the last <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets all the <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError { get; }

    /// <summary>
    /// Throws if the result has any _error.
    /// </summary>
    void ThrowIfError();
}
