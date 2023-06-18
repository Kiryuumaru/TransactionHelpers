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
    /// <inheritdoc/>
    public virtual Error? Error { get; protected set; }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccess { get => Error == null; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public virtual bool IsError { get => Error != null; }

    /// <summary>
    /// Creates an instance of <see cref="Response"/>
    /// </summary>
    public Response()
    {

    }

    /// <summary>
    /// Creates an instance of <see cref="Response"/>
    /// </summary>
    /// <param name="error">The error of the response.</param>
    [JsonConstructor]
    public Response(Error? error)
    {
        Error = error;
    }

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
    public virtual void Append(params IResponse[] responses)
    {
        if (responses.LastOrDefault() is IResponse lastResponse)
        {
            Error = lastResponse.Error;
        }
    }

    /// <summary>
    /// Append an error to the response.
    /// </summary>
    /// <param name="error">The error to append.</param>
    public virtual void Append(Error? error)
    {
        Error = error;
    }

    /// <summary>
    /// Append an exception to the response.
    /// </summary>
    /// <param name="exception">The exception to append.</param>
    public virtual void Append(Exception? exception)
    {
        Append(new Error(exception));
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
    private Error? error;

    /// <summary>
    /// Gets the <typeparamref name="TResult"/> of the operation.
    /// </summary>
    public virtual TResult? Result { get; protected set; }

    /// <inheritdoc/>
    public Error? Error
    {
        get => error == null && Result == null ? new Error(new EmptyResultException()) : error;
        set => error = value;
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
    /// Creates an instance of <see cref="Response{TResult}"/>
    /// </summary>
    public Response()
    {

    }

    /// <summary>
    /// Creates an instance of <see cref="Response{TResult}"/>
    /// </summary>
    /// <param name="result">The result to append.</param>
    public Response(TResult? result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates an instance of <see cref="Response{TResult}"/>
    /// </summary>
    /// <param name="error">The error of the response.</param>
    public Response(Error? error)
    {
        Error = error;
    }

    /// <summary>
    /// Creates an instance of <see cref="Response{TResult}"/>
    /// </summary>
    /// <param name="result">The result of the response.</param>
    /// <param name="error">The error of the response.</param>
    [JsonConstructor]
    public Response(TResult? result, Error? error)
    {
        Result = result;
        Error = error;
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

    /// <summary>
    /// Append <see cref="IResponse"/> to the response.
    /// </summary>
    /// <param name="responses">The responses <see cref="IResponse"/> to append.</param>
    public virtual void Append(params IResponse[] responses)
    {
        if (responses.LastOrDefault() is IResponse lastResponse)
        {
            if (lastResponse.Error?.Exception is not EmptyResultException)
            {
                Error = lastResponse.Error;
            }
            if (lastResponse.GetType().GetProperty(nameof(Result)) is PropertyInfo propertyInfo)
            {
                Type resultType = typeof(TResult);
                if (resultType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    object? result = propertyInfo.GetValue(lastResponse);
                    if (result is TResult typedResult)
                    {
                        Result = typedResult;
                    }
                    else if (result == null)
                    {
                        Result = default;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Append an error to the response.
    /// </summary>
    /// <param name="error">The error to append.</param>
    public virtual void Append(Error? error)
    {
        if (error != null)
        {
            Result = default;
        }
        Error = error;
    }

    /// <summary>
    /// Append an exception to the response.
    /// </summary>
    /// <param name="exception">The exception to append.</param>
    public virtual void Append(Exception? exception)
    {
        Append(new Error(exception));
    }

    /// <summary>
    /// Append <typeparamref name="TResult"/> to the response.
    /// </summary>
    /// <param name="result">The result to append.</param>
    public virtual void Append(TResult? result)
    {
        if (result != null)
        {
            Error = default;
        }
        Result = result;
    }
}