namespace SatSolverCore.Tests;

public static class ExtensionMethods
{
    public static IEnumerable<string> Lines(this StreamReader reader)
    {
        while (true)
        {
            string? line = reader.ReadLine();

            if (line == null)
            {
                yield break;
            }

            yield return line;
        }
    }
}
