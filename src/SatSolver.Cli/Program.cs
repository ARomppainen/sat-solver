using System.CommandLine;
using System.CommandLine.Parsing;

using SatSolver.Core;
using SatSolver.Dimacs;
using SatSolver.Shared;

namespace SatSolver.Cli;

static class Program
{
    static int Main(string[] args)
    {
        Option<FileInfo> fileOption = new("--file", "-f")
        {
            Description = "The path to a DIMACS file to be used as input."
        };

        Option<int> timeoutOption = new("--timeout", "-t")
        {
            Description = "Abort the execution after given number of seconds."
        };

        Option<bool> verboseOption = new("--verbose", "-v")
        {
            Description = "Print additional details about solver execution."
        };

        RootCommand rootCommand = new("SAT-Solver CLI");
        rootCommand.Options.Add(fileOption);
        rootCommand.Options.Add(timeoutOption);
        rootCommand.Options.Add(verboseOption);

        rootCommand.SetAction(parseResult => Action(
            parseResult, rootCommand, fileOption, timeoutOption, verboseOption));

        return rootCommand.Parse(args).Invoke();
    }

    private static int Action(
        ParseResult parseResult,
        RootCommand rootCommand,
        Option<FileInfo> fileOption,
        Option<int> timeoutOption,
        Option<bool> verboseOption
    )
    {
        if (parseResult.Errors.Count > 0)
        {
            foreach (ParseError parseError in parseResult.Errors)
            {
                Console.Error.WriteLine(parseError.Message);
            }
            return 1;
        }

        FileInfo? parsedFile = parseResult.GetValue(fileOption);

        if (parsedFile == null)
        {
            rootCommand.Parse("--help").Invoke();
            return 0;
        }

        if (!parsedFile.Exists)
        {
            Console.Error.WriteLine($"File does not exist: {parsedFile.FullName}");
            return 1;
        }

        int timeoutSeconds = parseResult.GetValue(timeoutOption);

        if (timeoutSeconds < 0)
        {
            Console.Error.WriteLine($"Invalid argument for timeout: {timeoutSeconds}");
            return 1;
        }

        bool verbose = parseResult.GetValue(verboseOption);

        using StreamReader reader = parsedFile.OpenText();

        Formula formula = DimacsParser.Parse(parsedFile.FullName, reader.Lines());
        TimeSpan? timeout = timeoutSeconds > 0 ? new TimeSpan(0, 0, timeoutSeconds) : null;
        SolveResult result = new Solver(formula).Solve(timeout);

        PrintResult(result);

        if (verbose)
        {
            PrintStatistics(result.Statistics);
        }

        return 0;
    }

    private static void PrintResult(SolveResult result)
    {
        if (result.Type == SolveResult.ResultType.UNSATISFIABLE)
        {
            Console.WriteLine("s UNSATISFIABLE");
            return;
        }

        if (result.Type == SolveResult.ResultType.UNKNOWN)
        {
            Console.WriteLine("s UNKNOWN");
            return;
        }

        Console.Write("s SATISFIABLE");

        if (result.Assignment.Count == 0)
        {
            Console.WriteLine("\nv 0");
            return;
        }

        int i = 0;
        int n = result.Assignment.Count;
        while (i < n)
        {
            Console.Write("\nv ");
            for (int j = 0; j < 20 && i < n; ++i, ++j)
            {
                Console.Write(result.Assignment[i]);
                Console.Write(' ');
            }
        }
        Console.WriteLine("0");
    }

    private static void PrintStatistics(SolverStatistics stats)
    {
        Console.WriteLine("c");
        Console.WriteLine("c Statistics");
        Console.WriteLine("c");
        Console.WriteLine("{0, -15}{1, 15}", "c conflicts:", stats.Conflicts);
        Console.WriteLine("{0, -15}{1, 15}", "c decisions:", stats.Decisions);
        Console.WriteLine("{0, -15}{1, 15}", "c propagations:", stats.Propagations);
        Console.WriteLine("{0, -15}{1, 15}{2, 8}", "c process time:", (stats.Milliseconds / 1000.0).ToString("F02"), "seconds");
    }
}
