namespace SatSolverCore.Clause;

internal class ClauseNary(List<int> literals) : IClause
{
    private readonly List<int> _literals = literals;

    /// <summary>
    /// Index to 1st watched literal
    /// </summary>
    private int _watched1 = 0;

    /// <summary>
    /// Index to 2nd watched literal
    /// </summary>
    private int _watched2 = 1;

    public int Watched1 => _literals[_watched1];

    public int Watched2 => _literals[_watched2];

    public (bool, int, int) FalsifyFirst(IPartialAssignment assignment)
    {
        if (assignment.IsTrue(Watched2))
        {
            // clause already satisfied, no need to reassign watched literals
            return (false, 0, 0);
        }

        if (assignment.IsFalse(Watched2))
        {
            // conflict
            return (true, 0, 0);
        }

        int n = _literals.Count;
        int propagate = Watched2;
        int watched = 0;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched1) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsFalse(_literals[j]))
            {
                propagate = 0;
                _watched1 = j;
                watched = _literals[j];
                break;
            }
        }

        return (false, propagate, watched);
    }

    public (bool, int, int) FalsifySecond(IPartialAssignment assignment)
    {
        if (assignment.IsTrue(Watched1))
        {
            // clause already satisfied, no need to reassign watched literals
            return (false, 0, 0);
        }

        if (assignment.IsFalse(Watched1))
        {
            // conflict
            return (true, 0, 0);
        }

        int n = _literals.Count;
        int propagate = Watched1;
        int watched = 0;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched2) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsFalse(_literals[j]))
            {
                propagate = 0;
                _watched2 = j;
                watched = _literals[j];
                break;
            }
        }

        return (false, propagate, watched);
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", _literals)}]";
    }
}
