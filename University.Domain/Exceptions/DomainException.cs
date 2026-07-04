namespace University.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain rule violation occurs.
/// Used by domain entities to enforce business logic and constraints.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">The error message describing the domain rule violation.</param>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message describing the domain rule violation.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
