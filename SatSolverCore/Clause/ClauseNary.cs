using System.Diagnostics;

namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause with N literals.
/// </summary>
public class ClauseNary : IClause
{
    private int _watchIndex1;
    private int _watchIndex2;

    /// <inheritdoc />
    public List<int> Literals { get; }

    /// <inheritdoc />
    public int Watched1 => Literals[_watchIndex1];

    /// <inheritdoc />
    public int Watched2 => Literals[_watchIndex2];

    public ClauseNary(List<int> literals, int watchIndex1, int watchIndex2)
    {
        Literals = literals;
        _watchIndex1 = watchIndex1;
        _watchIndex2 = watchIndex2;

        Debug.Assert(watchIndex1 != watchIndex2);
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
            int j = (i + _watchIndex1) % n;

            if (j == _watchIndex1 || j == _watchIndex2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-Literals[j]))
            {
                _watchIndex1 = j;
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
            int j = (i + _watchIndex2) % n;

            if (j == _watchIndex1 || j == _watchIndex2)
            {
                continue;
            }

            if (!assignment.IsAssigned(-Literals[j]))
            {
                _watchIndex2 = j;
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
