using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TransactionHelpers.Interface;
using TransactionHelpers.Exceptions;

namespace TransactionHelpers;

/// <summary>
/// The responses for all API request.
/// </summary>
public class Response : IResponse
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
    /// Appends the last <see cref="IResponse"/>.
    /// </summary>
    public virtual IResponse? AppendResponse
    {
        init
        {
            if (value is IResponse response)
            {
                Error = response.Error;
            }
        }
    }

    /// <summary>
    /// Appends the last <see cref="IResponse"/>.
    /// </summary>
    public virtual IResponse?[]? AppendResponses
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
    public static implicit operator Response(Error error)
    {
        return new() { Error = error };
    }

    /// <summary>
    /// Implicit operator for <see cref="TransactionHelpers.Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Response(Exception exception)
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
/// The responses for all API request.
/// </summary>
/// <typeparam name="TResult">
/// The type of the operation response.
/// </typeparam>
public class Response<TResult> : Response
{
    private TResult? result;

    /// <summary>
    /// Gets the <typeparamref name="TResult"/> of the operation.
    /// </summary>
    public virtual TResult? Result
    {
        get => result;
        init => result = value;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Result))]
    public virtual bool HasResult { get => Result != null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Result))]
    public virtual bool HasNoResult { get => Result == null; }

    /// <summary>
    /// Appends the last <see cref="IResponse"/>.
    /// </summary>
    public override IResponse? AppendResponse
    {
        init
        {
            if (value is IResponse lastResponse)
            {
                Error? error = null;
                TResult? result = default;
                object? objToLook = lastResponse;
                while (objToLook?.GetType().GetProperty(nameof(Result)) is PropertyInfo propertyInfo)
                {
                    objToLook = propertyInfo.GetValue(objToLook);
                    if (typeof(TResult).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (objToLook is TResult typedResult)
                        {
                            result = typedResult;
                            break;
                        }
                        else if (objToLook == null)
                        {
                            result = default;
                            break;
                        }
                    }
                }
                Result = result;
                Error = error;
            }
        }
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="result">
    /// The <typeparamref name="TResult"/> to return.
    /// </param>
    public static implicit operator Response<TResult>(TResult result)
    {
        return new Response<TResult>() { Result = result };
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="Error"/> to return.
    /// </param>
    public static implicit operator Response<TResult>(Error error)
    {
        return new Response<TResult>() { Error = error };
    }

    /// <summary>
    /// Implicit operator for <see cref="Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Response<TResult>(Exception exception)
    {
        return new Response<TResult>() { AppendException = exception };
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public virtual void ThrowIfErrorOrHasNoResult()
    {
        if (IsError)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
        else if (HasNoResult)
        {
            throw new EmptyResultException();
        }
    }
}