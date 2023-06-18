using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TransactionHelpers;

/// <summary>
/// Represents the error holder for all transactions.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets the <see cref="System.Exception"/> of the error.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception { get; protected set; }

    /// <summary>
    /// Gets the message of the error.
    /// </summary>
    public string? Message { get; protected set; }

    /// <summary>
    /// Creates an instance of <see cref="Error"/>.
    /// </summary>
    public Error()
    {

    }

    /// <summary>
    /// Creates an instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="exception">The <see cref="System.Exception"/> of the error.</param>
    public Error(Exception? exception)
    {
        Exception = exception;
        if (exception != null)
        {
            Message = exception.Message;
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="message">The message of the error.</param>
    public Error(string? message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates an instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="exception">The <see cref="System.Exception"/> of the error.</param>
    /// <param name="message">The message of the error.</param>
    [JsonConstructor]
    public Error(Exception? exception, string? message)
    {
        Exception = exception;
        if (string.IsNullOrEmpty(message) && exception != null)
        {
            Message = exception.Message;
        }
        else
        {
            Message = message;
        }
    }
}
