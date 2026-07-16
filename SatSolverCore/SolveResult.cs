namespace SatSolverCore;

public class SolveResult
{
    private readonly bool _sat;
    private readonly List<int> _assignment;

    public bool IsSat { get { return _sat; } }

    public List<int> Assignment { get { return _assignment; } }

    private SolveResult(bool type, List<int> assignment)
    {
        _sat = type;
        _assignment = assignment;
    }

    public static SolveResult Sat(List<int> assignment)
    {
        return new SolveResult(true, assignment);
    }

    public static SolveResult Unsat()
    {
        return new SolveResult(false, []);
    }

    public override string ToString()
    {
        if (!_sat)
        {
            return "unsat";
        }

        return string.Join(' ', _assignment);
    }
}
