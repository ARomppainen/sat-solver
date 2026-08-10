using System.Diagnostics;

namespace SatSolver.Core;

/// <summary>
/// Represents a clause with N literals (N > 2).
/// </summary>
public class Clause
{
    private int _watchIndex1;
    private int _watchIndex2;

    /// <summary>
    /// The list of literals in the clause.
    /// </summary>
    public List<int> Literals { get; }

    /// <summary>
    /// 1st watched literal value
    /// </summary>
    public int Watched1 => Literals[_watchIndex1];

    /// <summary>
    /// 2nd watched literal value
    /// </summary>
    public int Watched2 => Literals[_watchIndex2];

    public Clause(List<int> literals, int watchIndex1 = 0, int watchIndex2 = 1)
    {
        Literals = literals;
        _watchIndex1 = watchIndex1;
        _watchIndex2 = watchIndex2;

        Debug.Assert(watchIndex1 != watchIndex2);
    }

    /// <summary>
    /// Falsify the first watched literal
    /// </summary>
    /// <param name="assignment">current truth assignment</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
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

    /// <summary>
    /// Falsify the second watched literal
    /// </summary>
    /// <param name="assignment">current truth assignment</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
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
