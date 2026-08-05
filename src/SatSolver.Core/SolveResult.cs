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

    private SolveResult(ResultType type, List<int> assignment)
    {
        Type = type;
        Assignment = assignment;
    }

    /// <summary>
    /// Create satisfiable result instance.
    /// </summary>
    /// <param name="assignment">the satisfying assignment</param>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Satisfiable(List<int> assignment)
    {
        return new SolveResult(ResultType.SATISFIABLE, assignment);
    }

    /// <summary>
    /// Create unsatisfiable result instance.
    /// </summary>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Unsatisfiable()
    {
        return new SolveResult(ResultType.UNSATISFIABLE, []);
    }

    public static SolveResult Unknown()
    {
        return new SolveResult(ResultType.UNKNOWN, []);
    }

    public enum ResultType
    {
        SATISFIABLE,
        UNSATISFIABLE,
        UNKNOWN
    }
}
