namespace SatSolverCore;

/// <summary>
/// Propositional logic formula in conjunctive normal form
/// </summary>
public record Formula(string Name, int NumberOfVars, List<Clause> Clauses, List<int> UnaryClauses, bool HasEmptyClause)
{
    public override string ToString()
    {
        return Name;
    }
}
