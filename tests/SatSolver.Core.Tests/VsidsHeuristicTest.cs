using NSubstitute;

using SatSolver.Shared;

namespace SatSolver.Core.Tests;

[Trait("Category", "Unit")]
public class VsidsHeuristicTest
{
    [Fact]
    public void Choose_ShouldReturnVariableWithHighestScore()
    {
        List<List<int>> clauses = [
            [1, 2],
            [-2, 3],
            [3, 4],
            [2, -5]
        ];
        Formula formula = new("test", 5, clauses);
        VsidsHeuristic vsids = new(formula, 1, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsUnassigned(Arg.Any<int>()).Returns(true);

        int result = vsids.Choose(assignment);
        Assert.Equal(2, result);
    }

    [Fact]
    public void Choose_ShouldUseVariableNumberAsTiebreak()
    {
        List<List<int>> clauses = [
            [1, 2],
            [-2, 3],
            [2, 4],
            [-3, 4],
            [3, -5]
        ];
        Formula formula = new("test", 5, clauses);
        VsidsHeuristic vsids = new(formula, 1, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsUnassigned(Arg.Any<int>()).Returns(true);

        int result = vsids.Choose(assignment);
        Assert.Equal(2, result);
    }

    [Fact]
    public void Update_ShouldIncrementActivityScoreOfVariables()
    {
        List<List<int>> clauses = [
            [1, 2],
            [-2, 3],
            [2, 4],
            [-3, 4],
            [3, -5]
        ];
        Formula formula = new("test", 5, clauses);
        VsidsHeuristic vsids = new(formula, 1, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsUnassigned(Arg.Any<int>()).Returns(true);

        vsids.Update([-3, 1]);

        int result = vsids.Choose(assignment);
        Assert.Equal(3, result);
    }
}
