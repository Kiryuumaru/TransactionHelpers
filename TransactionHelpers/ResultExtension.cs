using System;
using System.Linq;
using System.Reflection;
using TransactionHelpers.Interface;

namespace TransactionHelpers;

/// <summary>
/// Provides fluent extension methods for <see cref="IResult"/> and <see cref="IResult{TValue}"/>.
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Adds errors to the specified result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which errors are added.</param>
    /// <param name="errors">An array of nullable errors to add.</param>
    /// <returns>The result with added errors.</returns>
    /// <remarks>
    /// Errors will only be added if they are not null and contain both exception and message information.
    /// </remarks>
    public static T WithError<T>(this T result, params Error?[]? errors)
        where T : IResult
    {
        if (errors != null)
        {
            foreach (var error in errors)
            {
                if (error == null || error.Exception == null || string.IsNullOrEmpty(error.Message))
                {
                    continue;
                }
                result.InternalErrors.Add(error);
            }
        }
        return result;
    }

    /// <summary>
    /// Adds exceptions as errors to the specified result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which exceptions are added as errors.</param>
    /// <param name="exceptions">An array of nullable exceptions to add.</param>
    /// <returns>The result with added errors.</returns>
    public static T WithError<T>(this T result, params Exception?[]? exceptions)
        where T : IResult
    {
        if (exceptions != null)
        {
            foreach (var exception in exceptions)
            {
                if (exception == null)
                {
                    continue;
                }
                result.InternalErrors.Add(new Error() { Exception = exception });
            }
        }
        return result;
    }

    /// <summary>
    /// Adds an error with a message, code, and detail to the specified result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which the error is added.</param>
    /// <param name="message">The error message to add.</param>
    /// <param name="code">An optional error code to add.</param>
    /// <param name="detail">Optional additional details about the error.</param>
    /// <returns>The result with the added error.</returns>
    public static T WithError<T>(this T result, string? message, string? code = null, object? detail = null)
        where T : IResult
    {
        if (string.IsNullOrEmpty(message))
        {
            return result;
        }
        result.InternalErrors.Add(new Error() { Message = message, Code = code, Detail = detail });
        return result;
    }

    /// <summary>
    /// Incorporates the errors of the specified results into the current result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which errors are incorporated.</param>
    /// <param name="appendResultValues">Append values if the results have the same value type.</param>
    /// <param name="results">An array of nullable results to incorporate errors from.</param>
    /// <returns>The result with incorporated errors.</returns>
    public static T WithResult<T>(this T result, bool appendResultValues, params IResult?[]? results)
        where T : IResult
    {
        if (results != null)
        {
            foreach (var r in results)
            {
                if (r == null)
                {
                    continue;
                }
                result.WithError(r.Errors.ToArray());
                if (appendResultValues)
                {
                    if (result.InternalValueType != null)
                    {
                        IResult resultToLook = r;
                        while (resultToLook.InternalValue is IResult childResult)
                        {
                            resultToLook = childResult;
                        }
                        if (resultToLook.InternalValueType != null &&
                            result.InternalValueType.IsAssignableFrom(resultToLook.InternalValueType))
                        {
                            result.InternalValue = resultToLook.InternalValue;
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Incorporates the errors of the specified results into the current result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which errors are incorporated.</param>
    /// <param name="results">An array of nullable results to incorporate errors from.</param>
    /// <returns>The result with incorporated errors.</returns>
    public static T WithResult<T>(this T result, params IResult?[]? results)
        where T : IResult
    {
        return WithResult(result, true, results);
    }

    /// <summary>
    /// Sets the value of the result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="result">The result to which the value is set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The result with the value set.</returns>
    public static T WithValue<T, TValue>(this T result, TValue? value)
        where T : IResult<TValue>
    {
        result.InternalValue = value;
        return result;
    }

    /// <summary>
    /// Sets the value of the result to empty.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="result">The result to which the value is set.</param>
    /// <returns>The result with the value set to null.</returns>
    public static T WithEmptyValue<T, TValue>(this T result)
        where T : IResult<TValue>
    {
        result.InternalValue = null;
        return result;
    }
}
