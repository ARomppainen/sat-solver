namespace SatSolver.Core;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
public class WatchedLiterals
{
    private readonly int _nVar;
    private readonly LinkedList<int>[] _binaryWatchlist;
    private readonly LinkedList<Clause>[] _watchlist;

    public WatchedLiterals(int numberOfVars)
    {
        _nVar = numberOfVars;

        int n = 2 * numberOfVars + 1;
        _binaryWatchlist = new LinkedList<int>[n];
        _watchlist = new LinkedList<Clause>[n];

        for (int i = 0; i < n; ++i)
        {
            _binaryWatchlist[i] = new();
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
        if (literals.Count == 2)
        {
            _binaryWatchlist[literals[0] + _nVar].AddLast(literals[1]);
            _binaryWatchlist[literals[1] + _nVar].AddLast(literals[0]);
        }
        else if (literals.Count > 2)
        {
            Clause clause = new(literals);

            _watchlist[literals[0] + _nVar].AddLast(clause);
            _watchlist[literals[1] + _nVar].AddLast(clause);
        }
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
        List<int>? conflict = FindUnitLiteralsBinary(unitLiterals, literal, assignment);

        if (conflict != null)
        {
            return conflict;
        }

        return FindUnitLiterals(unitLiterals, literal, assignment);
    }

    private List<int>? FindUnitLiteralsBinary(
        Queue<(int, List<int>?)> unitLiterals,
        int literal,
        IPartialAssignment assignment
    )
    {
        LinkedListNode<int>? node = _binaryWatchlist[literal + _nVar].First;

        while (node != null)
        {
            int other = node.Value;
            node = node.Next;

            if (assignment.IsAssigned(-other))
            {
                return [literal, other];
            }

            if (!assignment.IsAssigned(other))
            {
                unitLiterals.Enqueue((other, [literal, other]));
            }
        }

        return null;
    }

    private List<int>? FindUnitLiterals(
        Queue<(int, List<int>?)> unitLiterals,
        int literal,
        IPartialAssignment assignment
    )
    {
        LinkedList<Clause> list = _watchlist[literal + _nVar];
        LinkedListNode<Clause>? node = list.First;

        while (node != null)
        {
            var clause = node.ValueRef;

            var result = clause.Falsify(literal, assignment);

            if (result.IsConflict)
            {
                return clause.Literals;
            }

            if (result.UnitLiteral != 0)
            {
                unitLiterals.Enqueue((result.UnitLiteral, clause.Literals));
            }

            if (result.NewWatchedLiteral != 0)
            {
                _watchlist[result.NewWatchedLiteral + _nVar].AddLast(clause);
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
}
