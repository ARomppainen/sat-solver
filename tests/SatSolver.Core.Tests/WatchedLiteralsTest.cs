using System.Diagnostics;

using NSubstitute;

namespace SatSolver.Core.Tests;

[Trait("Category", "Unit")]
public class WatchedLiteralsTest
{
    [Theory]
    [InlineData(typeof(WatchedLiteralsV1))]
    [InlineData(typeof(WatchedLiteralsV2))]
    public void FindUnitLiterals_ShouldEnqueueFoundUnitLiterals(Type type)
    {
        var watched = Activator.CreateInstance(type, 7);

        var addMethod = type.GetMethod("Add");
        var findUnitLiteralsMethod = type.GetMethod("FindUnitLiterals");

        Debug.Assert(addMethod != null);
        Debug.Assert(findUnitLiteralsMethod != null);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(Arg.Any<int>()).Returns(false);
        assignment.IsAssigned(4).Returns(true);

        List<List<int>> clauses = [
            [-1, 2],
            [-1, 3],
            [-4, 5],
            [-2, -3, 4],
            [-4, 6],
            [-5, -6, 7]
        ];
        clauses.ForEach(clause => addMethod.Invoke(watched, [clause]));

        Queue<(int, List<int>?)> queue = new();

        var conflict = findUnitLiteralsMethod.Invoke(watched, [-4, assignment, queue]);
        Assert.Null(conflict);

        Assert.Equal(2, queue.Count);
        var first = queue.Dequeue();
        var second = queue.Dequeue();

        Assert.Equal(5, first.Item1);
        Assert.Equal(6, second.Item1);
    }

    [Theory]
    [InlineData(typeof(WatchedLiteralsV1))]
    [InlineData(typeof(WatchedLiteralsV2))]
    public void FindUnitLiterals_ShouldReturnConflictClause(Type type)
    {
        var watched = Activator.CreateInstance(type, 2);

        var addMethod = type.GetMethod("Add");
        var findUnitLiteralsMethod = type.GetMethod("FindUnitLiterals");

        Debug.Assert(addMethod != null);
        Debug.Assert(findUnitLiteralsMethod != null);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(1).Returns(true);
        assignment.IsAssigned(2).Returns(true);
        assignment.IsAssigned(-1).Returns(false);
        assignment.IsAssigned(-2).Returns(false);

        List<List<int>> clauses = [
            [1, 2],
            [1, -2],
            [-1, 2],
            [-1, -2],
        ];
        clauses.ForEach(clause => addMethod.Invoke(watched, [clause]));


        Queue<(int, List<int>?)> queue = new();

        var conflict = (List<int>?)findUnitLiteralsMethod.Invoke(watched, [-2, assignment, queue]);

        Assert.NotNull(conflict);
        Assert.Equal(2, conflict.Count);
        Assert.Contains(-1, conflict);
        Assert.Contains(-2, conflict);
    }
}
