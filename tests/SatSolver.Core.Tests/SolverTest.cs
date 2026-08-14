using SatSolver.Shared;

namespace SatSolver.Core.Tests;

[Trait("Category", "Unit")]
public class SolverTest
{
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
