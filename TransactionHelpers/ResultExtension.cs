using System;
using System.Linq;
using System.Reflection;
using TransactionHelpers.Interface;

namespace TransactionHelpers;

/// <summary>
/// Provides fluent extension methods for <see cref="Result"/> and <see cref="Result{TValue}"/>.
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
        where T : Result
    {
        if (errors != null)
        {
            foreach (var error in errors)
            {
                if (error == null || error.Exception == null || string.IsNullOrEmpty(error.Message))
                {
                    continue;
                }
                result.InternalError.Add(error);
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
        where T : Result
    {
        if (exceptions != null)
        {
            foreach (var exception in exceptions)
            {
                if (exception == null)
                {
                    continue;
                }
                result.InternalError.Add(new Error() { Exception = exception });
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
        where T : Result
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
        where T : Result<TValue>
    {
        result.InternalValue = value;
        return result;
    }

    /// <summary>
    /// Incorporates the results and values of the specified results into the current result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="result">The result to which errors are incorporated.</param>
    /// <param name="results">An array of nullable results to incorporate errors from.</param>
    /// <returns>The result with incorporated errors and values.</returns>
    public static T WithResult<T, TValue>(this T result, params IResult?[]? results)
        where T : Result<TValue>
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
                object? objToLook = r;
                while (objToLook?.GetType().GetProperty(nameof(result.Value)) is PropertyInfo propertyInfo)
                {
                    objToLook = propertyInfo.GetValue(objToLook);
                    if (typeof(TValue).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (objToLook is TValue typedResult)
                        {
                            result.InternalValue = typedResult;
                        }
                        else if (objToLook == null)
                        {
                            result.InternalValue = default;
                        }
                    }
                }
            }
        }
        return result;
    }
}