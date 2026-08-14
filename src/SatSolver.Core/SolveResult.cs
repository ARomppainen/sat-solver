namespace SatSolver.Core;

/// <summary>
/// Return value of <see cref="Solver.Solve(TimeSpan?)"/>.
/// </summary>
public class SolveResult(SolveResultType type, List<int> assignment, SolverStatistics statistics)
{
    /// <summary>
    /// The satisfying assignment if the formula was satisfiable.
    /// </summary>
    public List<int> Assignment { get; } = assignment;

    public SolveResultType Type { get; } = type;

    public SolverStatistics Statistics { get; } = statistics;
}
