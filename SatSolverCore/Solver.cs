using SatSolverCore.Decision;

namespace SatSolverCore;

/// <summary>
/// This class is the main entry point for interacting with the SAT solver.
/// </summary>
public static class Solver
{
    /// <summary>
    /// Tries to find a satistying truth assignment for a given propositional
    /// logic formula using conflict-driven clause learning (CDCL) algorithm.
    /// </summary>
    /// <param name="formula">The propositional logic formula to be solved.</param>
    /// <returns>'satisfiable' result with a truth assignment or 'unsatisfiable' result</returns>
    public static SolveResult Solve(Formula formula)
    {
        if (formula.Clauses.Any(clause => clause.Count == 0))
        {
            // Formulas with an empty clause are unsatisfiable
            return SolveResult.Unsatisfiable();
        }

        // IDecisionMaker decisionMaker = new Looping(formula.NumberOfVars);
        IDecisionMaker decisionMaker = new PrioritizeOccurrences(formula);
        SolverState state = new(formula, decisionMaker);

        while (true)
        {
            bool conflict = state.UnitPropagate();

            if (conflict)
            {
                if (state.DecisionLevel == 0)
                {
                    return SolveResult.Unsatisfiable();
                }

                (List<int> clause, int level) = state.AnalyzeConflict();

                state.AddClause(clause);
                state.Backjump(level);
            }
            else
            {
                if (state.AllVariablesHaveValues)
                {
                    return SolveResult.Satisfiable(state.Assignment());
                }

                state.MakeDecision();
            }
        }
    }
}
