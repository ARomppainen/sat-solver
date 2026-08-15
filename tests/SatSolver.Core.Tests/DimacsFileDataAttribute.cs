using System.Reflection;

using SatSolver.Dimacs;
using SatSolver.Shared;

using Xunit.Sdk;
using Xunit.v3;

namespace SatSolver.Core.Tests;

public class DimacsFileDataAttribute(string filePath) : DataAttribute
{
    private readonly string _filePath = filePath;

    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        return DimacsFileReader.ReadFormulas(_filePath)
            .Select(formula => new TheoryDataRow<Formula>(formula))
            .ToList();
    }

    public override bool SupportsDiscoveryEnumeration()
    {
        return false;
    }
}
