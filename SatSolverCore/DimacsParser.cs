using System.Diagnostics;

namespace SatSolverCore;

/// <summary>
/// Parser for DIMACS formatted data.
/// </summary>
/// <remarks>
/// See <see href="https://acl2.org/doc/?topic=SATLINK____DIMACS"/> for a
/// detailed explanation about the input format.
/// </remarks>
public static class DimacsParser
{
    /// <summary>
    /// Parses DIMACS formatted input into a formula.
    /// </summary>
    /// <param name="name">See <see cref="Formula.Name"/></param>
    /// <param name="data">DIMACS data</param>
    /// <returns>parsed formula</returns>
    /// <exception cref="DimacsParseException"></exception>
    public static Formula Parse(string name, IEnumerable<string> data)
    {
        using var lines = data.GetEnumerator();

        if (!lines.MoveNext())
        {
            throw new DimacsParseException("Unexpected end of input");
        }

        while (IsCommentOrWhiteSpace(lines.Current))
        {
            if (!lines.MoveNext())
            {
                throw new DimacsParseException("Unexpected end of input");
            }
        }

        (int numberOfVars, int numberOfClauses) = ParseProblemLine(lines.Current);

        List<List<int>> clauses = new(numberOfClauses);

        for (int i = 0; i < numberOfClauses; ++i)
        {
            if (!lines.MoveNext())
            {
                throw new DimacsParseException("Unexpected end of input");
            }

            if (lines.Current.IsWhiteSpace())
            {
                continue;
            }

            var literals = ParseLiterals(lines.Current, numberOfVars);
            clauses.Add(literals);
        }

        return new Formula(name, numberOfVars, clauses);
    }

    private static bool IsCommentOrWhiteSpace(string line)
    {
        return !string.IsNullOrEmpty(line) && line[0] == 'c' || line.IsWhiteSpace();
    }

    private static (int, int) ParseProblemLine(string line)
    {
        var parts = line.Split(" ");

        Debug.Assert(parts.Length == 4);
        Debug.Assert("p" == parts[0]);
        Debug.Assert("cnf" == parts[1]);

        if (!int.TryParse(parts[2], out int numberOfVars))
        {
            throw new DimacsParseException($"Expected an integer: {parts[2]}");
        }

        if (numberOfVars < 0)
        {
            throw new DimacsParseException($"Value should not be negative: {numberOfVars}");
        }

        if (!int.TryParse(parts[3], out int numberOfClauses))
        {
            throw new DimacsParseException($"Expected an integer: {parts[3]}");
        }

        if (numberOfClauses < 0)
        {
            throw new DimacsParseException($"Value should not be negative: {numberOfClauses}");
        }

        return (numberOfVars, numberOfClauses);
    }

    private static List<int> ParseLiterals(string line, int numberOfVars)
    {
        var literals = new List<int>();

        foreach (var part in line.Split(" "))
        {
            if (!int.TryParse(part, out int value))
            {
                throw new DimacsParseException($"Expected an integer: {part}");
            }

            if (value == 0)
            {
                break;
            }

            if (value > numberOfVars || value < -numberOfVars)
            {
                throw new DimacsParseException($"Value out of range: {value}");
            }

            literals.Add(value);
        }

        return literals;
    }
}
