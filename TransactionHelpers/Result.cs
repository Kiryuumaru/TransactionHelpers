﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TransactionHelpers.Interface;
using TransactionHelpers.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TransactionHelpers;

/// <summary>
/// The results for all API request.
/// </summary>
public class Result : IResult
{
    internal List<Error> InternalError = [];

    /// <inheritdoc/>
    [JsonIgnore]
    public virtual Error? Error => InternalError.LastOrDefault();

    /// <inheritdoc/>
    public virtual IReadOnlyList<Error> Errors
    {
        get => InternalError.AsReadOnly();
        set => InternalError = [.. value];
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
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success()
    {
        return !IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool SuccessAndHasValue()
    {
        return !IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult
    {
        this.WithResult(appendResultValues, resultAppend);
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend)
        where TAppend : IResult
    {
        this.WithResult(resultAppend);
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(appendResultValues, resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool Success<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult
    {
        this.WithResult(appendResultValues, resultAppend);
        if (typeof(TAppend).GetProperty(nameof(IResult<object>.HasNoValue), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) is PropertyInfo hasNoValuePropertyInfo &&
            hasNoValuePropertyInfo.GetValue(resultAppend) is bool hasNoValue)
        {
            return !resultAppend.IsError && !hasNoValue;
        }

        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend>(TAppend resultAppend)
        where TAppend : IResult
    {
        this.WithResult(resultAppend);
        if (typeof(TAppend).GetProperty(nameof(IResult<object>.HasNoValue), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) is PropertyInfo hasNoValuePropertyInfo &&
            hasNoValuePropertyInfo.GetValue(resultAppend) is bool hasNoValue)
        {
            return !resultAppend.IsError && !hasNoValue;
        }

        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(appendResultValues, resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError && !resultAppend.HasNoValue;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool SuccessAndHasValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppend, TAppendValue>(TAppend resultAppend, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError && !resultAppend.HasNoValue;
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
    public override Error? Error => base.Error;

    /// <inheritdoc/>
    public override IReadOnlyList<Error> Errors
    {
        get => base.Errors;
        set => base.Errors = [.. value];
    }

    /// <inheritdoc/>
    public virtual TValue? Value
    {
        get => InternalValue;
        set => InternalValue = value;
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
            throw EmptyResultException.Default;
        }
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Value))]
    public virtual TValue GetValueOrThrow()
    {
        ThrowIfErrorOrHasNoValue();
        return Value;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool Success(out TValue? value)
    {
        value = Value;
        return !IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool SuccessAndHasValue([NotNullWhen(true)] out TValue? value)
    {
        value = Value;
        return !IsError && !HasNoValue;
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