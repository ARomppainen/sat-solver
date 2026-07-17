namespace SatSolverCore;

public static class Solver
{
    public static SolveResult Solve(Formula formula)
    {
        if (formula.HasEmptyClause)
        {
            return SolveResult.Unsat();
        }

        SolverState state = new(formula);

        if (state.DecideUnaryClauses())
        {
            return SolveResult.Unsat();
        }

        return Solve(state);
    }

    private static SolveResult Solve(SolverState state)
    {
        if (state.IsFalsified())
        {
            return SolveResult.Unsat();
        }

        if (state.Propagate())
        {
            return SolveResult.Unsat();
        }

        if (state.IsSatisfied())
        {
            state.AssignRemainingLiteralsToTrue();

            return SolveResult.Sat(state.Assignment());
        }

        int literal = state.ChooseUnassignedLiteral();
        SolveResult res;

        // BEGIN decide true
        if (state.Decide(literal))
        {
            return SolveResult.Unsat();
        }

        res = Solve(state);
        if (res.IsSat)
        {
            return res;
        }
        state.Backtrack();
        // END decide true

        // BEGIN decide false
        if (state.Decide(-literal))
        {
            return SolveResult.Unsat();
        }

        res = Solve(state);
        if (res.IsSat)
        {
            return res;
        }
        state.Backtrack();
        // END decide false

        return SolveResult.Unsat();
    }
}
