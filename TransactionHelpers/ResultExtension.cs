﻿using System;
using System.Diagnostics.CodeAnalysis;
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
    /// <param name="message">A string message exceptions to add.</param>
    /// <returns>The result with added errors.</returns>
    public static T WithError<T>(this T result, string? message)
        where T : IResult
    {
        if (string.IsNullOrEmpty(message))
        {
            return result;
        }
        if (result is Result typedResult)
        {
            typedResult.InternalError.Add(new Error() { Message = message });
        }
        return result;
    }

    /// <summary>
    /// Adds exceptions as errors to the specified result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="result">The result to which exceptions are added as errors.</param>
    /// <param name="errorCode">A string error code of the exceptions to add.</param>
    /// <param name="message">A string message exceptions to add.</param>
    /// <returns>The result with added errors.</returns>
    public static T WithError<T>(this T result, string? errorCode, string? message)
        where T : IResult
    {
        if (string.IsNullOrEmpty(errorCode) && string.IsNullOrEmpty(message))
        {
            return result;
        }
        if (result is Result typedResult)
        {
            typedResult.InternalError.Add(new Error()
            {
                ErrorCode = errorCode,
                Message = message
            });
        }
        return result;
    }

    /// <summary>
    /// Incorporates the errors of the specified results into the current result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="result">The result to which errors are incorporated.</param>
    /// <param name="appendResultValues">Append values if the results has the same value type.</param>
    /// <param name="resultToAppend">A result to incorporate errors from.</param>
    /// <returns>The result with incorporated errors.</returns>
    public static T WithResult<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResult>(this T result, bool appendResultValues, TResult? resultToAppend)
        where T : IResult
        where TResult : IResult
    {
        if (resultToAppend == null)
        {
            return result;
        }
        result.WithError(resultToAppend.Errors.ToArray());
        if (appendResultValues &&
            typeof(T).GetField(nameof(Result<object>.InternalValue), BindingFlags.NonPublic | BindingFlags.Instance) is FieldInfo resultValFieldInfo)
        {
            object? objToLook = resultToAppend;
#pragma warning disable IL2075
            while (objToLook?.GetType().GetProperty(nameof(IResult<object>.Value)) is PropertyInfo propertyInfo)
#pragma warning restore IL2075
            {
                objToLook = propertyInfo.GetValue(objToLook);
                if (resultValFieldInfo.FieldType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    resultValFieldInfo.SetValue(result, objToLook);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Incorporates the errors of the specified results into the current result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="result">The result to which errors are incorporated.</param>
    /// <param name="resultToAppend">A results to incorporate errors from.</param>
    /// <returns>The result with incorporated errors.</returns>
    public static T WithResult<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResult>(this T result, TResult? resultToAppend)
        where T : IResult
        where TResult : IResult
    {
        return WithResult(result, true, resultToAppend);
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

    /// <summary>
    /// Sets the value of the result to empty.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="result">The result to which the value is set.</param>
    /// <returns>The result with the value set.</returns>
    public static T WithEmptyValue<T, TValue>(this T result)
        where T : IResult<TValue>
    {
        if (result is Result<TValue> typedResult)
        {
            typedResult.InternalValue = default;
        }
        return result;
    }
}