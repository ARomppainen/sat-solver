using System.Diagnostics;

namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with N literals.
/// </summary>
internal class ClauseNary : IClause
{
    /// <inheritdoc />
    public List<int> Literals { get; }

    /// <summary>
    /// Index to 1st watched literal
    /// </summary>
    private int _watched1;

    /// <summary>
    /// Index to 2nd watched literal
    /// </summary>
    private int _watched2;

    /// <inheritdoc />
    public int Watched1 => Literals[_watched1];

    /// <inheritdoc />
    public int Watched2 => Literals[_watched2];

    public ClauseNary(List<int> literals, IPartialAssignment assignment)
    {
        Literals = literals;
        _watched1 = 0;
        _watched2 = 1;

        Debug.Assert(assignment.IsUnassigned(Watched1), $"Expected the first literal to be unassigned: {this}");

        if (assignment.IsAssigned(-literals[1]))
        {
            for (int i = 2; i < literals.Count; ++i)
            {
                if (!assignment.IsAssigned(-literals[i]))
                {
                    _watched2 = i;
                    break;
                }
            }
        }
    }

    /// <inheritdoc />
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(Watched2))
        {
            return FalsifyResult.NoChanges();
        }

        int n = Literals.Count;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched1) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-Literals[j]))
            {
                _watched1 = j;
                return FalsifyResult.UpdateWatchlist(Watched1);
            }
        }

        if (assignment.IsAssigned(-Watched2))
        {
            return FalsifyResult.Conflict();
        }

        return FalsifyResult.Propagate(Watched2);
    }

    /// <inheritdoc />
    public FalsifyResult FalsifySecond(IPartialAssignment assignment)
    {
        if (assignment.IsAssigned(Watched1))
        {
            return FalsifyResult.NoChanges();
        }

        int n = Literals.Count;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched2) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-Literals[j]))
            {
                _watched2 = j;
                return FalsifyResult.UpdateWatchlist(Watched2);
            }
        }

        if (assignment.IsAssigned(-Watched1))
        {
            return FalsifyResult.Conflict();
        }

        return FalsifyResult.Propagate(Watched1);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{string.Join(", ", Literals)}]";
    }
}
