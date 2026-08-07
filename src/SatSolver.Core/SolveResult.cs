namespace SatSolver.Core;

/// <summary>
/// Represents possible return values of <see cref="Solver.Solve(Formula)"/>.
/// </summary>
public class SolveResult
{
    /// <summary>
    /// The satisfying assignment if the formula was satisfiable.
    /// </summary>
    public List<int> Assignment { get; }

    public ResultType Type { get; }

    public SolverStatistics Statistics { get; }

    private SolveResult(ResultType type, List<int> assignment, SolverStatistics statistics)
    {
        Type = type;
        Assignment = assignment;
        Statistics = statistics;
    }

    /// <summary>
    /// Create satisfiable result instance.
    /// </summary>
    /// <param name="assignment">the satisfying assignment</param>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Satisfiable(List<int> assignment, SolverStatistics statistics)
    {
        return new SolveResult(ResultType.SATISFIABLE, assignment, statistics);
    }

    /// <summary>
    /// Create unsatisfiable result instance.
    /// </summary>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Unsatisfiable(SolverStatistics statistics)
    {
        return new SolveResult(ResultType.UNSATISFIABLE, [], statistics);
    }

    public static SolveResult Unknown(SolverStatistics statistics)
    {
        return new SolveResult(ResultType.UNKNOWN, [], statistics);
    }

    public enum ResultType
    {
        SATISFIABLE,
        UNSATISFIABLE,
        UNKNOWN
    }
}
