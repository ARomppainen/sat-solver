namespace SatSolverCore.Clause;

public interface IClause
{
    public int Watched1 { get; }
    public int Watched2 { get; }
    public (bool, int, int) FalsifyFirst(IPartialAssignment assignment);
    public (bool, int, int) FalsifySecond(IPartialAssignment assignment);
}
