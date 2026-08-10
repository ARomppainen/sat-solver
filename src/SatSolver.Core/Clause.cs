namespace SatSolver.Core;

/// <summary>
/// Represents a clause with N literals (N > 2).
/// </summary>
public class Clause(List<int> literals)
{
    /// <summary>
    /// The list of literals in the clause.
    /// </summary>
    public List<int> Literals { get; } = literals;

    /// <summary>
    /// Falsify a watched literal
    /// </summary>
    /// <param name="literal">the falsified literal</param>
    /// <param name="assignment">current truth assignment</param>
    /// <returns><see cref="FalsifyResult"/> instance</returns>
    public FalsifyResult Falsify(int literal, IPartialAssignment assignment)
    {
        if (Literals[0] == literal)
        {
            Literals[0] = Literals[1];
            Literals[1] = literal;
        }

        if (assignment.IsAssigned(Literals[0]))
        {
            return FalsifyResult.NoChanges();
        }

        for (int i = 2; i < Literals.Count; ++i)
        {
            if (!assignment.IsAssigned(-Literals[i]))
            {
                Literals[1] = Literals[i];
                Literals[i] = literal;
                return FalsifyResult.UpdateWatchlist(Literals[1]);
            }
        }

        if (assignment.IsAssigned(-Literals[0]))
        {
            return FalsifyResult.Conflict();
        }

        return FalsifyResult.Propagate(Literals[0]);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{string.Join(", ", Literals)}]";
    }
}
