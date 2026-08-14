using SatSolver.Shared;

namespace SatSolver.Core.Tests;

[Trait("Category", "Integration")]
public class SolverIntegrationTest
{
    [Theory]
    [DimacsFileData("testdata/kissat/sat")]
    public void Solve_ShouldReturnSatisfiableResult_WhenFormulaIsSatisfiable(Formula formula)
    {
        SolveResult result = new Solver(formula).Solve();

        Assert.Equal(SolveResultType.SATISFIABLE, result.Type);
        Assert.Equal(formula.NumberOfVars, result.Assignment.Count);

        // Assert that the formula is really satisfied, i.e. the truth assignment τ contains
        // at least one literal l per clause such that τ(l) = TRUE.
        foreach (List<int> clause in formula.Clauses)
        {
            Assert.Contains(clause, result.Assignment.Contains);
        }
    }

    [Theory]
    [DimacsFileData("testdata/kissat/unsat")]
    public void Solve_ShouldReturnUnsatisfiableResult_WhenFormulaIsNotSatisfiable(Formula formula)
    {
        SolveResult result = new Solver(formula).Solve();

        Assert.Equal(SolveResultType.UNSATISFIABLE, result.Type);
    }

    [Fact]
    public void Solve_ShouldReturnUnknownResult_WhenTimeoutHappens()
    {
        Formula formula = new("test", 5, [
            [1, 2, 3],
            [2, 3, 4],
            [3, 4, 5],
            [-1, -2, -3],
            [-2, -3, -4],
            [-3, -4, -5],
        ]);

        SolveResult result = new Solver(formula).Solve(new(0, 0, 0));

        Assert.Equal(SolveResultType.UNKNOWN, result.Type);
    }
}
