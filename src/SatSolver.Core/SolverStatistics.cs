namespace SatSolver.Core;

/// <summary>
/// Captured statistics of <see cref="Solver" execution./>
/// </summary>
public class SolverStatistics
{
    /// <summary>The number of conflicts.</summary>
    public int Conflicts { get; set; } = 0;

    /// <summary>The number of decisions.</summary>
    public int Decisions { get; set; } = 0;

    /// <summary>The number of propagated literals.</summary>
    public int Propagations { get; set; } = 0;

    /// <summary>Solver processing time in milliseconds.</summary>
    public long Milliseconds { get; set; } = 0;
}
