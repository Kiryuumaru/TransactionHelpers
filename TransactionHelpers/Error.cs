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
        init
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
        init
        {
            messageException = new Exception(value);
        }
    }
}
