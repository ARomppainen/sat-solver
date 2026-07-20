# SAT-solver

[![CI](https://github.com/ARomppainen/sat-solver/actions/workflows/main.yml/badge.svg)](https://github.com/ARomppainen/sat-solver/actions/workflows/main.yml)
[![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=ARomppainen_sat-solver&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=ARomppainen_sat-solver)

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

### Code style

Apply style preferences and static analysis recommendations to solution

```
dotnet format
```

For more information, see the following guide:
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format

### Test execution

Execute test suites

```
dotnet test
```

### Code quality metrics

Code quality metrics are available at [SonarQube Cloud](https://sonarcloud.io/project/overview?id=ARomppainen_sat-solver).
