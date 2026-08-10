using System.Diagnostics;

namespace SatSolver.Core;

/// <summary>
/// Data structure that orchestrates the two-watched-literal scheme.
/// </summary>
public class WatchedLiterals
{
    private readonly int _nVar;
    private readonly LinkedList<int>[] _binaryWatchlist;
    private readonly LinkedList<Clause>[] _watchlist1;
    private readonly LinkedList<Clause>[] _watchlist2;

    public WatchedLiterals(int numberOfVars)
    {
        _nVar = numberOfVars;

        int n = 2 * numberOfVars + 1;
        _binaryWatchlist = new LinkedList<int>[n];
        _watchlist1 = new LinkedList<Clause>[n];
        _watchlist2 = new LinkedList<Clause>[n];

        for (int i = 0; i < n; ++i)
        {
            _binaryWatchlist[i] = new();
            _watchlist1[i] = new();
            _watchlist2[i] = new();
        }
    }

    /// <summary>
    /// Add new clause to be tracked. This can be an initial clause found in the
    /// formula or a learned clause.
    /// </summary>
    /// <param name="clause">The clause to be tracked.</param>
    public void Add(Clause clause)
    {
        _watchlist1[clause.Watched1 + _nVar].AddLast(clause);
        _watchlist2[clause.Watched2 + _nVar].AddLast(clause);
    }

    public void AddBinary(List<int> literals)
    {
        Debug.Assert(literals.Count == 2);

        _binaryWatchlist[literals[0] + _nVar].AddLast(literals[1]);
        _binaryWatchlist[literals[1] + _nVar].AddLast(literals[0]);
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
        List<int>? conflict = FindUnitLiteralsBinary(unitLiterals, literal, assignment);

        if (conflict != null)
        {
            return conflict;
        }

        conflict = FindUnitLiterals(unitLiterals, literal, assignment, _watchlist1, FalsifyFirst, _nVar);

        if (conflict != null)
        {
            return conflict;
        }

        return FindUnitLiterals(unitLiterals, literal, assignment, _watchlist2, FalsifySecond, _nVar);
    }

    private List<int>? FindUnitLiteralsBinary(
        Queue<(int, List<int>?)> unitLiterals,
        int literal,
        IPartialAssignment assignment
    )
    {
        LinkedListNode<int>? node = _binaryWatchlist[literal + _nVar].First;

        while (node != null)
        {
            int other = node.Value;
            node = node.Next;

            if (assignment.IsAssigned(-other))
            {
                return [literal, other];
            }

            if (!assignment.IsAssigned(other))
            {
                unitLiterals.Enqueue((other, [literal, other]));
            }
        }

        return null;
    }

    private static List<int>? FindUnitLiterals(
        Queue<(int, List<int>?)> unitLiterals,
        int literal,
        IPartialAssignment assignment,
        LinkedList<Clause>[] watchlist,
        Func<Clause, IPartialAssignment, FalsifyResult> falsifyLiteral,
        int n
    )
    {
        LinkedList<Clause> list = watchlist[literal + n];
        LinkedListNode<Clause>? node = list.First;

        while (node != null)
        {
            var clause = node.ValueRef;

            var result = falsifyLiteral(clause, assignment);

            if (result.IsConflict)
            {
                return clause.Literals;
            }

            if (result.UnitLiteral != 0)
            {
                unitLiterals.Enqueue((result.UnitLiteral, clause.Literals));
            }

            if (result.NewWatchedLiteral != 0)
            {
                watchlist[result.NewWatchedLiteral + n].AddLast(clause);
                var previous = node;
                node = node.Next;
                list.Remove(previous);
            }
            else
            {
                node = node.Next;
            }
        }

        return null;
    }

    private static FalsifyResult FalsifyFirst(Clause clause, IPartialAssignment assignment)
    {
        return clause.FalsifyFirst(assignment);
    }

    private static FalsifyResult FalsifySecond(Clause clause, IPartialAssignment assignment)
    {
        return clause.FalsifySecond(assignment);
    }
}
