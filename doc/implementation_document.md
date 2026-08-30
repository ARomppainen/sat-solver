# Implementation document

## Structure of the solution

The solution is split into multiple .NET projects

- `SatSolver.Cli`: A command line application for running the solver.
- `SatSolver.Core`: A library project that contains the main algorithm.
- `SatSolver.Core.Tests`: Unit and integration tests for the core library.
- `SatSolver.Dimacs`: A library project for DIMACS file parsing utilities.
- `SatSolver.Dimacs.Tests`: Unit tests for DIMACS parser.
- `SatSolver.Perf`: A performance analysis and reporting tool for the solver.
- `SatSolver.Shared`: A library project for shared data structures and extension methods.


## Class descriptions

The following classes are directly related to the [CDCL](https://en.wikipedia.org/wiki/Conflict-driven_clause_learning) algorithm

- `Solver`: The core part that ties everything together. Takes a single `Formula` as constructor parameter and has a `Solve()` method for running the algorithm. Contains functionality for running unit propagation, backjumping and clause learning.
- `VsidsHeuristic`: The class responsible for directing the search algorithm. Uses Variable State Independent Decaying Sum ([VSIDS](https://en.wikipedia.org/wiki/Boolean_satisfiability_algorithm_heuristics#Variable_State_Independent_Decaying_Sum)) heuristic for making the decisions. The implementation has two variants; The default one iterates over all variables to decide the unassigned literal with the highest activity score, while the second one uses a priority queue.
- `PartialAssignment`: The class responsible for keeping track of the current partial truth assignment. Contains functionality for reverting parts of the assignment (backjumping), and generating new learned clauses based on conflicts. Two different clause learning algorithms are implemented; The `AnalyzeConflictSimple` method uses the simple clause learning algorithm described in the course material. The `AnalyzeConflict` learns a clause based on the first unique implication point cut.
- `WatchedLiteralsV1` / `WatchedLiteralsV2`: The class responsible for efficient unit propagation through two-watched-literals scheme. `WatchedLiteralsV2` is a more performant implementation. For each clause of arity N, where N > 1, two literals are marked as *watched*. The following observations are important:
  - If the watched literals are non-false, the clause is not a unit clause.
  - When one of the watched literals becomes false, we try to find another non-false literal to replace the old one. If we cannot and if the other watched literal is unassigned, the clause is a unit clause.
  - When backjumping, it is not necesssary to update the watched literals.

Supporting classes

- `DimacsParser`: Parser for the DIMACS formatted input files. Throws a `DimacsParseException` if an error is encountered during the parse operation. The parse operation returns a `Formula`.
- `MaxHeap`: Maximum binary heap implementation that supports updates. Used by `VsidsHeuristic` when the `USE_MAX_HEAP` conditional compilation flag is set.


## Possible shortcomings and suggestions for improvement

SAT solvers and the CDCL algorithm are heavily researched topics, so there is much that could be improved. Some of the major components that are present in other CDCL solvers that I have left out from my implementation include

- Preprocessing
- Restarts
- Forgetting learned clauses
- Other heuristics for making decisions
- Parallelization

It is often possible to preprocess (and sometimes in-process) the formula to produce a set of clauses that is more simple to solve and satisfiable if and only if the original formula is satisfiable. The relevant techniques include Bounded Variable Elimination and subsumption. I did not implement any preprocessing techniques in my solver. I did mention pure literal elimination in the project specification, but when I tried it, the performance impact was negligible (only a few formulas in my test suite contained pure literals).

Restarts are used often in CDCL solvers to escape from tricky parts of the search space. The restart strategy can be based on, for example, geometric progression or other numeric sequences.

Reducing the number of learned clauses is necessary for the efficiency of the two-watched-literal scheme, when the formulas become larger. This can be done periodically, for example, by removing half of the learned clauses. It is also necessary to increment the number of possible learned clauses to ensure completeness of the search. The question then becomes which clauses should be kept and which should be removed. A popular metric for this is called Literal Block Distance (LBD), which counts the number of distinct decision levels in the learned clause (lower number is better).

There are many heuristics other than VSIDS making decisions. For example, Learning Rate Based (LRB) and Conflict History Based (CHB) heuristics can outperform VSIDS in some cases. Some solvers also utilize randomness to make decisions (e.g. every 100th decision is made randomly). Furthermore, my implementation does not consider polarity at all when making decisions (value *true* is always assigned to the chosen variable).

I initially considered implementing parallel processing to my solver, but I never proceeded with my initial plan. Some state-of-the-art solvers utilize parallelization but many do not. One of the technical challenges is finding a way to efficiently share the learned clauses between processes or threads (sychronization overhead). Another challenge is splitting the search space efficiently (all paths in the search space are not equally deep).


## Use of large language models

I used GitHub Copilot Student to help me use .NET related tooling (e.g. `dotnet-trace` and `PerfView`) which I had not used previously. Copilot was also helpful for describing the different DIMACS formulas kissat used (though I am still a bit uncertain about some of them). I also tried to use Copilot for debugging (e.g. during [week 3](weekly_report_3.md)), but I did not find it helpful for that purpose. This version of Copilot uses [automatically selected models](https://docs.github.com/en/copilot/reference/ai-models/supported-models#supported-ai-models-per-copilot-plan).

Google search AI overview was somewhat helpful (and hard to avoid...) for simple explanations of SAT related topics. I believe that it uses Gemini 3. The binary heap usage was inspired by one of the AI overview results (although, I do recall reading about it through other sources as well).

Generative AI tools were not used for other purposes.


## List of sources

Automated Reasoning. (2020, August 28). Lecture 06-1 SAT solver optimizations: 2-watched literals [Video]. YouTube. https://www.youtube.com/watch?v=n3e-f0vMHz8

Automated Reasoning. (2020, August 28). Lecture 06-2 SAT solver optimizations: storage [Video]. YouTube. https://www.youtube.com/watch?v=AB5Mq0R6zos

Automated Reasoning. (2020, August 28). Lecture 06-3 SAT solver optimizations: runtime choices  [Video]. YouTube. https://www.youtube.com/watch?v=-eD_DAUElE4

Eén, N., Sörensson, N. (2004). An Extensible SAT-solver. In: Giunchiglia, E., Tacchella, A. (eds) Theory and Applications of Satisfiability Testing. SAT 2003. Lecture Notes in Computer Science, vol 2919. Springer, Berlin, Heidelberg. https://doi-org.libproxy.helsinki.fi/10.1007/978-3-540-24605-3_37

Junttila, T. (2020). CS-E3220: Propositional Satisfiability and SAT Solvers. Retrieved July 6, 2026, from https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/
