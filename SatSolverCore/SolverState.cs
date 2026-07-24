using SatSolverCore.Clause;
using SatSolverCore.Decision;

namespace SatSolverCore;

/// <summary>
/// SolverState keeps track of all the state related to the CDCL algorithm.
/// </summary>
public class SolverState
{
    private readonly int _numberOfVars;
    private readonly PartialAssignment _assignment;
    private readonly Queue<(int, IClause?)> _propagateQueue;
    private readonly WatchedLiterals _watched;
    private readonly IDecisionMaker _decisionMaker;

    /// <summary>
    /// The current decision level (a non-negative integer).
    /// </summary>
    public int DecisionLevel { get; private set; }

    /// <summary>
    /// true if the current truth assignment contains values for all the
    /// variables in the formula; otherwise, false.
    /// </summary>
    public bool AllVariablesHaveValues => _assignment.Count == _numberOfVars;

    /// <summary>
    /// Initializes a new instance of the SolverState class with a specified
    /// propositional logic formula and given IDecisionMaker instance.
    /// </summary>
    /// <param name="formula">A propositional logic formula.</param>
    /// <param name="decisionMaker">A decision maker instance.</param>
    public SolverState(Formula formula, IDecisionMaker decisionMaker)
    {
        _numberOfVars = formula.NumberOfVars;
        _assignment = new(formula.NumberOfVars);
        _propagateQueue = [];
        _watched = new();
        _decisionMaker = decisionMaker;

        DecisionLevel = 0;

        formula.Clauses.ForEach(AddClause);
    }

    /// <summary>
    /// Decide a truth value for the next unassigned literal.
    /// </summary>
    public void MakeDecision()
    {
        ++DecisionLevel;
        int literal = _decisionMaker.ChooseUnassignedLiteral(_assignment);
        _propagateQueue.Enqueue((literal, null));
    }

    /// <summary>
    /// Check for new unit clauses based on the last decided literal.
    /// </summary>
    /// <returns>The conflict clause if propagation lead to a conflict; otherwise, null.</returns>
    public IClause? UnitPropagate()
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

            _assignment.Add(literal, DecisionLevel, reason);

            IClause? conflict = _watched.TryFindUnitLiterals(-literal, _assignment, _propagateQueue);

            if (conflict != null)
            {
                _propagateQueue.Clear();
                return conflict;
            }
        }

        return null;
    }

    /// <summary>
    /// Perform a backjump to given decision level.
    /// </summary>
    /// <param name="level">The decision level to backjump into (a non-negative integer).</param>
    public void Backjump(int level)
    {
        _assignment.Backjump(level);
        DecisionLevel = level;
    }

    /// <summary>
    /// Returns a list representation of the current truth assignment.
    /// </summary>
    /// <returns>A list represenation of the current truth assignment.</returns>
    public List<int> Assignment()
    {
        return _assignment.ToList();
    }

    private void AddClause(List<int> literals)
    {
        IClause clause = ClauseFactory.Create(literals, _assignment);
        _watched.Add(clause);

        if (literals.Count == 1)
        {
            _propagateQueue.Enqueue((literals[0], clause));
        }
    }

    /// <summary>
    /// Add new learned clause.
    /// </summary>
    /// <param name="literals">The list of literals in the clause.</param>
    public void LearnClause(List<int> literals)
    {
        IClause clause = ClauseFactory.Create(literals, _assignment);
        _watched.Add(clause);
        _propagateQueue.Enqueue((literals[0], clause));
    }

    /// <summary>
    /// Analyzes the current partial assignment and returns a conflict clause
    /// that can be learned by the solver.
    /// </summary>
    /// <returns>
    /// a tuple where the elements are <br/>
    ///   1) the conflicting clause <br/>
    ///   2) the decision level to backjump into
    /// </returns>
    public (List<int>, int) AnalyzeConflict(IClause conflict)
    {
        return _assignment.AnalyzeConflict(conflict, DecisionLevel);
    }
}
