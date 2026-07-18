# Weekly report 2

This was a very productive week. I implemented a simple CLI application for running the SAT solver manually. After that, I added most of the test cases from the kissat-project to verify the behavior of my solver. The testing revealed some bugs related to unary / empty clauses that I was able to fix.

Next, I started working on unit propagation. I was also able to get it working along with the two-watched-literal optimization. Some of the code is still quite crude and contains a lot of duplication. After these changes, the full test run of 123 integration test cases took roughly 6 seconds to execute with all test cases passing.

My algorithm implementation so far has been based on the [DPLL pseudocode](https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/dpll.html). It uses recursion and I started to wonder if an iterative approach would be more performant. I was able to rewrite the algorithm without recursion and the test execution time dropped to less than 3 seconds (~50% reduction).

Finally, I also implemented the simple clause learning described in the course material. This naive implementation was very straightforward, but it seems to slow down the algorithm slightly based on the test suite execution time. I will investigate for ways to limit the number learned clauses at some point. I also tried to rewrite the algorithm to follow the [CDCL preudocode](https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/cdcl.html), but ultimately did not get it working. I might come back to this as some point with a fresh set of eyes.

Next week, I should focus more on code quality. I should start adding unit test cases and start documenting the code. I should also add code coverage reporting.

## Time spent

18 hours
