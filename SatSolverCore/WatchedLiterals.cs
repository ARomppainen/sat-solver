namespace SatSolverCore;

public class WatchedLiterals
{
    private readonly Dictionary<int, LinkedList<Clause>> watchlist;

    public WatchedLiterals(Formula formula)
    {
        watchlist = [];

        foreach (var clause in formula.Clauses)
        {
            AddOrAppend(clause.Watched1, clause);
            AddOrAppend(clause.Watched2, clause);
        }
    }

    private void AddOrAppend(int literal, Clause clause)
    {
        if (watchlist.TryGetValue(literal, out var list))
        {
            list.AddLast(new LinkedListNode<Clause>(clause));
        }
        else
        {
            watchlist.Add(literal, new LinkedList<Clause>([clause]));
        }
    }


    public bool TryAssignToFalse(int literal, PartialAssignment assignment, out List<int> propagated)
    {
        HashSet<int> result = [];

        if (watchlist.TryGetValue(literal, out var list))
        {
            var node = list.First;
            while (node != null)
            {
                var clause = node.ValueRef;

                (bool conflict, int propagate, int watched) = clause.AssignToFalse(literal, assignment);

                if (conflict)
                {
                    propagated = [];
                    return false;
                }

                if (propagate != 0)
                {
                    result.Add(propagate);
                }

                if (watched != 0)
                {
                    AddOrAppend(watched, clause);
                    var remove = node;
                    node = remove.Next;
                    list.Remove(remove);
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        propagated = [.. result];
        return true;
    }
}