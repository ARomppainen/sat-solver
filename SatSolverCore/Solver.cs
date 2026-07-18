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

        if (state.IsSatisfied())
        {
            state.AssignRemainingLiteralsToTrue();
            return SolveResult.Sat(state.Assignment());
        }

        Stack<ValueTuple<int, int>> decisions = new();
        int currentLevel = 0;

        int literal = state.ChooseUnassignedLiteral();
        decisions.Push(ValueTuple.Create(-literal, 1));
        decisions.Push(ValueTuple.Create(literal, 1));

        while (decisions.Count > 0)
        {
            (int decision, int level) = decisions.Pop();

            int backtrack = currentLevel - level + 1;

            for (int i = 0; i < backtrack; ++i)
            {
                state.Backtrack();
            }

            currentLevel = level - 1;

            if (state.Decide(decision))
            {
                continue;
            }

            currentLevel = level;

            if (state.Propagate())
            {
                continue;
            }

            if (state.IsSatisfied())
            {
                state.AssignRemainingLiteralsToTrue();
                return SolveResult.Sat(state.Assignment());
            }

            literal = state.ChooseUnassignedLiteral();
            decisions.Push(ValueTuple.Create(-literal, level + 1));
            decisions.Push(ValueTuple.Create(literal, level + 1));
        }

        return SolveResult.Unsat();
    }
}
