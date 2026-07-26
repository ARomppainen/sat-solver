namespace SatSolverCore;

/// <summary>
/// Interface for objects that need to be able to react to backtracking.
/// </summary>
public interface IUndo
{
    /// <summary>
    /// Update the state of the object after a given variable is removed from
    /// the decision trail.
    /// </summary>
    /// <param name="variable">Variable that was removed from the decision
    /// trail.</param>
    void Undo(int variable);
}
