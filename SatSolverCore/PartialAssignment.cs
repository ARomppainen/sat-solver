namespace SatSolverCore;

public class PartialAssignment
{
    private Stack<int> _stack;
    private HashSet<int> _set;

    private PartialAssignment()
    {
        _stack = new Stack<int>();
        _set = new HashSet<int>();
    }

    public static PartialAssignment Empty()
    {
        return new PartialAssignment();
    }

    public bool DecideUnaryClauses(List<int> unaryClauses)
    {
        foreach (int unary in unaryClauses)
        {
            if (_set.Contains(unary))
            {
                continue;
            }

            if (_set.Contains(-unary))
            {
                // conflict
                return true;
            }

            Push(unary);
        }

        return false;
    }

    public void Push(int variable)
    {
        _stack.Push(variable);
        _set.Add(variable);
    }

    public void Pop()
    {
        int value = _stack.Pop();
        _set.Remove(value);
    }

    public bool IsAssignedTrue(int variable)
    {
        return _set.Contains(variable);
    }

    public bool IsAssignedFalse(int variable)
    {
        return _set.Contains(-variable);
    }

    public bool IsUnassigned(int variable)
    {
        return !_set.Contains(variable) && !_set.Contains(-variable);
    }

    public List<int> ToList()
    {
        return [.. _stack];
    }
}
