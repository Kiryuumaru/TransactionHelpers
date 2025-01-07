using System;
using System.Collections.Generic;
using System.Text;
using TransactionHelpers.Interface;

namespace TransactionHelpers;

/// <summary>
/// Represents the result of an append operation, encapsulating the value to append, any errors, and flags indicating whether to append the value and errors.
/// </summary>
public class ResultAppend
{
    /// <summary>
    /// Gets or sets the value to append.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the collection of errors encountered during the append operation.
    /// </summary>
    public IEnumerable<Error?>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the collection of results encountered during the append operation.
    /// </summary>
    public IEnumerable<IResult?>? Results { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to append the value.
    /// </summary>
    public bool ShouldAppendValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to append the errors.
    /// </summary>
    public bool ShouldAppendErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to replace the errors.
    /// </summary>
    public bool ShouldReplaceErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to append the result values.
    /// </summary>
    public bool ShouldAppendResultValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to append the result errors.
    /// </summary>
    public bool ShouldAppendResultErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to replace the result errors.
    /// </summary>
    public bool ShouldReplaceResultErrors { get; set; }
}
