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
        set
        {
            if (value == null || value.Exception == null || string.IsNullOrEmpty(value.Message))
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

    /// <inheritdoc/>
    public virtual void ThrowIfError()
    {
        if (Error != null)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
    }

    /// <summary>
    /// Append <see cref="IResponse"/> to the response.
    /// </summary>
    /// <param name="responses">The responses <see cref="IResponse"/> to append.</param>
    /// <returns>The resulting <see cref="Response"/> after the append.</returns>
    public virtual Response Append(params IResponse[] responses)
    {
        if (responses.LastOrDefault() is IResponse lastResponse)
        {
            return new()
            {
                Error = lastResponse.Error
            };
        }
        return this;
    }

    /// <summary>
    /// Append an error to the response.
    /// </summary>
    /// <param name="error">The error to append.</param>
    /// <returns>The resulting <see cref="Response"/> after the append.</returns>
    public virtual Response Append(Error? error)
    {
        return new()
        {
            Error = error
        };
    }

    /// <summary>
    /// Append an exception to the response.
    /// </summary>
    /// <param name="exception">The exception to append.</param>
    /// <returns>The resulting <see cref="Response"/> after the append.</returns>
    public virtual Response Append(Exception? exception)
    {
        return new()
        {
            Error = new Error() { Exception = exception }
        };
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
    public Error? Error
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
            if (value == null || value.Exception == null || string.IsNullOrEmpty(value.Message))
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

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public virtual void ThrowIfError()
    {
        if (IsError)
        {
            throw Error.Exception ?? new Exception(Error.Message);
        }
    }

    /// <summary>
    /// Append <see cref="IResponse"/> to the response.
    /// </summary>
    /// <param name="responses">The responses <see cref="IResponse"/> to append.</param>
    /// <returns>The resulting <see cref="Response{TResult}"/> after the append.</returns>
    public virtual Response<TResult> Append(params IResponse[] responses)
    {
        if (responses.LastOrDefault() is IResponse lastResponse)
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
            return new()
            {
                Result = result,
                Error = error,
            };
        }
        return this;
    }

    /// <summary>
    /// Append an error to the response.
    /// </summary>
    /// <param name="error">The error to append.</param>
    /// <returns>The resulting <see cref="Response{TResult}"/> after the append.</returns>
    public virtual Response<TResult> Append(Error? error)
    {
        return new()
        {
            Result = result,
            Error = error,
        };
    }

    /// <summary>
    /// Append <typeparamref name="TResult"/> to the response.
    /// </summary>
    /// <param name="result">The result to append.</param>
    /// <returns>The resulting <see cref="Response{TResult}"/> after the append.</returns>
    public virtual Response<TResult> Append(TResult? result)
    {
        return new()
        {
            Error = error,
            Result = result
        };
    }

    /// <summary>
    /// Append an exception to the response.
    /// </summary>
    /// <param name="exception">The exception to append.</param>
    /// <returns>The resulting <see cref="Response{TResult}"/> after the append.</returns>
    public virtual Response<TResult> Append(Exception? exception)
    {
        return Append(new Error()
        {
            Exception = exception
        });
    }
}