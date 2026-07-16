namespace SatSolverCore.Tests;

public class SolverTest
{
    [Theory]
    [DimacsFileData("testdata/kissat/sat")]
    public void TestSolverKissatSat(Formula formula)
    {
        var result = Solver.Solve(formula);

        Assert.True(result.IsSat);
        Assert.Equal(formula.NumberOfVars, result.Assignment.Count);
    }

    [Theory]
    [DimacsFileData("testdata/kissat/unsat")]
    public void TestSolverKissatUnsat(Formula formula)
    {
        var result = Solver.Solve(formula);

        Assert.False(result.IsSat);
    }
}
