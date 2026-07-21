namespace SatSolverCore;

/// <summary>
/// Represents a propositional logic formula in conjunctive normal form
/// </summary>
/// <param name="Name">'name' of the formula, e.g. the name of the DIMACS file (for debugging purposes)</param>
/// <param name="NumberOfVars">the number of variables in the formula</param>
/// <param name="Clauses">list of clauses</param>
public record Formula(string Name, int NumberOfVars, List<List<int>> Clauses)
{

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
