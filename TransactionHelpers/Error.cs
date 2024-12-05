using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TransactionHelpers;

/// <summary>
/// Represents the _error holder for all transactions.
/// </summary>
public class Error
{
    private Exception? exception;
    private Exception? messageException;
    private string? errorCode;

    /// <summary>
    /// Gets the <see cref="System.Exception"/> of the _error.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception
    {
        get
        {
            if (exception == null && messageException != null)
            {
                return messageException;
            }
            return exception;
        }
        set
        {
            exception = value;
        }
    }

    /// <summary>
    /// Gets the message of the _error.
    /// </summary>
    public string? Message
    {
        get
        {
            if (exception != null && messageException == null)
            {
                return exception.Message;
            }
            return messageException?.Message;
        }
        set
        {
            messageException = new Exception(value);
        }
    }

    /// <summary>
    /// Gets the error code of the _error.
    /// </summary>
    public string? ErrorCode
    {
        get
        {
            return errorCode;
        }
        set
        {
            errorCode = value;
        }
    }
}
