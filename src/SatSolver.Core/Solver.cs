using System.Diagnostics;

using SatSolver.Shared;

namespace SatSolver.Core;

/// <summary>
/// Solver class keeps track of all state related to the CDCL algorithm.
/// </summary>
public class Solver
{
    private readonly int _numberOfVars;
    private readonly VsidsHeuristic _decisionMaker;
    private readonly PartialAssignment _assignment;
    private readonly Queue<(int, List<int>?)> _propagateQueue;
    private readonly WatchedLiterals _watched;
    private readonly SolverStatistics _stats;
    private bool _containsEmptyClause;
    private int _decisionLevel;

    /// <summary>
    /// Initializes a new instance of the Solver class with a specified
    /// propositional logic formula.
    /// </summary>
    /// <param name="formula">A propositional logic formula.</param>
    /// <param name="decayThreshold">The frequency of how often the heuristic scores are adjusted.</param>
    /// <param name="decayFactor">The factor used for adjusting the heuristic scores.</param>
    public Solver(Formula formula, double decayFactor = 0.9, int decayThreshold = 16)
    {
        _numberOfVars = formula.NumberOfVars;
        _decisionMaker = new(formula, decayThreshold, decayFactor);
        _assignment = new(formula.NumberOfVars, _decisionMaker);
        _propagateQueue = [];
        _watched = new(formula.NumberOfVars);
        _stats = new();
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
            return SolveResult.Unsatisfiable(_stats);
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        long end = (long)(timeout ?? new TimeSpan(1, 0, 0, 0, 0)).TotalMilliseconds;

        while (true)
        {
            List<int>? conflict = Propagate();

            if (conflict != null)
            {
                if (_decisionLevel == 0)
                {
                    stopwatch.Stop();
                    _stats.Milliseconds = stopwatch.ElapsedMilliseconds;
                    return SolveResult.Unsatisfiable(_stats);
                }

#if USE_SIMPLE_CLAUSE_LEARNING
                (List<int> clause, int level) = _assignment.AnalyzeConflictSimple();
#else
                (List<int> clause, int level) = _assignment.AnalyzeConflict(conflict, _decisionLevel);
#endif

                _assignment.Backjump(level);
                _decisionLevel = level;
                LearnClause(clause);
            }
            else
            {
                if (_assignment.Count == _numberOfVars)
                {
                    stopwatch.Stop();
                    _stats.Milliseconds = stopwatch.ElapsedMilliseconds;
                    return SolveResult.Satisfiable(_assignment.ToList(), _stats);
                }

                Decide();
            }

            if (stopwatch.ElapsedMilliseconds >= end)
            {
                stopwatch.Stop();
                _stats.Milliseconds = stopwatch.ElapsedMilliseconds;
                return SolveResult.Unknown(_stats);
            }
        }
    }

    /// <summary>
    /// Decide a truth value for the next unassigned literal.
    /// </summary>
    private void Decide()
    {
        _stats.Decisions++;
        _decisionLevel++;
        int literal = _decisionMaker.Choose(_assignment);
        _propagateQueue.Enqueue((literal, null));
    }

    /// <summary>
    /// Check for new unit clauses.
    /// </summary>
    /// <returns>The conflict clause if propagation lead to a conflict; otherwise, null.</returns>
    private List<int>? Propagate()
    {
        while (_propagateQueue.Count > 0)
        {
            (int literal, List<int>? reason) = _propagateQueue.Dequeue();

            if (_assignment.IsAssigned(literal))
            {
                continue;
            }

            if (_assignment.IsAssigned(-literal))
            {
                _propagateQueue.Clear();
                return reason;
            }

            _stats.Propagations++;
            _assignment.Add(literal, _decisionLevel, reason);

            List<int>? conflict = _watched.FindUnitLiterals(-literal, _assignment, _propagateQueue);

            if (conflict != null)
            {
                _stats.Conflicts++;
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

        if (literals.Count == 1)
        {
            _propagateQueue.Enqueue((literals[0], literals));
            return;
        }

        IClause clause = ClauseFactory.Create(literals, _assignment);
        _watched.Add(clause);
    }

    private void LearnClause(List<int> literals)
    {
        _decisionMaker.Update(literals);
        _propagateQueue.Enqueue((literals[0], literals));

        if (literals.Count >= 2)
        {
            IClause clause = ClauseFactory.Create(literals, _assignment);
            _watched.Add(clause);
        }
    }
}
