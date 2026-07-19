namespace SatSolverCore;

public static class Solver
{
    public static SolveResult Solve(Formula formula)
    {
        IDecisionMaker decisionMaker = new DecisionMaker(formula.NumberOfVars);
        SolverState state = new(formula, decisionMaker);

        if (state.HasEmptyClause)
        {
            return SolveResult.Unsat();
        }

        while (true)
        {
            bool conflict = state.UnitPropagate();

            if (conflict)
            {
                if (state.DecisionLevel == 0)
                {
                    return SolveResult.Unsat();
                }

                (List<int> clause, int level) = state.AnalyzeConflict();

                state.AddClause(clause);
                state.Backjump(level);
            }
            else
            {
                if (state.AllVariablesHaveValues)
                {
                    return SolveResult.Sat(state.Assignment());
                }

                state.MakeDecision();
            }
        }
    }
}
