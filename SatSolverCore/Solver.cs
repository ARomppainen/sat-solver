using SatSolverCore.Clause;

namespace SatSolverCore;

/// <summary>
/// Solver class keeps track of all state related to the CDCL algorithm.
/// </summary>
public class Solver
{
    private readonly int _numberOfVars;
    private readonly Vsids _vsids;
    private readonly PartialAssignment _assignment;
    private readonly Queue<(int, IClause?)> _propagateQueue;
    private readonly WatchedLiterals _watched;
    private bool _containsEmptyClause;
    private int _decisionLevel;

    /// <summary>
    /// Initializes a new instance of the Solver class with a specified
    /// propositional logic formula.
    /// </summary>
    /// <param name="formula">A propositional logic formula.</param>
    public Solver(Formula formula)
    {
        _numberOfVars = formula.NumberOfVars;
        _vsids = new(formula);
        _assignment = new(formula.NumberOfVars, _vsids);
        _propagateQueue = [];
        _watched = new(formula.NumberOfVars);
        _containsEmptyClause = false;
        _decisionLevel = 0;

        formula.Clauses.ForEach(AddClause);
    }

    /// <summary>
    /// The main entry point for interacting with the SAT solver. Tries to find
    /// a satistying truth assignment for a given propositional logic formula
    /// using conflict-driven clause learning (CDCL) algorithm.
    /// </summary>
    /// <param name="timeout">An amount of time after which the execution of the solver is aborted (optional).</param>
    /// <returns>'satisfiable' result with a truth assignment or 'unsatisfiable' result</returns>
    public SolveResult Solve(TimeSpan? timeout = null)
    {
        if (_containsEmptyClause)
        {
            // Formula with an empty clause is unsatisfiable
            return SolveResult.Unsatisfiable();
        }

        DateTime start = DateTime.UtcNow;
        DateTime end = timeout.HasValue ? start + timeout.Value : start.AddYears(1);

        while (true)
        {
            IClause? conflict = Propagate();

            if (conflict != null)
            {
                if (_decisionLevel == 0)
                {
                    return SolveResult.Unsatisfiable();
                }

                (List<int> clause, int level) = _assignment.AnalyzeConflict(conflict, _decisionLevel);

                _assignment.Backjump(level);
                _decisionLevel = level;
                LearnClause(clause);
            }
            else
            {
                if (_assignment.Count == _numberOfVars)
                {
                    return SolveResult.Satisfiable(_assignment.ToList());
                }

                Decide();
            }

            if (DateTime.UtcNow > end)
            {
                return SolveResult.Unknown();
            }
        }
    }

    /// <summary>
    /// Decide a truth value for the next unassigned literal.
    /// </summary>
    private void Decide()
    {
        _decisionLevel++;
        int literal = _vsids.Choose(_assignment);
        _propagateQueue.Enqueue((literal, null));
    }

    /// <summary>
    /// Check for new unit clauses.
    /// </summary>
    /// <returns>The conflict clause if propagation lead to a conflict; otherwise, null.</returns>
    private IClause? Propagate()
    {
        while (_propagateQueue.Count > 0)
        {
            (int literal, IClause? reason) = _propagateQueue.Dequeue();

            if (_assignment.IsAssigned(literal))
            {
                continue;
            }

            if (_assignment.IsAssigned(-literal))
            {
                _propagateQueue.Clear();
                return reason;
            }

            _assignment.Add(literal, _decisionLevel, reason);

            IClause? conflict = _watched.FindUnitLiterals(-literal, _assignment, _propagateQueue);

            if (conflict != null)
            {
                _propagateQueue.Clear();
                return conflict;
            }
        }

        return null;
    }

    private void AddClause(List<int> literals)
    {
        if (literals.Count == 0)
        {
            _containsEmptyClause = true;
            return;
        }

        IClause clause = ClauseFactory.Create(literals, _assignment);
        _watched.Add(clause);

        if (literals.Count == 1)
        {
            _propagateQueue.Enqueue((literals[0], clause));
        }
    }

    private void LearnClause(List<int> literals)
    {
        IClause clause = ClauseFactory.Create(literals, _assignment);
        _watched.Add(clause);
        _propagateQueue.Enqueue((literals[0], clause));
        _vsids.Update(literals);
    }
}
