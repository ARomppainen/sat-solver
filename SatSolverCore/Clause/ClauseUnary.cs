namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with one literal.
/// </summary>
/// <param name="literal">the literal value</param>
internal class ClauseUnary(int literal) : IClause
{
    /// <inheritdoc />
    public int Literal => literal;

    /// <inheritdoc />
    public int Watched1 => literal;

    /// <inheritdoc />
    public int Watched2 => 0;

    /// <inheritdoc />
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment)
    {
        return FalsifyResult.Conflict();
    }

    /// <inheritdoc />
    public FalsifyResult FalsifySecond(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Unary clauses");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Literal}]";
    }
}
