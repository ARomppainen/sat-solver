using SatSolverCore.Clause;

namespace SatSolverCore;

public class SolverState
{
    private readonly int _numberOfVars;
    private readonly PartialAssignment _assignment;
    private readonly List<int> _unaryClauses;
    private readonly WatchedLiterals _watched;
    private readonly IDecisionMaker _decisionMaker;
    private bool _hasEmptyClause;
    private int _decisionLevel;

    public int DecisionLevel => _decisionLevel;
    public bool HasEmptyClause => _hasEmptyClause;
    public bool AllVariablesHaveValues => _assignment.Count == _numberOfVars;

    public SolverState(Formula formula, IDecisionMaker decisionMaker)
    {
        _numberOfVars = formula.NumberOfVars;
        _assignment = new(formula.NumberOfVars);
        _unaryClauses = [];
        _watched = new();
        _hasEmptyClause = false;
        _decisionLevel = 0;
        _decisionMaker = decisionMaker;

        formula.Clauses.ForEach(AddClause);
    }

    public void MakeDecision()
    {
        ++_decisionLevel;
        int literal = _decisionMaker.ChooseUnassignedLiteral(_assignment);
        _assignment.AddDecision(literal, _decisionLevel);
    }

    public bool UnitPropagate()
    {
        List<int> literals = [];

        int lastDecision = _assignment.LastDecision();
        if (lastDecision != 0)
        {
            if (!_watched.TryFindUnitLiterals(-lastDecision, _assignment, out List<int> unitLiterals))
            {
                return true;
            }

            unitLiterals.ForEach(literals.Add);
        }

        if (_decisionLevel == 0)
        {
            _unaryClauses.ForEach(literals.Add);
        }

        int i = 0;
        while (i < literals.Count)
        {
            int literal = literals[i];
            ++i;

            if (_assignment.IsTrue(literal))
            {
                continue;
            }

            if (!_watched.TryFindUnitLiterals(-literal, _assignment, out List<int> unitLiterals))
            {
                return true;
            }

            _assignment.AddPropagated(literal, _decisionLevel);
            unitLiterals.ForEach(literals.Add);
        }

        return false;
    }

    public void Backjump(int level)
    {
        _assignment.Backjump(level);
        _decisionLevel = level;
    }

    public List<int> Assignment()
    {
        return _assignment.ToList();
    }

    public void AddClause(List<int> literals)
    {
        IClause clause = ClauseFactory.Create(literals);

        if (literals.Count == 0)
        {
            _hasEmptyClause = true;
        }
        else if (literals.Count == 1)
        {
            _unaryClauses.Add(literals[0]);
        }

        _watched.Add(clause);
    }

    public (List<int>, int) AnalyzeConflict()
    {
        return _assignment.GetConflictClause();
    }
}
