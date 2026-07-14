namespace SatSolverCore;

public static class Solver
{
    public static SolveResult Solve(Formula formula)
    {

        var assignment = PartialAssignment.Empty();

        bool conflict = assignment.DecideUnaryClauses(formula.UnaryClauses);

        if (conflict)
        {
            return SolveResult.Unsat();
        }

        return Solve(formula, assignment);
    }

    private static SolveResult Solve(Formula formula, PartialAssignment assignment)
    {
        if (formula.Clauses.Any(clause => clause.IsFalsified(assignment)))
        {
            return SolveResult.Unsat();
        }

        if (formula.Clauses.All(clause => clause.IsSatisfied(assignment)))
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
