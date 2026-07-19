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

        if (File.Exists(path))
        {
            using StreamReader reader = File.OpenText(path);
            Formula formula = DimacsParser.Parse(path, reader.Lines());
            return [new TheoryDataRow<Formula>(formula)];
        }

        if (Directory.Exists(path))
        {
            List<TheoryDataRow<Formula>> formulas = [];

            foreach (string filepath in Directory.GetFiles(path, "*.cnf"))
            {
                using StreamReader reader = File.OpenText(filepath);
                Formula formula = DimacsParser.Parse(filepath, reader.Lines());
                formulas.Add(new TheoryDataRow<Formula>(formula));
            }

            if (formulas.Count == 0)
            {
                throw new ArgumentException($"No .cnf files found in the directory: {path}");
            }

            return formulas;
        }

        throw new ArgumentException($"File or directory does not exist: {path}");
    }

    public override bool SupportsDiscoveryEnumeration()
    {
        return false;
    }
}
