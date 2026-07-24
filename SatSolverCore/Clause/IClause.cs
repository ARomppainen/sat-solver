namespace SatSolverCore.Clause;

/// <summary>
/// Represents a clause (a disjunction of literals) that follows the
/// two-watched-literal scheme.
/// </summary>
/// <remarks>
/// The following invariant holds: <br/>
///   1) A watched literal may be false only if the other watched
///      literal is true or all the unwatched literals are false.
/// </remarks>
public interface IClause
{
    /// <summary>
    /// The list of literals in the clause.
    /// </summary>
    public List<int> Literals { get; }

    /// <summary>
    /// 1st watched literal value (or zero if there is no watched literal)
    /// </summary>
    public int Watched1 { get; }

    /// <summary>
    /// 2nd watched literal value (or zero if there is no watched literal)
    /// </summary>
    public int Watched2 { get; }

    /// <summary>
    /// Falsify the first watched literal
    /// </summary>
    /// <param name="assignment">current truth assignment</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public FalsifyResult FalsifyFirst(IPartialAssignment assignment);

    /// <summary>
    /// Falsify the second watched literal
    /// </summary>
    /// <param name="assignment">current truth assignment</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public FalsifyResult FalsifySecond(IPartialAssignment assignment);
}
