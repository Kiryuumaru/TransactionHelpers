using System;

namespace TransactionHelpers.Exceptions;

/// <summary>
/// Exception for responses with no result and no error.
/// </summary>
public class EmptyResultException : Exception
{
    internal EmptyResultException()
        : base("The response has no result.")
    {
        
    }
}
