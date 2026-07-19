namespace SatSolverCore;

public class PartialAssignment(int numberOfVars) : IPartialAssignment
{
    private readonly Stack<TrailElement> _trail = new(numberOfVars);
    private readonly HashSet<int> _set = new(numberOfVars);

    public int Count => _trail.Count;

    public int LastDecision()
    {
        return _trail.Where(t => t.IsDecided).Select(t => t.Literal).FirstOrDefault(0);
    }

    public bool IsTrue(int literal)
    {
        return _set.Contains(literal);
    }

    public bool IsFalse(int literal)
    {
        return _set.Contains(-literal);
    }

    public bool IsUnassigned(int literal)
    {
        return !_set.Contains(literal) && !_set.Contains(-literal);
    }

    public void AddDecision(int literal, int level)
    {
        _trail.Push(new(literal, level, true));
        _set.Add(literal);
    }

    public void AddPropagated(int literal, int level)
    {
        _trail.Push(new(literal, level, false));
        _set.Add(literal);
    }

    public void Backjump(int level)
    {
        while (_trail.Count > 0)
        {
            (int lit, int lvl, _) = _trail.Peek();

            if (lvl <= level)
            {
                break;
            }

            _trail.Pop();
            _set.Remove(lit);
        }
    }

    public (List<int>, int) GetConflictClause()
    {
        List<TrailElement> decisions = [.. _trail.Where(t => t.IsDecided)];
        int level = decisions.Count > 1 ? decisions[1].Level : 0;
        List<int> clause = [.. decisions.Select(t => -t.Literal)];
        return (clause, level);
    }

    public List<int> ToList()
    {
        return [.. _trail.Select(t => t.Literal).OrderBy(Math.Abs)];
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", _trail.Select(t => t.Literal).Reverse())}]";
    }

    private record struct TrailElement(int Literal, int Level, bool IsDecided);
}
