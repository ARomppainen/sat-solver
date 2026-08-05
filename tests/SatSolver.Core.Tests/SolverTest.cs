using SatSolver.Shared;

namespace SatSolver.Core.Tests;

public class SolverTest
{
    [Theory]
    [DimacsFileData("testdata/kissat/sat")]
    public void TestSolverKissatSat(Formula formula)
    {
        SolveResult result = new Solver(formula).Solve();

        Assert.Equal(SolveResult.ResultType.SATISFIABLE, result.Type);
        Assert.Equal(formula.NumberOfVars, result.Assignment.Count);
    }

    [Theory]
    [DimacsFileData("testdata/kissat/unsat")]
    public void TestSolverKissatUnsat(Formula formula)
    {
        SolveResult result = new Solver(formula).Solve();

        Assert.Equal(SolveResult.ResultType.UNSATISFIABLE, result.Type);
    }
}
