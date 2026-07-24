namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with two literals.
/// </summary>
/// <param name="literal1">1st literal</param>
/// <param name="literal2">2nd literal</param>
internal class ClauseBinary(int literal1, int literal2) : IClause
{
    /// <inheritdoc />
    public List<int> Literals => [literal1, literal2];

    /// <inheritdoc />
    public int Watched1 => literal1;

    /// <inheritdoc />
    public int Watched2 => literal2;

    /// <inheritdoc />
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(-literal2))
        {
            return FalsifyResult.Conflict();
        }

        if (assignment.IsAssigned(literal2))
        {
            // clause already satisfied
            return FalsifyResult.NoChanges();
        }

        return FalsifyResult.Propagate(literal2);
    }

    /// <inheritdoc />
    public FalsifyResult FalsifySecond(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(-literal1))
        {
            return FalsifyResult.Conflict();
        }

        if (assignment.IsAssigned(literal1))
        {
            // clause already satisfied
            FalsifyResult.NoChanges();
        }

        return FalsifyResult.Propagate(literal1);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{literal1}, {literal2}]";
    }
}
