using SatSolverCore.Clause;

namespace SatSolverCore.Tests.Clause;

public class ClauseFactoryTest
{
    [Theory]
    [InlineData(2)]
    [InlineData(3, -2)]
    [InlineData(4, -2, -3)]
    [InlineData(2, -2, -3, -4)]
    [InlineData(2, 2)]
    [InlineData(3, -2, 3)]
    public void WatchedLiteralAssignmentTest(int expected, params int[] decisions)
    {
        PartialAssignment assignment = new(5);
        for (int i = 0; i < decisions.Length; ++i)
        {
            assignment.AddDecision(decisions[i], i + 1);
        }

        IClause clause = ClauseFactory.Create([1, 2, 3, 4], assignment);

        Assert.Equal(1, clause.Watched1);
        Assert.Equal(expected, clause.Watched2);
    }
}
