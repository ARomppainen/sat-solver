# Weekly report 5

This week I had less time to work on the project than previous weeks. I spent some time further refactoring the solution to separate the DIMACS parser utilities and shared utilities from the core algorithm and to reduce code duplication. I also added unit test coverage for the parser. The added tests helped me to detect and fix one underlying bug that had been there since the beginning. For some reason, a few of the DIMACS files from kissat, like [xor3.cnf](../tests/SatSolver.Core.Tests/testdata/kissat/sat/xor3.cnf), contain empty rows. Now the parser will properly ignore them.

Futhermore, I wanted to update the algorithm to collect statistics of the program execution. I added a *verbose* command line option to the main program that will print out the statistics (the number of decisions, conflicts and propagated literals plus the execution time in seconds) as comments lines according to the SAT Competition format. This is useful for manual testing and also a nice to have feature when it is time to demo the program.

For a while now, my choice of parameter values for the VSIDS heuristic (decay threshold & decay multiplier) have been unoptimal. I updated the performance analysis tool to be able to benchmark the solver with different parameter values. After running the benchmark against my current performance test suite, I came up with new values for the threshold (100 → 16) and multiplier (0.995 → 0.9). I'm not sure how comprehensive my current test suite is yet, but at least the values are now based on some actual measurements.

I was not able to start writing the testing document yet, so I'll need to pick up on that next week.

## Hours worked

7 hours (+3 hours for the peer review)


---

Edit: 09-08-2026 21:10

- Updated wording
- I noticed that after the latest changes, my program is actually able to solve this [tricky sudoku problem](https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/solving.html) from [Gordon Royle's collection](https://web.archive.org/web/20120722180233/http://mapleta.maths.uwa.edu.au/~gordon/sudokumin.php) in roughly 30 seconds.
