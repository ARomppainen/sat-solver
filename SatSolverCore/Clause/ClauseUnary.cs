namespace SatSolverCore.Clause;

internal class ClauseUnary(int literal) : IClause
{
    public int Literal => literal;

    public int Watched1 => literal;

    public int Watched2 => 0;

    public (bool, int, int) FalsifyFirst(IPartialAssignment assignment)
    {
        return (true, 0, 0);
    }

    public (bool, int, int) FalsifySecond(IPartialAssignment assignment)
    {
        throw new NotSupportedException("Not supported by Unary clauses");
    }

    public override string ToString()
    {
        return $"[{Literal}]";
    }
}
