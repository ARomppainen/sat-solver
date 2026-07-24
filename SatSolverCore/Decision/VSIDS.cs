namespace SatSolverCore.Decision;

/// <summary>
/// Decision maker implementation that uses Variable State Independent Decaying
/// Sum (VSIDS) heuristic.
/// </summary>
public class VSIDS : IDecisionMaker
{
    private readonly int _nVars;
    private readonly double[] _scores;
    private readonly int[] _polarity;
    private int _decayCounter;
    private const int DecayThreshold = 100;
    private const double DecayFactor = 0.995;
    private const double RescaleThreshold = 1e+100;
    private const double RescaleFactor = 1e-100;

    /// <summary>
    /// Initializes a new instance of VSIDS class.
    /// </summary>
    /// <param name="formula">The formula to base the heuristic on.</param>
    public VSIDS(Formula formula)
    {
        _nVars = formula.NumberOfVars;
        _scores = new double[_nVars + 1];
        _polarity = new int[_nVars + 1];
        _decayCounter = 0;

        foreach (List<int> clause in formula.Clauses)
        {
            foreach (int literal in clause)
            {
                _scores[Math.Abs(literal)] += 1.0;
            }
        }

        for (int i = 1; i <= _nVars; ++i)
        {
            _polarity[i] = 1;
        }
    }

    /// <inheritdoc />
    public int ChooseUnassignedLiteral(IPartialAssignment assignment)
    {
        double max = -1;
        int literal = 0;

        for (int i = 1; i <= _nVars; ++i)
        {
            if (assignment.IsUnassigned(i))
            {
                if (_scores[i] > max)
                {
                    max = _scores[i];
                    literal = i;
                }
            }
        }

        int polarity = _polarity[literal];
        _polarity[literal] *= -1;

        return literal * polarity;
    }

    /// <summary>
    /// Update variable scores after a new clause is learned.
    /// </summary>
    /// <param name="learnedClause">The learned clause.</param>
    public void Update(List<int> learnedClause)
    {
        foreach (int literal in learnedClause)
        {
            _scores[Math.Abs(literal)] += 1.0;
        }

        _decayCounter++;

        if (_decayCounter >= DecayThreshold)
        {
            _decayCounter = 0;

            for (int i = 1; i <= _nVars; ++i)
            {
                _scores[i] *= DecayFactor;
            }
        }

        if (learnedClause.Any(l => _scores[Math.Abs(l)] > RescaleThreshold))
        {
            for (int i = 1; i <= _nVars; ++i)
            {
                _scores[i] *= RescaleFactor;
            }
        }
    }
}
