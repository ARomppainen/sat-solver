using System.Runtime.InteropServices;

namespace SatSolver.Core;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme. Uses Lists
/// as the backing data structure instead of LinkedLists (should be more
/// performant than V1).
/// </summary>
/// <remarks>
/// The watched literals are always the first two elements in the list literals,
/// i.e. clause[0] and clause[1].
/// </remarks>
public class WatchedLiteralsV2
{
    private readonly int _nVar;
    private readonly List<List<int>>[] _watchlist;

    public WatchedLiteralsV2(int numberOfVars)
    {
        _nVar = numberOfVars;

        int n = 2 * numberOfVars + 1;
        _watchlist = new List<List<int>>[n];

        for (int i = 0; i < n; ++i)
        {
            _watchlist[i] = [];
        }
    }

    /// <summary>
    /// Add new clause to be tracked. This can be an initial clause found in the
    /// formula or a learned clause.
    /// </summary>
    /// <param name="clause">The clause to be tracked.</param>
    public void Add(List<int> clause)
    {
        _watchlist[clause[0] + _nVar].Add(clause);
        _watchlist[clause[1] + _nVar].Add(clause);
    }

    /// <summary>
    /// Tries to find new unit literals after the given literal is set to false.
    /// </summary>
    /// <param name="literal">The literal that is set to false.</param>
    /// <param name="assignment">The current partial assignment.</param>
    /// <param name="unitLiterals">The queue of unit literals to append.</param>
    /// <returns>A conflicting clause is one was detected; otherwise, null.</returns>
    public List<int>? FindUnitLiterals(int literal, IPartialAssignment assignment, Queue<(int, List<int>?)> unitLiterals)
    {
        List<List<int>> watched = _watchlist[literal + _nVar];

        int i, j;
        for (i = 0, j = 0; i < watched.Count;)
        {
            List<int> clause = watched[i++];

            if (clause[0] == literal)
            {
                clause[0] = clause[1];
                clause[1] = literal;
            }

            if (assignment.IsAssigned(clause[0]))
            {
                watched[j++] = clause;
                continue;
            }

            if (WatchedLiteralsUtil.FindNewWatchedLiteral(literal, assignment, clause))
            {
                _watchlist[clause[1] + _nVar].Add(clause);
                continue;
            }

            watched[j++] = clause;

            if (assignment.IsAssigned(-clause[0]))
            {
                while (i < watched.Count)
                {
                    watched[j++] = watched[i++];
                }
                CollectionsMarshal.SetCount(watched, j);

                return clause;
            }

            unitLiterals.Enqueue((clause[0], clause));
        }

        CollectionsMarshal.SetCount(watched, j);
        return null;
    }
}
