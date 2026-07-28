namespace SatSolverCore;

public class VsidsSimple : IUndo
{
    private readonly int _nVars;
    private readonly double[] _scores;
    private int _decayCounter;
    private const int DecayThreshold = 100;
    private const double DecayFactor = 0.995;
    private const double RescaleThreshold = 1e+100;
    private const double RescaleFactor = 1e-100;

    public VsidsSimple(Formula formula)
    {
        _nVars = formula.NumberOfVars;
        _scores = new double[_nVars + 1];
        _decayCounter = 0;

        foreach (List<int> clause in formula.Clauses)
        {
            foreach (int literal in clause)
            {
                _scores[Math.Abs(literal)] += 1.0;
            }
        }
    }

    public int Choose(IPartialAssignment assignment)
    {
        double max = -1;
        int literal = 0;

        for (int i = 1; i <= _nVars; ++i)
        {
            if (assignment.IsUnassigned(i) && _scores[i] > max)
            {
                max = _scores[i];
                literal = i;
            }
        }

        return literal;
    }

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

    public void Undo(int variable)
    {
        // Do nothing
    }
}
