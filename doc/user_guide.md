# User guide

## Requirements

The program uses [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Install the SDK based on your operating system. The project has been tested on Windows 10 and Ubuntu 22.04.3.

## Compilation

Compile a debug version of the solution:

```sh
dotnet build
```

After compilation, the executable file is found in `src\SatSolver.Cli\bin\Debug\net10.0` folder.

Compile a release version of the solution:

```cmd
dotnet build -c Release
```

The release build is an optimized version, that does not include debugging symbols or debug assertions.

After compilation, the executable file is found in `src\SatSolver.Cli\bin\Release\net10.0` folder.

## Running the application

SatSolverCli is a command line application for [SAT solving](https://en.wikipedia.org/wiki/SAT_solver).

```sh
Description:
  SAT-Solver CLI

Usage:
  SatSolverCli [options]

Options:
  -f, --file <file>        The path to a DIMACS file to be used as input.
  -t, --timeout <timeout>  Abort the execution after given number of seconds.
  -?, -h, --help           Show help and usage information
  --version                Show version information
```

The CLI application works with [DIMACS formatted](#program-input) files. The [testdata](../SatSolverCore.Tests/testdata/) directory contains multiple files you can use. The `--file` option is mandatory for actually running the solver. Here are some examples of how to run the CLI application in Windows environment.

By running the compiled executable directly
```sh
SatSolver.Cli.exe --file path\to\dimacs\file
```

The `--timeout` option aborts the execution after given number of seconds 
```sh
SatSolver.Cli.exe --file path\to\dimacs\file --timeout 10
```

Alternatively, `dotnet run` command can be used. This also builds the project.
```
dotnet run --project src\SatSolver.Cli -- --file path\to\dimacs\file
```

The `-c` option can be set to use release version
```
dotnet run --project src\SatSolver.Cli -c Release -- --file path\to\dimacs\file
```

The `--no-build` option skips the build step
```
dotnet run --no-build --project src\SatSolver.Cli -c Release -- --file path\to\dimacs\file
```

## Program input

The input files follow [DIMACS](https://acl2.org/doc/?topic=SATLINK____DIMACS) format. Here is an example of such file:
```
c This is a comment line
p cnf 3 4
1 2 -3 0
1 -2 3 0
-1 2 -3 0
-1 -2 3 0
```

The file may contain any number of comment lines at the top, which begin with `c` character. These lines are ignored by the solver.

The line starting with `p` is the problem line. The solver supports only `cnf` ([Conjunctive Normal Form](https://en.wikipedia.org/wiki/Conjunctive_normal_form)) formulas. The last two numbers are the number variables and the number of clauses in the formula.

The rest of the lines after the problem line are clauses. The variables in a clause are represented as integers; negative integers correspond to negated variables. The solver assumes that each line contains a single clause. Each line must end with the number 0.

For example, the line `1 2 -3 0` represents the clause $(x_1 \lor x_2 \lor \neg x_3)$.

The example file represents the formula

$$
(x_1 \lor x_2 \lor \neg x_3)
\land
(x_1 \lor \neg x_2 \lor x_3)
\land
(\neg x_1 \lor x_2 \lor \neg x_3)
\land
(\neg x_1 \lor \neg x_2 \lor x_3)
$$

## Program output

If the solver is started successfully, the program output follows the [SAT Competition](https://satcompetition.github.io/2026/output.html) format.

If the formula is satisfiable, the output will contain value lines starting with `v`. The final value line will end with `0`.

```
s SATISFIABLE
v 1 2 3 -4 5 0
```

This output corresponds with the truth assignment $x_1 = 1, x_2 = 1, x_3 = 1, x_4 = 0, x_5 = 1$. 

The output may contain multiple value lines

```
s SATISFIABLE
v 1 -2 3 4 -5 6 -7 -8 -9 -10 11 12 13 14 15 -16 -17 -18 -19 20
v -21 -22 23 -24 25 -26 -27 28 -29 -30 -31 32 -33 -34 -35 -36 37 -38 -39 -40
v 41 -42 -43 44 -45 -46 -47 48 -49 50 -51 -52 53 -54 -55 56 -57 58 -59 60
v 61 62 63 64 65 0
```

If a satisfying assignment was not possible, the program will output

```
s UNSATISFIABLE
```

If the solver execution was aborted due to timeout, the program will output

```
s UNKNOWN
```
