using NSubstitute;

using SatSolverCore.Clause;

namespace SatSolverCore.Tests.Clause;

public class ClaseuUnaryTest
{
    [Fact]
    public void PublicGetters_ShouldReturnCorrectValues()
    {
        ClauseUnary clause = new(3);

        Assert.Single(clause.Literals);
        Assert.Equal(3, clause.Literals[0]);

        Assert.Equal(3, clause.Watched1);
        Assert.Equal(0, clause.Watched2);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnConflict()
    {
        ClauseUnary clause = new(3);
        var assignment = Substitute.For<IPartialAssignment>();

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldThrow()
    {
        ClauseUnary clause = new(3);
        var assignment = Substitute.For<IPartialAssignment>();

        Assert.Throws<NotSupportedException>(() => clause.FalsifySecond(assignment));
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        Assert.Equal("[1]", new ClauseUnary(1).ToString());
        Assert.Equal("[-2]", new ClauseUnary(-2).ToString());
    }
}
