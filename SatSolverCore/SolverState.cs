namespace SatSolverCore;

public class SolverState
{
    private readonly Formula _formula;
    private readonly PartialAssignment _assignment;

    private readonly List<WatchedClause> _clauses;

    private readonly Dictionary<int, LinkedList<WatchedClause>> _watchlist;

    private List<int> _propagated;

    public SolverState(Formula formula)
    {
        _formula = formula;
        _assignment = new(formula.NumberOfVars);
        _clauses = new(formula.Clauses.Count);
        _watchlist = [];
        _propagated = [];


        foreach (Clause clause in formula.Clauses)
        {
            WatchedClause watched = new(clause);
            _clauses.Add(watched);

            AddToWatchlist(watched.Watched1, watched);
            AddToWatchlist(watched.Watched2, watched);
        }
    }

    public bool IsFalsified()
    {
        return _clauses.Any(clause => clause.IsFalsified(_assignment));
    }

    public bool IsSatisfied()
    {
        return _clauses.All(clause => clause.IsSatisfied(_assignment));
    }

    public bool DecideUnaryClauses()
    {
        foreach (int literal in _formula.UnaryClauses)
        {
            if (_assignment.IsAssignedTrue(literal))
            {
                continue;
            }

            if (_assignment.IsAssignedTrue(-literal))
            {
                // conflict
                return true;
            }

            _assignment.Decide(literal);
        }

        return false;
    }

    public bool Decide(int literal)
    {
        _assignment.Decide(literal);

        if (!TryAssignToFalse(-literal, out List<int> propagated))
        {
            _propagated = [];
            return true;
        }

        _propagated = propagated;

        return false;
    }

    public bool Propagate()
    {
        int i = 0;
        while (i < _propagated.Count)
        {
            int literal = _propagated[i];
            ++i;

            if (_assignment.IsUnassigned(literal))
            {
                _assignment.Propagate(literal);

                if (!TryAssignToFalse(-literal, out List<int> propagated))
                {
                    _propagated = [];
                    return true;
                }

                _propagated.AddRange(propagated);
            }
        }

        return false;
    }

    public void Backtrack()
    {
        _assignment.Backtrack();
    }

    public int ChooseUnassignedLiteral()
    {
        for (int i = 1; i < _formula.NumberOfVars; ++i)
        {
            if (_assignment.IsUnassigned(i))
            {
                return i;
            }
        }

        return _formula.NumberOfVars;
    }

    public void AssignRemainingLiteralsToTrue()
    {
        _assignment.AssignRemainingLiteralsToTrue(_formula);
    }

    public List<int> Assignment()
    {
        return _assignment.ToList();
    }

    private void AddToWatchlist(int literal, WatchedClause clause)
    {
        if (_watchlist.TryGetValue(literal, out var list))
        {
            list.AddLast(new LinkedListNode<WatchedClause>(clause));
        }
        else
        {
            _watchlist.Add(literal, new LinkedList<WatchedClause>([clause]));
        }
    }

    private bool TryAssignToFalse(int literal, out List<int> propagated)
    {
        propagated = [];

        if (_watchlist.TryGetValue(literal, out var list))
        {
            var node = list.First;
            while (node != null)
            {
                var clause = node.ValueRef;

                (bool conflict, int propagate, int watched) = clause.AssignToFalse(literal, _assignment);

                if (conflict)
                {
                    LearnClause();
                    propagated = [];
                    return false;
                }

                if (propagate != 0)
                {
                    propagated.Add(propagate);
                }

                if (watched != 0)
                {
                    AddToWatchlist(watched, clause);
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

        return true;
    }

    private void LearnClause()
    {
        List<int> literals = _assignment.GetConflictTrail();

        if (literals.Count > 1)
        {
            Clause clause = new(literals);
            WatchedClause watched = new(clause);
            _clauses.Add(watched);

            AddToWatchlist(watched.Watched1, watched);
            AddToWatchlist(watched.Watched2, watched);
        }
    }
}
