namespace SatSolverCore.Decision;

/// <summary>
/// Decision maker implementation that generates decisions in circular a loop.
/// </summary>
/// <param name="numberOfVars">The number of variables in the formula.</param>
public class Looping(int numberOfVars) : IDecisionMaker
{
    private readonly int _nVars = numberOfVars;
    private int _literal = 1;

    /// <inheritdoc />
    public int ChooseUnassignedLiteral(IPartialAssignment assignment)
    {
        int n = _nVars - 1;

        for (int i = 0; i < n; ++i)
        {
            if (assignment.IsUnassigned(_literal))
            {
                return _literal;
            }
            else
            {
                _literal++;

                if (_literal > _nVars)
                {
                    _literal = 1;
                }
            }
        }

        return _literal;
    }

    public void Backjump(int lastDecision)
    {
        // Do nothing
    }
}
