namespace SatSolverCore;

public class PartialAssignment
{
    private Stack<ValueTuple<int, bool>> _stack;
    private HashSet<int> _set;

    public int Count { get { return _stack.Count; } }

    private PartialAssignment()
    {
        _stack = new Stack<ValueTuple<int, bool>>();
        _set = new HashSet<int>();
    }

    public static PartialAssignment Empty()
    {
        return new PartialAssignment();
    }

    public bool DecideUnaryClauses(List<int> unaryClauses)
    {
        foreach (int literal in unaryClauses)
        {
            if (_set.Contains(literal))
            {
                continue;
            }

            if (_set.Contains(-literal))
            {
                // conflict
                return true;
            }

            Decide(literal);
        }

        return false;
    }

    public void Decide(int literal)
    {
        _stack.Push(ValueTuple.Create(literal, false));
        _set.Add(literal);
    }

    public void Propagate(int literal)
    {
        _stack.Push(ValueTuple.Create(literal, true));
        _set.Add(literal);
    }

    public void Backtrack()
    {
        bool propagated;

        do
        {
            (int literal, propagated) = _stack.Pop();
            _set.Remove(literal);
        } while (propagated);
    }

    public bool IsAssignedTrue(int literal)
    {
        return _set.Contains(literal);
    }

    public bool IsAssignedFalse(int literal)
    {
        return _set.Contains(-literal);
    }

    public bool IsUnassigned(int literal)
    {
        return !_set.Contains(literal) && !_set.Contains(-literal);
    }

    public List<int> ToList()
    {
        return [.. _stack.Select(t => t.Item1).OrderBy(Math.Abs)];
    }
}
