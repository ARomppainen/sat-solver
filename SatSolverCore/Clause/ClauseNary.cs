namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with N literals.
/// </summary>
/// <param name="literals">list of literals</param>
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

    /// <inheritdoc />
    public int Watched1 => _literals[_watched1];

    /// <inheritdoc />
    public int Watched2 => _literals[_watched2];

    /// <inheritdoc />
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(-Watched2))
        {
            return FalsifyResult.Conflict();
        }

        if (assignment.IsAssigned(Watched2))
        {
            // clause already satisfied, no need to reassign watched literals
            return FalsifyResult.NoChanges();
        }

        int n = _literals.Count;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched1) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-_literals[j]))
            {
                _watched1 = j;
                return FalsifyResult.UpdateWatchlist(Watched1);
            }
        }

        return FalsifyResult.Propagate(Watched2);
    }

    /// <inheritdoc />
    public FalsifyResult FalsifySecond(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(-Watched1))
        {
            return FalsifyResult.Conflict();
        }

        if (assignment.IsAssigned(Watched1))
        {
            // clause already satisfied, no need to reassign watched literals
            return FalsifyResult.NoChanges();
        }

        int n = _literals.Count;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched2) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-_literals[j]))
            {
                _watched2 = j;
                return FalsifyResult.UpdateWatchlist(Watched2);
            }
        }

        return FalsifyResult.Propagate(Watched1);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{string.Join(", ", _literals)}]";
    }
}
