﻿using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using TransactionHelpers.Exceptions;
using System.Text.Json.Serialization;

namespace TransactionHelpers.Interface;

/// <summary>
/// The interface for all results.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the last <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    [JsonIgnore]
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
    [JsonIgnore]
    bool IsError { get; }

    /// <summary>
    /// Throws if the result has any _error.
    /// </summary>
    void ThrowIfError();

    /// <summary>
    /// Checks if the result has an error.
    /// </summary>
    /// <returns>True if the result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success();

    /// <summary>
    /// Checks if the result has an error or no value.
    /// </summary>
    /// <returns>True if the result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue();

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results has the same value type.</param>
    /// <returns>True if the appended result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <returns>True if the appended result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results has the same value type.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error or no value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results has the same value type.</param>
    /// <returns>True if the appended result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error or no value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <returns>True if the appended result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error or no value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results has the same value type.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has an error or no value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;
}

/// <summary>
/// The interface for all results.
/// </summary>
public interface IResult<TValue> : IResult
{
    /// <summary>
    /// Gets the last <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    [JsonIgnore]
    new Error? Error { get; }

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
    [JsonIgnore]
    bool HasNoValue { get; }

    /// <summary>
    /// Throws if the result has any _error or has no value.
    /// </summary>
    /// <exception cref="EmptyResultException">the <see cref="IResult{TValue}.Value"/> has no value.</exception>
    [MemberNotNull(nameof(Value))]
    void ThrowIfErrorOrHasNoValue();

    /// <summary>
    /// Get value or throws if the result has any _error or has no value.
    /// </summary>
    /// <exception cref="EmptyResultException">the <see cref="IResult{TValue}.Value"/> has no value.</exception>
    [MemberNotNull(nameof(Value))]
    TValue GetValueOrThrow();

    /// <summary>
    /// Checks if the result has an error.
    /// </summary>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success(out TValue? value);

    /// <summary>
    /// Checks if the result has an error or no value.
    /// </summary>
    /// <returns>True if the result has an error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    new bool SuccessAndHasValue();

    /// <summary>
    /// Checks if the result has an error.
    /// </summary>
    /// <param name="value">The out result value.</param>
    /// <returns>false if the result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    bool SuccessAndHasValue([NotNullWhen(true)] out TValue? value);
}
