namespace SatSolverCore.Perf;

public record FormulaResult(string Name, int NumberOfVars, int NumberOfClauses, List<Sample> Samples);
