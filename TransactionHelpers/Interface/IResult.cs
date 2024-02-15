using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using TransactionHelpers.Exceptions;

namespace TransactionHelpers.Interface;

/// <summary>
/// The interface for all results.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the last <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    Error? Error { get; }

    /// <summary>
    /// Gets all the <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    bool IsError { get; }

    /// <summary>
    /// Throws if the result has any _error.
    /// </summary>
    void ThrowIfError();
}

/// <summary>
/// The interface for all results.
/// </summary>
public interface IResult<TValue> : IResult
{
    /// <summary>
    /// Gets the <typeparamref name="TValue"/> of the operation.
    /// </summary>
    TValue? Value { get; }

    /// <summary>
    /// Gets <c>true</c> whether the <see cref="IResult{TValue}.Value"/> has value; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    bool HasValue { get; }

    /// <summary>
    /// Gets <c>true</c> whether the <see cref="IResult{TValue}.Value"/> no has value; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    bool HasNoValue { get; }

    /// <summary>
    /// Throws if the result has any _error or has no value.
    /// </summary>
    /// <exception cref="EmptyResultException">the <see cref="IResult{TValue}.Value"/> has no value.</exception>
    [MemberNotNull(nameof(Value))]
    void ThrowIfErrorOrHasNoValue();
}
