using System.Diagnostics;

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
    /// Add new literal to the trail with given decision level.
    /// </summary>
    /// <param name="literal">literal value (a non-zero integer)</param>
    /// <param name="level">decision level (non-negative integer)</param>
    /// <param name="reason">the reason for the literal (null for decisions)</param>
    public void Add(int literal, int level, IClause? reason)
    {
        _trail.Push(literal);
        _level[Math.Abs(literal)] = level;
        _reason[Math.Abs(literal)] = reason;
        _assignment.Add(literal);
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
    /// Analyze the conflict to produce a new learned clause.
    /// As part of the analysis, part of the decision trail is also undone.
    /// </summary>
    /// <remarks>
    /// The learned clause is based on the first unique implication point (UIP) cut.
    /// This method uses the algorithm described in the MiniSat paper,
    /// see <see href="http://minisat.se/downloads/MiniSat.pdf"/> page 15, figure 10.
    /// </remarks>
    /// <param name="conflict">The conflict clause.</param>
    /// <param name="decisionLevel">Current decision level.</param>
    /// <returns>
    /// a tuple where the elements are <br/>
    ///   1) the learned clause <br/>
    ///   2) the decision level to backjump into
    /// </returns>
    public (List<int>, int) AnalyzeConflict(IClause conflict, int decisionLevel)
    {
        // Note that the actual value at index 0 will be set at the end of the method.
        List<int> learned = [0];

        int backjumpLevel = 0;
        int counter = 0;
        int p = 0;
        int pVar;
        IClause? pReason = conflict;
        HashSet<int> seenVariables = [];

        // Traverse the decision trail in reverse using breadth-first search.
        // Start with all the literals in the conflict clause.
        // Uses the list of 'reason' clauses (there is no reason to maintain
        // a separate implication graph).
        do
        {
            Debug.Assert(pReason != null);

            foreach (int q in pReason.Literals)
            {
                if (q == p)
                {
                    // Ignore the literal that was propagated by 'pReason'.
                    // Note that during first iteration, p = 0
                    // => all the literals in the conflict clause are checked
                    continue;
                }

                int qVar = Math.Abs(q);

                if (!seenVariables.Add(qVar))
                {
                    continue;
                }

                int qLevel = _level[qVar];

                if (qLevel == decisionLevel)
                {
                    ++counter;
                }
                else if (qLevel > 0)
                {
                    // Exclude variables from decision level 0.
                    // If included the clauses are still valid, but unnecessarily long.
                    learned.Add(q);
                    backjumpLevel = Math.Max(backjumpLevel, qLevel);
                }
            }

            // Select the next literal from the trail that is in the list of seen variables.
            do
            {
                p = _trail.Pop();
                pVar = Math.Abs(p);
                pReason = _reason[pVar];
                _assignment.Remove(p);
                _level[pVar] = 0;
                _reason[pVar] = null;
            } while (!seenVariables.Contains(pVar));

            --counter;
        } while (counter > 0);

        // After the loop, p will be the first unique implication point.
        learned[0] = -p;

        return (learned, backjumpLevel);
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
