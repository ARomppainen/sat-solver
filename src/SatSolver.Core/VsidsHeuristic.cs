using SatSolver.Shared;

namespace SatSolver.Core;

/// <summary>
/// An implementation of Variable State Independent Decaying Sum (VSIDS)
/// heuristic for making decisions.
/// </summary>
public class VsidsHeuristic : IUndo
{
    private readonly int _nVars;
    private readonly double[] _scores;
#if USE_MAX_HEAP
    private readonly MaxHeap _vars;
#endif
    private int _decayCounter;
    private readonly int _decayThreshold = 100;
    private readonly double _decayFactor = 0.995;
    private const double RescaleThreshold = 1e+100;
    private const double RescaleFactor = 1e-100;

    /// <summary>
    /// Initializes a new instance of Vsids class.
    /// </summary>
    /// <param name="formula">The formula to base the heuristic on.</param>
    /// <param name="decayThreshold">The frequency of how often the scores are adjusted.</param>
    /// <param name="decayFactor">The factor used for adjusting the scores.</param>
    public VsidsHeuristic(Formula formula, int decayThreshold, double decayFactor)
    {
        _nVars = formula.NumberOfVars;
        _scores = new double[_nVars + 1];
        _decayCounter = 0;
        _decayThreshold = decayThreshold;
        _decayFactor = decayFactor;

#if USE_MAX_HEAP
        _vars = MaxHeap.Create(_nVars, (a, b) => _scores[a] < _scores[b] ? -1 : 1);
#endif

        foreach (List<int> clause in formula.Clauses)
        {
            foreach (int literal in clause)
            {
                _scores[Math.Abs(literal)] += 1.0;
            }
        }
    }

    /// <summary>
    /// Choose the next unassigned literal to be added to the current partial
    /// truth assignment.
    /// </summary>
    /// <param name="assignment">The current partial truth assignment.</param>
    /// <returns>The literal value to be assigned.</returns>
    public int Choose(IPartialAssignment assignment)
    {
#if USE_MAX_HEAP
        int value;
        do
        {
            value = _vars.Pop();
        } while (!assignment.IsUnassigned(value));
        return value;
#else
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
#endif
    }

    /// <inheritdoc />
    public void Undo(int variable)
    {
#if USE_MAX_HEAP
        _vars.Push(variable);
#endif
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

        if (_decayCounter >= _decayThreshold)
        {
            _decayCounter = 0;

            for (int i = 1; i <= _nVars; ++i)
            {
                _scores[i] *= _decayFactor;
            }
        }

        if (learnedClause.Any(l => _scores[Math.Abs(l)] > RescaleThreshold))
        {
            for (int i = 1; i <= _nVars; ++i)
            {
                _scores[i] *= RescaleFactor;
            }
        }

#if USE_MAX_HEAP
        foreach (int literal in learnedClause)
        {
            int v = Math.Abs(literal);

            if (!_vars.Push(v))
            {
                _vars.UpHeap(v);
            }
        }
#endif
    }
}
