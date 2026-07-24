namespace SatSolverCore.Clause;

/// <summary>
/// Factory for instantiating <see cref="IClause"/> instances.
/// </summary>
public static class ClauseFactory
{
    /// <summary>
    /// Create a new <see cref="IClause"/> instance based on list of literals.
    /// </summary>
    /// <param name="literals">list of literals</param>
    /// <param name="assignment">current assignment</param>
    /// <returns>new concrete <see cref="IClause"/> instance</returns>
    public static IClause Create(List<int> literals, IPartialAssignment assignment)
    {
        return literals.Count switch
        {
            0 => throw new ArgumentException("Empty clauses are not supported."),
            1 => new ClauseUnary(literals[0]),
            2 => new ClauseBinary(literals[0], literals[1]),
            _ => new ClauseNary(literals, assignment)
        };
    }
}
