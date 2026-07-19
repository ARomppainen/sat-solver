namespace SatSolverCore.Clause;

internal class ClauseBinary(int literal1, int literal2) : IClause
{
    public int Watched1 => literal1;

    public int Watched2 => literal2;

    public (bool, int, int) FalsifyFirst(IPartialAssignment assignment)
    {
        if (assignment.IsFalse(literal2))
        {
            // conflict
            return (true, 0, 0);
        }

        if (assignment.IsTrue(literal2))
        {
            // clause already satisfied
            return (false, 0, 0);
        }

        // propagate 2nd literal
        return (false, literal2, 0);
    }

    public (bool, int, int) FalsifySecond(IPartialAssignment assignment)
    {
        if (assignment.IsFalse(literal1))
        {
            // conflict
            return (true, 0, 0);
        }

        if (assignment.IsTrue(literal1))
        {
            // clause already satisfied
            return (false, 0, 0);
        }

        // propagate 1st literal
        return (false, literal1, 0);
    }

    public override string ToString()
    {
        return $"[{literal1}, {literal2}]";
    }
}
