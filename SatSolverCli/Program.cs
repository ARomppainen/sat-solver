using System.CommandLine;
using System.CommandLine.Parsing;

using SatSolverCore;

Option<FileInfo> fileOption = new("--file", "-f")
{
    Description = "The path to a DIMACS file to be used as input."
};

Option<int> timeoutOption = new("--timeout", "-t")
{
    Description = "Abort the execution after given number of seconds."
};

RootCommand rootCommand = new("SAT-Solver CLI");
rootCommand.Options.Add(fileOption);
rootCommand.Options.Add(timeoutOption);

rootCommand.SetAction(async (parseResult) =>
{
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
        TimeSpan? timeout = null;

        if (parseResult.GetValue(timeoutOption) is int timeoutSeconds)
        {
            if (timeoutSeconds > 0)
            {
                timeout = new TimeSpan(0, 0, timeoutSeconds);
            }
            else if (timeoutSeconds < 0)
            {
                await Console.Error.WriteLineAsync($"Invalid argument for timeout: {timeoutSeconds}");
                return 1;
            }
        }

        SolveResult result = Solver.Solve(formula, timeout);

        switch (result.Type)
        {
            case SolveResult.ResultType.SATISFIABLE:
                if (result.Assignment.Count > 0)
                {
                    Console.WriteLine($"v {string.Join(' ', result.Assignment)} 0");
                }
                else
                {
                    Console.WriteLine("v 0");
                }
                break;
            case SolveResult.ResultType.UNSATISFIABLE: Console.WriteLine("s UNSATISFIABLE"); break;
            case SolveResult.ResultType.UNKNOWN: Console.WriteLine("s UNKNOWN"); break;
        }

        return 0;
    }

    rootCommand.Parse("--help").Invoke();
    return 0;
});

return await rootCommand.Parse(args).InvokeAsync();
