namespace SatSolver.Core;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
public class WatchedLiterals
{
    private readonly int _nVar;
    private readonly LinkedList<List<int>>[] _watchlist;

    public WatchedLiterals(int numberOfVars)
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
    /// <param name="literals">The clause to be tracked.</param>
    public void Add(List<int> literals)
    {
        _watchlist[literals[0] + _nVar].AddLast(literals);
        _watchlist[literals[1] + _nVar].AddLast(literals);
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

            if (FindNewWatchedLiteral(literal, assignment, clause))
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

    private static bool FindNewWatchedLiteral(int literal, IPartialAssignment assignment, List<int> clause)
    {
        for (int index = 2; index < clause.Count; ++index)
        {
            if (!assignment.IsAssigned(-clause[index]))
            {
                clause[1] = clause[index];
                clause[index] = literal;
                return true;
            }
        }

        return false;
    }
}
