using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TransactionHelpers.Exceptions;
using TransactionHelpers.Interface;

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
            if (value == null || value.Exception == null || value.Exception is EmptyResultException || string.IsNullOrEmpty(value.Message))
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
            Error = new Error() { Exception = value };
        }
    }

    /// <summary>
    /// Appends the last <see cref="IResponse"/>.
    /// </summary>
    public virtual IResponse? AppendResponse
    {
        init
        {
            if (value is IResponse lastResponse)
            {
                Error = lastResponse.Error;
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
            if (value?.LastOrDefault() is IResponse lastResponse)
            {
                AppendResponse = lastResponse;
            }
        }
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
public class Response<TResult> : IResponse
{
    private static readonly Error EmptyResultError = new()
    {
        Exception = new EmptyResultException()
    };

    private TResult? result;
    private Error? error;

    /// <summary>
    /// Gets the <typeparamref name="TResult"/> of the operation.
    /// </summary>
    public virtual TResult? Result
    {
        get => result;
        init
        {
            if (value != null)
            {
                Error = default;
            }
            result = value;
        }
    }

    /// <inheritdoc/>
    public virtual Error? Error
    {
        get
        {
            if (error == null && Result == null)
            {
                return EmptyResultError;
            }
            return error;
        }
        init
        {
            if (value == null || value.Exception == null || value.Exception is EmptyResultException || string.IsNullOrEmpty(value.Message))
            {
                error = null;
            }
            else
            {
                Result = default;
                error = value;
            }
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Result))]
    public virtual bool IsSuccess { get => error == null && Result != null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Result))]
    public virtual bool IsError { get => error != null || Result == null; }

    /// <summary>
    /// Appends an <see cref="Exception"/>.
    /// </summary>
    public virtual Exception? AppendException
    {
        init
        {
            Error = new Error() { Exception = value };
        }
    }

    /// <summary>
    /// Appends the last <see cref="IResponse"/>.
    /// </summary>
    public virtual IResponse? AppendResponse
    {
        init
        {
            if (value is IResponse lastResponse)
            {
                Error? error = null;
                TResult? result = default;
                if (lastResponse.Error?.Exception is not EmptyResultException)
                {
                    error = lastResponse.Error;
                }
                if (lastResponse.GetType().GetProperty(nameof(Result)) is PropertyInfo propertyInfo)
                {
                    Type resultType = typeof(TResult);
                    if (resultType.IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        object? resultObj = propertyInfo.GetValue(lastResponse);
                        if (resultObj is TResult typedResult)
                        {
                            result = typedResult;
                        }
                        else if (resultObj == null)
                        {
                            result = default;
                        }
                    }
                }
                Result = result;
                Error = error;
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
            if (value?.LastOrDefault() is IResponse lastResponse)
            {
                AppendResponse = lastResponse;
            }
        }
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public virtual void ThrowIfError()
    {
        if (IsError)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
    }
}