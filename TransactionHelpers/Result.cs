using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TransactionHelpers.Interface;
using TransactionHelpers.Exceptions;

namespace TransactionHelpers;

/// <summary>
/// The results for all API request.
/// </summary>
public class Result : IResult
{
    private Error? error;

    /// <inheritdoc/>
    public virtual Error? Error
    {
        get => error;
        init
        {
            if (value != null && (value.Exception == null || string.IsNullOrEmpty(value.Message)))
            {
                error = null;
            }
            else
            {
                error = value;
            }
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccess { get => Error == null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public virtual bool IsError { get => Error != null; }

    /// <summary>
    /// Appends an <see cref="Exception"/>.
    /// </summary>
    public virtual Exception? AppendException
    {
        init
        {
            Error = new() { Exception = value };
        }
    }

    /// <summary>
    /// Appends the last <see cref="IResult"/>.
    /// </summary>
    public virtual IResult? AppendResponse
    {
        init
        {
            if (value is IResult response)
            {
                Error = response.Error;
            }
        }
    }

    /// <summary>
    /// Appends the last <see cref="IResult"/>.
    /// </summary>
    public virtual IResult?[]? AppendResponses
    {
        init
        {
            if (value != null)
            {
                foreach (var response in value)
                {
                    AppendResponse = response;
                }
            }
        }
    }

    /// <summary>
    /// Implicit operator for <see cref="TransactionHelpers.Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="TransactionHelpers.Error"/> to return.
    /// </param>
    public static implicit operator Result(Error error)
    {
        return new() { Error = error };
    }

    /// <summary>
    /// Implicit operator for <see cref="TransactionHelpers.Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Result(Exception exception)
    {
        return new() { AppendException = exception };
    }

    /// <inheritdoc/>
    public virtual void ThrowIfError()
    {
        if (Error != null)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
    }
}

/// <summary>
/// The results for all API request.
/// </summary>
/// <typeparam name="TValue">
/// The type of the operation result.
/// </typeparam>
public class Result<TValue> : Result
{
    private TValue? value;

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> of the operation.
    /// </summary>
    public virtual TValue? Value
    {
        get => value;
        init => this.value = value;
    }

    /// <summary>
    /// Gets <c>true</c> whether the <see cref="Result{TValue}.Value"/> has value; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public virtual bool HasValue { get => Value != null; }

    /// <summary>
    /// Gets <c>true</c> whether the <see cref="Result{TValue}.Value"/> no has value; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    public virtual bool HasNoValue { get => Value == null; }

    /// <summary>
    /// Appends the last <see cref="IResult"/>.
    /// </summary>
    public override IResult? AppendResponse
    {
        init
        {
            if (value is IResult lastResponse)
            {
                Error? error = null;
                TValue? _value = default;
                object? objToLook = lastResponse;
                while (objToLook?.GetType().GetProperty(nameof(Value)) is PropertyInfo propertyInfo)
                {
                    objToLook = propertyInfo.GetValue(objToLook);
                    if (typeof(TValue).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (objToLook is TValue typedResult)
                        {
                            _value = typedResult;
                            break;
                        }
                        else if (objToLook == null)
                        {
                            _value = default;
                            break;
                        }
                    }
                }
                Value = _value;
                Error = error;
            }
        }
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="value">
    /// The <typeparamref name="TValue"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(TValue value)
    {
        return new Result<TValue>() { Value = value };
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="Error"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(Error error)
    {
        return new Result<TValue>() { Error = error };
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Result<TValue>(Exception exception)
    {
        return new Result<TValue>() { AppendException = exception };
    }

    /// <summary>
    /// Throws if the result has any error or has no result.
    /// </summary>
    /// <exception cref="EmptyResultException">the <see cref="Result{TValue}.Value"/> has no value.</exception>
    [MemberNotNull(nameof(Value))]
    public virtual void ThrowIfErrorOrHasNoResult()
    {
        if (IsError)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
        else if (HasNoValue)
        {
            throw new EmptyResultException();
        }
    }
}