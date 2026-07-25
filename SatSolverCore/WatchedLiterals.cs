using SatSolverCore.Clause;

namespace SatSolverCore;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
public class WatchedLiterals
{
    private readonly int _nVar;
    private readonly LinkedList<IClause>[] _watchlist1;
    private readonly LinkedList<IClause>[] _watchlist2;

    public WatchedLiterals(int numberOfVars)
    {
        _nVar = numberOfVars;

        int n = 2 * numberOfVars + 1;
        _watchlist1 = new LinkedList<IClause>[n];
        _watchlist2 = new LinkedList<IClause>[n];

        for (int i = 0; i < n; ++i)
        {
            _watchlist1[i] = new();
            _watchlist2[i] = new();
        }
    }

    /// <summary>
    /// Add new clause to be tracked. This can be an initial clause found in the
    /// formula or a learned clause.
    /// </summary>
    /// <param name="clause">The clause to be tracked.</param>
    public void Add(IClause clause)
    {
        _watchlist1[clause.Watched1 + _nVar].AddLast(clause);

        int w2 = clause.Watched2;
        if (w2 != 0)
        {
            _watchlist2[w2 + _nVar].AddLast(clause);
        }
    }

    /// <summary>
    /// Tries to find new unit literals after the given literal is set to false.
    /// </summary>
    /// <param name="literal">The literal that is set to false.</param>
    /// <param name="assignment">The current partial assignment.</param>
    /// <param name="unitLiterals">The queue of unit literals to append.</param>
    /// <returns>A conflicting clause is one was detected; otherwise, null.</returns>
    public IClause? TryFindUnitLiterals(int literal, IPartialAssignment assignment, Queue<(int, IClause?)> unitLiterals)
    {
        IClause? conflict = FindUnitLiterals(unitLiterals, literal, assignment, _watchlist1, FalsifyFirst, _nVar);

        if (conflict != null)
        {
            return conflict;
        }

        return FindUnitLiterals(unitLiterals, literal, assignment, _watchlist2, FalsifySecond, _nVar);
    }

    private static IClause? FindUnitLiterals(
        Queue<(int, IClause?)> unitLiterals,
        int literal,
        IPartialAssignment assignment,
        LinkedList<IClause>[] watchlist,
        Func<IClause, IPartialAssignment, FalsifyResult> falsifyLiteral,
        int n
    )
    {
        LinkedList<IClause> list = watchlist[literal + n];
        LinkedListNode<IClause>? node = list.First;

        while (node != null)
        {
            var clause = node.ValueRef;

            var result = falsifyLiteral(clause, assignment);

            if (result.IsConflict)
            {
                return clause;
            }

            if (result.UnitLiteral != 0)
            {
                unitLiterals.Enqueue((result.UnitLiteral, clause));
            }

            if (result.NewWatchedLiteral != 0)
            {
                watchlist[result.NewWatchedLiteral + n].AddLast(clause);
                var previous = node;
                node = node.Next;
                list.Remove(previous);
            }
            else
            {
                node = node.Next;
            }
        }

        return null;
    }

    private static FalsifyResult FalsifyFirst(IClause clause, IPartialAssignment assignment)
    {
        return clause.FalsifyFirst(assignment);
    }

    private static FalsifyResult FalsifySecond(IClause clause, IPartialAssignment assignment)
    {
        return clause.FalsifySecond(assignment);
    }
}
