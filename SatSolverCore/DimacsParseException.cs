namespace SatSolverCore;

/// <summary>
/// Represents error that occur during parsing of DIMACS formatted input.
/// </summary>
public class DimacsParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DimacsParseException class.
    /// </summary>
    public DimacsParseException()
    { }

    /// <summary>
    /// Initializes a new instance of the Exception class with a specified error
    /// message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DimacsParseException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the DimacsParseException class with a
    /// specified error message and a reference to the inner exception that is
    /// the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the
    /// current exception.</param>
    public DimacsParseException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
