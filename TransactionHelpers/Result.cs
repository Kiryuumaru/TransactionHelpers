using System;
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
    List<Error> IResult.InternalErrors { get; set; } = [];

    object? IResult.InternalValue { get; set; }

    Type? IResult.InternalValueType { get; set; }

    /// <inheritdoc/>
    [JsonIgnore]
    public virtual Error? Error => (this as IResult).InternalErrors.LastOrDefault();

    /// <inheritdoc/>
    public virtual IReadOnlyList<Error> Errors
    {
        get => (this as IResult).InternalErrors.AsReadOnly();
        set => Append(new() { Errors = value, ShouldReplaceErrors = true });
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
    public bool Success<TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult
    {
        this.WithResult(appendResultValues, resultAppend);
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool Success<TAppend>(TAppend resultAppend)
        where TAppend : IResult
    {
        this.WithResult(resultAppend);
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success<TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(appendResultValues, resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool Success<TAppend, TAppendValue>(TAppend resultAppend, out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool SuccessAndHasValue<TAppend>(TAppend resultAppend, bool appendResultValues)
        where TAppend : IResult
    {
        this.WithResult(appendResultValues, resultAppend);

        if (resultAppend.InternalValueType != null)
        {
            return !resultAppend.IsError && resultAppend.InternalValue != null;
        }

        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool SuccessAndHasValue<TAppend>(TAppend resultAppend)
        where TAppend : IResult
    {
        this.WithResult(resultAppend);

        if (resultAppend.InternalValueType != null)
        {
            return !resultAppend.IsError && resultAppend.InternalValue != null;
        }

        return !resultAppend.IsError;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool SuccessAndHasValue<TAppend, TAppendValue>(TAppend resultAppend, bool appendResultValues, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(appendResultValues, resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError && !resultAppend.HasNoValue;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool SuccessAndHasValue<TAppend, TAppendValue>(TAppend resultAppend, [NotNullWhen(true)] out TAppendValue? value)
        where TAppend : IResult<TAppendValue>
    {
        this.WithResult(resultAppend);
        value = resultAppend.Value;
        return !resultAppend.IsError && !resultAppend.HasNoValue;
    }

    /// <inheritdoc/>
    public virtual void Append(ResultAppend resultAppend)
    {
        if (resultAppend.ShouldAppendValue)
        {
            if ((this as IResult).InternalValueType != null)
            {
                (this as IResult).InternalValue = resultAppend.Value;
            }
        }
        if (resultAppend.ShouldAppendErrors)
        {
            if (resultAppend.Errors != null)
            {
                foreach (var error in resultAppend.Errors)
                {
                    if (error == null)
                    {
                        continue;
                    }
                    (this as IResult).InternalErrors.Add(error);
                }
            }
        }
        if (resultAppend.ShouldReplaceErrors)
        {
            (this as IResult).InternalErrors = [.. resultAppend.Errors!];
        }
        if (resultAppend.ShouldAppendResultErrors || resultAppend.ShouldAppendResultValue)
        {
            if (resultAppend.Results != null)
            {
                var thisValueType = (this as IResult).InternalValueType;
                foreach (var result in resultAppend.Results)
                {
                    if (result == null)
                    {
                        continue;
                    }
                    if (resultAppend.ShouldAppendResultValue)
                    {
                        if (thisValueType != null)
                        {
                            IResult resultToLook = result;
                            while (!thisValueType.IsAssignableFrom(resultToLook.InternalValueType) && resultToLook.InternalValue is IResult childResult)
                            {
                                resultToLook = childResult;
                            }
                            if (thisValueType.IsAssignableFrom(resultToLook.InternalValueType))
                            {
                                Append(new() { Value = resultToLook.InternalValue, ShouldAppendValue = true });
                            }
                        }
                    }
                    if (resultAppend.ShouldAppendResultErrors)
                    {
                        Append(new() { Errors = [.. result.Errors], ShouldAppendErrors = true });
                    }
                    if (resultAppend.ShouldReplaceResultErrors)
                    {
                        Append(new() { Errors = [.. result.Errors], ShouldReplaceErrors = true });
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public virtual object Clone()
    {
        var clone = new Result();
        HandleClone(clone);
        return clone;
    }

    /// <summary>
    /// Handles the cloning of the current result instance.
    /// </summary>
    /// <param name="clone">The cloned result object.</param>
    protected virtual void HandleClone(object clone)
    {
        if (clone is IResult result)
        {
            result.InternalErrors = [.. (this as IResult).InternalErrors.Select(e => (e.Clone() as Error)!).Where(e => e != null)];
            result.InternalValue = (this as IResult).InternalValue;
            result.InternalValueType = (this as IResult).InternalValueType;
        }
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
    /// <summary>
    /// Creates a new instance of <see cref="Result{TValue}"/>.
    /// </summary>
    public Result()
    {
        (this as IResult).InternalValueType = typeof(TValue);
    }

    /// <inheritdoc/>
    [JsonIgnore]
    public override Error? Error => base.Error;

    /// <inheritdoc/>
    public override IReadOnlyList<Error> Errors
    {
        get => base.Errors;
        set => Append(new() { Errors = value, ShouldReplaceErrors = true });
    }

    /// <inheritdoc/>
    public virtual TValue? Value
    {
        get => (TValue?)(this as IResult).InternalValue;
        set => Append(new() { Value = value, ShouldAppendValue = true });
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

    /// <inheritdoc/>
    public override object Clone()
    {
        var clone = new Result<TValue>();
        HandleClone(clone);
        return clone;
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