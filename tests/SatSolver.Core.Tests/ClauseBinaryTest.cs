using NSubstitute;

using SatSolver.Core;

namespace SatSolverCore.Tests;

public class ClauseBinaryTest
{
    [Fact]
    public void PublicGetters_ShouldReturnCorrectValues()
    {
        ClauseBinary clause = new(5, 7);

        Assert.Equal(2, clause.Literals.Count);
        Assert.Equal(5, clause.Literals[0]);
        Assert.Equal(7, clause.Literals[1]);

        Assert.Equal(5, clause.Watched1);
        Assert.Equal(7, clause.Watched2);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnNoChanges()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(2).Returns(true);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnUnitLiteral()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(2, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnConflict()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);
        assignment.IsAssigned(-2).Returns(true);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnNoChanges()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(1).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnUnitLiteral()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-2).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(1, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnConflict()
    {
        ClauseBinary clause = new(1, 2);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);
        assignment.IsAssigned(-2).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        Assert.Equal("[1, -2]", new ClauseBinary(1, -2).ToString());
        Assert.Equal("[-2, 1]", new ClauseBinary(-2, 1).ToString());
    }
}
