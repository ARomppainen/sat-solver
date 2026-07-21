namespace SatSolverCore.Clause;

/// <summary>
/// A result of "falsify" operation on a clause with watched literals.
/// </summary>
/// <remarks>
/// The following invariants hold: <br/>
///   1) If IsConflict is true, then PropagatedLiteral = 0 and NewWatchedLiteral = 0 <br/>
///   2) If PropagatedLiteral != 0, then NewWatchedLiteral = 0 <br/>
///   3) If NewWatchedLiteral != 0, then PropagatedLiteral = 0 <br/>
/// </remarks>
/// <param name="IsConflict">true if falsifying the literal would make the clause unsatisfied; otherwise false</param>
/// <param name="PropagatedLiteral">literal that is ready to be propagated (if value is != 0)</param>
/// <param name="NewWatchedLiteral">the watched literal has changed and need to be updated (if value is != 0)</param>
public record struct FalsifyResult(bool IsConflict, int PropagatedLiteral, int NewWatchedLiteral)
{

    /// <summary>
    /// Falsifying the literal lead to conflict.
    /// </summary>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public static FalsifyResult Conflict()
    {
        return new FalsifyResult(true, 0, 0);
    }

    /// <summary>
    /// Falsifying the literal lead to no changes.
    /// </summary>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public static FalsifyResult NoChanges()
    {
        return new FalsifyResult(false, 0, 0);
    }

    /// <summary>
    /// Falsifying the literal made the clause a unit clause.
    /// </summary>
    /// <param name="literal">literal to be propagated</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public static FalsifyResult Propagate(int literal)
    {
        return new FalsifyResult(false, literal, 0);
    }

    /// <summary>
    /// The watched literal changed after the previous one was falsified.
    /// </summary>
    /// <param name="literal">new watched literal</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public static FalsifyResult UpdateWatchlist(int literal)
    {
        return new FalsifyResult(false, 0, literal);
    }
}
