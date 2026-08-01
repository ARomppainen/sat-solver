using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Globalization;

using SatSolverCore;
using SatSolverCore.Perf;

Option<string> pathOption = new("--path", "-p")
{
    Description = "The path to a folder to read .cnf files from",
    Required = true
};

Option<int> timeoutOption = new("--timeout", "-t")
{
    Description = "Amount of seconds to abort an individual solver execution",
    Required = true
};

Option<int> iterationsOption = new("--iterations", "-i")
{
    Description = "The number of iterations to run the solver per formula",
    Required = true
};

RootCommand rootCommand = new("SAT-Solver Performance Tool");
rootCommand.Options.Add(pathOption);
rootCommand.Options.Add(timeoutOption);
rootCommand.Options.Add(iterationsOption);

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

    string? path = parseResult.GetValue(pathOption);
    int timeout = parseResult.GetValue(timeoutOption);
    int iterations = parseResult.GetValue(iterationsOption);

    Console.WriteLine($"Reading formulas from directory: {path}");
    List<Formula> formulas = DimacsFileReader.ReadFormulas(path ?? "");

    Console.WriteLine($"{formulas.Count} formulas to sample");
    Console.WriteLine($"Iterations: {iterations}");
    Console.WriteLine($"Timeout: {timeout}");

    TimeSpan timeoutSeconds = new(0, 0, timeout);
    List<FormulaResult> results = [];

    for (int i = 0; i < formulas.Count; ++i)
    {
        Formula formula = formulas[i];

        Console.WriteLine($"({i + 1} / {formulas.Count}) Sampling {formula.Name}");
        List<Sample> samples = [];

        for (int iter = 1; iter <= iterations; ++iter)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            SolveResult solveResult = new Solver(formula).Solve(timeoutSeconds);
            stopwatch.Stop();
            samples.Add(new(iter, stopwatch.ElapsedMilliseconds, solveResult.Type == SolveResult.ResultType.UNKNOWN));

            if (solveResult.Type == SolveResult.ResultType.UNKNOWN)
            {
                // End sampling if solver timed out
                break;
            }
        }

        results.Add(new(formula.Name, formula.NumberOfVars, formula.Clauses.Count, samples));
    }

    Console.WriteLine("Name;NumberOfVars;NumberOfClauses;AvgMilliseconds;Timeout");
    foreach (FormulaResult result in results)
    {
        string name = result.Name.Split(Path.DirectorySeparatorChar)[^1];
        double avgMilliseconds = result.Samples.Average(row => row.Milliseconds);
        string avgMillisecondsStr = avgMilliseconds.ToString(CultureInfo.InvariantCulture); // Use decimal point
        string timeoutStr = result.Samples.Any(row => row.IsUnknown) ? "TRUE" : "FALSE";
        Console.WriteLine($"{name};{result.NumberOfVars};{result.NumberOfClauses};{avgMillisecondsStr};{timeoutStr}");
    }

    return 0;
});

return await rootCommand.Parse(args).InvokeAsync();
