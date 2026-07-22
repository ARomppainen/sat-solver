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
    private readonly List<int> _unaryClauses;
    private readonly Queue<int> _unitLiterals;
    private readonly WatchedLiterals _watched;
    private readonly IDecisionMaker _decisionMaker;

    /// <summary>
    /// The current decision level (a non-negative integer).
    /// </summary>
    public int DecisionLevel { get; private set; }

    /// <summary>
    /// true if the formula contains an empty clause; otherwise, false.
    /// </summary>
    public bool HasEmptyClause { get; private set; }

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
        _unaryClauses = [];
        _unitLiterals = [];
        _watched = new();
        _decisionMaker = decisionMaker;

        HasEmptyClause = false;
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
        _assignment.AddDecision(literal, DecisionLevel);
    }

    /// <summary>
    /// Check for new unit clauses based on the last decided literal.
    /// </summary>
    /// <returns>true if the last decision lead to a conflict; otherwise, false.</returns>
    public bool UnitPropagate()
    {
        _unitLiterals.Clear();

        int lastDecision = _assignment.GetLastDecision();
        if (lastDecision != 0)
        {
            if (!_watched.TryFindUnitLiterals(-lastDecision, _assignment, _unitLiterals))
            {
                return true;
            }
        }

        if (DecisionLevel == 0)
        {
            _unaryClauses.ForEach(_unitLiterals.Enqueue);
        }

        while (_unitLiterals.Count > 0)
        {
            int literal = _unitLiterals.Dequeue();

            if (_assignment.IsAssigned(literal))
            {
                continue;
            }

            if (!_watched.TryFindUnitLiterals(-literal, _assignment, _unitLiterals))
            {
                return true;
            }

            _assignment.AddPropagated(literal, DecisionLevel);
        }

        return false;
    }

    /// <summary>
    /// Perform a backjump to given decision level.
    /// </summary>
    /// <param name="level">The decision level to backjump into (a non-negative integer).</param>
    public void Backjump(int level)
    {
        _assignment.Backjump(level);
        _decisionMaker.Backjump(_assignment.GetLastDecision());
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

    /// <summary>
    /// Add new (possibly learned) clause.
    /// </summary>
    /// <param name="literals">The list of literals in the clause.</param>
    public void AddClause(List<int> literals)
    {
        IClause clause = ClauseFactory.Create(literals);

        if (literals.Count == 0)
        {
            HasEmptyClause = true;
        }
        else if (literals.Count == 1)
        {
            _unaryClauses.Add(literals[0]);
        }

        _watched.Add(clause);
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
    public (List<int>, int) AnalyzeConflict()
    {
        return _assignment.GetConflictClause();
    }
}
