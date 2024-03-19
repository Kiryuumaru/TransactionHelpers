using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TransactionHelpers.Interface;
using TransactionHelpers.Exceptions;
using System.Collections.Generic;

namespace TransactionHelpers;

/// <summary>
/// The results for all API request.
/// </summary>
public class Result : IResult
{
    internal readonly List<Error> InternalError = new();

    /// <inheritdoc/>
    [JsonIgnore]
    public virtual Error? Error => InternalError.LastOrDefault();

    /// <inheritdoc/>
    public virtual IReadOnlyList<Error> Errors
    {
        get => InternalError.AsReadOnly();
        init => InternalError = value.ToList();
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccess { get => Error == null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    [JsonIgnore]
    public virtual bool IsError { get => Error != null; }

    /// <inheritdoc/>
    public virtual void ThrowIfError()
    {
        if (Error != null)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool AppendIsError<TAppend>(TAppend resultAppend)
        where TAppend : IResult
    {
        this.WithResult(resultAppend);
        return resultAppend.IsError;
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="Error"/> to return.
    /// </param>
    public static implicit operator Result(Error? error)
    {
        return new Result().WithError(error);
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Result(Exception? exception)
    {
        return new Result().WithError(exception);
    }

    /// <summary>
    /// Implicit operator for <see cref="Result{TValue}"/> to <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result{TValue}"/> to convert.
    /// </param>
    public static implicit operator Error?(Result result)
    {
        return result.Error;
    }

    /// <summary>
    /// Implicit operator for <see cref="Result{TValue}"/> to <see cref="Exception"/> conversion.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result{TValue}"/> to convert.
    /// </param>
    public static implicit operator Exception?(Result result)
    {
        return result.Error?.Exception;
    }
}

/// <summary>
/// The results for all API request.
/// </summary>
/// <typeparam name="TValue">
/// The type of the operation result.
/// </typeparam>
public class Result<TValue> : Result, IResult<TValue>
{
    internal TValue? InternalValue;

    /// <inheritdoc/>
    [JsonIgnore]
    public new virtual Error? Error => base.Error;

    /// <inheritdoc/>
    public virtual TValue? Value
    {
        get => InternalValue;
        init => InternalValue = value;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Value))]
    public virtual bool HasValue { get => Value != null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Value))]
    [JsonIgnore]
    public virtual bool HasNoValue { get => Value == null; }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Value))]
    public virtual void ThrowIfErrorOrHasNoValue()
    {
        if (IsError)
        {
            throw base.Error.Exception ?? new Exception(base.Error.Message);
        }
        else if (HasNoValue)
        {
            throw new EmptyResultException();
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool AppendIsErrorOrHasNoValue<TAppend, TAppendValue>(TAppend resultAppend)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        return resultAppend.IsError || resultAppend.HasNoValue;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool AppendIsErrorOrHasNoValue<TAppend, TAppendValue>(TAppend resultAppend, [NotNullWhen(false)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        value = resultAppend.Value;
        return resultAppend.IsError || resultAppend.HasNoValue;
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="value">
    /// The <typeparamref name="TValue"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(TValue? value)
    {
        return new Result<TValue>().WithValue(value);
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="Error"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(Error error)
    {
        return new Result<TValue>().WithError(error);
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(Exception exception)
    {
        return new Result<TValue>().WithError(exception);
    }

    /// <summary>
    /// Implicit operator for <see cref="Result{TValue}"/> to <typeparamref name="TValue"/> conversion.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result{TValue}"/> to convert.
    /// </param>
    public static implicit operator TValue?(Result<TValue> result)
    {
        return result.Value;
    }
}