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
    private List<Error> _errors = new();

    /// <inheritdoc/>
    public virtual Error? Error => _errors.LastOrDefault();

    /// <inheritdoc/>
    public virtual IReadOnlyList<Error> Errors => _errors.AsReadOnly();

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
    /// Append errors to result.
    /// </summary>
    /// <param name="errors">The errors to append.</param>
    /// <returns>The same result instance</returns>
    public virtual Result WithError(params Error?[]? errors)
    {
        if (errors != null)
        {
            foreach (var error in errors)
            {
                if (error == null || error.Exception == null || string.IsNullOrEmpty(error.Message))
                {
                    continue;
                }
                _errors.Add(error);
            }
        }
        return this;
    }

    /// <summary>
    /// Append errors to result.
    /// </summary>
    /// <param name="exceptions">The exceptions to append.</param>
    /// <returns>The same result instance.</returns>
    public virtual Result WithError(params Exception?[]? exceptions)
    {
        if (exceptions != null)
        {
            foreach (var exception in exceptions)
            {
                if (exception == null)
                {
                    continue;
                }
                _errors.Add(new Error() { Exception = exception });
            }
        }
        return this;
    }

    /// <summary>
    /// Append errors to result.
    /// </summary>
    /// <param name="results">The results to append.</param>
    /// <returns>The same result instance.</returns>
    public virtual Result WithResult(params IResult?[]? results)
    {
        if (results != null)
        {
            foreach (var result in results)
            {
                if (result == null)
                {
                    continue;
                }
                foreach (var error in result.Errors)
                {
                    _errors.Add(error);
                }
            }
        }
        return this;
    }

    /// <summary>
    /// Implicit operator for <see cref="TransactionHelpers.Error"/> conversion.
    /// </summary>
    /// <param name="error">
    /// The <see cref="TransactionHelpers.Error"/> to return.
    /// </param>
    public static implicit operator Result(Error? error)
    {
        return new Result().WithError(error);
    }

    /// <summary>
    /// Implicit operator for <see cref="TransactionHelpers.Error"/> conversion.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to return.
    /// </param>
    public static implicit operator Result(Exception? exception)
    {
        return new Result().WithError(exception);
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
    private TValue? _value;

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> of the operation.
    /// </summary>
    public virtual TValue? Value => _value;

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
    /// Throws if the result has any _error or has no result.
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

    /// <summary>
    /// Add value to result.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The same result instance.</returns>
    public virtual Result<TValue> WithValue(TValue? value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    /// Append errors to result.
    /// </summary>
    /// <param name="errors">The errors to append.</param>
    /// <returns>The same result instance</returns>
    public new virtual Result<TValue> WithError(params Error?[]? errors)
    {
        base.WithError(errors);
        return this;
    }

    /// <summary>
    /// Append errors to result.
    /// </summary>
    /// <param name="exceptions">The exceptions to append.</param>
    /// <returns>The same result instance.</returns>
    public new virtual Result<TValue> WithError(params Exception?[]? exceptions)
    {
        base.WithError(exceptions);
        return this;
    }

    /// <summary>
    /// Append errors to result.
    /// </summary>
    /// <param name="results">The results to append.</param>
    /// <returns>The same result instance.</returns>
    public new virtual Result<TValue> WithResult(params IResult?[]? results)
    {
        if (results != null)
        {
            foreach (var result in results)
            {
                if (result == null)
                {
                    continue;
                }
                WithError(result.Errors.ToArray());
                object? objToLook = result;
                while (objToLook?.GetType().GetProperty(nameof(Value)) is PropertyInfo propertyInfo)
                {
                    objToLook = propertyInfo.GetValue(objToLook);
                    if (typeof(TValue).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (objToLook is TValue typedResult)
                        {
                            _value = typedResult;
                        }
                        else if (objToLook == null)
                        {
                            _value = default;
                        }
                    }
                }
            }
        }
        return this;
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
}