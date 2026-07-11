namespace SatSolverCore.Tests;

public class SolverTest
{
    [Theory]
    [DimacsFileData("testdata/sample.cnf")]
    [DimacsFileData("testdata/sample2.cnf")]
    public void TestSolverSat(Formula formula)
    {
        var result = Solver.Solve(formula);

        Assert.True(result.IsSat);
    }

    [Theory]
    [DimacsFileData("testdata/nosolution.cnf")]
    public void TestSolverUnsat(Formula formula)
    {
        var result = Solver.Solve(formula);

        Assert.False(result.IsSat);
    }
}
