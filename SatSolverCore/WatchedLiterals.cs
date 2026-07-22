using SatSolverCore.Clause;

namespace SatSolverCore;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
public class WatchedLiterals
{
    private readonly Dictionary<int, LinkedList<IClause>> _watchlist1;
    private readonly Dictionary<int, LinkedList<IClause>> _watchlist2;

    public WatchedLiterals()
    {
        _watchlist1 = [];
        _watchlist2 = [];
    }

    /// <summary>
    /// Add new clause to be tracked. This can be an initial clause found in the
    /// formula or a learned clause.
    /// </summary>
    /// <param name="clause">The clause to be tracked.</param>
    public void Add(IClause clause)
    {
        int w1 = clause.Watched1;
        if (w1 != 0)
        {
            Add(w1, clause, _watchlist1);

            int w2 = clause.Watched2;
            if (w2 != 0)
            {
                Add(w2, clause, _watchlist2);
            }
        }
    }

    private static void Add(int literal, IClause clause, Dictionary<int, LinkedList<IClause>> watchlist)
    {
        if (watchlist.TryGetValue(literal, out var list))
        {
            list.AddLast(new LinkedListNode<IClause>(clause));
        }
        else
        {
            watchlist.Add(literal, new LinkedList<IClause>([clause]));
        }
    }

    /// <summary>
    /// Tries to find new unit literals after the given literal is set to false.
    /// </summary>
    /// <param name="literal">The literal that is set to false.</param>
    /// <param name="assignment">The current partial assignment.</param>
    /// <param name="unitLiterals">The queue of unit literals to append.</param>
    /// <returns>true if a conflicting clause was not detected; otherwise, false.</returns>
    public bool TryFindUnitLiterals(int literal, IPartialAssignment assignment, Queue<int> unitLiterals)
    {
        if (FindUnitLiterals(unitLiterals, literal, assignment, _watchlist1, FalsifyFirst))
        {
            return false;
        }

        if (FindUnitLiterals(unitLiterals, literal, assignment, _watchlist2, FalsifySecond))
        {
            return false;
        }

        return true;
    }

    private static bool FindUnitLiterals(
        Queue<int> unitLiterals,
        int literal,
        IPartialAssignment assignment,
        Dictionary<int, LinkedList<IClause>> watchlist,
        Func<IClause, IPartialAssignment, FalsifyResult> falsifyLiteral
    )
    {
        if (!watchlist.TryGetValue(literal, out var list))
        {
            return false;
        }

        LinkedListNode<IClause>? node = list.First;

        while (node != null)
        {
            var clause = node.ValueRef;

            var result = falsifyLiteral(clause, assignment);

            if (result.IsConflict)
            {
                return true;
            }

            if (result.PropagatedLiteral != 0)
            {
                unitLiterals.Enqueue(result.PropagatedLiteral);
            }

            if (result.NewWatchedLiteral != 0)
            {
                Add(result.NewWatchedLiteral, clause, watchlist);
                var previous = node;
                node = node.Next;
                list.Remove(previous);
            }
            else
            {
                node = node.Next;
            }
        }

        return false;
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
