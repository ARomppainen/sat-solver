namespace SatSolver.Core;

public static class WatchedLiteralsUtil
{
    public static bool FindNewWatchedLiteral(int literal, IPartialAssignment assignment, List<int> clause)
    {
        for (int index = 2; index < clause.Count; ++index)
        {
            if (!assignment.IsAssigned(-clause[index]))
            {
                clause[1] = clause[index];
                clause[index] = literal;
                return true;
            }
        }

        return false;
    }
}
