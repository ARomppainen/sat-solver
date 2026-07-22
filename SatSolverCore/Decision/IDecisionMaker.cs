namespace SatSolverCore.Decision;

/// <summary>
/// Decision makers are responsible for choosing the literals that are added to
/// the partial truth assignment. They can use heuristics or other methods to
/// make the "best" decision (the best guess).
/// </summary>
public interface IDecisionMaker
{
    /// <summary>
    /// Choose the next unassigned literal to be added to the current partial
    /// truth assignment.
    /// </summary>
    /// <param name="assignment">current partial truth assignment</param>
    /// <returns>the literal value to be assigned</returns>
    int ChooseUnassignedLiteral(IPartialAssignment assignment);

    /// <summary>
    /// Update state after a backjump if needed.
    /// </summary>
    /// <param name="lastDecision">Last decided literal after the backjump (or zero)</param>
    void Backjump(int lastDecision);
}
