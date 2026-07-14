namespace SatSolverCore;

/// <summary>
/// Propositional logic formula in conjunctive normal form
/// </summary>
public record Formula(int NumberOfVars, List<Clause> Clauses, List<int> UnaryClauses);
