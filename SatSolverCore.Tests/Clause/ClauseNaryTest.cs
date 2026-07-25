using SatSolverCore.Clause;

namespace SatSolverCore.Tests.Clause;

public class ClauseNaryTest
{
    [Fact]
    public void PublicGetters_ShouldReturnCorrectValues()
    {
        ClauseNary clause = new([-3, 5, -7], 1, 2);

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
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(2, 1, null);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnUnitLiteral()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-1, 1, null);
        assignment.Add(-3, 1, null);
        assignment.Add(-4, 1, null);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(2, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnConflict()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-1, 1, null);
        assignment.Add(-2, 1, null);
        assignment.Add(-3, 1, null);
        assignment.Add(-4, 1, null);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifyFirst_ShouldReturnWatchlistUpdate()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-1, 1, null);

        FalsifyResult result = clause.FalsifyFirst(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(3, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnNoChanges()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(1, 1, null);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnUnitLiteral()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-2, 1, null);
        assignment.Add(-3, 1, null);
        assignment.Add(-4, 1, null);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(1, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnConflict()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-1, 1, null);
        assignment.Add(-2, 1, null);
        assignment.Add(-3, 1, null);
        assignment.Add(-4, 1, null);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.True(result.IsConflict);
        Assert.Equal(0, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void FalsifySecond_ShouldReturnWatchlistUpdate()
    {
        ClauseNary clause = new([1, 2, 3, 4], 0, 1);

        PartialAssignment assignment = new(5);
        assignment.Add(-2, 1, null);

        FalsifyResult result = clause.FalsifySecond(assignment);

        Assert.False(result.IsConflict);
        Assert.Equal(3, result.NewWatchedLiteral);
        Assert.Equal(0, result.UnitLiteral);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        ClauseNary clause = new([1, 2, -3, 4, -5], 0, 1);

        Assert.Equal("[1, 2, -3, 4, -5]", clause.ToString());
    }
}
