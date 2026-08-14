namespace SatSolver.Core;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
/// <remarks>
/// The watched literals are always the first two elements in the list literals,
/// i.e. clause[0] and clause[1].
/// </remarks>
public class WatchedLiteralsV1
{
    private readonly int _nVar;
    private readonly LinkedList<List<int>>[] _watchlist;

    public WatchedLiteralsV1(int numberOfVars)
    {
        _nVar = numberOfVars;

        int n = 2 * numberOfVars + 1;
        _watchlist = new LinkedList<List<int>>[n];

        for (int i = 0; i < n; ++i)
        {
            _watchlist[i] = new();
        }
    }

    /// <summary>
    /// Add new clause to be tracked. This can be an initial clause found in the
    /// formula or a learned clause.
    /// </summary>
    /// <param name="clause">The clause to be tracked.</param>
    public void Add(List<int> clause)
    {
        _watchlist[clause[0] + _nVar].AddLast(clause);
        _watchlist[clause[1] + _nVar].AddLast(clause);
    }

    /// <summary>
    /// Tries to find new unit literals after the given literal is set to false.
    /// </summary>
    /// <param name="literal">The literal that is set to false.</param>
    /// <param name="assignment">The current partial assignment.</param>
    /// <param name="unitLiterals">The queue of unit literals to append.</param>
    /// <returns>A conflicting clause is one was detected; otherwise, null.</returns>
    public List<int>? FindUnitLiterals(int literal, IPartialAssignment assignment, Queue<(int, List<int>?)> unitLiterals)
    {
        LinkedList<List<int>> list = _watchlist[literal + _nVar];
        LinkedListNode<List<int>>? node = list.First;

        while (node != null)
        {
            List<int> clause = node.ValueRef;

            if (clause[0] == literal)
            {
                clause[0] = clause[1];
                clause[1] = literal;
            }

            if (assignment.IsAssigned(clause[0]))
            {
                node = node.Next;
                continue;
            }

            if (WatchedLiteralsUtil.FindNewWatchedLiteral(literal, assignment, clause))
            {
                _watchlist[clause[1] + _nVar].AddLast(clause);
                var previous = node;
                node = node.Next;
                list.Remove(previous);
                continue;
            }
            else
            {
                node = node.Next;
            }

            if (assignment.IsAssigned(-clause[0]))
            {
                return clause;
            }

            unitLiterals.Enqueue((clause[0], clause));
        }

        return null;
    }
}
