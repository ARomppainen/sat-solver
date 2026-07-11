namespace SatSolverCore;

[Serializable]
public class DimacsParseError : Exception
{
    public DimacsParseError()
    { }

    public DimacsParseError(string message)
        : base(message)
    { }

    public DimacsParseError(string message, Exception innerException)
        : base(message, innerException)
    { }
}
