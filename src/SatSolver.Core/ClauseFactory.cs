using System.Diagnostics;

namespace SatSolver.Core;

/// <summary>
/// Factory for instantiating <see cref="IClause"/> instances.
/// </summary>
public static class ClauseFactory
{
    /// <summary>
    /// Create a new <see cref="IClause"/> instance based on list of literals.
    /// </summary>
    /// <param name="literals">list of literals</param>
    /// <param name="assignment">current assignment</param>
    /// <returns>new concrete <see cref="IClause"/> instance</returns>
    public static IClause Create(List<int> literals, IPartialAssignment assignment)
    {
        return literals.Count switch
        {
            0 => throw new ArgumentException("Empty clauses are not supported."),
            1 => throw new ArgumentException("Unary clauses are not supported."),
            2 => new ClauseBinary(literals[0], literals[1]),
            _ => CreateNary(literals, assignment)
        };


    }

    private static ClauseNary CreateNary(List<int> literals, IPartialAssignment assignment)
    {
        int index1 = 0;
        int index2 = 1;

        Debug.Assert(
            assignment.IsUnassigned(literals[0]),
            $"Expected the first literal in a clause to be unassigned: [{string.Join(", ", literals)}]"
        );

        if (assignment.IsAssigned(-literals[1]))
        {
            for (int i = 2; i < literals.Count; ++i)
            {
                if (!assignment.IsAssigned(-literals[i]))
                {
                    index2 = i;
                    break;
                }
            }
        }

        return new ClauseNary(literals, index1, index2);
    }
}
