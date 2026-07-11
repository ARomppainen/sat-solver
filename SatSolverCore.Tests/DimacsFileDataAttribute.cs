using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace SatSolverCore.Tests;

public class DimacsFileDataAttribute(string filePath) : DataAttribute
{
    private readonly string _filePath = filePath;

    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        string path = Path.IsPathRooted(_filePath)
            ? _filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"File does not exist: {path}");
        }

        using StreamReader reader = File.OpenText(path);

        Formula formula = DimacsParser.Parse(reader.Lines());

        return [new TheoryDataRow<Formula>(formula)];
    }

    public override bool SupportsDiscoveryEnumeration()
    {
        return false;
    }
}