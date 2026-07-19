# SAT-solver

Aineopintojen harjoitustyö: Algoritmit ja tekoäly, 2026 (heinä-elokuu).

## Documentation

- [Project specification](./doc/project_specification.md)
- [Weekly report 1](./doc/weekly_report_1.md)
- [Weekly report 2](./doc/weekly_report_2.md)


## Development guide

### Requirements

The project uses [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Install the SDK based on the guide. The project has been tested on Windows 10 and Ubuntu 22.04.3.

### The CLI application

The CLI application uses [DIMACS](https://acl2.org/doc/?topic=SATLINK____DIMACS) formatted files as input. Take a look at the [testdata](./SatSolverCore.Tests/testdata/) folder for examples.

#### Running the solver

Linux

```sh
dotnet run --project SatSolverCli -- --file SatSolverCore.Tests/testdata/kissat/sat/and1.cnf
```

Windows

```cmd
dotnet run --project SatSolverCli -- --file SatSolverCore.Tests\testdata\kissat\sat\and1.cnf
```

### Test execution

Execute test suites (without code coverage)

```
dotnet test
```

### Code coverage report generation

Ensure that [ReportGenerator](https://github.com/danielpalme/ReportGenerator) tool is installed

```cmd
dotnet tool install --global dotnet-reportgenerator-globaltool
```

If you are working on a Linux system, add the dotnet tools directory to `PATH` environment variable.

```sh
export PATH="$HOME/.dotnet/tools:$PATH"
```

Generate a coverage report in XML format

```cmd
dotnet run --project SatSolverCore.Tests -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
```

Convert the XML report into an HTML report

Linux

```sh
reportgenerator -reports:SatSolverCore.Tests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml -targetdir:CoverageReport
```

Windows

```cmd
ReportGenerator -reports:SatSolverCore.Tests\bin\Debug\net10.0\TestResults\coverage.cobertura.xml -targetdir:CoverageReport
```

For more information, see the following guide:
https://xunit.net/docs/getting-started/v3/code-coverage-with-mtp
