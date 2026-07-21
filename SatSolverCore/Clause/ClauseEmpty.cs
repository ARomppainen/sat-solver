namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with zero literals.
/// </summary>
internal class ClauseEmpty : IClause
{
    /// <inheritdoc />
    public int Watched1 => 0;

    /// <inheritdoc />
    public int Watched2 => 0;

    /// <inheritdoc />
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Empty clauses");
    }

    /// <inheritdoc />
    public FalsifyResult FalsifySecond(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Empty clauses");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "[]";
    }
}
