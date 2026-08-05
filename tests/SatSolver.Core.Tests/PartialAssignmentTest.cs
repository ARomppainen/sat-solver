using NSubstitute;

using SatSolver.Core;

namespace SatSolverCore.Tests;

public class PartialAssignmentTest
{
    [Fact]
    public void Count_ShouldReturnNumberOfElements()
    {
        PartialAssignment assignment = new(2, Substitute.For<IUndo>());
        Assert.Equal(0, assignment.Count);

        assignment.Add(1, 1);
        Assert.Equal(1, assignment.Count);

        assignment.Add(-2, 2);
        Assert.Equal(2, assignment.Count);
    }

    [Fact]
    public void Backjump_ShouldUndoPartOfTrail()
    {
        PartialAssignment assignment = new(10, Substitute.For<IUndo>());

        assignment.Add(10, 0, new ClauseUnary(10));
        assignment.Add(1, 1);
        assignment.Add(-2, 1, new ClauseBinary(-1, -2));
        assignment.Add(3, 2);
        assignment.Add(-4, 3);
        assignment.Add(5, 3, new ClauseBinary(4, 5));

        Assert.Equal(6, assignment.Count);

        assignment.Backjump(2);
        Assert.Equal(4, assignment.Count);

        assignment.Backjump(1);
        Assert.Equal(3, assignment.Count);

        assignment.Backjump(0);
        Assert.Equal(1, assignment.Count);
    }

    [Fact]
    public void Backjump_ShouldCallUndo()
    {
        IUndo undo = Substitute.For<IUndo>();
        PartialAssignment assignment = new(10, undo);

        assignment.Add(10, 0, new ClauseUnary(10));
        assignment.Add(1, 1);
        assignment.Add(-2, 1, new ClauseBinary(-1, -2));
        assignment.Add(3, 2);
        assignment.Add(-4, 3);
        assignment.Add(5, 3, new ClauseBinary(4, 5));

        assignment.Backjump(2);

        undo.Received().Undo(5);
        undo.Received().Undo(4);
    }

    [Fact]
    public void IsAssigned_ShouldReturnCorrectValue()
    {
        PartialAssignment assignment = new(4, Substitute.For<IUndo>());
        assignment.Add(2, 1);
        assignment.Add(-3, 2);

        Assert.False(assignment.IsAssigned(1));
        Assert.True(assignment.IsAssigned(2));
        Assert.False(assignment.IsAssigned(3));
        Assert.False(assignment.IsAssigned(4));

        Assert.False(assignment.IsAssigned(-1));
        Assert.False(assignment.IsAssigned(-2));
        Assert.True(assignment.IsAssigned(-3));
        Assert.False(assignment.IsAssigned(-4));
    }

    [Fact]
    public void IsUnassigned_ShouldReturnCorrectValue()
    {
        PartialAssignment assignment = new(4, Substitute.For<IUndo>());
        assignment.Add(2, 1);
        assignment.Add(-3, 2);

        Assert.True(assignment.IsUnassigned(1));
        Assert.False(assignment.IsUnassigned(2));
        Assert.False(assignment.IsUnassigned(3));
        Assert.True(assignment.IsUnassigned(4));

        Assert.True(assignment.IsUnassigned(-1));
        Assert.False(assignment.IsUnassigned(-2));
        Assert.False(assignment.IsUnassigned(-3));
        Assert.True(assignment.IsUnassigned(-4));
    }

    [Fact]
    public void AnalyzeConflict_ShouldReturnLearnedClause()
    {
        PartialAssignment assignment = new(12, Substitute.For<IUndo>());

        assignment.Add(1, 1);
        assignment.Add(-2, 1, new ClauseBinary(-1, -2));
        assignment.Add(3, 1, new ClauseBinary(-1, 3));
        assignment.Add(-4, 1, new ClauseBinary(-3, -4));
        assignment.Add(5, 1, new ClauseNary([2, 4, 5]));
        assignment.Add(-6, 2);
        assignment.Add(-7, 2, new ClauseNary([-5, 6, -7]));
        assignment.Add(8, 2, new ClauseNary([2, 7, 8]));
        assignment.Add(-9, 2, new ClauseBinary(-8, -9));
        assignment.Add(10, 2, new ClauseBinary(-8, 10));
        assignment.Add(11, 2, new ClauseNary([9, -10, 11]));
        assignment.Add(-12, 2, new ClauseBinary(-10, -12));

        (List<int> clause, int level) = assignment.AnalyzeConflict(new ClauseBinary(-11, 12), 2);

        Assert.Single(clause);
        Assert.Equal(-8, clause[0]);
        Assert.Equal(0, level);
    }

    [Fact]
    public void ToList_ShouldReturnOrderedList()
    {
        PartialAssignment assignment = new(5, Substitute.For<IUndo>());

        assignment.Add(3, 1);
        assignment.Add(-1, 2);
        assignment.Add(2, 3);
        assignment.Add(-4, 4);
        assignment.Add(5, 5);

        Assert.Equivalent(new List<int>([-1, 2, 3, -4, 5]), assignment.ToList());
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        PartialAssignment assignment = new(5, Substitute.For<IUndo>());

        assignment.Add(3, 1);
        assignment.Add(-1, 2);
        assignment.Add(2, 3);
        assignment.Add(-4, 4);
        assignment.Add(5, 5);

        Assert.Equal("[3, -1, 2, -4, 5]", assignment.ToString());
    }
}
