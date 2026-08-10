using NSubstitute;

namespace SatSolver.Core.Tests;

[Trait("Category", "Unit")]
public class ClauseTest
{
    [Fact]
    public void PublicGetters_ShouldReturnCorrectValues()
    {
        Clause clause = new([-3, 5, -7], 1, 2);

        Assert.Equal(3, clause.Literals.Count);
        Assert.Equal(-3, clause.Literals[0]);
        Assert.Equal(5, clause.Literals[1]);
        Assert.Equal(-7, clause.Literals[2]);

        Assert.Equal(5, clause.Watched1);
        Assert.Equal(-7, clause.Watched2);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnNoChanges()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

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
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);
        assignment.IsAssigned(-3).Returns(true);
        assignment.IsAssigned(-4).Returns(true);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(2, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnConflict()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);
        assignment.IsAssigned(-2).Returns(true);
        assignment.IsAssigned(-3).Returns(true);
        assignment.IsAssigned(-4).Returns(true);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnWatchlistUpdate()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(3, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnNoChanges()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

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
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-2).Returns(true);
        assignment.IsAssigned(-3).Returns(true);
        assignment.IsAssigned(-4).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(1, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnConflict()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-1).Returns(true);
        assignment.IsAssigned(-2).Returns(true);
        assignment.IsAssigned(-3).Returns(true);
        assignment.IsAssigned(-4).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnWatchlistUpdate()
    {
        Clause clause = new([1, 2, 3, 4], 0, 1);

        var assignment = Substitute.For<IPartialAssignment>();
        assignment.IsAssigned(-2).Returns(true);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(3, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        Clause clause = new([1, 2, -3, 4, -5], 0, 1);

        Assert.Equal("[1, 2, -3, 4, -5]", clause.ToString());
    }
}
