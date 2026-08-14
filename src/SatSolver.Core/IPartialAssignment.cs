namespace SatSolver.Core;

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
    /// Check if the literal is part of the current truth assignment.
    /// </summary>
    /// <param name="literal">literal value (a non-zero integer)</param>
    /// <returns>true if the literal is assigned; otherwise, false</returns>
    public bool IsAssigned(int literal);

    /// <summary>
    /// Check if the variable is unassigned (i.e. neither the positive or the
    /// negated literal are part of the current truth assignment).
    /// </summary>
    /// <param name="variable">literal value (a non-zero integer)</param>
    /// <returns>true if the literal is unassigned; otherwise, false</returns>
    public bool IsUnassigned(int variable);
}
