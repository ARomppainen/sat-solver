namespace SatSolverCore;

/// <summary>
/// Class for various extension methods.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Creates an iterator for all lines in a given stream reader.
    /// </summary>
    /// <param name="reader"><see cref="StreamReader"/> instance</param>
    /// <returns>iterator of all lines</returns>
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
