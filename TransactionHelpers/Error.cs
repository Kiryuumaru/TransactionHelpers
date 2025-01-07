using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TransactionHelpers;

/// <summary>
/// Represents the error holder for all transactions, encapsulating exception details, error message, and error code.
/// </summary>
public class Error : ICloneable
{
    private Exception? exception;
    private Exception? messageException;
    private string? code;
    private object? detail;

    /// <summary>
    /// Gets or sets the <see cref="System.Exception"/> associated with the error.
    /// If <see cref="Exception"/> is not set, it returns the <see cref="Message"/> if available.
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
    /// Gets or sets the error message.
    /// If <see cref="Exception"/> is set and <see cref="Message"/> is not, it returns the message from <see cref="Exception"/>.
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
    /// Gets or sets the error code associated with the error.
    /// </summary>
    public string? Code
    {
        get
        {
            return code;
        }
        set
        {
            code = value;
        }
    }

    /// <summary>
    /// Gets or sets additional details about the error.
    /// </summary>
    public object? Detail
    {
        get
        {
            return detail;
        }
        set
        {
            detail = value;
        }
    }

    /// <inheritdoc/>
    public virtual object Clone()
    {
        return new Error
        {
            Exception = Exception is ICloneable cloneableException ? (Exception)cloneableException.Clone() : Exception,
            Message = Message,
            Code = Code,
            Detail = Detail is ICloneable cloneableDetail ? cloneableDetail.Clone() : Detail
        };
    }
}
