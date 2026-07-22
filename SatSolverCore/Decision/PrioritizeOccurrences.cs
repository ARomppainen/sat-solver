namespace SatSolverCore.Decision;

/// <summary>
/// Decision maker implementation that generates decisions in order that
/// prioritizes literals based on the number of occurrences.
/// </summary>
public class PrioritizeOccurrences : IDecisionMaker
{
    private readonly int _nVars;
    private readonly int[] _sequence;
    private readonly int[] _scores;
    private readonly int[] _literalIndexInSequence;
    private int _index;

    /// <summary>
    /// Initializes a new instance of the PrioritizeOccurrences class.
    /// </summary>
    /// <param name="formula">The formula to base the heuristic on.</param>
    public PrioritizeOccurrences(Formula formula)
    {
        _nVars = formula.NumberOfVars;
        _sequence = new int[_nVars];
        _scores = new int[_nVars + 1];
        _literalIndexInSequence = new int[_nVars + 1];

        for (int i = 0; i < _nVars; ++i)
        {
            _sequence[i] = i + 1;
        }

        foreach (List<int> clause in formula.Clauses)
        {
            foreach (int literal in clause)
            {
                _scores[Math.Abs(literal)]++;
            }
        }

        Array.Sort(_sequence, (a, b) => _scores[b] - _scores[a]);

        for (int i = 0; i < _nVars; ++i)
        {
            _literalIndexInSequence[_sequence[i]] = i;
        }
    }

    /// <inheritdoc />
    public int ChooseUnassignedLiteral(IPartialAssignment assignment)
    {
        for (int i = _index; i < _nVars - 1; ++i)
        {
            if (assignment.IsUnassigned(_sequence[i]))
            {
                return _sequence[i];
            }
        }

        return _sequence[_nVars - 1];
    }

    /// <inheritdoc />
    public void Backjump(int lastDecision)
    {
        _index = _literalIndexInSequence[Math.Abs(lastDecision)];
    }
}
