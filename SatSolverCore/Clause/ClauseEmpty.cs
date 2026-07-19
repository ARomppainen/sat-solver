namespace SatSolverCore.Clause;

internal class ClauseEmpty : IClause
{
    public int Watched1 => 0;

    public int Watched2 => 0;

    public (bool, int, int) FalsifyFirst(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Empty clauses");
    }

    public (bool, int, int) FalsifySecond(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Empty clauses");
    }

    public override string ToString()
    {
        return "[]";
    }
}
