namespace SatSolverCore;

public class Clause(List<int> literals)
{
    public List<int> Literals { get; private set; } = literals;

    public int Watched1 { get { return Literals[_watched1]; } }

    public int Watched2 { get { return Literals[_watched2]; } }

    /// <summary>
    /// Index to 1st watched literal
    /// </summary>
    private int _watched1 = 0;

    /// <summary>
    /// Index to 2nd watched literal
    /// </summary>
    private int _watched2 = 1;

    public bool IsFalsified(PartialAssignment assignment)
    {
        return Literals.All(assignment.IsAssignedFalse);
    }

    public bool IsSatisfied(PartialAssignment assignment)
    {
        return Literals.Any(assignment.IsAssignedTrue);
    }

    public ValueTuple<bool, int, int> AssignToFalse(int literal, PartialAssignment assignment)
    {
        if (literal == Watched1)
        {
            return AssignToFalseFirst(assignment);
        }

        if (literal == Watched2)
        {
            return AssignToFalseSecond(assignment);
        }

        throw new Exception($"Could not match literal {literal} to watched literals");
    }

    private ValueTuple<bool, int, int> AssignToFalseFirst(PartialAssignment assignment)
    {
        int propagate = 0;
        int watched = 0;

        int n = Literals.Count;

        if (assignment.IsAssignedTrue(Watched2))
        {
            // clause already satisfied, no need to reassign watched literals
            return ValueTuple.Create(false, 0, 0);
        }

        if (assignment.IsAssignedFalse(Watched2))
        {
            // conflict
            return ValueTuple.Create(true, 0, 0);
        }

        bool isUnitClause = true;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched1) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (assignment.IsUnassigned(Literals[j]))
            {
                _watched1 = j;
                watched = Literals[j];
                isUnitClause = false;
                break;
            }
        }

        if (isUnitClause)
        {
            propagate = Watched2;
        }

        return ValueTuple.Create(false, propagate, watched);
    }

    private ValueTuple<bool, int, int> AssignToFalseSecond(PartialAssignment assignment)
    {
        int propagate = 0;
        int watched = 0;

        int n = Literals.Count;

        if (assignment.IsAssignedTrue(Watched1))
        {
            // clause already satisfied, no need to reassign watched literals
            return ValueTuple.Create(false, 0, 0);
        }

        if (assignment.IsAssignedFalse(Watched1))
        {
            // conflict
            return ValueTuple.Create(true, 0, 0);
        }

        bool isUnitClause = true;

        for (int i = 0; i < n; ++i)
        {
            int j = (i + _watched2) % n;

            if (j == _watched1 || j == _watched2)
            {
                continue;
            }

            if (assignment.IsUnassigned(Literals[j]))
            {
                _watched2 = j;
                watched = Literals[j];
                isUnitClause = false;
                break;
            }
        }

        if (isUnitClause)
        {
            propagate = Watched1;
        }

        return ValueTuple.Create(false, propagate, watched);
    }
}
