using System.CommandLine;
using System.CommandLine.Parsing;

using SatSolverCore;

Option<FileInfo> fileOption = new("--file", "-f")
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
        await Console.Error.WriteLineAsync(parseError.Message);
    }
    return 1;
}

if (parseResult.GetValue(fileOption) is FileInfo parsedFile)
{
    if (!parsedFile.Exists)
    {
        await Console.Error.WriteLineAsync($"File does not exist: {parsedFile.FullName}");
        return 1;
    }

    using StreamReader reader = parsedFile.OpenText();

    Formula formula = DimacsParser.Parse(parsedFile.FullName, reader.Lines());
    SolveResult result = Solver.Solve(formula);

    if (result.IsSatisfiable)
    {
        Console.WriteLine("s SATISFIABLE");
        if (result.Assignment.Count > 0)
        {
            Console.WriteLine($"v {string.Join(' ', result.Assignment)} 0");
        }
        else
        {
            Console.WriteLine("v 0");
        }
    }
    else
    {
        Console.WriteLine("s UNSATISFIABLE");
    }

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
