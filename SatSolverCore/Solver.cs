namespace SatSolverCore;

public static class Solver
{
    public static SolveResult Solve(Formula formula)
    {
        return Solve(formula, PartialAssignment.Empty());
    }

    private static SolveResult Solve(Formula formula, PartialAssignment assignment)
    {
        if (formula.Clauses.Any(clause => IsFalsified(clause, assignment)))
        {
            return SolveResult.Unsat();
        }

        if (formula.Clauses.All(clause => IsSatisfied(clause, assignment)))
        {
            return SolveResult.Sat(assignment.ToList());
        }

        int literal = ChooseUnassignedLiteral(formula, assignment);

        assignment.Push(literal);
        var res = Solve(formula, assignment);
        if (res.IsSat)
        {
            return res;
        }
        assignment.Pop();

        assignment.Push(-literal);
        res = Solve(formula, assignment);
        if (res.IsSat)
        {
            return res;
        }
        assignment.Pop();

        return SolveResult.Unsat();
    }

    private static bool IsFalsified(List<int> clause, PartialAssignment assignment)
    {
        return clause.All(assignment.IsAssignedFalse);
    }

    private static bool IsSatisfied(List<int> clause, PartialAssignment assignment)
    {
        return clause.Any(assignment.IsAssignedTrue);
    }

    private static int ChooseUnassignedLiteral(Formula formula, PartialAssignment assignment)
    {
        for (int i = 1; i < formula.NumberOfVars; ++i)
        {
            if (assignment.IsUnassigned(i))
            {
                return i;
            }
        }

        return formula.NumberOfVars;
    }
}
