using System.Diagnostics;

namespace SatSolverCore;

public static class DimacsParser
{
    public static Formula Parse(IEnumerable<string> data)
    {
        using var lines = data.GetEnumerator();

        if (!lines.MoveNext())
        {
            throw new DimacsParseError("Unexpected end of input");
        }

        while (IsComment(lines.Current))
        {
            if (!lines.MoveNext())
            {
                throw new DimacsParseError("Unexpected end of input");
            }
        }

        (int numberOfVars, int numberOfClauses) = ParseProblemLine(lines.Current);

        var clauses = new List<List<int>>(numberOfClauses);

        for (int i = 0; i < numberOfClauses; ++i)
        {
            if (!lines.MoveNext())
            {
                throw new DimacsParseError("Unexpected end of input");
            }

            var clause = ParseClause(lines.Current, numberOfVars);
            clauses.Add(clause);
        }

        return new Formula(numberOfVars, clauses);
    }

    private static bool IsComment(string line)
    {
        return !string.IsNullOrEmpty(line) && line[0] == 'c';
    }

    private static Tuple<int, int> ParseProblemLine(string line)
    {
        var parts = line.Split(" ");

        Debug.Assert(parts.Length == 4);
        Debug.Assert("p" == parts[0]);
        Debug.Assert("cnf" == parts[1]);

        if (!int.TryParse(parts[2], out int numberOfVars))
        {
            throw new DimacsParseError($"Expected an integer: {parts[2]}");
        }

        if (numberOfVars <= 0)
        {
            throw new DimacsParseError($"Value should be positive: {numberOfVars}");
        }

        if (!int.TryParse(parts[3], out int numberOfClauses))
        {
            throw new DimacsParseError($"Expected an integer: {parts[3]}");
        }

        if (numberOfClauses <= 0)
        {
            throw new DimacsParseError($"Value should be positive: {numberOfClauses}");
        }

        return Tuple.Create(numberOfVars, numberOfClauses);
    }

    private static List<int> ParseClause(string line, int numberOfVars)
    {
        var clause = new List<int>();

        foreach (var part in line.Split(" "))
        {
            if (!int.TryParse(part, out int value))
            {
                throw new DimacsParseError($"Expected an integer: {part}");
            }

            if (value == 0)
            {
                break;
            }

            if (value > numberOfVars || value < -numberOfVars)
            {
                throw new DimacsParseError($"Value out of range: {value}");
            }

            clause.Add(value);
        }

        return clause;
    }
}
