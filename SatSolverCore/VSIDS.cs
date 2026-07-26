namespace SatSolverCore;

/// <summary>
/// An implementation of Variable State Independent Decaying Sum (VSIDS)
/// heuristic for making decisions.
/// </summary>
public class Vsids : IUndo
{
    private readonly int _nVars;
    private readonly double[] _scores;
    private readonly MaxHeap _vars;
    private int _decayCounter;
    private const int DecayThreshold = 100;
    private const double DecayFactor = 0.995;
    private const double RescaleThreshold = 1e+100;
    private const double RescaleFactor = 1e-100;

    /// <summary>
    /// Initializes a new instance of Vsids class.
    /// </summary>
    /// <param name="formula">The formula to base the heuristic on.</param>
    public Vsids(Formula formula)
    {
        _nVars = formula.NumberOfVars;
        _scores = new double[_nVars + 1];
        _vars = MaxHeap.Create(_nVars, (a, b) => _scores[a] < _scores[b] ? -1 : 1);
        _decayCounter = 0;

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
        int value;
        do
        {
            value = _vars.Pop();
        } while (!assignment.IsUnassigned(value));
        return value;
    }

    /// <inheritdoc />
    public void Undo(int variable)
    {
        _vars.Push(variable);
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

        foreach (int literal in learnedClause)
        {
            int v = Math.Abs(literal);

            if (!_vars.Push(v))
            {
                _vars.UpHeap(v);
            }
        }
    }
}
