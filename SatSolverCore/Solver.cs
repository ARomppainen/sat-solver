namespace SatSolverCore;

public static class Solver
{
    public static SolveResult Solve(Formula formula)
    {
        if (formula.HasEmptyClause)
        {
            return SolveResult.Unsat();
        }

        var assignment = PartialAssignment.Empty();

        bool conflict = assignment.DecideUnaryClauses(formula.UnaryClauses);

        if (conflict)
        {
            return SolveResult.Unsat();
        }

        var watched = new WatchedLiterals(formula);

        return Solve(formula, assignment, watched, []);
    }

    private static SolveResult Solve(Formula formula, PartialAssignment assignment, WatchedLiterals watched, List<int> propagated)
    {
        if (formula.Clauses.Any(clause => clause.IsFalsified(assignment)))
        {
            return SolveResult.Unsat();
        }

        int i = 0;
        while (i < propagated.Count)
        {
            int p = propagated[i];
            ++i;

            if (assignment.IsUnassigned(p))
            {
                if (!watched.TryAssignToFalse(-p, assignment, out List<int> more))
                {
                    return SolveResult.Unsat();
                }

                assignment.Propagate(p);

                foreach (int p2 in more)
                {
                    propagated.Add(p2);
                }
            }
        }

        if (formula.Clauses.All(clause => clause.IsSatisfied(assignment)))
        {
            return SolveResult.Sat(assignment.ToList());
        }

        int literal = ChooseUnassignedLiteral(formula, assignment);

        // BEGIN choose true
        if (!watched.TryAssignToFalse(-literal, assignment, out List<int> prop1))
        {
            return SolveResult.Unsat();
        }
        assignment.Decide(literal);

        var res = Solve(formula, assignment, watched, prop1);
        if (res.IsSat)
        {
            return res;
        }
        assignment.Backtrack();
        // END choose true

        // BEGIN choose false
        if (!watched.TryAssignToFalse(literal, assignment, out List<int> prop2))
        {
            return SolveResult.Unsat();
        }
        assignment.Decide(-literal);
        res = Solve(formula, assignment, watched, prop2);
        if (res.IsSat)
        {
            return res;
        }
        assignment.Backtrack();
        // END choose false

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
