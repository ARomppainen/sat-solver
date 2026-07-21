namespace SatSolverCore;

/// <summary>
/// A partial truth assignment keeps track of the truth values that are assigned
/// to the variables.
/// </summary>
/// <remarks>
/// The variables are represented using (non-zero) integers:
/// <list type="bullet">
/// <item>positive integers represent variables: a, b, c</item>
/// <item>negative integers represent negated variables: ¬a, ¬b, ¬c</item>
/// </list>
/// </remarks>
public interface IPartialAssignment
{
    /// <summary>
    /// Check if the variable is part of the current truth assignment.
    /// </summary>
    /// <param name="variable">variable value (a non-zero integer)</param>
    /// <returns>true if the variable is assigned; otherwise, false</returns>
    public bool IsAssigned(int variable);

    /// <summary>
    /// Check if the literal is unassigned (i.e. the variable or its negation is
    /// not part of the current truth assignment).
    /// </summary>
    /// <param name="literal">literal value (a non-zero integer)</param>
    /// <returns>true if the literal is unassigned; otherwise, false</returns>
    public bool IsUnassigned(int literal);
}
