namespace SatSolverCore;

/// <summary>
/// Propositional logic formula in conjunctive normal form
/// </summary>
public record Formula(string Name, int NumberOfVars, List<List<int>> Clauses)
{
    public override string ToString()
    {
        return Name;
    }
}
