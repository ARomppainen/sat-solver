namespace SatSolverCore;

public class PartialAssignment(int numberOfVars)
{
    private readonly Stack<ValueTuple<int, bool>> _stack = new(numberOfVars);
    private readonly HashSet<int> _set = new(numberOfVars);

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

    public void AssignRemainingLiteralsToTrue(Formula formula)
    {
        for (int i = 1; i <= formula.NumberOfVars; ++i)
        {
            if (IsUnassigned(i))
            {
                _stack.Push(ValueTuple.Create(i, false));
            }
        }
    }

    public List<int> ToList()
    {
        return [.. _stack.Select(t => t.Item1).OrderBy(Math.Abs)];
    }
}
