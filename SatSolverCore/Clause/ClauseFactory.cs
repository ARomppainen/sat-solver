namespace SatSolverCore.Clause;

public static class ClauseFactory
{
    public static IClause Create(List<int> literals)
    {
        return literals.Count switch
        {
            0 => new ClauseEmpty(),
            1 => new ClauseUnary(literals[0]),
            2 => new ClauseBinary(literals[0], literals[1]),
            _ => new ClauseNary(literals)
        };
    }
}
