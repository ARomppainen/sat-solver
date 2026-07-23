using SatSolverCore.Clause;

namespace SatSolverCore;

public class PartialAssignment(int numberOfVars) : IPartialAssignment
{
    private readonly Stack<int> _trail = new(numberOfVars);
    private readonly int[] _level = new int[numberOfVars + 1];
    private readonly IClause?[] _reason = new IClause?[numberOfVars + 1];
    private readonly HashSet<int> _assignment = new(numberOfVars);

    /// <summary>
    /// Gets the number of assigned literals.
    /// </summary>
    public int Count => _trail.Count;

    /// <inheritdoc />
    public bool IsAssigned(int variable)
    {
        return _assignment.Contains(variable);
    }

    /// <inheritdoc />
    public bool IsUnassigned(int literal)
    {
        return !_assignment.Contains(literal) && !_assignment.Contains(-literal);
    }

    /// <summary>
    /// Add new decided literal to the trail with given decision level.
    /// </summary>
    /// <param name="literal">literal value (a non-zero integer)</param>
    /// <param name="level">decision level (non-negative integer)</param>
    public void AddDecision(int literal, int level)
    {
        _trail.Push(literal);
        _level[Math.Abs(literal)] = level;
        _reason[Math.Abs(literal)] = null;
        _assignment.Add(literal);
    }

    /// <summary>
    /// Add new propagated literal to the trail with given decision level.
    /// </summary>
    /// <param name="literal">literal value (a non-zero integer)</param>
    /// <param name="level">decision level (non-negative integer)</param>
    /// <param name="reason">the clause the was the reason for the propagated literal</param>
    public void AddPropagated(int literal, int level, IClause reason)
    {
        _trail.Push(literal);
        _level[Math.Abs(literal)] = level;
        _reason[Math.Abs(literal)] = reason;
        _assignment.Add(literal);
    }

    /// <summary>
    /// Get the last decided literal.
    /// </summary>
    /// <returns>the last decided literal (or zero if there are not decisions)</returns>
    public int GetLastDecision()
    {
        return _trail.Where(t => _reason[Math.Abs(t)] == null).FirstOrDefault(0);
    }

    /// <summary>
    /// Backjump to given decision level. Removes all decided and propagated
    /// literals with a greater decision level from the trail.
    /// </summary>
    /// <param name="level"></param>
    public void Backjump(int level)
    {
        while (_trail.Count > 0)
        {
            int lit = _trail.Peek();
            int v = Math.Abs(lit);

            if (_level[v] <= level)
            {
                break;
            }

            _trail.Pop();
            _assignment.Remove(lit);
            _level[v] = 0;
            _reason[v] = null;
        }
    }

    /// <summary>
    /// Get a conflict clause that can be learned by the solver.
    /// </summary>
    /// <returns>
    /// a tuple where the elements are <br/>
    ///   1) the conflict clause <br/>
    ///   2) the decision level to backjump into
    /// </returns>
    public (List<int>, int) GetConflictClause()
    {
        List<int> decisions = [.. _trail.Where(t => _reason[Math.Abs(t)] == null)];

        // Backjump into the second highest decision level
        int level = decisions.Count > 1 ? _level[Math.Abs(decisions[1])] : 0;

        // Note that the literals in the clause are in reverse decision order.
        // E.g., if the decisions are 4, -2 and 5, the clause will be [5, -2, 4].
        // This ordering is currently important, because the clauses with three
        // or more literals will always be assigned to "watch" the first two
        // literals in the list. Since backjump will remove the literal "5" from
        // the trail, the learned clause will become a unit literal.
        List<int> clause = [.. decisions.Select(t => -t)];

        return (clause, level);
    }

    /// <summary>
    /// Returns a list representation of the assignment.
    /// </summary>
    /// <returns>A list representation of the assignment.</returns>
    public List<int> ToList()
    {
        return [.. _trail.OrderBy(Math.Abs)];
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{string.Join(", ", _trail.Reverse())}]";
    }
}
