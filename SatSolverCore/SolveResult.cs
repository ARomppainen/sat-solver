namespace SatSolverCore;

/// <summary>
/// Represents possible return values of <see cref="Solver.Solve(Formula)"/>.
/// </summary>
public class SolveResult
{
    /// <summary>
    /// true if the formula was satisfiable; otherwise, false.
    /// </summary>
    public bool IsSatisfiable { get; }

    /// <summary>
    /// The satisfying assignment if the formula was satisfiable.
    /// </summary>
    public List<int> Assignment { get; }

    private SolveResult(bool type, List<int> assignment)
    {
        IsSatisfiable = type;
        Assignment = assignment;
    }

    /// <summary>
    /// Create satisfiable result instance.
    /// </summary>
    /// <param name="assignment">the satisfying assignment</param>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Satisfiable(List<int> assignment)
    {
        return new SolveResult(true, assignment);
    }

    /// <summary>
    /// Create unsatisfiable result instance.
    /// </summary>
    /// <returns>SolveResult instance</returns>
    public static SolveResult Unsatisfiable()
    {
        return new SolveResult(false, []);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (!IsSatisfiable)
        {
            return "unsat";
        }

        return string.Join(' ', Assignment);
    }
}
