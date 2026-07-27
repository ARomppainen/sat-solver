# Implementation document

## Structure of the solution

The solution is split into multiple .NET projects

- `SatSolverCli`: A command line application for running the solver
- `SatSolverCore`: A library project that contains the main algorithm, supporting data structures and file parsing utilities
- `SatSolverCore.Test`: Unit and integration tests for the core library

## Class descriptions

The following classes are directly related to the [CDCL](https://en.wikipedia.org/wiki/Conflict-driven_clause_learning) algorithm

- `Solver`: The core part that ties everything together. Takes a single `Formula` as constructor parameter and contains a `Solve()` method for running the algorithm. Contains functionality for running unit propagation, backjumping and clause learning. Does not support learned clause deletion or restarts (yet).
- `Vsids`: The class responsible for directing the search algorithm. Uses Variable State Independent Decaying Sum ([VSIDS](https://en.wikipedia.org/wiki/Boolean_satisfiability_algorithm_heuristics#Variable_State_Independent_Decaying_Sum)) heuristic for making the decisions. The implementation uses `MaxHeap` data structure for efficient decision making.
- `PartialAssignment`: The class responsible for keeping track of the current partial truth assignment. Contains functionality for reverting parts of the assignment (backjumping), and generating new learned clauses based on conflicts.
- `WatchedLiterals`: The class responsible for efficient unit propagation through two-watched-literals scheme. For each clause, two literals are marked as *watched* (unary clauses are an exception). The following observations are key:
  - If the watched literals are non-false, the clause is not a unit clause
  - If one of the watched literals becomes false, we try to find another non-false literal to replace the old one. If we cannot, the clause is a unit clause.
  - When backjumping, it is not necesssary to update the watched literals.
- `ClauseUnary`, `ClauseBinary`, `ClauseNary`: These classes represent clauses with arity 1, 2 and N respectively. They keep track of the clause specific watched literals. Each class implements the `IClause` interface. Clauses are intantiated through `ClauseFactory`.
- `FalsifyResult`: Result type for updating the updating one of the watched literals to false. Falsification can lead to
  - no updates (clause was already satisfied)
  - a watchlist update (new watched literal was assigned)
  - unit propagation (the clause became a unit clause)
  - a conflict (all literals in the clause were assigned to false)

Supporting classes

- `DimacsParser`: Parser for the DIMACS formatted input files. Throws a `DimacsParseException` if an error is encountered during the parse operation. The parse operation returns a `Formula`.
- `MaxHeap`: Maximum binary heap implementation that supports updates. Used by `Vsids`.
- `SolveResult`: The result type of `Solver.Solve()`.
