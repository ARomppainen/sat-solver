using System.CommandLine;
using System.CommandLine.Parsing;

using SatSolverCore;

Option<FileInfo> fileOption = new("--file", ["-f"])
{
    Description = "The path to a DIMACS file to be used as input."
};

RootCommand rootCommand = new("SAT-Solver CLI");
rootCommand.Options.Add(fileOption);

ParseResult parseResult = rootCommand.Parse(args);

if (parseResult.Errors.Count > 0)
{
    foreach (ParseError parseError in parseResult.Errors)
    {
        Console.Error.WriteLine(parseError.Message);
    }
    return 1;
}

if (parseResult.GetValue(fileOption) is FileInfo parsedFile)
{
    if (!parsedFile.Exists)
    {
        Console.Error.WriteLine($"File does not exist: {parsedFile.FullName}");
        return 1;
    }

    using StreamReader reader = parsedFile.OpenText();

    Formula formula = DimacsParser.Parse(parsedFile.FullName, reader.Lines());
    SolveResult result = Solver.Solve(formula);

    Console.WriteLine(result);
    return 0;
}

Console.WriteLine("Description:");
Console.WriteLine($"  {rootCommand.Description}");
Console.WriteLine();
Console.WriteLine("Usage:");
Console.WriteLine("  SatSolverCli.exe [options]");
Console.WriteLine();
Console.WriteLine("Options:");
Console.WriteLine($"  {fileOption.Name}, {fileOption.Aliases.Aggregate(string.Join)} {fileOption.Description}");
return 0;
