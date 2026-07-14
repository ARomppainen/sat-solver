namespace SatSolverCore;

public class Clause(List<int> Literals)
{
    public bool IsFalsified(PartialAssignment assignment)
    {
        return Literals.All(assignment.IsAssignedFalse);
    }

    public bool IsSatisfied(PartialAssignment assignment)
    {
        return Literals.Any(assignment.IsAssignedTrue);
    }
}
