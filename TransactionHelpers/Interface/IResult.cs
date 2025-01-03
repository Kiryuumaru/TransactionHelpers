using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using TransactionHelpers.Exceptions;
using System.Text.Json.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace TransactionHelpers.Interface;

/// <summary>
/// Represents the result of an operation, encapsulating success status, errors, and value.
/// </summary>
public interface IResult
{
    internal List<Error> InternalErrors { get; set; }

    internal object? InternalValue { get; set; }

    internal Type? InternalValueType { get; set; }

    /// <summary>
    /// Gets the last <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    [JsonIgnore]
    Error? Error { get; }

    /// <summary>
    /// Gets all the <see cref="TransactionHelpers.Error"/> instances of the operation.
    /// </summary>
    IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Gets a value indicating whether the operation is successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation has an error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    [JsonIgnore]
    bool IsError { get; }

    /// <summary>
    /// Throws an exception if the result has any error.
    /// </summary>
    void ThrowIfError();

    /// <summary>
    /// Checks if the result has no error.
    /// </summary>
    /// <returns>True if the result has no error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success();

    /// <summary>
    /// Checks if the result has no error and has a value.
    /// </summary>
    /// <returns>True if the result has no error and has a value, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue();

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results have the same value type.</param>
    /// <returns>True if the appended result has no error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <returns>True if the appended result has no error, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<TAppend>(TAppend resultAppend)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results have the same value type.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success<TAppend, TAppendValue>(TAppend resultAppend, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error and has a value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results have the same value type.</param>
    /// <returns>True if the appended result has no error and has a value, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error and has a value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <returns>True if the appended result has no error and has a value, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<TAppend>(TAppend resultAppend)
        where TAppend : IResult;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error and has a value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="appendResultValues">Append values if the results have the same value type.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>;

    /// <summary>
    /// Appends the specified result and checks if the appended result has no error and has a value.
    /// </summary>
    /// <typeparam name="TAppend">The type of result to append.</typeparam>
    /// <typeparam name="TAppendValue">The type of result value.</typeparam>
    /// <param name="resultAppend">The result to append.</param>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the appended result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool SuccessAndHasValue<TAppend, TAppendValue>(TAppend resultAppend, [NotNullWhen(true)] out TAppendValue? value)
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
    /// Gets the value of the operation.
    /// </summary>
    TValue? Value { get; }

    /// <summary>
    /// Gets a value indicating whether the result has a value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the result has no value.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [JsonIgnore]
    bool HasNoValue { get; }

    /// <summary>
    /// Throws an exception if the result has any error or has no value.
    /// </summary>
    /// <exception cref="EmptyResultException">Thrown if the result has no value.</exception>
    [MemberNotNull(nameof(Value))]
    void ThrowIfErrorOrHasNoValue();

    /// <summary>
    /// Gets the value or throws an exception if the result has any error or has no value.
    /// </summary>
    /// <exception cref="EmptyResultException">Thrown if the result has no value.</exception>
    [MemberNotNull(nameof(Value))]
    TValue GetValueOrThrow();

    /// <summary>
    /// Checks if the result has no error and has a value.
    /// </summary>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success(out TValue? value);

    /// <summary>
    /// Checks if the result has no error and has a value.
    /// </summary>
    /// <returns>True if the result has no error and has a value, otherwise false.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    new bool SuccessAndHasValue();

    /// <summary>
    /// Checks if the result has no error and has a value.
    /// </summary>
    /// <param name="value">The out result value.</param>
    /// <returns>False if the result has an error or no value, otherwise true.</returns>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    bool SuccessAndHasValue([NotNullWhen(true)] out TValue? value);
}
