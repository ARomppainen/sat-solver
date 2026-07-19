namespace SatSolverCore;

public class DecisionMaker(int numberOfVars) : IDecisionMaker
{
    private readonly IEnumerator<int> _sequence = Sequence(numberOfVars).GetEnumerator();

    public int ChooseUnassignedLiteral(IPartialAssignment assignment)
    {
        while (true)
        {
            _sequence.MoveNext();

            if (assignment.IsUnassigned(_sequence.Current))
            {
                return _sequence.Current;
            }
        }
    }

    private static IEnumerable<int> Sequence(int numberOfVars)
    {
        int i = 1;

        while (true)
        {
            yield return i;
            yield return -i;

            ++i;

            if (i > numberOfVars)
            {
                i = 1;
            }
        }
    }
}
