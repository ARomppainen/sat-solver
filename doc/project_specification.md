# Project specification

This is a project for the course [Aineopintojen harjoitustyö: Algoritmit ja tekoäly](https://studies.helsinki.fi/kurssit/toteutus/otm-c9b77522-6ae8-4c18-9a65-12e37b9a316a), that was held during July-August 2026 as part of Computer Science Bachelor's curriculum (*tietojenkäsittelytieteen kandidaatti (TKT)*) in University of Helsinki. The project documentation will be written in English.

In this project, I will explore the Boolean satisfiability problem (SAT).


## Core

The core of the project is to implement a SAT solver using DPLL algorithm with unit propagation and [simple clause learning](https://algolabra-hy.github.io/topics-en#simple-clause-learning). This could be also considered a Conflict Driven Clause Learning (CDCL) algorithm.

The program will read a propositional formula in conjunctive normal form and returns either a truth assignment that satisfies the formula ('sat') or informs the caller if no such assigment exists ('unsat').

I will also try out various optimization techniques. These will include

- The two-watched-literal technique for efficient unit propagation
- Pure literal elimination through preprocessing
- Parallel processing through search space splitting

If I have time, I can also look into and compare various branching heuristics.


## Computational complexity

The Boolean satisfiability problem is NP-complete. The worst case computational complexity for DPLL algorithm is $O(2^n)$, where n is the number of variables. This is a direct result of trying out every possible combination, since each boolean variable can take two different values. The worst case space complexity is $O(n)$.

Through the use of various heuristics and optimization techniques, SAT solvers have become quite efficient in the last 20 years or so.
For example, a runtime of $O(1.307^n)$ is possible for [3-SAT](https://dl.acm.org/doi/10.1145/3313276.3316359) (a restricted version of the same problem where each clause can have at most three literals).

## Inputs and outputs

The boolean formulas are assumed to be in conjunctive normal form, for example:

$$
(x_1 \lor x_2 \lor \neg x_3) \land (\neg x_2 \lor \neg x_3) \land (\neg x_2 \lor x_3 \lor x_4)
$$

The input for the program will come from a [DIMACS](https://acl2.org/doc/?topic=SATLINK____DIMACS) formatted file.
The following example matches the formula above:

```
p cnf 4 3
1 2 -3 0
-2 -3 0
-2 3 4 0
```

The same formula is satisfied by the assignment:

$$
x_1 = 1, x_2 = 1, x_3 = 0, x_4 = 1
$$

The following program output represents the same assignment:

```
1 2 -3 4
```


## Implementation

This project will be implemented in C# using [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) runtime.

[xUnit](https://xunit.net/index.html?tabs=cs) will be used for testing.

The implementation will be split into multiple projects:

- `SatSolverCore`: Library project that will contain the main data structures and algorithms
- `SatSolverCore.Tests`: Test cases for the core
- `SatSolverCli`: Simple command line interface built on top of the core


## Sources

Automated Reasoning. (2020, August 28). Lecture 06-1 SAT solver optimizations: 2-watched literals [Video]. YouTube. https://www.youtube.com/watch?v=n3e-f0vMHz8

Automated Reasoning. (2020, August 28). Lecture 06-2 SAT solver optimizations: storage [Video]. YouTube. https://www.youtube.com/watch?v=AB5Mq0R6zos

Automated Reasoning. (2020, August 28). Lecture 06-3 SAT solver optimizations: runtime choices  [Video]. YouTube. https://www.youtube.com/watch?v=-eD_DAUElE4

Blanchette, J. C., Fleury, M., Lammich, P., & Weidenbach, C. (2018). A Verified SAT Solver Framework with Learn, Forget, Restart, and Incrementality. Journal of Automated Reasoning, 61(1–4), 333–365. https://doi.org/10.1007/s10817-018-9455-7

Junttila, T. (2020). CS-E3220: Propositional Satisfiability and SAT Solvers. Retrieved July 6, 2026, from https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/


## Peer reviews

I am proficient in Python, Java, C# and JavaScript/TypeScript. If needed, I can also review projects written in C, C++, Rust or even Haskell.

Feel free to leave peer reviews in Finnish or English.


## Changelog

- 2026-06-11: Initial version
