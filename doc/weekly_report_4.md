# Weekly report 4

This week, I started by analyzing the program execution with profiling tools. I used [dotnet-trace](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace) for collecting diagnostics and [PerfView](https://github.com/microsoft/perfview) for visualizing them. Both of these tools were completely new to me, but I got accustomed to them very quickly.

This analysis revealed two things: (1) Using data structures that relied on hashing (HashSet and Dictionary) added unnecessary overhead. (2) The naive VSIDS implementation, that used linear search to make decisions, was quite slow. I was able to replace all instances of the mentioned data structures with simple arrays. I also implemented a version of VSIDS that maintains a sorted list of decisions using a custom max heap data structure.

During the week, I got feedback in Labtool that I should consider analyzing the performance impact of the various optimizations. That was already on my todo list, but this was the push I needed to actually implement that. I wrote a rather simple [script](../src/SatSolver.Perf/Program.cs) that benchmarks the solver with a given test suite (a folder with .cnf files) and reports the results in CSV format. This allowed me to import the results to Excel for further analysis and visualization. I also re-added parts of the older implementation using [conditional compilation symbols](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation) to be able compare the results.

I created a performance test suite using various tools. The [cnfgen tool](https://massimolauria.net/cnfgen/), that was mentioned in the course materials, was rather versatile for generating test cases, especially unsatisfiable ones. I also used this [online tool](https://homes.luddy.indiana.edu/sabry/cnf.html) for generating satisfiable factoring problems. Lastly, I added some Sudoku problems using the [sudoku-encode.py](https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/solving.html) script, since I wanted to include some harder problems that were still satisfiable. I will continue refining this set of formulas.

The initial results of this benchmarking were unsurprising. The improved clause learning algorithm enabled the program to solve harder problems that the simple version was not able to handle (for example, all of the sudoku problems). The VSIDS implementation with MaxHeap seemed to have the largest positive impact in the harder problems. On the other hard, when solving easier formulas, it seemed to be a source of additional overhead.

This week, I also wrote a more comprehensive [user guide](./user_guide.md), that should be helpful for the peer reviewer. I also started to work on the [implementation document](./implementation_document.md). I should start writing the testing document next.

## Hours worked

18 hours
