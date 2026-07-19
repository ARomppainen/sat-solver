namespace SatSolverCore;

public interface IPartialAssignment
{
    public bool IsTrue(int literal);
    public bool IsFalse(int literal);
    public bool IsUnassigned(int literal);
}
