﻿using System.Diagnostics.CodeAnalysis;
using System;

namespace TransactionHelpers.Interface;

/// <summary>
/// The interface for all responses.
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Gets the <see cref="TransactionHelpers.Error"/> of the operation.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets <c>true</c> whether the operation is successful; otherwise, <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError { get; }

    /// <summary>
    /// Throws if the response has any error.
    /// </summary>
    void ThrowIfError();
}
