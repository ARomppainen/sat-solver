using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;

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

    List<FormulaResult> results = [];

    for (int i = 0; i < formulas.Count; ++i)
    {
        Formula formula = formulas[i];

        Console.WriteLine($"({i + 1} / {formulas.Count}) Sampling {formula.Name}");
        List<Sample> samples = [];

        for (int iter = 1; iter <= iterations; ++iter)
        {
            Solver solver = new(formula);
            Stopwatch stopwatch = Stopwatch.StartNew();
            SolveResult solveResult = solver.Solve();
            stopwatch.Stop();
            samples.Add(new(iter, stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds, solveResult.Type == SolveResult.ResultType.UNKNOWN));
        }

        results.Add(new(formula.Name, formula.NumberOfVars, formula.Clauses.Count, samples));
    }

    Console.WriteLine("Name;NumberOfVars;NumberOfClauses;AvgTicks;AvgMilliseconds;Timeouts");
    foreach (FormulaResult result in results)
    {
        string name = result.Name.Split(Path.DirectorySeparatorChar)[^1];
        double avgTicks = result.Samples.Average(row => row.Ticks);
        double avgMilliseconds = result.Samples.Average(row => row.Milliseconds);
        int timeouts = result.Samples.Count(row => row.IsUnknown);
        Console.WriteLine($"{name};{result.NumberOfVars};{result.NumberOfClauses};{avgTicks};{avgMilliseconds};{timeouts}");
    }

    return 0;
});

return await rootCommand.Parse(args).InvokeAsync();
