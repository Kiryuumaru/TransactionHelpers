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
                if (result is Result typedResult)
                {
                    typedResult.InternalError.Add(error);
                }
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
                if (result is Result typedResult)
                {
                    typedResult.InternalError.Add(new Error() { Exception = exception });
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Adds exceptions as errors to the specified result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which exceptions are added as errors.</param>
    /// <param name="messages">An array of string message exceptions to add.</param>
    /// <returns>The result with added errors.</returns>
    public static T WithError<T>(this T result, params string?[]? messages)
        where T : IResult
    {
        if (messages != null)
        {
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }
                if (result is Result typedResult)
                {
                    typedResult.InternalError.Add(new Error() { Message = message });
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
        if (results != null)
        {
            foreach (var r in results)
            {
                if (r == null)
                {
                    continue;
                }
                result.WithError(r.Errors.ToArray());
                if (result.GetType().GetField(nameof(Result<object>.InternalValue), BindingFlags.NonPublic | BindingFlags.Instance) is FieldInfo resultValFieldInfo)
                {
                    object? objToLook = r;
                    while (objToLook?.GetType().GetProperty(nameof(IResult<object>.Value)) is PropertyInfo propertyInfo)
                    {
                        objToLook = propertyInfo.GetValue(objToLook);
                        if (resultValFieldInfo.FieldType.IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            resultValFieldInfo.SetValue(result, objToLook);
                        }
                    }
                }
            }
        }
        return result;
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
        if (result is Result<TValue> typedResult)
        {
            typedResult.InternalValue = value;
        }
        return result;
    }
}