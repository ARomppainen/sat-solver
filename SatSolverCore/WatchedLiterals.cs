using SatSolverCore.Clause;

namespace SatSolverCore;

public class WatchedLiterals
{
    private readonly Dictionary<int, LinkedList<IClause>> _watchlist1;
    private readonly Dictionary<int, LinkedList<IClause>> _watchlist2;

    public WatchedLiterals()
    {
        _watchlist1 = [];
        _watchlist2 = [];
    }

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

    public bool TryFindUnitLiterals(int literal, IPartialAssignment assignment, out List<int> unitLiterals)
    {
        unitLiterals = [];

        if (FindUnitLiterals(unitLiterals, literal, assignment, _watchlist1, FalsifyFirst))
        {
            unitLiterals = [];
            return false;
        }

        if (FindUnitLiterals(unitLiterals, literal, assignment, _watchlist2, FalsifySecond))
        {
            unitLiterals = [];
            return false;
        }

        return true;
    }

    private static bool FindUnitLiterals(
        List<int> unitLiterals,
        int literal,
        IPartialAssignment assignment,
        Dictionary<int, LinkedList<IClause>> watchlist,
        Func<IClause, IPartialAssignment, (bool, int, int)> falsifyLiteral
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

            (bool conflict, int propagate, int watched) = falsifyLiteral(clause, assignment);

            if (conflict)
            {
                return true;
            }

            if (propagate != 0)
            {
                unitLiterals.Add(propagate);
            }

            if (watched != 0)
            {
                Add(watched, clause, watchlist);
                var remove = node;
                node = node.Next;
                list.Remove(remove);
            }
            else
            {
                node = node.Next;
            }
        }

        return false;
    }

    private static (bool, int, int) FalsifyFirst(IClause clause, IPartialAssignment assignment)
    {
        return clause.FalsifyFirst(assignment);
    }

    private static (bool, int, int) FalsifySecond(IClause clause, IPartialAssignment assignment)
    {
        return clause.FalsifySecond(assignment);
    }
}
